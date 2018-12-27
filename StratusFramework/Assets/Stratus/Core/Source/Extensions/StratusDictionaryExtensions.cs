using System;
using System.Collections.Generic;


namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Copies every element of the list into the stack.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack">The stack.</param>
		/// <param name="list">The list</param>
		public static void Copy<T>(this Stack<T> stack, List<T> list)
		{
			foreach (T element in list)
			{
				stack.Push(element);
			}
		}

		/// <summary>
		/// Adds the given key-value pair if the key has not already been used
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddIfMissing<T, U>(this Dictionary<T, U> dictionary, T key, U value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, value);
			}
		}

		/// <summary>
		/// Adds the given key-value pair if the key is not present, also adding the necessary list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddListIfMissing<T, U>(this Dictionary<T, List<U>> dictionary, T key, U value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new List<U>());
			}
			dictionary[key].Add(value);
		}

		public static void InvokeIfKeyPresent<T, U>(this Dictionary<T, List<U>> dictionary, T key, System.Action<U> invoke)
		{
			if (dictionary.ContainsKey(key))
			{
				foreach (U item in dictionary[key])
				{
					invoke(item);
				}
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="list"></param>
		/// <param name="keyFunction"></param>
		public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, IEnumerable<Value> list, Func<Value, Key> keyFunction)
		{
			foreach (Value element in list)
			{
				dictionary.Add(keyFunction(element), element);
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value.
		/// This will not attempt to add elements with duplicate kaeys.
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="list"></param>
		/// <param name="keyFunction"></param>
		public static void AddRangeUnique<Key, Value>(this Dictionary<Key, Value> dictionary, IEnumerable<Value> list, Func<Value, Key> keyFunction)
		{
			foreach (Value element in list)
			{
				Key key = keyFunction(element);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, element);
				}
			}
		}

		public static Value GetValueOrError<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, string errorMessage = null)
		{
			if (!dictionary.ContainsKey(key))
			{
				throw new ArgumentNullException(errorMessage != null ? errorMessage : $"The key {key} could not be found!");
			}

			return dictionary[key];
		}

		public static Value GetValueOrNull<Key, Value>(this Dictionary<Key, Value> dictionary, Key key)
			where Value : class
		{
			if (!dictionary.ContainsKey(key))
			{
				return null;
			}

			return dictionary[key];
		}


		public static Value GetValueAddIfMissing<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, Func<Key, Value> valueFunction)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, valueFunction(key));
			}
			return dictionary[key];
		}




	}
}
