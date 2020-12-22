using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// A two-way dictionary
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public class StratusBictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
	{
		//-------------------------------------------------------------------------/
		// Fields
		//-------------------------------------------------------------------------/
		private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
		private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

		//-------------------------------------------------------------------------/
		// Properties
		//-------------------------------------------------------------------------/
		/// <summary>
		/// Maps T1 to T2
		/// </summary>
		public Indexer<T1, T2> forward { get; private set; }
		/// <summary>
		/// Maps T2 to T1
		/// </summary>
		public Indexer<T2, T1> reverse { get; private set; }
		/// <summary>
		/// Indexer given T2
		/// </summary>
		public T1 this[T2 index]
		{
			get => _reverse[index];
		}
		/// <summary>
		/// Indexer given T1
		/// </summary>
		public T2 this[T1 index]
		{
			get => _forward[index];
		}
		/// <summary>
		/// Length of elements
		/// </summary>
		public int Count => _forward.Count;

		//-------------------------------------------------------------------------/
		// CTOR
		//-------------------------------------------------------------------------/
		public StratusBictionary()
		{
			forward = new Indexer<T1, T2>(_forward);
			reverse = new Indexer<T2, T1>(_reverse);
		}

		//-------------------------------------------------------------------------/
		// Methods
		//-------------------------------------------------------------------------/
		public void Add(T1 t1, T2 t2)
		{
			_forward.Add(t1, t2);
			_reverse.Add(t2, t1);
		}

		public bool Contains(T1 item) => _forward.ContainsKey(item);
		public bool Contains(T2 item) => _reverse.ContainsKey(item);

		public bool Remove(T1 t1)
		{
			if (_forward.ContainsKey(t1))
			{
				T2 t2 = _forward[t1];
				if (_reverse.ContainsKey(t2))
				{
					_forward.Remove(t1);
					_reverse.Remove(t2);
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}

		public bool Remove(T2 t2)
		{
			if (_reverse.ContainsKey(t2))
			{
				T1 t1 = _reverse[t2];
				if (_forward.ContainsKey(t1))
				{
					_reverse.Remove(t2);
					_forward.Remove(t1);
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}

		public void Clear()
		{
			_forward.Clear();
			_reverse.Clear();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
		{
			return _forward.GetEnumerator();
		}

		public class Indexer<T3, T4>
		{
			private readonly Dictionary<T3, T4> _dictionary;

			public Indexer(Dictionary<T3, T4> dictionary)
			{
				_dictionary = dictionary;
			}

			public T4 this[T3 index]
			{
				get { return _dictionary[index]; }
				set { _dictionary[index] = value; }
			}

			public bool Contains(T3 key)
			{
				return _dictionary.ContainsKey(key);
			}
		}
	}

}