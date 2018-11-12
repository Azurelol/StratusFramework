using Stratus.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Retrieves a specific attribute from the given type, if it is present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			return AttributeUtility.FindAttribute<T>(type);
		}

		/// <summary>
		/// Returns a dictionary of all attributes of a given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Dictionary<Type, Attribute> MapAttributes(this Type type)
		{
			return AttributeUtility.MapAttributes(type);
		}
	}
}