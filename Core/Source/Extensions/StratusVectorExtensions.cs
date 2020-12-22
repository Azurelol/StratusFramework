using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Checks the specified value is within the range of this vector
		/// </summary>
		/// <param name="range">A vector containing a min-max range.</param>
		/// <param name="value">The value to check.</param>
		/// <returns>True if the value is within the range, false otherwise</returns>
		public static bool ContainsExclusive(this Vector2 range, float value)
		{
			if (value > range.x && value < range.y)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks the specified value is within the range of this vector
		/// </summary>
		/// <param name="range">A vector containing a min-max range.</param>
		/// <param name="value">The value to check.</param>
		/// <returns>True if the value is within the range, false otherwise</returns>
		public static bool ContainsInclusive(this Vector2 range, float value)
		{
			if (value >= range.x && value <= range.y)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the average between the values of the vector.
		/// </summary>
		/// <param name="range">The vector containing two values.</param>
		/// <returns></returns>
		public static float Average(this Vector2 range)
		{
			return ((range.x + range.y) / 2f);
		}			   

		/// <summary>
		/// Calculates a random Vector3 starting from this one
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="minDist"></param>
		/// <param name="maxDist"></param>
		/// <param name="keepVertical"></param>
		/// <returns></returns>
		public static Vector3 CalculateRandomPosition(this Vector3 vec, float minDist, float maxDist, bool keepVertical = true)
		{
			Vector3 randomPos = vec;
			// Calculate a random radius from the given range
			float radius = UnityEngine.Random.Range(minDist, maxDist);
			// Randomly change the x and z values of the position
			randomPos.x += UnityEngine.Random.Range(-radius, radius);
			randomPos.z += UnityEngine.Random.Range(-radius, radius);
			if (!keepVertical)
			{
				randomPos.y += UnityEngine.Random.Range(-radius, radius);
			}

			return randomPos;
		}

		/// <summary>
		/// Given a target and a specified distance, calculates the position
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="dist"></param>
		/// <returns></returns>
		public static Vector3 CalculatePositionAtDistanceFromTarget(this Vector3 vec, Vector3 target, float dist)
		{
			Vector3 dir = target - vec;
			return target + (dir * dist);
		}

		/// <summary>
		/// Returns a Vector2 with only the XY components
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector2 XY(this Vector3 vec)
		{
			return new Vector2(vec.x, vec.y);
		}

		/// <summary>
		/// Returns a Vector2 with only the XZ components
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector2 XZ(this Vector3 vec)
		{
			return new Vector2(vec.x, vec.z);
		}

		/// <summary>
		/// Returns a Vector2 with only the YZ components
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector2 YZ(this Vector3 vec)
		{
			return new Vector2(vec.y, vec.z);
		}

		/// <summary>
		/// Attempts to parse a Vector2 in either the format:
		/// (a,b) or a,b
		/// </summary>
		public static Vector2 ParseVector2(string value)
		{
			// Remove the parentheses
			if (value.StartsWith("(") && value.EndsWith(")"))
			{
				value = value.Substring(1, value.Length - 2);
			}

			// split the items
			string[] values = value.Split(',');

			// store as a Vector3
			Vector2 result = new Vector2(
				float.Parse(values[0]),
				float.Parse(values[1]));

			return result;
		}

		/// <summary>
		/// Attempts to parse a Vector3 in either the format:
		/// (a,b,c) or a,b,c
		/// </summary>
		public static Vector3 ParseVector3(string value)
		{
			// Remove the parentheses
			if (value.StartsWith("(") && value.EndsWith(")"))
			{
				value = value.Substring(1, value.Length - 2);
			}

			// split the items
			string[] values = value.Split(',');

			// store as a Vector3
			Vector3 result = new Vector3(
				float.Parse(values[0]),
				float.Parse(values[1]),
				float.Parse(values[2]));

			return result;
		}

		/// <summary>
		/// Attempts to parse a Rect in either the format:
		/// (x,y,w,h) or x,y,w,h
		/// </summary>
		public static Rect ParseRect(string value)
		{
			// Remove the parentheses
			if (value.StartsWith("(") && value.EndsWith(")"))
			{
				value = value.Substring(1, value.Length - 2);
			}

			// split the items
			string[] values = value.Split(',');

			// store as a Vector3
			Rect result = new Rect(
				float.Parse(values[0]),
				float.Parse(values[1]),
				float.Parse(values[2]),
				float.Parse(values[3]));

			return result;
		}

	}


}