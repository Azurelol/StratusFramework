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
using System.Linq;

namespace Stratus 
{
  public static partial class Extensions
  {
    /// <summary>
    /// Shuffles the list using a randomized range based on its size.
    /// </summary>
    /// <typeparam name="T">The type of the list.</typeparam>
    /// <param name="list">A reference to the list.</param>
    /// <remarks>Courtesy of Mike Desjardins #UnityTips</remarks>
    /// <returns>A new, shuffled list.</returns>
    public static List<T> Shuffle<T>(this List<T> list)
    {
      for (int i = 0; i < list.Count; ++i)
      {
        T index = list[i];
        int randomIndex = UnityEngine.Random.Range(i, list.Count);
        list[i] = list[randomIndex];
        list[randomIndex] = index;
      }

      return list;
    }


    /// <summary>
    /// Returns a random element from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T Random<T>(this List<T> list)
    {
      int randomSelection = UnityEngine.Random.Range(0, list.Count);
      return list[randomSelection];
    }

    /// <summary>
    /// Finds out whether the list is currently empty or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <returns>True if the list is empty, false otherwise</returns>
    public static bool Empty<T>(this List<T> list)
    {
      if (list.Count == 0)
        return true;
      return false;
    }

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



    public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
    {
      return listToClone.Select(item => (T)item.Clone()).ToList();
    }

  }
}
