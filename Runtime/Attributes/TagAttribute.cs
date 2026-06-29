using UnityEngine;

namespace MM.Attribute
{
    /// <summary>
    /// This Attribute facilitates Tag assignation in the Inspector.
    /// When used with a string, any item from the unity Tag list can be chosen in a drop down menu.
    /// </summary>
    /// <example>
    /// [Tag]
    /// public string m_PlayerTag;
    /// </example>
    public class TagAttribute : PropertyAttribute { }
}
