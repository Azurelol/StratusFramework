using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Utlity class for generating content for a SelectableGrid
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SelectableGrid<T> where T : class
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public string[] content { get; private set; }
    public int columns { get; private set; }
    public int rows { get; private set; }
    public int selectedIndex { get; set; }
    public T selected => array[selectedIndex];
    public bool valid => rows > 0 && columns > 0;

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private List<string> columnHeaders = new List<string>();
    private List<Func<T, string>> columnFunctions = new List<Func<T, string>>();
    private T[] array;
    private List<T> list;
    private bool isList;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public SelectableGrid(T[] array)
    {
      this.array = array;
      this.isList = false;
      this.rows = this.array.Length;
      this.content = new string[rows * columns];
    }

    public SelectableGrid(List<T> list)
    {
      this.list = list;
      this.isList = true;
      this.rows = this.list.Count;
      this.content = new string[rows * columns];
    }

    //------------------------------------------------------------------------/
    // Adds a column to the grid
    //------------------------------------------------------------------------/
    public void AddColumns(params Func<T, string>[] columnFunctions)
    {
      foreach (var func in columnFunctions)
      {
        this.AddColumn(func);
      }
    }

    public void AddColumn(Func<T, string> columnFunction)
    {
      this.columnFunctions.Add(columnFunction);
      this.columns++;
    }

    public void Update()
    {
      for (int r = 0; r < this.rows; ++r)
      {
        for (int c = 0; c < this.columns; ++c)
        {
          int index = (r * this.columns) + c;
          T element = this.isList ? this.list[index] : this.array[index];
          this.content[index] = this.columnFunctions[index](element);
        }
      }
    }


  }

}