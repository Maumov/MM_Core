using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MM.EditorDrawer
{
	public class SplitScreenDrawer<T>
	{
		public struct Headers
		{
			public int m_Position;
			public string m_Label;
		}

		private EditorWindow m_Parent;

		private Vector2 m_LeftScroll;
		private Vector2 m_RightScroll;
		private GUILayoutOption m_ExpandWidth = GUILayout.ExpandWidth(true);
		private float m_LeftPanelWidth = 250f;
		private GUIStyle m_TitleToolBar;
		private List<int> m_Selected = new List<int>();

		public SplitScreenDrawer(EditorWindow i_ParentWindow)
		{
			m_TitleToolBar = new GUIStyle(EditorStyles.toolbar)
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter
			};
			m_Parent = i_ParentWindow;
			m_GetSelectionLabel = obj => new GUIContent(obj.ToString());
			m_DrawSelectedInfos = obj => { DrawAssetLabel(m_GetSelectionLabel(obj), false, EditorWindow.focusedWindow == m_Parent); };
		}

		public Func<T, GUIContent> m_GetSelectionLabel;
		public Action<T> m_DrawSelectedInfos;
		public Action<T> m_DoubleClickSelection;
		public Action m_DrawLeftPanelHeader;
		public Action m_DrawRightPanelHeader;

		public void Draw(List<T> i_Selection, List<Headers> i_Headers)
		{
			using (GUIScope.Horizontal())
			{
				using (GUIScope.ScrollView(ref m_LeftScroll, GUILayout.Width(m_LeftPanelWidth), m_ExpandWidth))
				{
					if (m_DrawLeftPanelHeader != null) m_DrawLeftPanelHeader();
					for (int index = 0; index < i_Selection.Count; index++)
					{
						var indexheader = i_Headers.FindIndex(a => a.m_Position == index);
						if (indexheader != -1)
						{
							EditorGUILayout.LabelField(i_Headers[indexheader].m_Label, m_TitleToolBar);
						}

						T obj = i_Selection[index];
						Rect rect = DrawAssetLabel(m_GetSelectionLabel(obj), m_Selected.Contains(index),
							EditorWindow.focusedWindow == m_Parent);
						HandleSelectableInputs(rect, index, obj);
					}
				}

				GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(2f));
				using (GUIScope.ScrollView(ref m_RightScroll, m_ExpandWidth))
				{
					if (m_DrawRightPanelHeader != null) m_DrawRightPanelHeader();
					foreach (int selected in m_Selected)
					{
						m_DrawSelectedInfos(i_Selection[selected]);
					}
				}
			}
		}

		private void HandleSelectableInputs(Rect i_Rect, int i_Index, T i_Obj)
		{
			var e = Event.current;
			if (i_Rect.Contains(e.mousePosition))
			{
				if (m_DoubleClickSelection != null && (e.type == EventType.MouseDown) && (e.clickCount == 2))
				{
					m_DoubleClickSelection(i_Obj);
					e.Use();
				}
				else if ((e.type == EventType.MouseUp) && (e.clickCount == 1))
				{
					if (!e.control && !e.shift)
					{
						m_Selected.Clear();
					}

					int pos = m_Selected.IndexOf(i_Index);
					if (pos != -1)
					{
						m_Selected.RemoveAt(pos);
					}
					else
					{
						if (e.shift && (m_Selected.Count > 0))
						{
							if (i_Index > m_Selected[m_Selected.Count - 1])
							{
								for (int i = m_Selected[m_Selected.Count - 1] + 1; i <= i_Index; i++)
								{
									m_Selected.Add(i);
								}

								m_Selected.Sort();
							}
							else if (i_Index < m_Selected[0])
							{
								for (int i = i_Index; i < m_Selected[0]; i++)
								{
									m_Selected.Add(i);
								}

								m_Selected.Sort();
							}
						}
						else
						{
							m_Selected.Add(i_Index);
							m_Selected.Sort();
						}
					}

					e.Use();
				}
			}
		}

		private static readonly Color m_ActiveSelectionColor = new Color(.24f, .37f, .59f);
		private static readonly Color m_InactiveSelectionColor = new Color(.35f, .35f, .35f, .35f);
		private static Rect DrawAssetLabel(GUIContent i_Label, bool i_Selected, bool i_Focused)
		{
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label, GUILayout.ExpandWidth(true));
			var style = EditorStyles.label;
			if (i_Selected)
			{
				using (GUIScope.Color(i_Focused ? m_ActiveSelectionColor : m_InactiveSelectionColor))
				{
					EditorGUI.DrawTextureAlpha(rect, EditorGUIUtility.whiteTexture);
				}

				style = EditorStyles.boldLabel;
			}

			EditorGUI.LabelField(rect, i_Label, style);
			return rect;
		}
	}
}