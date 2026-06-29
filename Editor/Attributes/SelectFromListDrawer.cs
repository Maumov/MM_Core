using System.Linq;
using System.Collections.Generic;
using MM.Extensions;
using UnityEngine;
using UnityEditor;
using MM.EditorDrawer;

[CustomPropertyDrawer(typeof(SelectFromListAttribute))]
public class SelectFromListDrawer : PropertyDrawer
{
	public override void OnGUI(Rect i_Position, SerializedProperty i_Property, GUIContent i_Label)
	{
		i_Position = EditorGUI.PrefixLabel(i_Position, i_Label);

		string listPath = (attribute as SelectFromListAttribute).ListPath;
		SerializedProperty list = i_Property.serializedObject.FindProperty(listPath);

		if (list == null)
		{
			EditorGUI.HelpBox(i_Position, "List was not found", MessageType.Error);
			return;
		}
		else if (list.arraySize == 0)
		{
			EditorGUI.HelpBox(i_Position, "List is empty", MessageType.Error);
			return;
		}
		else if (list.GetArrayElementAtIndex(0).propertyType != i_Property.propertyType)
		{
			EditorGUI.HelpBox(i_Position, "List is not of same type as field", MessageType.Error);
			return;
		}

		List<SerializedProperty> elements = new List<SerializedProperty>();
		for (int i = 0; i < list.arraySize; i++)
		{
			elements.Add(list.GetArrayElementAtIndex(i));
		}

		using (GUIScope.RemoveIndent())
		{
			List<string> names = elements.Select(x => x.ValueToString()).ToList();

			EditorGUI.BeginChangeCheck();
			int index = EditorGUI.Popup(i_Position, names.IndexOf(i_Property.ValueToString()), names.ToArray());

			if (EditorGUI.EndChangeCheck() || index == -1)
			{
				elements[index == -1 ? 0 : index].CopyValue(i_Property);
			}
		}
	}
}
