using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns a string joining the names of the enumerable.
		/// EG: "{a,b,c,...}"
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string JoinToString<T>(this IEnumerable<T> enumerable, string separator = ",")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(enumerable.ToStringArray().Join(separator));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Returns a string joining the names of the enumerable.
		/// EG: "{a,b,c,...}"
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string JoinToString<T>(this IEnumerable<T> enumerable, Func<T, string> nameFunc, string separator = ",")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(enumerable.ToStringArray(nameFunc).Join(separator));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] ToStringArray<T>(this IEnumerable<T> enumerable)
		{
			List<string> names = new List<string>();
			foreach (T entry in enumerable)
			{
				names.Add(entry.ToString());
			}
			return names.ToArray();
		}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] ToStringArray<T>(this IEnumerable<T> enumerable, Func<T, string> nameFunc)
		{
			List<string> names = new List<string>();
			foreach (T entry in enumerable)
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
		/// Returns the first element from the sequence
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T First<T>(this IEnumerable<T> source)
		{
			return Enumerable.First(source);
		}

		/// <summary>
		/// Returns the first element in a sequence that satisfies a specified condition.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T First<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return Enumerable.First(source, predicate);
		}

		/// <summary>
		/// Returns the first element of a sequence, or a default value if no element is found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T FirstOrDefault<T>(this IEnumerable<T> source)
		{
			return Enumerable.FirstOrDefault(source);
		}

		/// <summary>
		/// Returns the first element of a sequence, or a default value if no element is found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return Enumerable.FirstOrDefault(source, predicate);
		}

		/// <summary>
		/// Returns the last element of a sequence
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T Last<T>(this IEnumerable<T> source)
		{
			return Enumerable.Last(source);
		}

		/// <summary>
		/// Returns the last element of a sequence
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T Last<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return Enumerable.Last(source, predicate);
		}

		/// <summary>
		/// Returns the last element of a sequence
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T LastOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return Enumerable.LastOrDefault(source, predicate);
		}

		/// <summary>
		/// Checks whether this enumerables has elements with duplicate keys (uniquely identifying properties),
		/// given a function that determines the key for each element
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

		/// <summary>
		/// Checks whether this enumerables has elements with duplicate keys (uniquely identifying properties),
		/// given a function that determines the key for each element
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static bool HasDuplicateKeys<T>(this IEnumerable<T> enumerable)
			where T : IComparable
		{
			return !enumerable.All(new HashSet<T>().Add);
		}

		/// <summary>
		/// Finds the first element of this enumerable that matches the predicate function
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static T FindFirst<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
		{
			foreach (T element in enumerable)
			{
				if (predicate(element))
				{
					return element;
				}
			}
			return default(T);
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
		/// Returns the first element in this list that has a duplicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static T FindFirstDuplicate<T>(this IEnumerable<T> enumerable)
		{
			HashSet<T> hashset = new HashSet<T>();
			foreach (T element in enumerable)
			{
				if (hashset.Contains(element))
				{
					return element;
				}
				hashset.Add(element);
			}
			return default(T);
		}


		/// <summary>
		/// Returns an array with no null elements
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static T[] TruncateNull<T>(this IEnumerable<T> enumerable)
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

		/// <summary>
		/// Filters the elements of an IEnumerable based on a specified type.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
		{
			return Enumerable.OfType<TResult>(source);
		}

		/// <summary>
		/// Converts an enumerable from one type to another through a conversion function
		/// </summary>
		public static IEnumerable<U> Convert<T, U>(this IEnumerable<T> source, Func<T, U> function)
		{
			foreach (T item in source)
			{
				yield return function(item);
			}
		}

		/// <summary>
		/// Converts an enumerable from one type to an array of another through a conversion function
		/// </summary>
		public static U[] ToArray<T, U>(this IEnumerable<T> source, Func<T, U> function)
		{
			return source.Convert(function).ToArray();
		}

		/// <summary>
		/// Perform an action on each item.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T item in source)
			{
				action(item);
			}
		}

		/// <summary>
		/// Perform an action on each item, in reverse
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEachReverse<T>(this IEnumerable<T> source, Action<T> action)
		{
			source.Reverse().ForEach(action);
		}

		/// <summary>
		/// Perform an action on each item that isn't null
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEachNotNull<T>(this IEnumerable<T> source, Action<T> action)
			where T : class
		{
			if (source == null)
			{
				return;
			}

			foreach (T item in source)
			{
				if (item != null)
				{
					action(item);
				}
			}
		}

		/// <summary>
		/// Perform an action on each item that isn't null
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<U> ForEachNotNull<T, U>(this IEnumerable<T> source, Func<T, U> func)
			where T : class
		{
			if (source == null)
			{
				yield break;
			}

			foreach (T item in source)
			{
				if (item != null)
				{
					yield return func(item);
				}
			}
		}

		/// <summary>
		/// Perform an action on each item, with an iteration counter
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
		{
			int counter = 0;

			foreach (T item in source)
			{
				action(item, counter++);
			}

			return source;
		}

		/// <summary>
		/// Perform an action on each item that isn't null, with an iteration counter
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<T> ForEachNotNull<T>(this IEnumerable<T> source, Action<T, int> action)
			where T : class
		{
			int counter = 0;

			foreach (T item in source)
			{
				if (item != null)
				{
					action(item, counter++);
				}
			}

			return source;
		}

		/// <summary>
		/// Add a collection to the end of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="append">The collection to append.</param>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, IEnumerable<T> append)
		{
			foreach (T item in source)
			{
				yield return item;
			}

			foreach (T item in append)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Add a collection to the end of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="append">The collection to append.</param>
		public static IEnumerable<T> AppendWhere<T>(this IEnumerable<T> source, Predicate<T> predicate, IEnumerable<T> append)
		{
			foreach (T item in source)
			{
				yield return item;
			}

			foreach (T item in append)
			{
				if (predicate(item))
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Add a collection to the beginning of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="prepend">The collection to append.</param>
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, IEnumerable<T> prepend)
		{
			foreach (T item in prepend)
			{
				yield return item;
			}

			foreach (T item in source)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Add a collection to the beginning of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="prepend">The collection to append.</param>
		public static IEnumerable<T> PrependWhere<T>(this IEnumerable<T> source, Predicate<T> predicate, IEnumerable<T> prepend)
		{
			foreach (T item in prepend)
			{
				if (predicate(item))
				{
					yield return item;
				}
			}

			foreach (T item in source)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Filters a sequence of values based on a predicate. 
		/// Each element's index is used in the logic of the predicate function.
		/// </summary>
		public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return Enumerable.Where(source, predicate);
		}

		/// <summary>
		/// Finds the intersection of a group of groups.
		/// </summary>
		public static IEnumerable<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> groups)
		{
			HashSet<T> hashSet = null;

			foreach (IEnumerable<T> group in groups)
			{
				if (hashSet == null)
				{
					hashSet = new HashSet<T>(group);
				}
				else
				{
					hashSet.IntersectWith(group);
				}
			}

			return hashSet == null ? Enumerable.Empty<T>() : hashSet.AsEnumerable();
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the element with maximum value.
		/// </summary>
		public static T SelectMax<T>(this IEnumerable<T> source, Func<T, int> selector)
		{
			return Enumerable.Range(0, int.MaxValue)
			   .Zip(source, (index, element) => (selector(element), index, element)).Max().Item3;
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the element with minimum value.
		/// </summary>
		public static T SelectMin<T>(this IEnumerable<T> source, Func<T, int> selector)
		{
			return Enumerable.Range(0, int.MaxValue)
			   .Zip(source, (index, element) => (selector(element), index, element)).Min().Item3;
		}

		/// <summary>
		/// Sorts the elements of a sequence in ascending or descending order. 
		/// </summary>
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

		/// <summary>
		/// Performs a subsequent ordering of the elements in a sequence in ascending or descending order.
		/// </summary>
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
		/// Returns a dictionary from the given enumerable, given a function to get the key for each value
		/// </summary>
		public static Dictionary<Key, Value> ToDictionary<Key, Value>(this IEnumerable<Value> source, Func<Value, Key> keyFunction, bool unique = true)
		{
			Dictionary<Key, Value> dictionary = new Dictionary<Key, Value>();
			if (unique)
			{
				dictionary.AddRange(keyFunction, source);
			}
			else
			{
				dictionary.AddRangeUnique(keyFunction, source);
			}

			return dictionary;
		}

		/// <summary>
		/// Returns a dictionary from the given enumerable, given a function to get a value for each key and a predicate
		/// </summary>
		public static Dictionary<Key, Value> ToDictionary<Key, Value>(this IEnumerable<Key> source, Func<Key, Value> valueFunction, Predicate<Key> predicate = null)
		{
			Dictionary<Key, Value> dictionary = new Dictionary<Key, Value>();
			if (predicate != null)
			{
				dictionary.AddRangeWhere(valueFunction, predicate, source);
			}
			else
			{
				dictionary.AddRange(valueFunction, source);
			}

			return dictionary;
		}

		/// <summary>
		/// Builds a hashset out of the given enumerable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}

		public static ICollection<T> CacheToCollection<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable is ICollection<T>)
			{
				return (ICollection<T>)enumerable;
			}
			else
			{
				return enumerable.ToList();
			}
		}
	}
}