using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

namespace MM.CollectionEnum
{
	[Serializable]
	public abstract class CollectionEnum<T> : ScriptableObject where T : CollectionValue
	{
		[SerializeField]
		private static Dictionary<string, T> m_ValueReferences = new Dictionary<string, T>();

		protected CollectionEnum()
		{
			foreach (FieldInfo field in GetFields())
			{
				T element = (T)field.GetValue(null);
				if (m_ValueReferences.ContainsKey(element.ToString()))
				{
					continue;
				}

				m_ValueReferences.Add(element.ToString(), element);
			}
		}
		public T GetValue(string i_Value)
		{
			T value;
			m_ValueReferences.TryGetValue(i_Value, out value);
			return value;
		}
		public List<T> GetAllValues()
		{
			return m_ValueReferences.Values.ToList();
		}
		protected List<FieldInfo> GetFields()
		{
			return this.GetType()
				.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
				.ToList();
		}

		// public abstract void LoadValues();
	}
}
