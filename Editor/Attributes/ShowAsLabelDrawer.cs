using MM.EditorDrawer;
using MM.Extensions;
using UnityEngine;
using UnityEditor;

namespace MM.Attributes
{
	[CustomPropertyDrawer(typeof(ShowAsLabelAttribute))]
	public class ShowAsLabelDrawer : PropertyDrawer
	{
		private Texture2D m_InfoTexture = EditorGUIUtility.FindTexture("console.infoicon");
		private Texture2D m_CopyTexture = EditorGUIUtility.FindTexture("SceneLoadIn");

		public override void OnGUI(Rect i_Position, SerializedProperty i_Property, GUIContent i_Label)
		{
			i_Position = EditorGUI.PrefixLabel(i_Position, i_Label);

			string valueString = i_Property.ValueToString();
			if (string.IsNullOrEmpty(valueString))
			{
				// This needs to be here otherwise the PrefixLabel tends to disapear without it...
				GUI.Button(i_Position, "", EditorStyles.label);

				GUIContent content = new GUIContent((attribute as ShowAsLabelAttribute).MissingValueText, m_InfoTexture);
				GUI.Label(i_Position, content);
			}
			else
			{
				GUISplit.SplitResult split = GUISplit.SplitHorizontal(i_Position, -1f, 25f);
				GUI.Label(split.Next(), valueString);
				if (LargeContentButton(split.Next(), new GUIContent(m_CopyTexture)))
				{
					GUIUtility.systemCopyBuffer = valueString;
					Debug.Log("\"" + valueString + "\" was copied to clipboard");
				}
			}
		}

		private bool LargeContentButton(Rect i_Position, GUIContent i_Content)
		{
			if (GUI.Button(i_Position, ""))
			{
				return true;
			}

			i_Position.height += 2f;
			GUI.Label(i_Position, m_CopyTexture);

			return false;
		}

		public override float GetPropertyHeight(SerializedProperty i_Property, GUIContent i_Label)
		{
			string valueString = i_Property.ValueToString();
			if (string.IsNullOrEmpty(valueString))
			{
				return 22f;
			}
			else
			{
				return base.GetPropertyHeight(i_Property, i_Label);
			}
		}
	}
}
