using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Copies the values from another component of the same type onto this one
		/// </summary>
		/// <typeparam name="T">The component class</typeparam>
		/// <param name="self">The component to copy into.</param>
		/// <param name="source">The component being copied</param>
		/// <returns>A reference to the new component</returns>
		public static T CopyFrom<T>(this T self, T source) where T : Component
		{
			return CopyFrom((Component)self, (Component)source) as T;
		}


		/// <summary>
		/// Copies the values from another component of the same type onto this one
		/// </summary>
		/// <typeparam name="T">The component class</typeparam>
		/// <param name="self">The component to copy into.</param>
		/// <param name="source">The component being copied</param>
		/// <returns>A reference to the new component</returns>
		public static Component CopyFrom(this Component self, Component source)
		{
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), self);
			return self;
		}

		/// <summary>
		/// Copies the values from another scriptable object of the same type onto this one
		/// </summary>
		/// <typeparam name="T">The component class</typeparam>
		/// <param name="self">The component to copy into.</param>
		/// <param name="source">The component being copied</param>
		/// <returns>A reference to the new component</returns>
		public static ScriptableObject CopyFrom(this ScriptableObject self, ScriptableObject source)
		{
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), self);
			return self;
		}


		// Gets the positions of all vertices of this collider in wolrd space
		public static Vector3[] GetVertices(this BoxCollider b)
		{
			Vector3[] vertices = new Vector3[8];
			vertices[0] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f);
			vertices[1] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);
			vertices[2] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);
			vertices[3] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f);
			vertices[4] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f);
			vertices[5] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f);
			vertices[6] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);
			vertices[7] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);

			return vertices;
		}

		public static T[] GetComponentsInChildrenNotIncludeSelf<T>(this Component component, bool includeInactive)
			where T : Component
		{
			List<T> result = new List<T>(component.GetComponentsInChildren<T>(includeInactive));
			if (result.NotEmpty() && result[0].transform == component.transform)
			{
				result.RemoveFirst();

			}
			return result.ToArray();
		}

	}

}