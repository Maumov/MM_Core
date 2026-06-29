using UnityEngine;
using UnityEditor;

namespace MM.EditorDrawer
{
	public class InputBox : EditorWindow
	{
		private const string INPUT_CONTROL = "InputText";

		public delegate void InputBoxResult(bool i_Accepted, string i_Result, string i_UserData);

		private string m_Input = "";
		private string m_Text = "";
		private string m_UserData = "";
		private InputBoxResult m_Callback;

		public static void ShowInputDialog(string i_Title, string i_Text, InputBoxResult i_Callback, string i_UserData = "")
		{
			InputBox box = CreateInstance<InputBox>();
			box.Show(true);

			box.minSize = new Vector2(400f, 200f);
			box.titleContent = new GUIContent(i_Title);
			box.m_Text = i_Text;
			box.m_UserData = i_UserData;
			box.m_Callback = i_Callback;
		}

		private void OnGUI()
		{
			if (GUI.GetNameOfFocusedControl() != INPUT_CONTROL)
			{
				GUI.FocusControl(INPUT_CONTROL);
			}

			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{
					Event.current.Use();
					SendResult();
				}
				else if (Event.current.keyCode == KeyCode.Escape)
				{
					Event.current.Use();
					SendCancelled();
				}
			}

			GUILayout.Label(m_Text);

			GUI.SetNextControlName(INPUT_CONTROL);
			m_Input = EditorGUILayout.TextField(m_Input);

			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("OK"))
				{
					SendResult();
				}

				if (GUILayout.Button("Cancel"))
				{
					SendCancelled();
				}
			}
			GUILayout.EndHorizontal();
		}

		private void SendResult()
		{
			Close();

			if (m_Callback != null)
			{
				m_Callback(true, m_Input, m_UserData);
			}
		}

		private void SendCancelled()
		{
			Close();

			if (m_Callback != null)
			{
				m_Callback(false, "", m_UserData);
			}
		}
	}
}