using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  public class FilteredList<T>
  {
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

    public string filter { get; private set; }
    public List<Entry> entries { get; private set; }
    public int entryCount => entries.Count;
    public Item[] items { get; private set; }
    private Func<T, string> nameFunction { get; set; }
    public int maxIndex => entries.Count - 1;

    public FilteredList(T[] items, Func<T, string> nameFunction)
    {
      this.items =  new Item[items.Length];      
      for(int i = 0; i < items.Length; ++i)
      {
        T value = items[i];
        this.items[i] = new Item() { name = nameFunction(value), value = value };
      }
      this.nameFunction = nameFunction;
      this.entries = new List<Entry>();
      this.UpdateFilter("");
    }

    public bool UpdateFilter(string filter)
    {
      if (filter == this.filter)
        return false;

      this.filter = filter;
      this.entries.Clear();

      string filterLowercase = this.filter.ToLower();

      for (int i = 0; i < this.items.Length; ++i)
      {
        string name = items[i].name.ToLower();        
        if (string.IsNullOrEmpty(this.filter) || name.Contains(filterLowercase))
        {
          Entry entry = new Entry
          {
            index = i,
            item = items[i]
          };

          if (string.Equals(name, filter, StringComparison.CurrentCultureIgnoreCase))
            this.entries.Insert(0, entry);
          else
            this.entries.Add(entry);
        }
      }

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