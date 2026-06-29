using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using MM.Attribute;

namespace MM.Attribute
{
	[CustomPropertyDrawer(typeof(EnumAttribute))]
	public class EnumDrawer : PropertyDrawer
	{
		private EnumAttribute m_Attribute = null;
		private List<string> m_RealNames;
		private List<string> m_DisplayNames;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (m_Attribute == null)
			{
				m_Attribute = attribute as EnumAttribute;
			}

			if (m_RealNames == null)
			{
				m_RealNames = new List<string>(System.Enum.GetNames(m_Attribute.m_Enum));
				m_DisplayNames = new List<string>(m_RealNames);
				if (m_Attribute.m_DisplayFilter || !string.IsNullOrEmpty(m_Attribute.m_Remove))
				{
					for (int i = 0; i < m_DisplayNames.Count; i++)
					{
						if (!string.IsNullOrEmpty(m_Attribute.m_Remove))
						{
							m_DisplayNames[i] = m_DisplayNames[i].Replace(m_Attribute.m_Remove, "");
						}

						if (m_Attribute.m_DisplayFilter)
						{
							m_DisplayNames[i] = m_DisplayNames[i].Replace("_", "/");
						}
					}
				}
			}

			int index = EditorGUI.Popup(position, label.text, m_RealNames.IndexOf(property.stringValue), m_DisplayNames.ToArray());
			if (index != -1)
			{
				property.stringValue = m_RealNames[index];
			}
		}
	}
}
