using System.Collections.Generic;
using MM.Attribute;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MM.Attribute
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> tags = new List<string>(InternalEditorUtility.tags);
            EditorGUI.BeginChangeCheck();
            int n = EditorGUI.Popup(position, label.text, tags.IndexOf(property.stringValue), tags.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (n != -1)
                {
                    property.stringValue = tags[n];
                }
            }
        }
    }
}