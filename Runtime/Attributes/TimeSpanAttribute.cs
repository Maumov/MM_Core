using UnityEngine;
using System;

namespace MM.Attribute
{
    /// <summary>
    /// This Attribute allows to you to assign any combination of milliseconds, seconds, minutes, hours and days
    /// to a long value. When used with a long, all the fields necessary will be shown in the inspector. This
    /// would allow a designer to modify a long as if it were a TimeSpan.
    /// </summary>
    /// <example>
    /// [TimeSpan(TimeSpanAttribute.Seconds | TimeSpanAttribute.Minutes | TimeSpanAttribute.Hours)]
    /// public string m_WaitTime;
    /// --> result : H[000] M[001] S[020] (for 1 minute and 20 seconds)
    /// </example>
    public class TimeSpanAttribute : PropertyAttribute
    {
        [Flags]
        public enum Value
        {
            Milliseconds = 1,
            Seconds = 2,
            Minutes = 4,
            Hours = 8,
            Days = 16,
            All = Milliseconds | Seconds | Minutes | Hours | Days,
        }

        public Value m_Value;

        public TimeSpanAttribute(Value i_Value = Value.All)
        {
            m_Value = i_Value;
        }
    }
}