using System;

namespace MM.CollectionEnum
{
	[Serializable]
	public abstract class CollectionValue
	{
		public const string NULL_VALUE = "none";

		protected readonly string m_Value = null;

		protected CollectionValue(string i_Value)
		{
			m_Value = i_Value;
		}

		public override string ToString()
		{
			return m_Value;
		}

		#region COMPARISON OPERATORS
		public static bool operator ==(CollectionValue i_V1, CollectionValue i_V2)
		{
			bool v1Null = ReferenceEquals(i_V1, null) || i_V1.m_Value == NULL_VALUE;
			bool v2Null = ReferenceEquals(i_V2, null) || i_V2.m_Value == NULL_VALUE;

			return CompareValues(v1Null ? null : i_V1.m_Value, v2Null ? null : i_V2.m_Value);
		}

		public static bool operator !=(CollectionValue i_V1, CollectionValue i_V2)
		{
			return !(i_V1 == i_V2);
		}

		public static bool operator ==(CollectionValue i_V1, string i_V2)
		{
			bool v1Null = ReferenceEquals(i_V1, null) || i_V1.m_Value == NULL_VALUE;
			bool v2Null = ReferenceEquals(i_V2, null) || i_V2 == NULL_VALUE;

			return CompareValues(v1Null ? null : i_V1.m_Value, v2Null ? null : i_V2);
		}

		public static bool operator !=(CollectionValue i_V1, string i_V2)
		{
			return !(i_V1 == i_V2);
		}

		public static bool operator ==(string i_V1, CollectionValue i_V2)
		{
			bool v1Null = ReferenceEquals(i_V1, null) || i_V1 == NULL_VALUE;
			bool v2Null = ReferenceEquals(i_V2, null) || i_V2.m_Value == NULL_VALUE;

			return CompareValues(v1Null ? null : i_V1, v2Null ? null : i_V2.m_Value);
		}

		public static bool operator !=(string i_V1, CollectionValue i_V2)
		{
			return !(i_V1 == i_V2);
		}

		private static bool CompareValues(string i_V1, string i_V2)
		{
			return i_V1 == i_V2;
		}

		public override bool Equals(object i_Obj)
		{
			if (i_Obj == null)
			{
				return false;
			}

			CollectionValue value = i_Obj as CollectionValue;
			if (value != (CollectionValue)null)
			{
				return this == value;
			}

			string strValue = i_Obj as string;
			if (strValue != null)
			{
				return this == strValue;
			}

			throw new SystemException($"Could compare type {GetType()} whith type {i_Obj.GetType()}.");
		}

		public override int GetHashCode()
		{
			return m_Value.GetHashCode();
		}
		#endregion
	}
}
