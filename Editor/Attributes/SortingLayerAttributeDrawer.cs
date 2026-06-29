using System.Linq;
using MM.Attribute;
using UnityEditor;
using UnityEngine;

namespace MM.Attribute
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int index = -1;
            for (int i = 0; i < SortingLayer.layers.Length; i++)
            {
                if (SortingLayer.layers[i].name == property.stringValue)
                {
                    index = i;
                    break;
                }
            }
            EditorGUI.BeginChangeCheck();
            int n = EditorGUI.Popup(position, label, index, SortingLayer.layers.Select(a => new GUIContent(a.name)).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (n != -1)
                {
                    property.stringValue = SortingLayer.layers[n].name;
                }
                else
                {
                    property.stringValue = string.Empty;
                }
            }
        }
    }
}