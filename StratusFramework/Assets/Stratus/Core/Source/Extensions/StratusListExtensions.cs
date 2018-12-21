using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Removes all null values from this list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void TrimNull<T>(this List<T> list)
		{
			list.RemoveAll(x => x == null || x.Equals(null));
		}

		/// <summary>
		/// Finds the first element of this array that matches the predicate function
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static T FindFirst<T>(this T[] array, Func<T, bool> predicate)
		{
			foreach (T element in array)
			{
				if (predicate(element))
				{
					return element;
				}
			}
			return default(T);
		}

		/// <summary>
		/// Iterates over the given list, removing any invalid elements (described hy the validate functon)
		/// Returns true if any elements were removed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="predicate"></param>
		/// <param name="iterateFunction"></param>
		public static bool IterateAndRemoveInvalid<T>(this List<T> list, System.Action<T> iterateFunction, Predicate<T> predicate)
		{
			bool removed = false;
			List<T> invalid = new List<T>();
			foreach (T element in list)
			{
				// Remove invalid elements
				bool valid = predicate(element);
				removed |= valid;

				if (!valid)
				{
					invalid.Add(element);
					continue;
				}

				// Apply the iteration function
				iterateFunction(element);
			}

			list.RemoveAll(x => invalid.Contains(x));
			return removed;
		}

		public static bool IterateAndRemoveInvalid<T>(this List<T> list, System.Action<T> iterateFunction, Func<T, bool> predicate)
		{
			return IterateAndRemoveInvalid(list, iterateFunction, predicate.ToPredicate());
		}

		/// <summary>
		/// Iterates over the given list, removing any invalid elements (described hy the validate functon)    
		/// Returns true if any elements were removed
		/// </summary>    
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="predicate"></param>
		/// <param name="iterateFunction"></param>
		public static bool RemoveInvalid<T>(this List<T> list, Predicate<T> predicate)
		{
			bool removed = false;
			List<T> invalid = new List<T>();
			foreach (T element in list)
			{
				// Remove invalid elements
				bool valid = predicate(element);
				removed |= valid;

				if (!valid)
				{
					invalid.Add(element);
				}
			}

			list.RemoveAll(x => invalid.Contains(x));
			return removed;
		}

		public static bool RemoveInvalid<T>(this List<T> list, Func<T, bool> predicate)
		{
			return RemoveInvalid(list, predicate.ToPredicate());
		}





	}

}