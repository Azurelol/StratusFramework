using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// A generic dropdown list that refers to a list of any given Object type
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DropdownList<T> where T : UnityEngine.Object
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public string[] displayedOptions { get; private set; }
    public int selectedIndex { get; set; }
    public T selected => isList ? list[selectedIndex] : array[selectedIndex];

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private List<T> list;
    private T[] array;
    private bool isList;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public DropdownList(List<T> list, T initial = null)
    {
      this.list = list;
      isList = true;
      displayedOptions = list.Names();

      if (initial)
        SetIndex(initial);
    }

    public DropdownList(List<T> list, int index = 0)
    {
      this.list = list;
      isList = true;
      displayedOptions = list.Names();
      selectedIndex = 0;
    }

    public DropdownList(T[] array, T initial = null)
    {
      this.array = array;
      isList = false;
      displayedOptions = array.Names();

      if (initial)
        SetIndex(initial);
    }

    public DropdownList(T[] array, int index = 0)
    {
      this.array = array;
      isList = false;
      displayedOptions = array.Names();
      selectedIndex = 0;
    }

    /// <summary>
    /// Sets the current index of this list to that of the given element
    /// </summary>
    /// <param name="element"></param>
    public void SetIndex(T element)
    {
      if (isList)
        selectedIndex = list.FindIndex(x => x == element);
      else
        selectedIndex = array.FindIndex(x => x == element);
    }

  }
}