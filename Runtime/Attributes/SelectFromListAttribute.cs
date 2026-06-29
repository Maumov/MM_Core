using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFromListAttribute : PropertyAttribute
{
	private string m_ListPath;

	public string ListPath
	{
		get { return m_ListPath; }
	}

	public SelectFromListAttribute(string i_ListPath)
	{
		m_ListPath = i_ListPath;
	}
}
