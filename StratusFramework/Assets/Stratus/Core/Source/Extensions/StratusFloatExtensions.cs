using UnityEngine;
using System;

namespace Stratus
{
	public static partial class Extensions
	{

		/// <summary>
		/// Returns a linearly interpolated value from a (other) to itself (b) at a given t (0-1)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float LerpFrom(this float b, float a, float t)
		{
			return a.LerpTo(b, t);
		}

		/// <summary>
		/// Returns a linearly interpolated value from a (itself) to the target (b) at a given t (0-1)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float LerpTo(this float a, float b, float t)
		{
			return (1f - t) * a + t * b;
		}

	}
}