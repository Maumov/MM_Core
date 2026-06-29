using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
	public override void OnGUI(Rect i_Position, SerializedProperty i_Property, GUIContent i_Label)
	{
		GUIContent content = new GUIContent((attribute as LabelAttribute).Label);
		EditorGUI.PropertyField(i_Position, i_Property, content, true);
	}
}
