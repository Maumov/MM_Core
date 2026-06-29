using UnityEngine;

namespace MM.Attribute
{
    /// <summary>
    /// Use this on an Enum serialized field to show it as a Flag (multiple selection) in the inspector.
    /// The enum should be marked as [Flags] and values set to multiple of 2 to make it works properly.
    /// </summary>
    public class EnumFlagsAttribute : PropertyAttribute { }
}
