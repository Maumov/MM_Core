using System;
using MM.Attribute;
using UnityEditor;
using UnityEngine;

namespace MM.Attribute
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label, new GUIContent("Unable to render EnumFlags on a non-Enum property"));
                return;
            }
            if (fieldInfo.FieldType.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
            {
                EditorGUI.LabelField(position, label, new GUIContent("EnumFlags on a non-Flags enum will fail while serializing"));
                return;
            }
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumDisplayNames);
        }
    }
}