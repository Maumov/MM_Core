using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;
using System.Linq;
using System.Collections.Generic;

using MM.Attribute;

namespace MM.Attribute
{
	[CustomPropertyDrawer(typeof(ControllerParameterAttribute))]
	public class ControllerParameterDrawer : PropertyDrawer
	{
		private ControllerParameterAttribute m_Attribute;
		private RuntimeAnimatorController m_Controller;
		private string m_ParameterChosen = "";
		private string m_PropertyPathChosen = "";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String && property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.HelpBox(position, "Field must be a String or an Int", MessageType.Error);
				return;
			}

			Rect rect = new Rect(position);
			position = EditorGUI.PrefixLabel(position, label);

			// This empty button solves a bug where the PrefixLabel will be empty if there isn't a button right after
			rect.width = rect.width - position.width;
			GUI.Button(rect, "", EditorStyles.label);

			m_Attribute = attribute as ControllerParameterAttribute;

			AnimatorController c = AssetDatabase.LoadAssetAtPath<AnimatorController>(m_Attribute.m_ControllerPath);

			if (c == null)
			{
				EditorGUI.HelpBox(position, "There is no Controller at path supplied", MessageType.Error);
				return;
			}
			
			List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>();
			parameters.AddRange(c.parameters);

			int currentValue = property.propertyType == SerializedPropertyType.String
				? Animator.StringToHash(property.stringValue)
				: property.intValue;
			string valueName = "";
			if (currentValue != 0)
			{
				AnimatorControllerParameter parameter = parameters.Count == 0 ? null : parameters.FirstOrDefault(x => x.nameHash == currentValue);
				valueName = parameter != null ? parameter.name : "";
			}

			if (GUI.Button(position, valueName, EditorStyles.popup))
			{
				GenericMenu menu = new GenericMenu();
				foreach (AnimatorControllerParameter parameter in parameters)
				{
					MenuCallbackInfo info = new MenuCallbackInfo(parameter.name, property.propertyPath);
					bool isOn = parameter.nameHash == currentValue;
					menu.AddItem(new GUIContent(parameter.type + "/" + parameter.name), isOn, MenuCallback, info);
				}

				menu.DropDown(position);
			}

			if (m_ParameterChosen != "" && property.propertyPath == m_PropertyPathChosen)
			{
				if (property.propertyType == SerializedPropertyType.String)
				{
					property.stringValue = m_ParameterChosen;
				}
				else
				{
					property.intValue = Animator.StringToHash(m_ParameterChosen);
				}

				m_ParameterChosen = "";
				m_PropertyPathChosen = "";
			}
		}

		private void MenuCallback(object i_Info)
		{
			MenuCallbackInfo info = (MenuCallbackInfo)i_Info;
			m_ParameterChosen = info.m_Parameter;
			m_PropertyPathChosen = info.m_PropertyPath;
		}

		public class MenuCallbackInfo
		{
			public string m_Parameter;
			public string m_PropertyPath;

			public MenuCallbackInfo(string i_Parameter, string i_PropertyPath)
			{
				m_Parameter = i_Parameter;
				m_PropertyPath = i_PropertyPath;
			}
		}
	}
}
