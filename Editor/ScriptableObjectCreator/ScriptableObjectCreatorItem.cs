using System;
using UnityEngine;
using UnityEditor;

namespace MM.ScriptableObjectCreator
{
	public class ScriptableObjectCreatorItem
	{
		public string m_Name;
		public MonoScript m_Script = null;
		public ScriptableObjectCreatorItem m_Parent = null;

		public Action m_Clicked = null;
		public Action m_ClickedAsHeader = null;

		public static string m_LeftArrow = ((char)9664).ToString();
		public static string m_RightArrow = ((char)9654).ToString();
		public static GUIStyle m_DrawItem;
		public static GUIStyle m_DrawHeader;

		public bool DrawItem(bool i_Focus)
		{
			if (m_DrawItem == null)
			{
				m_DrawItem = new GUIStyle("PR Label");
				m_DrawItem.alignment = TextAnchor.MiddleLeft;
				m_DrawItem.padding.left -= 15;
				m_DrawItem.fixedHeight = EditorGUIUtility.singleLineHeight;
			}

			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
			bool focus = rect.Contains(Event.current.mousePosition) || i_Focus;

			// If the Item has a Script, show the Script preview, otherwise show a little triangle
			GUIContent content;
			if (m_Script != null)
			{
				content = new GUIContent(m_Name, AssetPreview.GetMiniThumbnail(m_Script));
			}
			else
			{
				content = new GUIContent(m_RightArrow + m_Name);
			}

			// Draw Item
			if (Event.current.type == EventType.Repaint)
			{
				m_DrawItem.Draw(rect, content, false, false, focus, focus);
			}

			// Item was selected
			if (Event.current.type == EventType.MouseDown && focus)
			{
				Event.current.Use();
				if (m_Clicked != null)
				{
					m_Clicked();
				}
			}

			return focus;
		}

		public bool DrawAsHeader()
		{
			if (m_DrawHeader == null)
			{
				m_DrawHeader = new GUIStyle(EditorStyles.boldLabel);
				m_DrawHeader.alignment = TextAnchor.MiddleCenter;

				// Create texture for Header Background
				Texture2D tex = new Texture2D(1, 1);
				tex.SetPixel(0, 0, new Color(0.7f, 0.7f, 0.7f));
				tex.Apply();

				m_DrawHeader.normal.background = tex;
			}

			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
			bool focus = rect.Contains(Event.current.mousePosition) && m_Parent != null;

			// If the Item has a Parent, show a little triangle, otherwise nothing
			GUIContent content;
			if (m_Parent != null)
			{
				content = new GUIContent(m_LeftArrow + m_Name);
			}
			else
			{
				content = new GUIContent(m_Name);
			}

			// Draw Item
			if (Event.current.type == EventType.Repaint)
			{
				m_DrawHeader.Draw(rect, content, false, false, focus, focus);
			}

			// Item was selected
			if (Event.current.type == EventType.MouseDown && focus)
			{
				Event.current.Use();
				if (m_ClickedAsHeader != null)
				{
					m_ClickedAsHeader();
				}
			}

			return focus;
		}
	}
}
