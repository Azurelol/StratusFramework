using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public class StratusSortedList<KeyType, ValueType> : SortedList<KeyType, ValueType>
	{
		private Func<ValueType, KeyType> keyFunction;

		public StratusSortedList(Func<ValueType, KeyType> keyFunction,
			int capacity = 0,
			IComparer<KeyType> comparer = null)
			: base(capacity, comparer)
		{
			this.keyFunction = keyFunction;
		}

		public StratusSortedList(IEnumerable<ValueType> values, Func<ValueType, KeyType> keyFunction,
			int capacity = 0,
			IComparer<KeyType> comparer = null)
			: this(keyFunction, capacity, comparer)
		{
			AddRange(values);
		}

		//public bool Add(KeyType key, ValueType value)
		//{
		//    if (dictionary.ContainsKey(key))
		//    {
		//        return false;
		//    }
		//    dictionary.Add(key, value);
		//    return true;
		//}

		public bool Add(ValueType value)
		{
			KeyType key = keyFunction(value);
			if (ContainsKey(key))
			{
				StratusDebug.LogError($"Item with {key} already exists in this collection!");
				return false;
			}
			Add(key, value);
			return true;
		}

		public int AddRange(IEnumerable<ValueType> values)
		{
			if (values == null)
			{
				return 0;
			}

			int failCount = 0;
			foreach (ValueType value in values)
			{
				if (!Add(value))
				{
					failCount++;
				}
			}
			return failCount;
		}

		public bool Remove(ValueType value)
		{
			KeyType key = keyFunction(value);
			if (!ContainsKey(key))
			{
				return false;
			}
			Remove(key);
			return true;
		}

		//public bool Remove(KeyType key)
		//{

		//}

	}

}