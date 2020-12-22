using System;
using System.Collections.Generic;
using System.Text;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Adds the given key-value pair if the key has not already been used
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static bool AddUnique<T, U>(this Dictionary<T, U> dictionary, T key, U value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, value);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Adds the value to the dictionary if not present, updates it otherwise
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddOrUpdate<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, Value value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="keyFunction"></param>
		public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Value, Key> keyFunction, IEnumerable<Value> values)
		{
			foreach (Value element in values)
			{
				dictionary.Add(keyFunction(element), element);
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="keyFunction"></param>
		public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Value, Key> keyFunction, params Value[] values)
		{
			dictionary.AddRange(keyFunction, (IEnumerable<Value>)values);
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="valueFunction"></param>
		public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Key, Value> valueFunction, IEnumerable<Key> keys)
		{
			foreach (Key key in keys)
			{
				dictionary.Add(key, valueFunction(key));
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value. and predicate
		/// </summary>
		public static void AddRangeWhere<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Key, Value> valueFunction, Predicate<Key> predicate, IEnumerable<Key> keys)
		{
			foreach (Key key in keys)
			{
				if (predicate(key))
				{
					dictionary.Add(key, valueFunction(key));
				}
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value. and predicate
		/// </summary>
		public static void AddRangeWhere<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Value, Key> keyFunction, Predicate<Value> predicate, IEnumerable<Value> values)
		{
			foreach (Value element in values)
			{
				if (predicate(element))
				{
					Key key = keyFunction(element);
					dictionary.Add(key, element);
				}
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value.
		/// This will not attempt to add elements with duplicate kaeys.
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="keyFunction"></param>
		public static void AddRangeUnique<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Value, Key> keyFunction, IEnumerable<Value> values)
		{
			foreach (Value element in values)
			{
				Key key = keyFunction(element);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, element);
				}
			}
		}

		/// <summary>
		/// Adds the given key-value pair if the key is not present, also adding the necessary list
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddIfMissing<Key, Value>(this Dictionary<Key, List<Value>> dictionary, Key key, Value value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new List<Value>());
			}
			dictionary[key].Add(value);
		}

		/// <summary>
		/// Invokes the given action on every element of the list if the key is present within the dictionary
		/// </summary>
		public static void TryInvoke<Key, Value>(this Dictionary<Key, List<Value>> dictionary, Key key, System.Action<Value> action)
		{
			if (dictionary.ContainsKey(key))
			{
				foreach (Value item in dictionary[key])
				{
					action(item);
				}
			}
		}

		/// <summary>
		/// Invokes the given action on the value within the dictionary if present
		/// </summary>
		public static void TryInvoke<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, System.Action<Value> action)
		{
			if (dictionary.ContainsKey(key))
			{
				action(dictionary[key]);
			}
		}

		/// <summary>
		/// Invokes the given function on the value within the dictionary if present
		/// </summary>
		public static Return TryInvoke<Key, Value, Return>(this Dictionary<Key, Value> dictionary, Key key, System.Func<Value, Return> action)
		{
			if (dictionary.ContainsKey(key))
			{
				return action(dictionary[key]);
			}
			return default(Return);
		}

		/// <summary>
		/// Returns the value from the dictionary if present, otherwise adds it (from a value function)
		/// </summary>
		public static Value GetValueOrAdd<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, Func<Key, Value> valueFunction)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, valueFunction(key));
			}
			return dictionary[key];
		}

		/// <summary>
		/// Returns the value from the dictionary if present, otherwise adds it (from a value function)
		/// </summary>
		public static Value GetValueOrDefault<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, Value defaultValue)
		{
			if (!dictionary.ContainsKey(key))
			{
				return defaultValue;
			}
			return dictionary[key];
		}

		/// <summary>
		/// Returns the value from the dictionary if present, otherwise returns null
		/// </summary>
		public static Value GetValueOrNull<Key, Value>(this Dictionary<Key, Value> dictionary, Key key)
			where Value : class
		{
			if (!dictionary.ContainsKey(key))
			{
				return null;
			}

			return dictionary[key];
		}

		/// <summary>
		/// Returns the value from the dictionary if present, otherwise throws a custom error message
		/// </summary>
		public static Value GetValueOrError<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, string errorMessage = null)
		{
			if (!dictionary.ContainsKey(key))
			{
				throw new ArgumentNullException(errorMessage != null ? errorMessage : $"The key {key} could not be found!");
			}

			return dictionary[key];
		}

		public static string ToKeyValueString<Key, Value>(this Dictionary<Key, Value> dictionary, char separator = ':', int padding = 1)
		{
			StringBuilder sb = new StringBuilder();
			foreach(var kp in dictionary)
			{
				sb.AppendLine($"{kp.Key} {separator} {kp.Value}");
			}
			return sb.ToString();
		}
	}
}
