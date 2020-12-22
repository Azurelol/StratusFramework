using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Shuffles the list using a randomized range based on its size.
		/// </summary>
		/// <typeparam name="T">The type of the list.</typeparam>
		/// <param name="list">A reference to the list.</param>
		/// <remarks>Courtesy of Mike Desjardins #UnityTips</remarks>
		/// <returns>A new, shuffled list.</returns>
		public static void Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; ++i)
			{
				T index = list[i];
				int randomIndex = UnityEngine.Random.Range(i, list.Count);
				list[i] = list[randomIndex];
				list[randomIndex] = index;
			}
		}

		/// <summary>
		/// Swaps 2 elements in a list by index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="indexA"></param>
		/// <param name="indexB"></param>
		public static void SwapAtIndex<T>(this IList<T> list, int indexA, int indexB)
		{
			T tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}

		/// <summary>
		/// Swaps 2 elements in a list by looking up the index of the values
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static void Swap<T>(this IList<T> list, T a, T b)
		{
			int indexA = list.IndexOf(a);
			int indexB = list.IndexOf(b);
			list.SwapAtIndex(indexA, indexB);
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
		/// Returns the last index of the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static int LastIndex<T>(this IList<T> list)
		{
			return list.Count - 1;
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
		/// Removes the last element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void RemoveFirst<T>(this IList<T> list)
		{
			if (list.NotEmpty())
			{
				list.RemoveAt(0);
			}
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
		/// Returns true if index is valid for this list
		/// </summary>
		public static bool HasIndex<T>(this IList<T> list, int index)
		{
			return (index >= 0) && ((list.Count - 1) >= index);
		}

		/// <summary>
		/// Returns the element at the given index, or the default (null for class types)
		/// </summary>
		public static T AtIndexOrDefault<T>(this IList<T> list, int index)
		{
			return list.HasIndex(index) ? list[index] : default(T);
		}

		/// <summary>
		/// Returns the element at the given index, or the given default value
		/// </summary>
		public static T AtIndexOrDefault<T>(this IList<T> list, int index, T defaultValue)
		{
			return list.HasIndex(index) ? list[index] : defaultValue;
		}

		/// <summary>
		/// Inverts the order of the elements in a sequence.
		/// </summary>
		public static IEnumerable<T> Reverse<T>(this IList<T> source)
		{
			return Enumerable.Reverse(source);
		}

		/// <summary>
		/// Determines whether a sequence contains a specified element by using the default equality comparer.
		/// </summary>
		public static bool Contains<T>(this IList<T> source, T value)
			where T : IComparable
		{
			return Enumerable.Contains(source, value);
		}

		/// <summary>
		/// Determines whether a sequence contains a specified element by using a specified IEqualityComparer<T>.
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool Contains<T>(this IList<T> source, T value, IEqualityComparer<T> comparer)
		{
			return Enumerable.Contains(source, value, comparer);
		}

		public static bool ContainsIndex<T>(this IList<T> source, int index)
		{
			if (source == null || source.Count== 0 || index < 0)
			{
				return false;
			}

			return index <= source.Count - 1;
		}

		/// <summary>
		///Returns the maximum value in a sequence of values.
		/// </summary>
		public static float Max(this IList<float> source)
		{
			return Enumerable.Max(source);
		}

		/// <summary>
		/// Returns the maximum value in a sequence of values.
		/// </summary>
		public static float Min(this IList<float> source)
		{
			return Enumerable.Min(source);
		}

		/// <summary>
		/// Returns the maximum value in a sequence of values.
		/// </summary>
		public static int Max(this IList<int> source)
		{
			return Enumerable.Max(source);
		}

		/// <summary>
		///Returns the maximum value in a sequence of values.
		/// </summary>
		public static int Min(this IList<int> source)
		{
			return Enumerable.Min(source);
		}

		/// <summary>
		/// Computes the sum of a sequence of numeric values.
		/// </summary>
		public static int Sum(this IList<int> source)
		{
			return Enumerable.Sum(source);
		}

		/// <summary>
		/// Computes the sum of a sequence of numeric values.
		/// </summary>
		public static float Sum(this IList<float> source)
		{
			return Enumerable.Sum(source);
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the maximum value.
		/// </summary>
		public static float Max<T>(this IList<T> source, Func<T, float> selector)
		{
			return Enumerable.Max(source, selector);
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the maximum value.
		/// </summary>
		public static int Max<T>(this IList<T> source, Func<T, int> selector)
		{
			return Enumerable.Max(source, selector);
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the minimum value.
		/// </summary>
		public static float Min<T>(this IList<T> source, Func<T, float> selector)
		{
			return Enumerable.Min(source, selector);
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the minimum value.
		/// </summary>
		public static float Min<T>(this IList<T> source, Func<T, int> selector)
		{
			return Enumerable.Min(source, selector);
		}

		/// <summary>
		/// Returns a specified number of contiguous elements from the start of a sequence.
		/// </summary>
		public static IEnumerable<T> Take<T>(this IList<T> source, int count)
		{
			return Enumerable.Take(source, count);
		}

		/// <summary>
		/// Returns elements from a sequence as long as a specified condition is true, and then skips the remaining elements.
		/// </summary>
		public static IEnumerable<T> TakeWhile<T>(this IList<T> source, Func<T, bool> predicate)
		{
			return Enumerable.TakeWhile(source, predicate);
		}


	}

}