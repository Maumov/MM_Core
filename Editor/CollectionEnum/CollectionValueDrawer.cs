using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MM.EditorDrawer;
using UnityEditor;
using UnityEngine;

namespace MM.CollectionEnum
{
    [CustomPropertyDrawer(typeof(CollectionValueAttribute))]
    public class CollectionValueDrawer : PropertyDrawer
    {
        private Type m_EnumType;
        private List<string> m_Values;

        public override void OnGUI(Rect i_Position, SerializedProperty i_Property, GUIContent i_Label)
        {
            if (i_Property.propertyType != SerializedPropertyType.String)
            {
                throw new Exception("The attribute Collection can only be used with a string field");
            }

            i_Position = EditorGUI.PrefixLabel(i_Position, i_Label);

            if (m_EnumType == null || m_Values == null)
            {
                m_EnumType = GetEnumType();
                m_Values = GetValues(m_EnumType);
            }

            using (GUIScope.RemoveIndent())
            {
                EditorGUI.BeginChangeCheck();
                int chosenValue = EditorGUI.Popup(i_Position, m_Values.IndexOf(i_Property.stringValue), m_Values.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    i_Property.stringValue = m_Values[chosenValue];
                }
            }
        }

        private List<string> GetValues(Type i_EnumType)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == "Assembly-CSharp");
            var type = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(i_EnumType) && !x.Name.Contains("Template")).FirstOrDefault();
            //const BindingFlags staticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

            return type.GetFields()
                .Select(x => x.GetValue(null))
                .Cast<CollectionValue>()
                .Select(x => x.ToString())
                .ToList();
        }

        private Type GetEnumType()
        {
            CollectionValueAttribute valueAttribute = (CollectionValueAttribute)attribute;
            Type valueType = valueAttribute.EnumType;
            Type genericType = GetSubclassOfRawGeneric(typeof(CollectionEnum<>), valueType);
            if (genericType != null)
            {
                return genericType;
            }

            if (valueType.IsSubclassOf(typeof(CollectionValue)))
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x
                        .GetTypes()
                        .Select(t => GetSubclassOfRawGeneric(typeof(CollectionEnum<>), t))
                        .Where(t => t != null)
                    )
                    .FirstOrDefault(x => x.GetGenericArguments().First() == valueType);
            }

            throw new Exception("Type received in parameter does not inherit from CollectionEnum or CollectionValue");
        }

        private static Type GetSubclassOfRawGeneric(Type i_Generic, Type i_ToCheck)
        {
            while (i_ToCheck != null && i_ToCheck != typeof(object))
            {
                var cur = i_ToCheck.IsGenericType ? i_ToCheck.GetGenericTypeDefinition() : i_ToCheck;
                if (i_Generic == cur)
                {
                    return i_ToCheck;
                }

                i_ToCheck = i_ToCheck.BaseType;
            }

            return null;
        }
    }
}
