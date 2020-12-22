using UnityEngine;
using System;
using System.ComponentModel.DataAnnotations;

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
		public static float LerpFrom(this float b, float a, [ParameterRange(0,1f)] float t)
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
		public static float LerpTo(this float a, float b, [ParameterRange(0, 1f)] float t)
		{
			return (1f - t) * a + t * b;
		}

		
		/// <summary>
		/// Returns this float as a percentage string (eg: 97.7%)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string ToPercentageString(this float f)
		{
			return $"{(f * 100.0f):0.00}%";
		}

		/// <summary>
		/// Returns this float as a percentage string, rounded as an integer
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string ToPercentageRoundedString(this float f)
		{
			return $"{(f * 100.0f):0}%";
		}

		/// <summary>
		/// Returns a string of the float rounded to 2 decimal places by default
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string ToRoundedString(this float f)
		{
			return $"{(f):0.00}%";
		}

		/// <summary>
		/// Converts this value to its percentage (dividing by 100)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static float ToPercent(this float f) => f * 0.01f;

	}
}