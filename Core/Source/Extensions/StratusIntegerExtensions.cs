using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Performs the action the specified number of times
		/// </summary>
		/// <param name="x"></param>
		/// <param name="action"></param>
		public static void Iterate(this int x, System.Action action)
		{
			for(int i = 0; i < x; ++i)
			{
				action();
			}
		}

		/// <summary>
		/// Performs the zero-indexed action the specified number of times
		/// </summary>
		/// <param name="x"></param>
		/// <param name="action"></param>
		public static void Iterate(this int x, System.Action<int> action)
		{
			for (int i = 0; i < x; ++i)
			{
				action(i);
			}
		}

		/// <summary>
		/// Performs the zero-indexed action the specified number of times,
		/// (From x-1 to 0)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="action"></param>
		public static void IterateReverse(this int x, System.Action<int> action)
		{
			for (int i = x - 1; i >= 0; --i)
			{
				action(i);
			}
		}
	}

}