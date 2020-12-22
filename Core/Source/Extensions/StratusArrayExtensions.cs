using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Finds the index of the given element in the array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static int FindIndex<T>(this T[] array, Predicate<T> match)
		{
			return Array.FindIndex(array, match);
		}

		/// <summary>
		/// Finds the index of the given element in the array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static int FindIndex<T>(this T[] array, T value)
		{
			return Array.FindIndex(array, x => x.Equals(value));
		}

		/// <summary>
		/// Finds the element of an array given a predicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static T Find<T>(this T[] array, Predicate<T> predicate)
		{
			return Array.Find(array, predicate);
		}

		/// <summary>
		/// Sorts the elements of an array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static void Sort<T>(this T[] array, IComparer<T> comparer)
		{
			Array.Sort(array, comparer);
		}

		/// <summary>
		/// Sorts the elements of an array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static void Sort<T>(this T[] array, Func<T, T, int> comparer)
		{
			Array.Sort(array, Comparer<T>.Create((x, y) => comparer(x, y)));
		}

		/// <summary>
		/// Sorts an array of elements that implement IComparable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		public static void Sort<T>(this T[] array) where T : IComparable
		{
			Array.Sort(array, (a, b) => a.CompareTo(b));
		}

		/// <summary>
		/// Checks whether the array contains the element that matches the condition
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static bool Contains<T>(this T[] array, Predicate<T> predicate)
		{
			return Array.Exists(array, predicate);
		}

		public static bool Contains<T>(this T[] array, T element)
		{
			return array.Contains((x) => x.Equals(element));
		}

		public static bool Contains<T>(this T[] array, T element, Func<T, T, bool> comparer)
		{
			foreach (T item in array)
			{
				if (comparer(item, element))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Contains<T>(this T[] array, T element, Comparer<T> comparer)
		{
			foreach (T item in array)
			{
				if (comparer.Compare(item, element) == 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a new array concantenating both arrays
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static T[] Concat<T>(this T[] first, T[] second)
		{			
			return Enumerable.Concat(first, second).ToArray();
		}

		/// <summary>
		/// Filters the left array with the contents of the right one. It will return a new
		/// array that omits elements from this array present in the other one.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static T[] Filter<T>(this T[] array, T[] filter)
		{
			T[] result = array.Where(x => filter.Contains(x)).ToArray();
			return result;
		}

		/// <summary>
		/// Copies this array, inserting the element to the front
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static T[] Prepend<T>(this T[] array, T element)
		{
			T[] newArray = new T[array.Length + 1];
			newArray[0] = element;
			Array.Copy(array, 0, newArray, 1, array.Length);
			return newArray;
		}

		/// <summary>
		/// Copies this array, inserting the elements to the back
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T[] Append<T>(this T[] array, params T[] values)
		{
			return array.Append((IEnumerable<T>)values).ToArray();
		}

		/// <summary>
		/// Copies the array, inserting the elements to the back if it fulfills the prediicate condition
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T[] AppendWhere<T>(this T[] array, Predicate<T> predicate, params T[] values)
		{
			return array.AppendWhere(predicate, (IEnumerable<T>)values).ToArray();
		}

		/// <summary>
		/// Copies this array, inserting the elements to the front
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T[] Prepend<T>(this T[] array, params T[] values)
		{
			return array.Prepend((IEnumerable<T>)values).ToArray();
		}

		/// <summary>
		/// Copies the array, inserting the elements to the front if they fulfill the prediicate condition
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T[] PrependWhere<T>(this T[] array, Predicate<T> predicate, params T[] values)
		{
			return array.PrependWhere(predicate, (IEnumerable<T>)values).ToArray();
		}

		/// <summary>
		/// Copies the array, without the first n (1 by default) elements present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T[] TruncateFront<T>(this T[] array, int length = 1)
		{
			T[] newArray = new T[array.Length - length];
			Array.Copy(array, 1, newArray, 0, array.Length - length);
			return newArray;
		}

		/// <summary>
		/// Copies the array, without the last n (1 by default) elements present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T[] TruncateBack<T>(this T[] array, int length = 1)
		{
			T[] newArray = new T[array.Length - length];
			Array.Copy(array, 0, newArray, 0, array.Length - length);
			return newArray;
		}

		/// <summary>
		/// Copies the array, without the selected element present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T[] Truncate<T>(this T[] array, T element)
		{
			int elementIndex = array.FindIndex(x => x.Equals(element));

			// If it's the first element
			if (elementIndex == -1)
			{
				return array;
			}
			else if (elementIndex == 0)
			{
				return array.TruncateFront();
			}
			// If it's the last element
			else if (elementIndex == array.Length)
			{
				return array.TruncateBack();
			}
			T[] newArray = new T[array.Length - 1];
			Array.Copy(array, 0, newArray, 0, elementIndex);
			Array.Copy(array, elementIndex + 1, newArray, elementIndex, array.Length - elementIndex - 1);
			return newArray;
		}

		public static bool IsNullOrEmpty<T>(this T[] array) => array == null || array.Length == 0;
		public static bool IsValid<T>(this T[] array) => array != null && array.Length > 0;

		/// <summary>
		///  Inverts the order of the elements in a sequence from the given index for a given length
		/// </summary>
		public static void Reverse<T>(this T[] array, int index, int length)
		{
			Array.Reverse(array, index, length);
		}

		/// <summary>
		/// Converts to an array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T[] ToArrayByCast<T>(this Array array)
		{
			return array.Cast<T>().ToArray();
		}

		public static int LengthOrZero<T>(this T[] array) => array != null ? array.Length : 0;

		public static bool ContainsIndex<T>(this T[] array, int index)
		{
			if (array == null || array.Length == 0 || index < 0)
			{
				return false;
			}

			return index <= array.Length - 1;
		}


	}
}