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
		/// <returns>The number of null elements removed</returns>
		public static int RemoveNull<T>(this List<T> list) where T : class
		{
			return list.RemoveAll(x => x == null || x.Equals(null));
		}

		/// <summary>
		/// Adds the given elements into the list (params T[] to IEnumerable<T>)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static void AddRange<T>(this List<T> list, params T[] values)
		{
			list.AddRange(values);
		}

		/// <summary>
		/// Iterates over the given list, removing any invalid elements (described hy the validate functon)
		/// Returns true if any elements were removed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="predicate"></param>
		/// <param name="action"></param>
		public static bool ForEachRemoveInvalid<T>(this List<T> list,
			System.Action<T> action,
			Predicate<T> predicate)
			where T : class
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
				action(element);
			}

			list.RemoveAll(x => invalid.Contains(x));
			return removed;
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

		/// <summary>
		/// Clones all the elements of this list, if they are cloneable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listToClone"></param>
		/// <returns></returns>
		public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}

		/// <summary>
		/// Adds all elements not already present into the given list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> values)
		{
			list.AddRange(values.Where(x => !list.Contains(x)));
		}

		/// <summary>
		/// Adds all elements not already present into the given list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static void AddRangeUnique<T>(this List<T> list, params T[] values)
		{
			list.AddRangeUnique((IEnumerable<T>)values);
		}

		/// <summary>
		/// Adds all values that fulfill the given predicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static int AddRangeWhere<T>(this List<T> list, Predicate<T> predicate, IEnumerable<T> values)
		{
			int count = 0;
			values.ForEach((x) =>
			{
				if (predicate(x))
				{
					list.Add(x);
					count++;
				}
			});
			return count;
		}

		/// <summary>
		/// Adds all values that fulfill the given predicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static int AddRangeWhere<T>(this List<T> list, Predicate<T> predicate, params T[] values)
		{
			return list.AddRangeWhere(predicate, (IEnumerable<T>)values);
		}

		/// <summary>
		/// Adds the items from another list, except null ones
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns>True if any null elements were detected</returns>
		public static void AddRangeNotNull<T>(this List<T> list, IEnumerable<T> values)
			where T : class
		{
			list.AddRange(values.Where(x => x != null));
		}

		/// <summary>
		/// Adds the items from another list, except null ones
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns>True if any null elements were detected</returns>
		public static void AddRangeNotNull<T>(this List<T> list, params T[] values)
			where T : class
		{
			list.AddRangeNotNull((IEnumerable<T>)values);
		}

		/// <summary>
		/// Adds the element if it's not null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool AddIfNotNull<T>(this List<T> list, T item)
		{
			if (item != null)
			{
				list.Add(item);
				return true;
			}
			return false;
		}

	}

}