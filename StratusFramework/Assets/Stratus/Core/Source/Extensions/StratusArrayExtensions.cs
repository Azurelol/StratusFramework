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
		/// Checks whether the array contains the element that matches the condition
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static void Exists<T>(this T[] array, Predicate<T> predicate)
		{
			Array.Exists(array, predicate);
		}

		public static bool Contains<T>(this T[] array, T element)
		{
			foreach (T item in array)
			{
				if (item.Equals(element))
				{
					return true;
				}
			}
			return false;
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
		public static T[] AddFront<T>(this T[] array, T element)
		{
			T[] newArray = new T[array.Length + 1];
			newArray[0] = element;
			Array.Copy(array, 0, newArray, 1, array.Length);
			return newArray;
		}

		/// <summary>
		/// Copies this array, inserting the element to the front
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static T[] AddBack<T>(this T[] array, T element)
		{
			T[] newArray = new T[array.Length + 1];
			Array.Copy(array, 0, newArray, 0, array.Length);
			newArray[newArray.Length - 1] = element;
			return newArray;
		}

		/// <summary>
		/// Copies the array, without the first element present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T[] RemoveFirst<T>(this T[] array)
		{
			T[] newArray = new T[array.Length - 1];
			Array.Copy(array, 1, newArray, 0, array.Length - 1);
			return newArray;
		}

		/// <summary>
		/// Copies the array, without the first element present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T[] RemoveBack<T>(this T[] array)
		{
			T[] newArray = new T[array.Length - 1];
			Array.Copy(array, 0, newArray, 0, array.Length - 1);
			return newArray;
		}

				/// <summary>
		/// Copies the array, without the selected element present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T[] Remove<T>(this T[] array, T element)
		{
			int elementIndex = array.FindIndex(x => x.Equals(element));


			// If it's the first element
			if (elementIndex == -1)
			{
				return array;
			}
			else if (elementIndex == 0)
			{
				return array.RemoveFirst();
			}
			// If it's the last element
			else if (elementIndex == array.Length)
			{
				return array.RemoveBack();
			}

			T[] newArray = new T[array.Length - 1];
			Array.Copy(array, 0, newArray, 0, elementIndex - 1);
			Array.Copy(array, elementIndex + 1, newArray, 0, array.Length - 1);
			return newArray;
		}
	}
}