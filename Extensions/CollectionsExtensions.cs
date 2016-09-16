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

namespace Prototype 
{
  public static class CollectionsExtensions
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
        int randomIndex = Random.Range(i, list.Count);
        list[i] = list[randomIndex];
        list[randomIndex] = index;
      }

      return list;
    }
    
  }
}
