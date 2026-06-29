using UnityEngine;
using UnityEditor;

namespace MM.Extensions
{
	public static class SerializedPropertyExtension
	{
		/// <summary>
		/// Copy all the values from one SerializedProperty to an other.
		///  - Note : Gradient Value will not be copied
		/// </summary>
		/// <param name="i_From">The original SerializedProperty</param>
		/// <param name="i_To">The SerializedProperty that will be copied to</param>
		/// <param name="i_CopyChildren">Parse the whole hierarchy if true, otherwise will only copy the siblings</param>
		public static void CopyProperties(this SerializedProperty i_From, SerializedProperty i_To, bool i_CopyChildren = true)
		{
			if (i_From.propertyType != i_To.propertyType)
			{
				Debug.LogError("ERROR: SerializedProperty \"i_From\" did not match exactly SerializedProperty \"i_To\"");
				return;
			}

			SerializedProperty fromEnd = i_From.GetEndProperty();
			SerializedProperty toEnd = i_To.GetEndProperty();

			bool reachedEnd = true;
			do
			{
				CopyValue(i_From, i_To);

				reachedEnd = i_From.Next(i_CopyChildren);
				reachedEnd &= i_To.Next(i_CopyChildren);

				if (reachedEnd)
				{
					if (i_From.propertyType != i_To.propertyType)
					{
						Debug.LogError("ERROR: SerializedProperty \"i_From\" did not match exactly SerializedProperty \"i_To\"");
						return;
					}

					if (i_From == fromEnd || i_To == toEnd)
					{
						Debug.LogError("ERROR: Reached the end of either of the serialization fields");
						reachedEnd = false;
					}
				}

				if (reachedEnd && i_From.isArray)
				{
					i_To.ClearArray();
					for (int i = 0; i < i_From.arraySize; i++)
					{
						i_To.InsertArrayElementAtIndex(i);
					}
				}
			}
			while (reachedEnd);
		}

		public static string ValueToString(this SerializedProperty i_SerializeProp)
		{
			switch (i_SerializeProp.propertyType)
			{
				case SerializedPropertyType.Character:
					return ((char)i_SerializeProp.intValue).ToString();

				case SerializedPropertyType.LayerMask:
					return LayerMask.LayerToName(i_SerializeProp.intValue);

				case SerializedPropertyType.Integer:
					return i_SerializeProp.intValue.ToString();

				case SerializedPropertyType.Boolean:
					return i_SerializeProp.boolValue.ToString();

				case SerializedPropertyType.Float:
					return i_SerializeProp.floatValue.ToString();

				case SerializedPropertyType.String:
					return i_SerializeProp.stringValue.ToString();

				case SerializedPropertyType.Color:
					return i_SerializeProp.colorValue.ToString();

				case SerializedPropertyType.ObjectReference:
					return i_SerializeProp.objectReferenceValue == null ? "" : i_SerializeProp.objectReferenceValue.name;

				case SerializedPropertyType.Enum:
					return i_SerializeProp.enumValueIndex.ToString();

				case SerializedPropertyType.Vector2:
					return i_SerializeProp.vector2Value.ToString();

				case SerializedPropertyType.Vector3:
					return i_SerializeProp.vector3Value.ToString();

				case SerializedPropertyType.Vector4:
					return i_SerializeProp.vector4Value.ToString();

				case SerializedPropertyType.Rect:
					return i_SerializeProp.rectValue.ToString();

				case SerializedPropertyType.AnimationCurve:
					return i_SerializeProp.animationCurveValue.ToString();

				case SerializedPropertyType.Bounds:
					return i_SerializeProp.boundsValue.ToString();

				case SerializedPropertyType.Quaternion:
					return i_SerializeProp.quaternionValue.ToString();

				default:
					throw new System.Exception("Can only return value of non generic types");
			}
		}

		public static void CopyValue(this SerializedProperty i_From, SerializedProperty i_To)
		{
			switch (i_From.propertyType)
			{
				case SerializedPropertyType.Character:
				case SerializedPropertyType.LayerMask:
				case SerializedPropertyType.Integer:
					i_To.intValue = i_From.intValue;
					break;

				case SerializedPropertyType.Boolean:
					i_To.boolValue = i_From.boolValue;
					break;

				case SerializedPropertyType.Float:
					i_To.floatValue = i_From.floatValue;
					break;

				case SerializedPropertyType.String:
					i_To.stringValue = i_From.stringValue;
					break;

				case SerializedPropertyType.Color:
					i_To.colorValue = i_From.colorValue;
					break;

				case SerializedPropertyType.ObjectReference:
					i_To.objectReferenceValue = i_From.objectReferenceValue;
					break;

				case SerializedPropertyType.Enum:
					i_To.enumValueIndex = i_From.enumValueIndex;
					break;

				case SerializedPropertyType.Vector2:
					i_To.vector2Value = i_From.vector2Value;
					break;

				case SerializedPropertyType.Vector3:
					i_To.vector3Value = i_From.vector3Value;
					break;

				case SerializedPropertyType.Vector4:
					i_To.vector4Value = i_From.vector4Value;
					break;

				case SerializedPropertyType.Rect:
					i_To.rectValue = i_From.rectValue;
					break;

				case SerializedPropertyType.AnimationCurve:
					i_To.animationCurveValue = i_From.animationCurveValue;
					break;

				case SerializedPropertyType.Bounds:
					i_To.boundsValue = i_From.boundsValue;
					break;

				case SerializedPropertyType.Quaternion:
					i_To.quaternionValue = i_From.quaternionValue;
					break;

				default:
					throw new System.Exception("Can only copy non generic value types");
			}
		}
	}
}