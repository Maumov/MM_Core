using UnityEngine;

namespace MM.Attributes
{
	public class ShowAsLabelAttribute : PropertyAttribute
	{
		private const string DEFAULT_MISSING_TEXT = "Field contains no information.";

		private string m_MissingValueText;

		public string MissingValueText
		{
			get { return m_MissingValueText; }
		}

		public ShowAsLabelAttribute(string i_MissingValueText = DEFAULT_MISSING_TEXT)
		{
			m_MissingValueText = i_MissingValueText;
		}
	}
}
