using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Clones all the elements of this list, if they are cloneable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listToClone"></param>
		/// <returns></returns>
		public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}

		/// <summary>
		/// Shuffles the list using a randomized range based on its size.
		/// </summary>
		/// <typeparam name="T">The type of the list.</typeparam>
		/// <param name="list">A reference to the list.</param>
		/// <remarks>Courtesy of Mike Desjardins #UnityTips</remarks>
		/// <returns>A new, shuffled list.</returns>
		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; ++i)
			{
				T index = list[i];
				int randomIndex = UnityEngine.Random.Range(i, list.Count);
				list[i] = list[randomIndex];
				list[randomIndex] = index;
			}

			return list;
		}

		/// <summary>
		/// Swaps 2 elements in a list by index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="indexA"></param>
		/// <param name="indexB"></param>
		public static void Swap<T>(this IList<T> list, int indexA, int indexB)
		{
			T tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}

		/// <summary>
		/// Swaps 2 elements in a list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="objA"></param>
		/// <param name="objB"></param>
		public static bool Swap<T>(this IList<T> list, T objA, T objB)
		{
			int indexA = list.IndexOf(objA);
			int indexB = list.IndexOf(objB);
			list[indexA] = objB;
			list[indexB] = objA;
			return true;
		}

		/// <summary>
		/// Returns a random element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T Random<T>(this IList<T> list)
		{
			int randomSelection = UnityEngine.Random.Range(0, list.Count);
			return list[randomSelection];
		}

		/// <summary>
		/// Returns the last element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T Last<T>(this IList<T> list)
		{
			return list[list.Count - 1];
		}

		/// <summary>
		/// Returns the first element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T First<T>(this IList<T> list)
		{
			return list[0];
		}

		/// <summary>
		/// Removes the last element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void RemoveLast<T>(this IList<T> list)
		{
			if (list.NotEmpty())
			{
				list.RemoveAt(list.Count - 1);
			}
		}

		/// <summary>
		/// Returns the first element if the list if there's one, or null 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T FirstOrNull<T>(this IList<T> list)
		{
			return list.NotEmpty() ? list[0] : default(T);
		}



		/// <summary>
		/// Given a list, returns an array of strings based on the naming function provided
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static string[] ToString<T>(this IList<T> list, Func<T, string> nameFunc)
		{
			string[] names = new string[list.Count];
			for (int i = 0; i < list.Count; ++i)
			{
				names[i] = nameFunc(list[i]);
			}

			return names;
		}

		/// <summary>
		/// Adds all elements not already present into the given list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="enumerable"></param>
		public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> enumerable)
		{
			list.AddRange(enumerable.Where(x => !list.Contains(x)));
		}

		/// <summary>
		/// Adds the element if it's not null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool AddIfNotNull<T>(this IList<T> list, T item)
		{
			if (item != null)
			{
				list.Add(item);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Adds the items from another list, except null ones
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool AddRangeNotNull<T>(this IList<T> list, IEnumerable<T> other) where T : class
		{
			bool foundNull = false;
			foreach (T item in other)
			{
				foundNull |= !(AddIfNotNull(list, item));
			}
			return foundNull;
		}

		/// <summary>
		/// Adds elements from the given enumerables given that they fulfill a predicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="other"></param>
		/// <param name="predicate"></param>
		public static void AddRangeFiltered<T>(this IList<T> list, IEnumerable<T> other, Predicate<T> predicate)
		{
			foreach (T item in other)
			{
				bool valid = predicate(item);
				if (valid)
				{
					list.Add(item);
				}
			}
		}

		public static void AddRangeFiltered<T>(this IList<T> list, IEnumerable<T> other, Func<T, bool> predicate)
		{
			AddRangeFiltered(list, other, predicate.ToPredicate());
		}

		public static bool HasIndex<T>(this IList<T> list, int index)
		{
			return (index >= 0) && ((list.Count - 1) >= index);
		}

		public static T AtIndexOrDefault<T>(this IList<T> list, int index)
		{
			return list.HasIndex(index) ? list[index] : default(T);
		}

		public static T AtIndexOrDefault<T>(this IList<T> list, int index, T defaultValue)
		{
			return list.HasIndex(index) ? list[index] : defaultValue;
		}



	}

}