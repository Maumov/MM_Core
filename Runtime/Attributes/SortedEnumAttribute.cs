using UnityEngine;

namespace MM.Attribute
{
    /// <summary>
    /// This Attribute facilitates enum assignation in the Inspector.
    /// When used with an enum, any item from the enum whille be shown alphabetically sorted.
    /// If DisplayFilter is true, they will also be grouped based on _ in their values name.
    /// </summary>
    /// <example>
    /// <code>
    /// [SortedEnum]
    /// public StateEnum m_CurrentState;
    /// </code>
    /// or 
    /// <code>
    /// [SortedEnum(true)]
    /// public LocId m_LocalizationId;
    /// </code>
    /// </example>
    public class SortedEnumAttribute : PropertyAttribute
    {
        public bool m_DisplayFilter;
        public SortedEnumAttribute(bool i_DisplayFilter = false)
        {
            m_DisplayFilter = i_DisplayFilter;
        }
    }
}
