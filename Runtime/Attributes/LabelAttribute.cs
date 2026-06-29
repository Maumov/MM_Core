using UnityEngine;

public class LabelAttribute : PropertyAttribute
{
	private string m_Label;

	public string Label
	{
		get { return m_Label; }
	}

	public LabelAttribute(string i_NewLabel)
	{
		m_Label = i_NewLabel;
	}
}
