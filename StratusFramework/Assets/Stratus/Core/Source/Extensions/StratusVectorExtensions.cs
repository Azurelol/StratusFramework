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
		public static bool Contains(this Vector2 range, float value)
		{
			if (value > range.x && value < range.y)
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
		/// Strips one of the components from the vector
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="axis"></param>
		/// <returns></returns>
		public static Vector3 Strip(this Vector3 vec, VectorAxis axis)
		{
			switch (axis)
			{
				case VectorAxis.X:
					return new Vector3(0f, vec.y, vec.z);
				case VectorAxis.Y:
					return new Vector3(vec.x, 0f, vec.z);
				case VectorAxis.Z:
					return new Vector3(vec.x, vec.y, 0f);
			}

			throw new System.Exception("Missing component");
		}

		/// <summary>
		/// Calculates a position in front of the transform at a given distance
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		public static Vector3 CalculateForwardPosition(this Transform transform, float distance)
		{
			return transform.position + (transform.forward * distance);
		}

		/// <summary>
		/// Calculates a position on a given normalized direction vector from the transform's position.
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="normalizedDirVec"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		public static Vector3 CalculatePositionAtDirection(this Transform transform, Vector3 normalizedDirVec, float distance)
		{
			return transform.position + (normalizedDirVec * distance);
		}
	}


}