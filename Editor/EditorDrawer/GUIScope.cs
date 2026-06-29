using UnityEditor;
using UnityEngine;

namespace MM.EditorDrawer
{
	public static class GUIScope
	{
		#region Grid

		public static GridScope Grid(int i_Rows, Vector2 i_ItemSize)
		{
			return new GridScope(i_Rows, i_ItemSize);
		}

		public class GridScope : GUI.Scope
		{
			private Vector2 m_Size;
			private GUILayoutOption[] m_GetRectOptions;
			private readonly int m_MaxRows;
			private int m_CurrentRow;

			public GridScope(int i_Rows, Vector2 i_ItemSize)
			{
				m_Size = i_ItemSize;
				m_MaxRows = i_Rows;

				m_GetRectOptions = new[]
				{
					GUILayout.MaxWidth(m_Size.x),
					GUILayout.MaxHeight(m_Size.y)
				};

				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
			}

			public Rect Next()
			{
				if (m_CurrentRow >= m_MaxRows)
				{
					m_CurrentRow = 0;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}

				m_CurrentRow++;

				return GUILayoutUtility.GetRect(m_Size.x, m_Size.y, m_GetRectOptions);
			}

			protected override void CloseScope()
			{
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
		}

		#endregion

		#region Horizontal

		public static GUI.Scope Horizontal(params GUILayoutOption[] i_Options)
		{
			return new EditorGUILayout.HorizontalScope(i_Options);
		}

		public static GUI.Scope Horizontal(GUIStyle i_Style, params GUILayoutOption[] i_Options)
		{
			return new EditorGUILayout.HorizontalScope(i_Style, i_Options);
		}

		#endregion

		#region Vertical

		public static GUI.Scope Vertical(params GUILayoutOption[] i_Options)
		{
			return new EditorGUILayout.VerticalScope(i_Options);
		}

		public static GUI.Scope Vertical(GUIStyle i_Style, params GUILayoutOption[] i_Options)
		{
			return new EditorGUILayout.VerticalScope(i_Style, i_Options);
		}

		#endregion

		#region ScrollView

		public static GUI.Scope ScrollView(ref Vector2 i_ScrollPostion, params GUILayoutOption[] i_Options)
		{
			return new ScrollViewScope(ref i_ScrollPostion, i_Options);
		}

		private class ScrollViewScope : GUI.Scope
		{
			public ScrollViewScope(ref Vector2 i_ScrollPostion, params GUILayoutOption[] i_Options)
			{
				i_ScrollPostion = EditorGUILayout.BeginScrollView(i_ScrollPostion, i_Options);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndScrollView();
			}
		}

		#endregion

		#region Indented

		public static GUI.Scope Indented()
		{
			return new IndentedScope();
		}

		private class IndentedScope : GUI.Scope
		{
			public IndentedScope()
			{
				EditorGUI.indentLevel++;
			}

			protected override void CloseScope()
			{
				EditorGUI.indentLevel--;
			}
		}

		public static GUI.Scope RemoveIndent()
		{
			return new RemoveIndentScope();
		}

		private class RemoveIndentScope : GUI.Scope
		{
			private readonly int m_PrevIndent;

			public RemoveIndentScope()
			{
				m_PrevIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
			}

			protected override void CloseScope()
			{
				EditorGUI.indentLevel = m_PrevIndent;
			}
		}

		#endregion

		#region Coloration

		public static GUI.Scope BackgroundColor(Color i_BackgroundColor)
		{
			return new BackgroundColorScope(i_BackgroundColor);
		}

		private class BackgroundColorScope : GUI.Scope
		{
			private readonly Color m_OldColor;

			public BackgroundColorScope(Color i_BackgroundColor)
			{
				m_OldColor = GUI.backgroundColor;
				GUI.backgroundColor = i_BackgroundColor;
			}

			protected override void CloseScope()
			{
				GUI.backgroundColor = m_OldColor;
			}
		}

		public static GUI.Scope Color(Color i_Color)
		{
			return new ColorScope(i_Color);
		}

		private class ColorScope : GUI.Scope
		{
			private readonly Color m_OldColor;

			public ColorScope(Color i_Color)
			{
				m_OldColor = GUI.color;
				GUI.color = i_Color;
			}

			protected override void CloseScope()
			{
				GUI.color = m_OldColor;
			}
		}

		public static GUI.Scope ContentColor(Color i_ContentColor)
		{
			return new ContentColorScope(i_ContentColor);
		}

		private class ContentColorScope : GUI.Scope
		{
			private readonly Color m_OldColor;

			public ContentColorScope(Color i_ContentColor)
			{
				m_OldColor = GUI.contentColor;
				GUI.contentColor = i_ContentColor;
			}

			protected override void CloseScope()
			{
				GUI.contentColor = m_OldColor;
			}
		}

		#endregion

		#region Disabled GUI

		public static GUI.Scope Enabled(bool i_Enabled)
		{
			return new GuiEnabledScope(i_Enabled);
		}

		private class GuiEnabledScope : GUI.Scope
		{
			private readonly bool m_OldValue;

			public GuiEnabledScope(bool i_Enabled)
			{
				m_OldValue = GUI.enabled;
				GUI.enabled = i_Enabled;
			}

			protected override void CloseScope()
			{
				GUI.enabled = m_OldValue;
			}
		}

		#endregion
	}
}