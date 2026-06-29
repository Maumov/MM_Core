using MM.Attribute;
using UnityEditor;
using UnityEngine;

namespace MM.Attribute
{
    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    public class BitMaskAttributeDrawer : PropertyDrawer
    {
        private BitMaskAttribute m_Attribute = null;
        private System.Type m_EnumType;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_Attribute == null)
            {
                m_Attribute = attribute as BitMaskAttribute;
            }

            if (fieldInfo.FieldType.IsEnum)
            {
                m_EnumType = fieldInfo.FieldType;
            }
            else
            {
                m_EnumType = m_Attribute.m_Enum;
            }

            var itemNames = System.Enum.GetNames(m_EnumType);
            var itemValues = System.Enum.GetValues(m_EnumType) as int[];
            int val = property.intValue;
            int maskVal = 0;
            for (int i = 0; i < itemValues.Length; i++)
            {
                if (itemValues[i] != 0)
                {
                    if ((val & itemValues[i]) == itemValues[i])
                        maskVal |= 1 << i;
                }
                else if (val == 0)
                    maskVal |= 1 << i;
            }
            int newMaskVal = EditorGUI.MaskField(position, label, maskVal, itemNames);
            int changes = maskVal ^ newMaskVal;
            for (int i = 0; i < itemValues.Length; i++)
            {
                if ((changes & (1 << i)) != 0) // has this list item changed?
                {
                    if ((newMaskVal & (1 << i)) != 0) // has it been set?
                    {
                        if (itemValues[i] == 0) // special case: if "0" is set, just set the val to 0
                        {
                            val = 0;
                            break;
                        }
                        else
                            val |= itemValues[i];
                    }
                    else // it has been reset
                    {
                        val &= ~itemValues[i];
                    }
                }
            }

            property.intValue = val;
        }
    }
}