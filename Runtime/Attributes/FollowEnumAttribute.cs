using UnityEngine;

namespace MM.Attribute
{
    /// <summary>
    /// This Attribute facilitates assignation in an an array the Inspector.
    /// When used with any type of array, the array will take the size of the 
    /// highest value from the enum.
    /// </summary>
    /// <example>
    /// public enum MyEnum
    /// {
    /// 	None = -1,
    /// 	Slow = 0,
    /// 	Medium,
    /// 	Fast,
    /// }
    /// 
    /// [FollowEnum(typeof(MyEnum))]
    /// public float[] m_Speeds;
    /// 
    /// --> It will look like this in the hierarchy : 
    /// Speeds
    /// 	Size		[0003]
    /// 	Slow		[1.5f]
    /// 	Medium		[3.0f]
    /// 	Fast		[4.5f]
    /// </example>
    public class FollowEnumAttribute : PropertyAttribute
    {
        public System.Type m_Enum;
        public int m_Add;
        public FollowEnumAttribute(System.Type i_Enum, int i_Add = 0)
        {
            m_Enum = i_Enum;
            m_Add = i_Add;
        }
    }
}
