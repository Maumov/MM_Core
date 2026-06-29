using System;
using UnityEngine;

namespace MM.CollectionEnum
{
    public class CollectionValueAttribute : PropertyAttribute
    {
        private Type m_EnumType;

        public Type EnumType
        {
            get { return m_EnumType; }
        }

        public CollectionValueAttribute(Type i_EnumType)
        {
            m_EnumType = i_EnumType;
        }
    }
}
