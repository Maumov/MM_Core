using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MM.EditorDrawer
{
	public static class GUISplit
	{
		public class SplitResult : IEnumerator<Rect>
		{
			private int m_Current = -1;
			private Rect[] m_Rects = null;

			public SplitResult(Rect[] i_Rects)
			{
				m_Rects = i_Rects;
			}

			public Rect Next()
			{
				MoveNext();
				return Current;
			}

			public Rect Merge(int i_Count)
			{
				Rect total = Current;
				for (int i = 0; i < i_Count; i++)
				{
					if (i == 0)
					{
						total = Next();
						continue;
					}

					Rect add = Next();
					total.height += add.height;
				}

				return total;
			}

			public bool MoveNext()
			{
				m_Current++;
				return m_Current < m_Rects.Length;
			}

			public void Reset()
			{
				m_Current = -1;
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}

			public Rect Current
			{
				get { return m_Rects[m_Current]; }
			}

			public void Dispose()
			{
				m_Rects = null;
			}
		}

		/// <summary>
		/// Splits vertically a Rect for easy use in the GUI
		/// </summary>
		/// <param name="i_Rect">The Rect that will be split</param>
		/// <param name="i_NbLines">The number of lines that will be returned</param>
		/// <param name="i_LineHeight">The height of each line</param>
		/// <returns>SplitResult class that allows the user to iterate through all rects</returns>
		public static SplitResult SplitVertical(Rect i_Rect, int i_NbLines, float i_LineHeight)
		{
			Rect[] rects = new Rect[i_NbLines];
			for (int i = 0; i < i_NbLines; i++)
			{
				rects[i] = new Rect(i_Rect.x, i_Rect.y + i * (i_LineHeight + 2f), i_Rect.width, i_LineHeight);
			}

			return new SplitResult(rects);
		}

		/// <summary>
		/// Splits horizontally a Rect for easy use in the GUI
		/// </summary>
		/// <param name="i_Rect">The Rect that will be split</param>
		/// <param name="i_Sizes">An array of float sizes. If a value is under 0, the size will be stretched to fit</param>
		/// <returns>SplitResult class that allows the user to iterate through all rects</returns>
		public static SplitResult SplitHorizontal(Rect i_Rect, params float[] i_Sizes)
		{
			int layoutCount = 0;
			float constantSize = 0f;
			for (int i = 0; i < i_Sizes.Length; i++)
			{
				if (i_Sizes[i] < 0)
				{
					layoutCount++;
				}
				else
				{
					constantSize += i_Sizes[i];
				}
			}

			float currentX = i_Rect.x;
			Rect[] rects = new Rect[i_Sizes.Length];
			for (int i = 0; i < i_Sizes.Length; i++)
			{
				rects[i] = new Rect(i_Rect);
				rects[i].x = currentX;
				if (i_Sizes[i] < 0)
				{
					rects[i].width = (i_Rect.width - constantSize) / (float)layoutCount;
				}
				else
				{
					rects[i].width = i_Sizes[i];
				}

				currentX += rects[i].width;
			}

			return new SplitResult(rects);
		}
	}
}
