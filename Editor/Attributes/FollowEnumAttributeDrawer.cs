using MM.Attribute;
using UnityEditor;
using UnityEngine;

namespace MM.Attribute
{
    [CustomPropertyDrawer(typeof(FollowEnumAttribute))]
    public class FollowEnumAttributeDrawer : PropertyDrawer
    {
        private FollowEnumAttribute m_Attribute = null;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_Attribute == null)
            {
                m_Attribute = attribute as FollowEnumAttribute;
            }

            string path = property.propertyPath;
            path = path.Substring(0, path.LastIndexOf('.'));
            path = path.Substring(0, path.LastIndexOf('.'));
            SerializedProperty arrayProperty = property.serializedObject.FindProperty(path);

            GUIContent newLabel = new GUIContent(label);
            if (arrayProperty != null && arrayProperty.isArray)
            {
                int[] values = System.Enum.GetValues(m_Attribute.m_Enum) as int[];
                int largestValue = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] > largestValue)
                    {
                        largestValue = values[i];
                    }
                }

                largestValue += m_Attribute.m_Add;
                if (largestValue > 0)
                {
                    while (arrayProperty.arraySize <= largestValue)
                    {
                        arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize - 1);
                    }

                    while (arrayProperty.arraySize > largestValue + 1 && arrayProperty.arraySize > 1)
                    {
                        arrayProperty.DeleteArrayElementAtIndex(arrayProperty.arraySize - 1);
                    }
                }

                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    if (arrayProperty.GetArrayElementAtIndex(i).propertyPath == property.propertyPath)
                    {
                        if (System.Enum.IsDefined(m_Attribute.m_Enum, i))
                        {
                            newLabel = new GUIContent(System.Enum.GetName(m_Attribute.m_Enum, i));
                        }
                    }
                }
            }

            EditorGUI.PropertyField(position, property, newLabel);
        }
    }
}