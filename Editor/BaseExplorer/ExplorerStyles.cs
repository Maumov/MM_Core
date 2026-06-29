using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MM.BaseExplorer
{
	public static class ExplorerStyles
	{
		public static GUIStyle Label
		{
			get { return EditorStyles.label; }
		}

		public static GUIStyle BoldLabel
		{
			get { return EditorStyles.boldLabel; }
		}

		private static GUIStyle m_Multiline = null;
		public static GUIStyle Multiline
		{
			get
			{
				if (m_Multiline == null)
				{
					m_Multiline = new GUIStyle(GUI.skin.label)
					{
						wordWrap = true,
						richText = true,
					};
				}

				return m_Multiline;
			}
		}

		public static GUIStyle Toolbar
		{
			get { return EditorStyles.toolbar; }
		}

		public static GUIStyle DropDown
		{
			get { return EditorStyles.toolbarDropDown; }
		}

		private static GUIStyle m_BtnNormal = null;
		public static GUIStyle BtnNormal
		{
			get { return m_BtnNormal ?? (m_BtnNormal = "toolbarbutton"); }
		}
		
		private static GUIStyle m_BtnActive = null;
		public static GUIStyle BtnActive
		{
			get { return m_BtnActive ?? (m_BtnActive = "TE toolbarbutton"); }
		}

		private static GUIStyle m_HeaderStyle = null;
		public static GUIStyle HeaderStyle
		{
			get { return m_HeaderStyle ?? (m_HeaderStyle = new GUIStyle("PreLabel") { alignment = TextAnchor.MiddleLeft }); }
		}

		public static GUIStyle RoundBox
		{
			get { return "HelpBox"; }
		}

		public static GUIStyle SearchField
		{
			get { return "ToolbarSeachTextField"; }
		}

		public static GUIStyle SearchFieldEnd
		{
			get { return "ToolbarSeachCancelButtonEmpty"; }
		}

		public static GUIStyle SearchFieldCancel
		{
			get { return "ToolbarSeachCancelButton"; }
		}

		public static GUIStyle GetButtonState(bool i_Pressed)
		{
			return i_Pressed ? BtnActive : BtnNormal;
		}
	}
}
