using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MM.EditorDrawer;
using System;

namespace MM.BaseExplorer
{
	public abstract class BaseExplorerWindow<T> : EditorWindow, IBaseExplorerWindow where T : BaseExplorerItem
	{
		private const float LIST_VIEW_WIDTH = 350f;
		
		private string m_SearchFilter = "";
		private List<T> m_AllItems = new List<T>();
		private List<T> m_FilteredItems = new List<T>();
		private HashSet<T> m_Selected = new HashSet<T>();

		private Rect m_ListRect;
		private Vector2 m_ListScroll;
		private Vector2 m_InspectorScroll;

		protected virtual void OnEnable()
		{
			if (RefreshOnEnable)
			{
				Refresh();
			}
		}

		private void OnGUI()
		{
			// Header
			using (GUIScope.Horizontal(ExplorerStyles.Toolbar, GUILayout.ExpandWidth(true)))
			{
				DrawHeader();

				GUILayout.FlexibleSpace();
				
				using (GUIScope.BackgroundColor(Color.green))
				{
					if (GUILayout.Button("Force Refresh", ExplorerStyles.BtnNormal, GUILayout.Width(110f)))
					{
						Refresh();
					}
				}
			}

			ManageKeyboardInputs();

			using (GUIScope.Horizontal())
			{
				// List
				using (GUIScope.Vertical(GUILayout.Width(LIST_VIEW_WIDTH), GUILayout.ExpandWidth(true)))
				{
					GUILayout.Space(5f);
					DrawList();
				}
				
				GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(2f));

				// Inspector
				using (GUIScope.Vertical(GUILayout.ExpandWidth(true)))
				{
					GUILayout.Space(5f);
					DrawInspector();
				}
			}
		}

		private int GetIndexCurrentSelected()
		{
			return m_Selected.Count == 0 ? -1 : m_FilteredItems.IndexOf(m_Selected.Last());
		}

		private void ManageKeyboardInputs()
		{
			if (m_FilteredItems.Count == 0)
			{
				return;
			}

			int currentSelected = GetIndexCurrentSelected();

			if (Event.current.type == EventType.KeyDown)
			{
				switch (Event.current.keyCode)
				{
					case KeyCode.UpArrow:
						ManageKeyboardNavigation(currentSelected - 1);
						Event.current.Use();
						break;

					case KeyCode.DownArrow:
						ManageKeyboardNavigation(currentSelected + 1);
						Event.current.Use();
						break;

					case KeyCode.Escape:
						if (EditorGUIUtility.editingTextField)
						{
							EditorGUIUtility.editingTextField = false;
						}
						else
						{
							Close();
						}

						Event.current.Use();
						break;
				}
			}

			if (Event.current.modifiers == EventModifiers.Control)
			{
				switch (Event.current.keyCode)
				{
					case KeyCode.A:
						SetSelected(m_FilteredItems.ToArray());

						Event.current.Use();
						break;

					case KeyCode.C:
						EditorGUIUtility.systemCopyBuffer = string.Join("\n", m_Selected.Select(x => x.Name).ToArray());

						Event.current.Use();
						break;
				}
			}
		}

		private void ManageKeyboardNavigation(int i_NewSelected)
		{
			int filteredCount = m_FilteredItems.Count;
			// The following line is a modulo that also works with negative numbers (because c# doesn't support that)
			T asset = m_FilteredItems[(i_NewSelected % filteredCount + filteredCount) % filteredCount];

			if (Event.current.shift)
			{
				AddSelected(asset);
			}
			else
			{
				SetSelected(asset);
			}
		}

		private void ManageMouseInputs(Rect i_Rect, T i_Item)
		{
			Rect rect = new Rect(i_Rect)
			{
				y = i_Rect.y - 1f,
				height = i_Rect.height + 2f,
			};
			
			if (!rect.Contains(Event.current.mousePosition))
			{
				return;
			}

			if (Event.current.type == EventType.MouseDown)
			{
				if (Event.current.control)
				{
					if (IsSelected(i_Item))
					{
						RemoveSelected(i_Item);
					}
					else
					{
						AddSelected(i_Item);
					}
				}
				else if (Event.current.shift)
				{
					int newIndex = m_FilteredItems.IndexOf(i_Item);
					int latestIndex = GetIndexCurrentSelected();

					int min = Math.Min(newIndex, latestIndex);
					int max = Math.Max(newIndex, latestIndex);
					AddSelected(m_FilteredItems.Where((x, i) => i >= min && i <= max).ToArray());
				}
				else
				{
					SetSelected(i_Item);
				}
			}
		}

		private void DrawList()
		{
			using (GUIScope.Horizontal(ExplorerStyles.Toolbar))
			{
				EditorGUILayout.LabelField(ItemTypeName, ExplorerStyles.HeaderStyle, GUILayout.Width(100f));

				EditorGUI.BeginChangeCheck();

				m_SearchFilter = EditorGUILayout.TextField(m_SearchFilter, ExplorerStyles.SearchField);

				if (EditorGUI.EndChangeCheck())
				{
					m_SearchFilter = m_SearchFilter.ToLower();
					Filter();
				}

				if (string.IsNullOrEmpty(m_SearchFilter))
				{
					GUILayout.Label("", ExplorerStyles.SearchFieldEnd);
				}
				else
				{
					if (GUILayout.Button("", ExplorerStyles.SearchFieldCancel))
					{
						m_SearchFilter = "";
						EditorGUI.FocusTextInControl("");
						EditorGUIUtility.editingTextField = false;
						Filter();
					}
				}
			}

			using (GUIScope.ScrollView(ref m_ListScroll))
			{
				foreach (T item in m_FilteredItems)
				{
					Rect rect = EditorGUILayout.GetControlRect();
					item.DrawInList(rect, IsSelected(item), focusedWindow == this);

					ManageMouseInputs(rect, item);
				}
			}

			if (Event.current.type == EventType.Repaint)
			{
				m_ListRect = GUILayoutUtility.GetLastRect();
			}
		}

