using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MM.EditorDrawer;

namespace MM.BaseExplorer
{
	public abstract class BaseExplorerItem
	{
		private static readonly Color m_ActiveSelectionColor = new Color(0.24f, 0.37f, 0.59f);
		private static readonly Color m_InactiveSelectionColor = new Color(0.35f, 0.35f, 0.35f, 0.35f);

		public string Name { get; protected set; }
		public GUIContent Content { get; protected set; }

		public BaseExplorerItem(string i_Name)
		{
			Name = i_Name;
			Content = new GUIContent(i_Name);
		}
		
		public virtual void DrawInList(Rect i_Position, bool i_Selected, bool i_WindowFocused)
		{
			if (i_Selected)
			{
				using (GUIScope.Color(i_WindowFocused ? m_ActiveSelectionColor : m_InactiveSelectionColor))
				{
					EditorGUI.DrawTextureAlpha(i_Position, EditorGUIUtility.whiteTexture);
				}

				EditorGUI.LabelField(i_Position, Content, ExplorerStyles.BoldLabel);
			}
			else
			{
				EditorGUI.LabelField(i_Position, Content, ExplorerStyles.Label);
			}
		}
		
		public virtual void DrawInspectorHeader(IBaseExplorerWindow i_Window)
		{
			GUILayout.Label(Name);
			GUILayout.FlexibleSpace();
		}
		
		public abstract void DrawInspectorContents(IBaseExplorerWindow i_Window);
	}
}
