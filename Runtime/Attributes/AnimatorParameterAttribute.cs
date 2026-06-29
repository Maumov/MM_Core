using UnityEngine;

namespace MM.Attribute
{
    /// <summary>
    /// This Attribute facilitates animator parameter assignation in the Inspector.
    /// When used with a int, any parameter from the animator can be chosen in a drop down menu.
    /// </summary>
    /// <example>
    /// // This version will find an animator on the same object
    /// [AnimatorParameter]
    /// public int m_IdleTrigger;
    /// 
    /// or
    /// 
    /// // This version will use the animator provided (string has to be exactly the same as the member)
    /// public Animator m_ChildAnimator;
    /// [AnimatorParameter("m_ChildAnimator")]
    /// public int m_WinBool;
    /// </example>
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        public string m_AnimatorObject;
        public AnimatorParameterAttribute()
        {
            m_AnimatorObject = "";
        }

        public AnimatorParameterAttribute(string i_Animator)
        {
            m_AnimatorObject = i_Animator;
        }
    }
}
