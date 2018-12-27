using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.OrderBy(selector);
			}
			else
			{
				return source.OrderByDescending(selector);
			}
		}

		public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.ThenBy(selector);
			}
			else
			{
				return source.ThenByDescending(selector);
			}
		}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] TypeNames<T>(this IEnumerable<T> enumerable)
		{
			List<string> names = new List<string>();
			foreach (T item in enumerable)
			{
				names.Add(item.GetType().Name);
			}
			return names.ToArray();
		}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] TypeNames(this IEnumerable<Type> enumerable)
		{
			List<string> names = new List<string>();
			foreach (Type item in enumerable)
			{
				names.Add(item.Name);
			}
			return names.ToArray();
		}

		/// <summary>
		/// Checks whether this list has elements with duplicate keys, given a function
		/// that extracts the key for each element
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static bool HasDuplicateKeys<T>(this IEnumerable<T> enumerable, Func<T, string> keyFunction)
		{
			HashSet<string> hashset = new HashSet<string>();
			foreach (T element in enumerable)
			{
				string key = keyFunction(element);
				if (hashset.Contains(key))
				{
					return true;
				}
				hashset.Add(key);
			}
			return false;
		}

		///// <summary>
		///// Returns an array of strings, consisting of the names identified on their name property
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="enumerable"></param>
		///// <returns></returns>
		//public static string[] Names<T>(this IEnumerable<T> enumerable) where T : UnityEngine.Object
		//{
		//	List<string> names = new List<string>();
		//	foreach (var entry in enumerable)
		//	{
		//		names.Add(entry.name);
		//	}
		//	return names.ToArray();
		//}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] Names<T>(this IEnumerable<T> enumerable, Func<T, string> nameFunc)
		{
			List<string> names = new List<string>();
			foreach (var entry in enumerable)
			{
				names.Add(nameFunc(entry));
			}
			return names.ToArray();
		}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] Names<T>(this IEnumerable<T> enumerable)
		{
			List<string> names = new List<string>();
			foreach (var entry in enumerable)
			{
				names.Add(entry.ToString());
			}
			return names.ToArray();
		}

		/// <summary>
		/// Returns the first element in this list that has a duplicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static T FindFirstDuplicate<T>(this IEnumerable<T> enumerable, Func<T, string> keyFunction)
		{
			HashSet<string> hashset = new HashSet<string>();
			foreach (T element in enumerable)
			{
				string key = keyFunction(element);
				if (hashset.Contains(key))
				{
					return element;
				}
				hashset.Add(key);
			}
			return default(T);
		}


		/// <summary>
		/// Returns an array with no null elements
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static T[] TrimNull<T>(this IEnumerable<T> enumerable)
		{
			List<T> list = new List<T>();
			foreach (T item in enumerable)
			{
				if (item != null)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public static U[] OfType<T, U>(this IEnumerable<T> enumerable)
			where T : class
			where U : class, T
		{
			return enumerable.Select(c => c as U).Where(c => c != null).ToArray();
		}
	}

	public abstract class ConstrainedEnumParser<TClass> where TClass : class
		// value type constraint S ("TEnum") depends on reference type T ("TClass") [and on struct]
	{
		// internal constructor, to prevent this class from being inherited outside this code
		internal ConstrainedEnumParser()
		{
		}
		// Parse using pragmatic/adhoc hard cast:
		//  - struct + class = enum
		//  - 'guaranteed' call from derived <System.Enum>-constrained type EnumUtils
		public static TEnum Parse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct, TClass
		{
			return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
		}
		public static bool TryParse<TEnum>(string value, out TEnum result, bool ignoreCase = false, TEnum defaultValue = default(TEnum)) where TEnum : struct, TClass // value type constraint S depending on T
		{
			bool didParse = Enum.TryParse(value, ignoreCase, out result);
			if (didParse == false)
			{
				result = defaultValue;
			}
			return didParse;
		}
		public static TEnum ParseOrDefault<TEnum>(string value, bool ignoreCase = false, TEnum defaultValue = default(TEnum)) where TEnum : struct, TClass // value type constraint S depending on T
		{
			if (string.IsNullOrEmpty(value)) { return defaultValue; }
			if (Enum.TryParse(value, ignoreCase, out TEnum result)) { return result; }
			return defaultValue;
		}
	}



}