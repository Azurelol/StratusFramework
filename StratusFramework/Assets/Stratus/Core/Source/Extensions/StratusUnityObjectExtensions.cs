using System;
using System.Reflection;
using OdinSerializer;
using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		public static bool IsDestroyed(this UnityEngine.Object obj)
		{
			return obj == null || obj.Equals(null);
		}
	}
}