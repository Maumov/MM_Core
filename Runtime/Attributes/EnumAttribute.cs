using UnityEngine;
using System.Collections.Generic;

/*
 * This Attribute facilitates enum assignation in the Inspector.
 * When used with a string, any item from the enum can be chosen in a drop down menu.
 * 
 * HOW TO USE : 
 * // The string will now have the value of the selected item
 * [Enum(typeof(StateEnum))]
 * public string m_CurrentState;
 * 
 * This value can be used as a string or it can be cast back into the enum : (StateEnum)m_CurrentState.
 * Be cautious though, an Enum cast can be quite heavy.
 */

namespace MM.Attribute
{
	public class EnumAttribute : PropertyAttribute
	{
		public System.Type m_Enum;
		public bool m_DisplayFilter;
		public string m_Remove;
		public EnumAttribute(System.Type i_Enum, bool i_DisplayFilter = false, string i_Remove = "")
		{
			m_Enum = i_Enum;
			m_DisplayFilter = i_DisplayFilter;
			m_Remove = i_Remove;
		}
	}
}
