using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MM.Extensions
{
	public static class TransformExtensions
	{
		public enum CompareType
		{
			Equals,
			Contains,
			StartsWith,
			EndsWith,
		}

		/// <summary>
		/// Search through children of a Transform to find a child that has a name that matches to the search
		/// </summary>
		/// <param name="i_Parent">The root Transform to start looking. This object is included in the search.</param>
		/// <param name="i_Name">The string value to compare to the children names</param>
		/// <param name="i_Compare">The comparison type made in during the search</param>
		/// <returns></returns>
		public static Transform FindDeepChild(this Transform i_Parent, string i_Name, CompareType i_Compare = CompareType.Equals)
		{
			Queue<Transform> toProcess = new Queue<Transform>();
			toProcess.Enqueue(i_Parent);
			Transform current;

			while (toProcess.Count > 0)
			{
				current = toProcess.Dequeue();
				switch (i_Compare)
				{
					case CompareType.Equals:
						if (current.name.Equals(i_Name))
						{
							return current;
						}

						break;

					case CompareType.Contains:
						if (current.name.Contains(i_Name))
						{
							return current;
						}

						break;

					case CompareType.StartsWith:
						if (current.name.StartsWith(i_Name))
						{
							return current;
						}

						break;

					case CompareType.EndsWith:
						if (current.name.EndsWith(i_Name))
						{
							return current;
						}

						break;

				}

				for (int i = 0; i < current.childCount; i++)
				{
					toProcess.Enqueue(current.GetChild(i));
				}
			}

			return null;
		}

		/// <summary>
		/// Sets object and all children to the specified layer
		/// </summary>
		/// <param name="i_Obj">Object root</param>
		/// <param name="i_Layer">String value of the layer</param>
		public static void SetLayerRecursive(this Transform i_Obj, string i_Layer)
		{
			i_Obj.SetLayerRecursive(LayerMask.NameToLayer(i_Layer));
		}

		/// <summary>
		/// Sets object and all children to the specified layer
		/// </summary>
		/// <param name="i_Obj">Object root</param>
		/// <param name="i_Layer">Integer value of the layer</param>
		public static void SetLayerRecursive(this Transform i_Obj, int i_Layer)
		{
			Queue<Transform> toProcess = new Queue<Transform>();
			toProcess.Enqueue(i_Obj);
			Transform current;

			while (toProcess.Count > 0)
			{
				current = toProcess.Dequeue();
				current.gameObject.layer = i_Layer;

				for (int i = 0; i < current.childCount; i++)
				{
					toProcess.Enqueue(current.GetChild(i));
				}
			}
		}

		/// <summary>
		/// Sets object and all children to static
		/// </summary>
		/// <param name="i_Obj">Object root</param>
		/// <param name="i_Static">Bool value of static state</param>
		public static void SetStaticRecursive(this Transform i_Obj, bool i_Static)
		{
			Queue<Transform> toProcess = new Queue<Transform>();
			toProcess.Enqueue(i_Obj);
			Transform current;

			while (toProcess.Count > 0)
			{
				current = toProcess.Dequeue();
				current.gameObject.isStatic = i_Static;

				for (int i = 0; i < current.childCount; i++)
				{
					toProcess.Enqueue(current.GetChild(i));
				}
			}
		}
	}
}
