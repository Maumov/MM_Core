using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MM.Attribute;
using UnityEditor;

namespace MM.Attribute
{
    [CustomPropertyDrawer(typeof(SortedEnumAttribute))]
    public class SortedEnumDrawer : PropertyDrawer
    {
        private SortedEnumAttribute m_Attribute;
        private Dictionary<string, int> m_NamesLink;
        private List<string> m_Names;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label, new GUIContent("Unable to render EnumFlags on a non-Enum property"));
                return;
            }

            if (m_Attribute == null)
            {
                m_Attribute = attribute as SortedEnumAttribute;
            }

            if (m_NamesLink == null)
            {
                m_Names = new List<string>(property.enumDisplayNames);
                m_NamesLink = new Dictionary<string, int>(m_Names.Count);

                if (m_Attribute.m_DisplayFilter)
                {
                    m_Names = m_Names.Select(a => a.Replace("_", "/")).ToList();
                }

                for (int i = 0; i < m_Names.Count; i++)
                {
                    m_NamesLink.Add(m_Names[i], i);
                }

                m_Names.Sort();
            }

            string name = property.enumDisplayNames[property.enumValueIndex];
            if (m_Attribute.m_DisplayFilter)
            {
                name = name.Replace("_", "/");
            }
            int convertedIndex = m_Names.IndexOf(name);

            EditorGUI.BeginChangeCheck();
            int index = EditorGUI.Popup(position, label, convertedIndex, m_Names.Select(s => new GUIContent(s)).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (index != -1)
                {
                    property.enumValueIndex = m_NamesLink[m_Names[index]];
                }
            }
        }
    }
}