		private void DrawInspector()
		{
			if (SupportMultiSelect)
			{
				using (GUIScope.Horizontal(ExplorerStyles.Toolbar))
				{
					DrawInspectorHeader();
				}
			}

			using (GUIScope.ScrollView(ref m_InspectorScroll, GUILayout.ExpandWidth(true)))
			{
				foreach (T item in m_Selected)
				{
					GUILayout.Space(5f);
					using (GUIScope.Horizontal(ExplorerStyles.Toolbar))
					{
						item.DrawInspectorHeader(this);
					}
					
					GUILayout.Space(5f);
					item.DrawInspectorContents(this);
				}
			}
		}

		protected virtual void DrawInspectorHeader()
		{
			GUILayout.Label(m_Selected.Count + " Items Selected", ExplorerStyles.HeaderStyle);
			GUILayout.FlexibleSpace();
		}

		protected void Filter()
		{
			m_FilteredItems = m_AllItems
				.Where(x => x.Name.ToLower().Contains(m_SearchFilter))
				.ToList();
			m_FilteredItems = FilterItems(m_FilteredItems);

			m_Selected = new HashSet<T>(m_Selected.ToList().Intersect(m_FilteredItems));
			RepositionScrollList(m_Selected.LastOrDefault());
			
			Repaint();
		}

		public static void DropdownButton(GUIContent i_Label, GUIContent[] i_DropdownContent, int i_CurrentSelected, System.Action<int> i_SelectedCallback)
		{
			Rect rect = GUILayoutUtility.GetRect(i_Label, ExplorerStyles.DropDown);
			if (GUI.Button(rect, i_Label, ExplorerStyles.DropDown))
			{
				if (i_DropdownContent.Length == 0)
				{
					return;
				}

				EditorUtility.DisplayCustomMenu(rect, i_DropdownContent, i_CurrentSelected,
					(object i_Userdata, string[] i_Options, int i_Selected) =>
					{
						i_SelectedCallback(i_Selected);
					},
					null);
			}
		}

		public HashSet<T> GetSelected()
		{
			return m_Selected;
		}

		public bool IsSelected(BaseExplorerItem i_Item)
		{
			return m_Selected.Contains((T)i_Item);
		}

		public void AddSelected(params BaseExplorerItem[] i_Items)
		{
			foreach (BaseExplorerItem item in i_Items)
			{
				m_Selected.Add((T)item);
			}
			
			RepositionScrollList(i_Items.Last());

			Repaint();
			GUIUtility.ExitGUI();
		}

		public void SetSelected(params BaseExplorerItem[] i_Items)
		{
			m_Selected.Clear();
			AddSelected(i_Items);
			
			RepositionScrollList(m_Selected.Last());
		}

		private void RepositionScrollList(BaseExplorerItem i_Items)
		{
			int indexof = m_FilteredItems.IndexOf((T)i_Items);
			if (indexof == -1)
			{
				return;
			}
			
			float itemPosition = indexof * 18f;
			if (itemPosition < m_ListScroll.y)
			{
				m_ListScroll.y = itemPosition;
			}
			else if (itemPosition + 18f > m_ListScroll.y + m_ListRect.height)
			{
				m_ListScroll.y = itemPosition - m_ListRect.height + 18f;
			}
		}

		public void RemoveSelected(params BaseExplorerItem[] i_Items)
		{
			foreach (T item in i_Items)
			{
				if (IsSelected(item))
				{
					m_Selected.Remove(item);
				}
			}

			Repaint();
			GUIUtility.ExitGUI();
		}

		public void RemoveCompletely(params BaseExplorerItem[] i_Items)
		{
			foreach (T item in i_Items)
			{
				if (m_AllItems.Contains(item))
				{
					m_AllItems.Remove(item);
				}
				
				if (IsSelected(item))
				{
					m_Selected.Remove(item);
				}
			}

			Filter();
			GUIUtility.ExitGUI();
		}

		public float GetListWidth()
		{
			return LIST_VIEW_WIDTH;
		}

		public float GetInspectorWidth()
		{
			return position.width - GetListWidth() - 20f;
		}

		public void Refresh()
		{
			ForceRefresh(RefreshCompleted);
		}
		
		private void RefreshCompleted(List<T> i_Items)
		{
			EditorApplication.delayCall +=
			() =>
			{
				m_AllItems = i_Items;
				Filter();
			};
		}
		
		protected abstract void DrawHeader();
		protected abstract void ForceRefresh(Action<List<T>> i_RefreshCompleted);
		protected abstract List<T> FilterItems(List<T> i_Items);

		protected abstract bool SupportMultiSelect { get; }
		protected abstract bool RefreshOnEnable { get; }
		protected abstract string ItemTypeName { get; }
	}
}
