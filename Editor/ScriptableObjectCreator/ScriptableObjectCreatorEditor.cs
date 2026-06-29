using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MM.ScriptableObjectCreator
{
	[CustomEditor(typeof(ScriptableObjectCreator))]
	public class ScriptableObjectCreatorEditor : Editor
	{
		delegate string ToolbarSearchField(string i_Text, params GUILayoutOption[] i_Options);

		private const string ControlName_Search = "ScriptSearch";
		private const string ControlName_Create = "CreateText";

		private bool m_ShowWindow;
		private bool m_WaitUp;
		private int m_CurrentSelected = -1;
		private string m_SearchText;
		private string m_CreateText;
		private Vector3 m_Scroll;
		private ToolbarSearchField m_SearchFieldDrawer;

		private ScriptableObjectCreatorItem m_RootItem;
		private ScriptableObjectCreatorItem m_SearchItem;
		private ScriptableObjectCreatorItem m_CreateItem;
		private ScriptableObjectCreatorItem m_CurrentItem;
		private List<ScriptableObjectCreatorItem> m_Items = new List<ScriptableObjectCreatorItem>();

		private void OnEnable()
		{
			m_SearchText = EditorPrefs.GetString(target.GetType().ToString() + ControlName_Search, string.Empty);

			m_SearchFieldDrawer = (ToolbarSearchField)Delegate.CreateDelegate(typeof(ToolbarSearchField),
				typeof(EditorGUILayout).GetMethod("ToolbarSearchField",
					BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any,
					new[] {typeof(string), typeof(GUILayoutOption[])}, null));

			CreateAllItems();
		}

		public override void OnInspectorGUI()
		{
			// If any change, reset the current selected
			if (m_CurrentSelected != -1 && !CanShow(m_Items[m_CurrentSelected]))
			{
				m_CurrentSelected = -1;
			}

			// Check for up, down, enter and escape
			ManageInput();

			// Focus to the current input
			if (m_CurrentItem == m_CreateItem)
			{
				EditorGUI.FocusTextInControl(ControlName_Create);
			}
			else
			{
				EditorGUI.FocusTextInControl(ControlName_Search);
			}

			// Start Layout
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();

				using (new GUILayout.VerticalScope())
				{
					// Simple button to reproduce the Add Component behaviour on a GameObject
					if (GUILayout.Button("\tAdd Scriptable Object \t", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
					{
						m_ShowWindow = true;
					}

					if (m_ShowWindow)
					{
						using (new GUILayout.VerticalScope("box"))
						{
							// Draw the search bar on the top
							DrawSearch();

							// Draw the Header (title/folder) of the current view, when focused, deselect
							bool headerFocused = m_CurrentItem.DrawAsHeader();
							if (headerFocused)
							{
								m_CurrentSelected = -1;
							}

							using (GUILayout.ScrollViewScope scope = new GUILayout.ScrollViewScope(m_Scroll))
							{
								if (m_CurrentItem == m_CreateItem)
								{
									// Specific method for create layout
									DrawCreate();
								}
								else
								{
									for (int i = 0; i < m_Items.Count; i++)
									{
										ScriptableObjectCreatorItem item = m_Items[i];
										if (CanShow(item))
										{
											// Item is drawn and returns if it was focused while drawn
											bool focused = item.DrawItem(m_CurrentSelected == i);
											if (focused)
											{
												m_CurrentSelected = i;
											}
										}
									}
								}

								m_Scroll = scope.scrollPosition;
							}
						}
					}
				}

				GUILayout.FlexibleSpace();
			}
		}

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		private void CreateAllItems()
		{
			// Get all classes that inherit ScriptableObject from the Unity Assemblies, we will ignore our own classes that inherit from those
			List<Type> inheritTypes =
				AppDomain.CurrentDomain.GetAssemblies()
					.Select(x =>
					{
						return x
							.GetTypes()
							.Where(t =>
								t != null &&
								!string.IsNullOrEmpty(t.Namespace) &&
								t.Namespace.StartsWith("Unity") &&
								t.IsSubclassOf(typeof(ScriptableObject)) &&
								!t.IsSubclassOf(typeof(Editor))
							);
					})
					.SelectMany(x => x)
					.ToList();


			// Find all scripts in the project, each script found will be associated with a ScriptableObjectCreatorItem
			List<MonoScript> scripts = AssetDatabase.FindAssets("t:MonoScript")
				.Select(x => AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(x)))
				.Where(t =>
					t.GetClass() != null &&
					!t.GetClass().IsAbstract &&
					t.GetClass().IsSubclassOf(typeof(ScriptableObject)) &&
					!inheritTypes.Any(x => t.GetClass().IsSubclassOf(x))
				)
				.ToList();

			m_Items.Clear();

			// Create the Root Item, it will be the first Item selected
			m_RootItem = new ScriptableObjectCreatorItem();
			m_RootItem.m_Name = "Scriptable Object";
			m_Items.Add(m_RootItem);
			m_CurrentItem = m_RootItem;

			// Create the Unsorted Item, it will contain all ScriptableObjects that are not ordered by Namespace
			ScriptableObjectCreatorItem unsorted = new ScriptableObjectCreatorItem();
			unsorted.m_Name = "Unsorted";
			unsorted.m_Parent = m_RootItem;
			unsorted.m_Clicked = () => { m_CurrentItem = unsorted; };
			unsorted.m_ClickedAsHeader = () => { m_CurrentItem = unsorted.m_Parent; };
			m_Items.Add(unsorted);

			foreach (MonoScript script in scripts)
			{
				GetFolderHierarchy(script.GetClass());
				ScriptableObjectCreatorItem lastFolder = null;
				string[] hierarchy = GetFolderHierarchy(script.GetClass());
				for (int i = 0; i < hierarchy.Length; i++)
				{
					// Check if it already exists, if it does exist just keep going
					ScriptableObjectCreatorItem exists = m_Items.FirstOrDefault(x => x.m_Name == hierarchy[i]);
					if (exists == null)
					{
						// Use the Namespace info to create Folders
						ScriptableObjectCreatorItem folder = new ScriptableObjectCreatorItem();
						folder.m_Name = hierarchy[i];
						folder.m_Clicked = () => { m_CurrentItem = folder; };
						folder.m_ClickedAsHeader = () => { m_CurrentItem = folder.m_Parent; };

						if (i == 0)
						{
							folder.m_Parent = m_RootItem;
						}
						else
						{
							folder.m_Parent = lastFolder;
						}

						m_Items.Add(folder);
						lastFolder = folder;
					}
					else
					{
						lastFolder = exists;
					}
				}

				// Create a Script Item
				ScriptableObjectCreatorItem item = new ScriptableObjectCreatorItem();
				item.m_Name = script.GetClass().Name;
				item.m_Script = script;
				item.m_Clicked = () => { AddScript(item.m_Script); };

				if (lastFolder != null)
				{
					item.m_Parent = lastFolder;
				}
				else
				{
					item.m_Parent = unsorted;
				}

				m_Items.Add(item);
			}

			// Sort all items by Name
			m_Items.Sort((a, b) => a.m_Name.CompareTo(b.m_Name));

			// Create Item that will allow the user to create their own scripts. It will always be at the bottom of the list.
			m_CreateItem = new ScriptableObjectCreatorItem();
			m_CreateItem.m_Name = "New Script";
			m_CreateItem.m_Parent = m_RootItem;
			m_CreateItem.m_Clicked = () =>
			{
				m_CreateItem.m_Parent = m_CurrentItem;
				m_CurrentItem = m_CreateItem;
				if (string.IsNullOrEmpty(m_SearchText))
				{
					m_CreateText = "NewScriptableObject";
				}
				else
				{
					m_CreateText = m_SearchText;
				}

				m_SearchText = string.Empty;
			};
			m_CreateItem.m_ClickedAsHeader = () => { m_CurrentItem = m_CurrentItem.m_Parent; };
			m_Items.Add(m_CreateItem);

			// Create Item that will show that the user is currently searching. It doesn't need to be in the items list.
			m_SearchItem = new ScriptableObjectCreatorItem();
			m_SearchItem.m_Name = "Search";
			m_SearchItem.m_Parent = m_RootItem;
			m_SearchItem.m_ClickedAsHeader = () =>
			{
				m_SearchText = string.Empty;
				m_CurrentItem = m_RootItem;
				EditorGUI.FocusTextInControl(null);
			};
		}

		private string[] GetFolderHierarchy(Type i_Type)
		{
			object[] attribute = i_Type.GetCustomAttributes(typeof(AddScriptableObjectMenuAttribute), true);
			if (attribute.Length > 0)
			{
				return (attribute[0] as AddScriptableObjectMenuAttribute).m_Menu.Split('/');
			}
			else if (!string.IsNullOrEmpty(i_Type.Namespace))
			{
				return i_Type.Namespace.Split('.');
			}
			else
			{
				return new string[0];
			}
		}

		private bool CanShow(ScriptableObjectCreatorItem i_Item)
		{
			// If the user is currently searching
			if (m_CurrentItem == m_SearchItem)
			{
				// Check if it is Script Item and the Name contains the search text || it's the Create Item
				if ((i_Item.m_Script != null && i_Item.m_Name.ToLower().Contains(m_SearchText.ToLower())) || i_Item == m_CreateItem)
				{
					return true;
				}
			}
			// Check if the Current Item is the parent of the Item || it's the Create Item
			else if (i_Item.m_Parent == m_CurrentItem || i_Item == m_CreateItem)
			{
				return true;
			}

			return false;
		}

		private void AddScript(MonoScript i_Script)
		{
			// Changes the Script of a SerializedObject (which in this cas, points to a ScriptableObject)
			serializedObject.Update();
			serializedObject.FindProperty("m_Script").objectReferenceValue = i_Script;
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawSearch()
		{
			GUI.SetNextControlName(ControlName_Search);
			EditorGUI.BeginChangeCheck();
			m_SearchText = m_SearchFieldDrawer(m_SearchText);
			if (EditorGUI.EndChangeCheck())
			{
				// If searching for nothing, the Root Item will be the Current Item
				if (string.IsNullOrEmpty(m_SearchText))
				{
					m_CurrentItem = m_RootItem;
					m_CurrentSelected = -1;
				}
				// Else we're searching and the Current Item is the Search Item
				else
				{
					m_CurrentItem = m_SearchItem;
					m_CurrentSelected = 0;
				}

				// Save the Search Text (Unity does this for Add Component so we'll do the same)
				EditorPrefs.SetString(target.GetType().ToString() + ControlName_Search, m_SearchText);
			}
		}

		private void DrawCreate()
		{
			using (new GUILayout.VerticalScope())
			{
				GUILayout.Label("Name");
				GUI.SetNextControlName(ControlName_Create);
				m_CreateText = EditorGUILayout.TextField(m_CreateText);
				GUILayout.Label("Script will be added to ScriptableObject after the project reloads", EditorStyles.miniBoldLabel);

				GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.5f);

				if (!m_WaitUp && Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
				{
					CreateNewScript(m_CreateText);
				}

				if (Event.current.type == EventType.KeyUp)
				{
					// Because we check for KeyUp, we need to wait till the previous Input has completed.
					m_WaitUp = false;
				}

				if (GUILayout.Button("Create and Add"))
				{
					CreateNewScript(m_CreateText);
				}
			}
		}

		private void CreateNewScript(string i_ScriptName)
		{
			try
			{
				// Text for the new Script
				string newFile = "using System.Collections;" +
				                 "\nusing System.Collections.Generic;" +
				                 "\nusing UnityEngine;" +
				                 "\n\npublic class " +
				                 m_CreateText +
				                 " : ScriptableObject" +
				                 "\n{" +
				                 "\n\n}\n";
				// The path for the new Script
				string path = "Assets/" + m_CreateText + ".cs";

				// Write the stream
				StreamWriter writer = File.CreateText(path);
				writer.WriteLine(newFile);
				writer.Close();

				// Refresh, then Get the newly created MonoScript
				AssetDatabase.Refresh();
				MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

				if (script != null)
				{
					AddScript(script);
				}
			}
			catch
			{
				// StreamWriter failed or something like that.
				EditorUtility.DisplayDialog("Error", "Could not create script", "ok");
			}
		}

		private void ManageInput()
		{
			if (Event.current.type == EventType.KeyDown)
			{
				// Move down the Current Selected until one is found that can be drawn
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					do
					{
						m_CurrentSelected++;
					}
					while (m_CurrentSelected < m_Items.Count && !CanShow(m_Items[m_CurrentSelected]));

					if (m_CurrentSelected >= m_Items.Count)
					{
						m_CurrentSelected = -1;
					}

					Event.current.Use();
				}
				// Move up the Current Selected until one is found that can be drawn
				else if (Event.current.keyCode == KeyCode.UpArrow)
				{
					do
					{
						m_CurrentSelected--;
					}
					while (m_CurrentSelected >= 0 && !CanShow(m_Items[m_CurrentSelected]));

					if (m_CurrentSelected < -1)
					{
						for (int i = m_Items.Count - 1; i >= 0; i--)
						{
							if (CanShow(m_Items[i]))
							{
								m_CurrentSelected = i;
								break;
							}
						}
					}

					Event.current.Use();
				}
				// Check for the Return Key to select items
				else if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{
					if (GUI.GetNameOfFocusedControl() != ControlName_Create)
					{
						if (m_CurrentSelected != -1 && m_Items[m_CurrentSelected].m_Clicked != null)
						{
							m_Items[m_CurrentSelected].m_Clicked();
						}

						Event.current.Use();
						m_WaitUp = true;
					}
				}
				// Check for the Escape Key to go to the previous Folder
				else if (Event.current.keyCode == KeyCode.Escape)
				{
					if (m_CurrentItem.m_ClickedAsHeader != null)
					{
						m_CurrentItem.m_ClickedAsHeader();
					}

					Event.current.Use();
				}
			}
		}
	}
}
