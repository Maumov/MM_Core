using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MM.BaseExplorer
{
	public class FilterToggles
	{
		public bool[] Active { get; private set; }
		private GUIContent[] m_Contents;
		private System.Action m_ChangeCallback;

		private string m_PrefsKey;

		public FilterToggles(GUIContent[] i_Contents, string i_PrefsKey, System.Action i_ChangeCallback)
		{
			Active = i_Contents.Select(x => true).ToArray();
			m_Contents = i_Contents;
			m_ChangeCallback = i_ChangeCallback;

			m_PrefsKey = i_PrefsKey;
		}

		public void UpdateQuantities(int[] i_Quantities)
		{
			if (i_Quantities.Length != Active.Length)
			{
				throw new System.Exception("Did not receive array of same length");
			}

			for (int i = 0; i < i_Quantities.Length; i++)
			{
				m_Contents[i].text = i_Quantities[i].ToString();
			}
		}

		public void Draw()
		{
			for (int i = 0; i < m_Contents.Length; i++)
			{
				if (GUILayout.Button(m_Contents[i], ExplorerStyles.GetButtonState(Active[i])))
				{
					Active[i] = !Active[i];
					EditorPrefs.SetBool(m_PrefsKey + "." + i, Active[i]);
					if (m_ChangeCallback != null)
					{
						m_ChangeCallback();
					}
				}
			}
		}

		public void LoadValues()
		{
			for (int i = 0; i < m_Contents.Length; i++)
			{
				Active[i] = EditorPrefs.GetBool(m_PrefsKey + "." + i, true);
			}
		}
	}
}
