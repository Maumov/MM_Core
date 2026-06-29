using System;
using MM.Attribute;
using UnityEditor;
using UnityEngine;

namespace MM.Attribute
{
    [CustomPropertyDrawer(typeof(TimeSpanAttribute))]
    public class TimeSpanAttributeDrawer : PropertyDrawer
    {
        private TimeSpanAttribute m_Attribute;
        private int m_NbFlags;

        private int m_MillisecondsValue;
        private int m_SecondsValue;
        private int m_MinutesValue;
        private int m_HoursValue;
        private int m_DaysValue;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_Attribute == null)
            {
                m_Attribute = attribute as TimeSpanAttribute;

                TimeSpan t = new TimeSpan(property.longValue);
                m_NbFlags = 0;
                if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Milliseconds))
                {
                    m_NbFlags++;
                    m_MillisecondsValue = t.Milliseconds;
                }
                if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Seconds))
                {
                    m_NbFlags++;
                    m_SecondsValue = t.Seconds;
                }
                if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Minutes))
                {
                    m_NbFlags++;
                    m_MinutesValue = t.Minutes;
                }
                if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Hours))
                {
                    m_NbFlags++;
                    m_HoursValue = t.Hours;
                }
                if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Days))
                {
                    m_NbFlags++;
                    m_DaysValue = t.Days;
                }
            }

            EditorGUI.BeginChangeCheck();

            position = EditorGUI.PrefixLabel(position, label);
            position.width = (float) position.width/m_NbFlags;

            if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Days))
            {
                m_DaysValue = DrawField(position, m_DaysValue, "D");
                position.x += position.width;
            }
            if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Hours))
            {
                m_HoursValue = DrawField(position, m_HoursValue, "H");
                position.x += position.width;
            }
            if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Minutes))
            {
                m_MinutesValue = DrawField(position, m_MinutesValue, "M");
                position.x += position.width;
            }
            if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Seconds))
            {
                m_SecondsValue = DrawField(position, m_SecondsValue, "S");
                position.x += position.width;
            }
            if (IsFlagSet(m_Attribute.m_Value, TimeSpanAttribute.Value.Milliseconds))
            {
                m_MillisecondsValue = DrawField(position, m_MillisecondsValue, "ms");
            }

            if (EditorGUI.EndChangeCheck())
            {
                TimeSpan t = new TimeSpan(m_DaysValue, m_HoursValue, m_MinutesValue, m_SecondsValue, m_MillisecondsValue);
                property.longValue = t.Ticks;
            }
        }

        private int DrawField(Rect i_Pos, int i_Value, string i_Label)
        {
            Rect pos = new Rect(i_Pos);
            pos.width = 20f;
            EditorGUI.LabelField(pos, i_Label);

            pos.x += pos.width;
            pos.width = i_Pos.width - pos.width;
            return EditorGUI.IntField(pos, i_Value);
        }

        private bool IsFlagSet(TimeSpanAttribute.Value i_Value, TimeSpanAttribute.Value i_Flag)
        {
            return (i_Value & i_Flag) == i_Flag;
        }
    }
}