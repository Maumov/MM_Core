using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MM.EditorDrawer
{
	public class UnityIcons : EditorWindow
	{
		[MenuItem("Window/Unity Icons")]
		private static void OpenWindow()
		{
			GetWindow<UnityIcons>("Unity Icons");
		}

		private static readonly Vector2 ICON_ITEM_SIZE = new Vector2(100f, 100f);
		private static readonly Vector2 STYLE_ITEM_SIZE = new Vector2(200f, 50f);

		private string m_SearchFilter = "";
		private State m_State;
		private Vector2 m_StyleScroll;
		private Vector2 m_IconScroll;

		private List<Texture> m_AllIcons = new List<Texture>();
		private IEnumerable<Texture> m_FilteredIcons;

		private List<GUIStyle> m_AllStyles = new List<GUIStyle>();
		private IEnumerable<GUIStyle> m_FilteredStyles;

		private enum State
		{
			Icons,
			Styles,
		}

		private GUIStyle m_BoxStyle = null;

		public GUIStyle BoxStyle
		{
			get
			{
				return m_BoxStyle ?? (m_BoxStyle = new GUIStyle("HelpBox")
				{
					alignment = TextAnchor.MiddleCenter
				});
			}
		}

		private GUIStyle m_CenteredStyle = null;

		public GUIStyle CenteredStyle
		{
			get
			{
				return m_CenteredStyle ?? (m_CenteredStyle = new GUIStyle("Label")
				{
					alignment = TextAnchor.MiddleCenter
				});
			}
		}

		private GUIStyle m_RightStyle = null;

		public GUIStyle RightStyle
		{
			get
			{
				return m_RightStyle ?? (m_RightStyle = new GUIStyle("Label")
				{
					alignment = TextAnchor.MiddleRight
				});
			}
		}

		private void OnGUI()
		{
			using (GUIScope.Horizontal())
			{
				if (GUILayout.Toggle(m_State == State.Styles, "Styles", EditorStyles.toolbarButton))
				{
					m_State = State.Styles;
				}

				if (GUILayout.Toggle(m_State == State.Icons, "Icons", EditorStyles.toolbarButton))
				{
					m_State = State.Icons;
				}
			}

			UpdateLists();

			string result = DrawSearch(m_SearchFilter);
			if (result != m_SearchFilter)
			{
				m_SearchFilter = result;
				FilterLists();
			}

			switch (m_State)
			{
				case State.Styles:
					DrawList(m_FilteredStyles, DrawStyle, STYLE_ITEM_SIZE, ref m_StyleScroll);
					break;

				case State.Icons:
					DrawList(m_FilteredIcons, DrawIcon, ICON_ITEM_SIZE, ref m_IconScroll);
					break;
			}
		}

		private void FilterLists()
		{
			m_FilteredIcons = m_AllIcons
				.Where(x => x.name.ToLower().Contains(m_SearchFilter));
			m_FilteredStyles = m_AllStyles
				.Where(x => x.name.ToLower().Contains(m_SearchFilter));
		}

		private void UpdateLists()
		{
			if (m_AllStyles.Count == 0)
			{
				m_AllStyles = GUI.skin.customStyles
					.OrderBy(x => x.name)
					.ToList();
			}

			if (m_FilteredStyles == null)
			{
				m_FilteredStyles = m_AllStyles;
			}
			
			if (m_AllIcons.Count == 0)
			{
				m_AllIcons = Resources.FindObjectsOfTypeAll<Texture>()
					.OrderBy(x => x.name)
					.ToList();
			}

			if (m_FilteredIcons == null)
			{
				m_FilteredIcons = m_AllIcons;
			}
		}

		private string DrawSearch(string i_Search)
		{
			using (GUIScope.Horizontal())
			{
				EditorGUI.BeginChangeCheck();

				i_Search = EditorGUILayout.TextField(i_Search, (GUIStyle)"ToolbarSeachTextField");

				if (EditorGUI.EndChangeCheck())
				{
					i_Search = i_Search.ToLower();
				}

				if (string.IsNullOrEmpty(i_Search))
				{
					GUILayout.Label("", (GUIStyle)"ToolbarSeachCancelButtonEmpty");
				}
				else
				{
					if (GUILayout.Button("", (GUIStyle)"ToolbarSeachCancelButton"))
					{
						i_Search = "";
						EditorGUI.FocusTextInControl("");
						EditorGUIUtility.editingTextField = false;
					}
				}
			}

			return i_Search;
		}

		private void DrawList<T>(IEnumerable<T> i_List, Action<Rect, T> i_DrawCallback, Vector2 i_ItemSize, ref Vector2 i_Scroll)
		{
			using (GUIScope.ScrollView(ref i_Scroll))
			{
				using (GUIScope.GridScope grid = GUIScope.Grid(Mathf.FloorToInt(position.width / i_ItemSize.x), i_ItemSize))
				{
					foreach (T item in i_List)
					{
						if (item == null)
						{
							continue;
						}

						Rect rect = grid.Next();
						if (!IsInScreen(rect.y, m_StyleScroll, i_ItemSize))
						{
							continue;
						}

						i_DrawCallback(rect, item);
					}
				}
			}
		}

		private void DrawStyle(Rect i_Rect, GUIStyle i_Style)
		{
			i_Rect.x += 5f;
			i_Rect.y += 5f;
			i_Rect.width -= 10f;
			i_Rect.height -= 10f;

			Rect bg = new Rect(i_Rect)
			{
				height = i_Rect.height - EditorGUIUtility.singleLineHeight,
			};
			GUI.Label(bg, "", BoxStyle);

			Rect activeStyle = new Rect(bg)
			{
				width = bg.width / 2f
			};
			if (Event.current.type == EventType.Repaint)
			{
				i_Style.Draw(activeStyle, "Inactive", false, false, false, false);
			}

			Rect inactiveStyle = new Rect(bg)
			{
				x = bg.x + bg.width / 2f,
				width = bg.width / 2f
			};
			if (Event.current.type == EventType.Repaint)
			{
				i_Style.Draw(inactiveStyle, "Pressed", false, false, true, false);
			}
			
			Rect label = new Rect(i_Rect)
			{
				height = EditorGUIUtility.singleLineHeight,
				y = i_Rect.y + (i_Rect.height - EditorGUIUtility.singleLineHeight)
			};
			if (GUI.Button(label, i_Style.name))
			{
				GUIUtility.systemCopyBuffer = $"(GUIStyle)\"{i_Style.name}\"";
				Debug.Log("Copied to buffer : " + GUIUtility.systemCopyBuffer);
			}
		}

		private void DrawIcon(Rect i_Rect, Texture i_Resource)
		{
			Rect texture = new Rect(i_Rect)
			{
				height = i_Rect.height - EditorGUIUtility.singleLineHeight,
			};
			GUI.Label(texture, i_Resource, BoxStyle);

			Rect size = new Rect(texture)
			{
				height = EditorGUIUtility.singleLineHeight,
				y = texture.y + (texture.height - EditorGUIUtility.singleLineHeight)
			};
			GUI.Label(size, i_Resource.width + "x" + i_Resource.height, RightStyle);

			Rect label = new Rect(i_Rect)
			{
				height = EditorGUIUtility.singleLineHeight,
				y = i_Rect.y + (i_Rect.height - EditorGUIUtility.singleLineHeight)
			};
			if (GUI.Button(label, i_Resource.name))
			{
				const string findTextureText = "EditorGUIUtility.FindTexture(\"{0}\")";
				const string loadTextureText = "(Texture)EditorGUIUtility.Load(\"{0}\")";

				Texture2D findTexture = EditorGUIUtility.FindTexture(i_Resource.name);
				string toCopy = string.Format(findTexture == null ? loadTextureText : findTextureText, i_Resource.name);
				GUIUtility.systemCopyBuffer = toCopy;
				Debug.Log("Copied to buffer : " + toCopy);
			}

			if ((i_Resource.width > ICON_ITEM_SIZE.x || i_Resource.height > ICON_ITEM_SIZE.y) && texture.Contains(Event.current.mousePosition))
			{
				Rect overview = new Rect(texture)
				{
					x = texture.x - (i_Resource.width / 2f),
					y = texture.y - (i_Resource.height / 2f),
					width = i_Resource.width,
					height = i_Resource.height,
				};
				GUI.Label(overview, i_Resource, CenteredStyle);
			}
		}

		private bool IsInScreen(float i_ItemYPos, Vector2 i_ScrollPos, Vector2 i_ItemSize)
		{
			return i_ItemYPos > i_ScrollPos.y - i_ItemSize.y || i_ItemYPos < i_ScrollPos.y + position.height;
		}
	}
}