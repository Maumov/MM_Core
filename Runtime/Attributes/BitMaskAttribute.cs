using UnityEngine;


namespace MM.Attribute
{
    /// <summary>
    /// Code taken from : http://answers.unity3d.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html
    /// This Attribute turns an int or an enum into a BitMask. By using the attribute you can select
    /// multiple elements of an enum.
    /// </summary>
    /// <example>
    /// [Flags]	// [Flags] must be written
    /// public enum MyEnum
    /// {
    ///		None = 0, // Optional
    ///		A = 1,	// Obviously, all elements mus be a power of two
    ///		B = 2,
    ///		C = 4,
    ///		All = A | B | C, // Optional
    /// }
    /// 
    /// [BitMask]
    /// public MyEnum m_Enum;
    /// 
    /// or
    /// 
    /// [BitMask(typeof(MyEnum))]
    /// public int m_Enum;
    /// </example>
    public class BitMaskAttribute : PropertyAttribute
    {
        public System.Type m_Enum;

        public BitMaskAttribute(System.Type i_Enum = null)
        {
            m_Enum = i_Enum;
        }
    }
}
