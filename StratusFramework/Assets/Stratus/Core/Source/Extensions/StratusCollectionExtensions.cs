using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns true if the list is empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The list.</param>
		/// <returns>True if the list is empty, false otherwise</returns>
		public static bool Empty<T>(this ICollection<T> collection)
		{
			return collection.Count == 0;
		}

		/// <summary>
		/// Returns true if the list is empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The list.</param>
		/// <returns>True if the list is empty, false otherwise</returns>
		public static bool Empty<T>(this Stack<T> collection)
		{
			return collection.Count == 0;
		}

		/// <summary>
		/// Returns true if the array is not empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The array.</param>
		/// <returns>True if the array is not empty, false otherwise</returns>
		public static bool NotEmpty<T>(this ICollection<T> collection)
		{
			return collection.Count > 0;
		}

		/// <summary>
		/// Returns true if the array is not empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The array.</param>
		/// <returns>True if the array is not empty, false otherwise</returns>
		public static bool NotEmpty<T>(this Stack<T> collection)
		{
			return collection.Count > 0;
		}

		/// <summary>
		/// Returns true if the list not null or empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static bool NotNullOrEmpty<T>(this ICollection<T> collection)
		{
			return collection != null && collection.Count > 0;
		}
	}
}