using UnityEngine;

namespace MM.Attribute
{
    /// <summary>
    /// This Attribute facilitates SortingLayer assignation in the Inspector.
    /// When used with a string, any item from the unity SortingLayer list can be chosen in a drop down menu.
    /// </summary>
    /// <example>
    /// [SortingLayer]
    /// public string m_Layer;
    /// </example>
    public class SortingLayerAttribute : PropertyAttribute { }
}
