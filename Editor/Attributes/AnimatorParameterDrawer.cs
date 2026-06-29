using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;
using System.Linq;
using System.Collections.Generic;

using MM.Attribute;

namespace MM.Attribute
{
	[CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
	public class AnimatorParameterDrawer : PropertyDrawer
	{
		private bool m_UseAnimator;	// Otherwise yse StateMachineBehaviour
		private AnimatorParameterAttribute m_Attribute;
		private Animator m_Animator;
		private StateMachineBehaviour m_Behaviour;
		private string m_OwnPath = "";
		private string m_ParameterChosen = "";
		private string m_PathChosen = "";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String && property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.HelpBox(position, "Must be used with a String or an Int", MessageType.Error);
				return;
			}

			Rect rect = new Rect(position);
			position = EditorGUI.PrefixLabel(position, label);

			// This empty button solves a bug where the PrefixLabel will show nothing if there isn't a button right after
			rect.width = rect.width - position.width;
			GUI.Button(rect, "", EditorStyles.label);

			SetupDrawer(position, property);
			ShowErrors(position);

			List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>();

			if (m_UseAnimator && m_Animator != null)
			{
				parameters.AddRange(m_Animator.parameters);
			}
			else if (!m_UseAnimator && m_Behaviour != null)
			{
				StateMachineBehaviourContext[] context = AnimatorController.FindStateMachineBehaviourContext(m_Behaviour);
				parameters.AddRange(context[0].animatorController.parameters);
			}

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
					MenuCallbackInfo info = new MenuCallbackInfo(parameter.name, m_OwnPath);
					bool isOn = parameter.nameHash == currentValue;
					menu.AddItem(new GUIContent(parameter.type + "/" + parameter.name), isOn, MenuCallback, info);
				}

				menu.DropDown(position);
			}

			if (m_ParameterChosen != "" && m_OwnPath == m_PathChosen)
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
			}
		}

		private void SetupDrawer(Rect i_Position, SerializedProperty i_Property)
		{
			if (m_Attribute == null)
			{
				m_Attribute = attribute as AnimatorParameterAttribute;
			}

			if (m_OwnPath != i_Property.propertyPath)
			{
				m_Animator = null;
				m_Behaviour = null;
				m_OwnPath = i_Property.propertyPath;
			}

			if (m_Animator == null)
			{
				if (string.IsNullOrEmpty(m_Attribute.m_AnimatorObject))
				{
					if (i_Property.serializedObject.targetObject is Component)
					{
						m_Animator = ((Component)i_Property.serializedObject.targetObject).GetComponent<Animator>();
					}
				}
				else
				{
					SerializedProperty animProperty = FindPropertyFromPath(i_Property, m_Attribute.m_AnimatorObject);
					if (animProperty != null)
					{
						m_Animator = animProperty.objectReferenceValue as Animator;
					}
					else
					{
						EditorGUI.HelpBox(i_Position, "Animator supplied is nonexistant!", MessageType.Error);
						return;
					}
				}

				if (m_Animator == null)
				{
					m_UseAnimator = false;
				}
			}

			if (m_Behaviour == null)
			{
				if (i_Property.serializedObject.targetObject is StateMachineBehaviour)
				{
					m_Behaviour = (StateMachineBehaviour)i_Property.serializedObject.targetObject;
				}

				if (m_Behaviour == null)
				{
					m_UseAnimator = true;
				}
			}
		}

		private void ShowErrors(Rect position)
		{
			if (m_UseAnimator)
			{
				if (m_Animator == null)
				{
					EditorGUI.HelpBox(position, "Missing Animator!", MessageType.Error);
					return;
				}
				else if (m_Animator.runtimeAnimatorController == null)
				{
					EditorGUI.HelpBox(position, "Missing Controller!", MessageType.Error);
					return;
				}
				else if (!m_Animator.isInitialized)
				{
					EditorGUI.HelpBox(position, "Prefab must be in the Scene to be modified!", MessageType.Warning);
					return;
				}
			}
		}

		private SerializedProperty FindPropertyFromPath(SerializedProperty i_Property, string i_AnimatorPath)
		{
			string path = i_Property.propertyPath;
			if (path.EndsWith("]"))
			{
				path = path.Substring(0, path.LastIndexOf("."));
				path = path.Substring(0, path.LastIndexOf("."));
				int index = path.LastIndexOf(".");
				path = path.Substring(0, index < 0 ? 0 : index);
			}
			else
			{
				int index = path.LastIndexOf(".");
				path = path.Substring(0, index < 0 ? 0 : index);
			}

			if (!string.IsNullOrEmpty(path))
			{
				path += ".";
			}

			return i_Property.serializedObject.FindProperty(path + i_AnimatorPath);
		}

		private void MenuCallback(object i_Info)
		{
			MenuCallbackInfo info = (MenuCallbackInfo)i_Info;
			m_ParameterChosen = info.m_Parameter;
			m_PathChosen = info.m_Path;
		}

		public class MenuCallbackInfo
		{
			public string m_Path;
			public string m_Parameter;

			public MenuCallbackInfo(string i_Parameter, string i_Path)
			{
				m_Parameter = i_Parameter;
				m_Path = i_Path;
			}
		}
	}
}
