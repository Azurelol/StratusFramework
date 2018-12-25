using System;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;

namespace Stratus
{
	public static partial class Extensions
	{
		private const BindingFlags bindingFlagsFullSearch =
			System.Reflection.BindingFlags.Public 
			| System.Reflection.BindingFlags.NonPublic 
			| System.Reflection.BindingFlags.Static
			| System.Reflection.BindingFlags.Instance;

		/// <summary>
		/// Returns true if the given type is an array or list
		/// </summary>
		/// <param name="listType"></param>
		/// <returns></returns>
		public static bool IsArrayOrList(this Type listType)
		{
			return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
		}

		/// <summary>
		/// If the given type is a list, returns the underlying element type
		/// </summary>
		/// <param name="listType"></param>
		/// <returns></returns>
		public static Type GetArrayOrListElementType(this Type listType)
		{
			if (listType.IsArray)
			{
				return listType.GetElementType();
			}
			if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return listType.GetGenericArguments()[0];
			}

			return null;
		}

		public static bool HasDefaultConstructor(this Type t)
		{
			return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
		}

		public static FieldInfo GetFieldExhaustive(this Type t, string name)
		{
			return t.GetField(name, bindingFlagsFullSearch);
		}


	}

}