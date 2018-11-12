using System;
using System.Collections.Generic;

namespace Stratus
{
	public class FilteredList<T>
	{
		//--------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------/
		public struct Entry
		{
			public int index;
			public Item item;
		}

		public class Item
		{
			public string name;
			public T value;
		}

		protected struct EntriesHook
		{
			public List<T> values;
			public System.Action onFinished;
		}


		private string _filter;

		//--------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------/
		public string filter
		{
			get { return _filter; }
			private set { UpdateFilter(value); }
		}

		public List<Entry> currentEntries { get; private set; }
		public Item[] entries { get; private set; }
		public string[] displayOptions { get; private set; }
		public int currentEntryCount => this.currentEntries.Count;
		private Func<T, string> nameFunction { get; set; }
		public int maxIndex => this.currentEntries.Count - 1;
		
		//--------------------------------------------------------------------------/
		// CTOR
		//--------------------------------------------------------------------------/
		public FilteredList(T[] items, Func<T, string> nameFunction)
		{
			this.entries = new Item[items.Length];
			for (int i = 0; i < items.Length; ++i)
			{
				T value = items[i];
				this.entries[i] = new Item() { name = nameFunction(value), value = value };
			}
			this.nameFunction = nameFunction;
			this.currentEntries = new List<Entry>();
			this.UpdateFilter(string.Empty);
		}

		//--------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------/
		public bool UpdateFilter(string filter)
		{
			// Set the filter
			if (filter == this._filter)
			{
				return false;
			}
			this._filter = filter;
			string filterLowercase = this.filter.ToLower();

			// Update the entries
			this.currentEntries.Clear();

			// Optional
			List<string> displayOptions = new List<string>();
			
			for (int i = 0; i < this.entries.Length; ++i)
			{
				string name = this.entries[i].name.ToLower();
				if (string.IsNullOrEmpty(this.filter) || name.Contains(filterLowercase))
				{
					Entry entry = new Entry
					{
						index = i,
						item = this.entries[i]
					};

					if (string.Equals(name, filter, StringComparison.CurrentCultureIgnoreCase))
					{
						this.currentEntries.Insert(0, entry);
						displayOptions.Insert(0, entries[i].name);
					}
					else
					{
						this.currentEntries.Add(entry);
						displayOptions.Add(entries[i].name);
					}					
				}
			}

			this.displayOptions = displayOptions.ToArray();
			return true;
		}
	}

	public class FilteredStringList : FilteredList<string>
	{
		private static Func<string, string> stringNameFunction { get; } = (string value) => value;		

		public FilteredStringList(string[] items) : base(items, stringNameFunction)
		{
		}

	}

}