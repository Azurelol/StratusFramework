/******************************************************************************/
/*!
@file   CollectionsExtensions.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;


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
      foreach(var element in list)
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
        dictionary.Add(key, value);
    }

    /// <summary>
    /// Adds the given list to the dictionary, provided a function that will extract the key for each value
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="list"></param>
    /// <param name="keyFunction"></param>
    public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, List<Value> list, Func<Value, Key> keyFunction)
    {
      foreach (var element in list)
        dictionary.Add(keyFunction(element), element);
    }

    public static Value TryGetValue<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, string errorMessage = null) 
    {
      if (!dictionary.ContainsKey(key))
        throw new ArgumentNullException(errorMessage != null ? errorMessage  : $"The key {key} could not be found!");
      return dictionary[key];
    }
    



  }
}
