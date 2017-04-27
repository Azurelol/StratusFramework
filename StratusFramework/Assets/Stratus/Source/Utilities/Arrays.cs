/******************************************************************************/
/*!
@file   Utilities.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;

namespace Stratus
{
  public class ArrayNavigate
  {
    public enum Direction { Up, Down, Left, Right }
  }
  
  /// <summary>
  /// Provides a generic way to navigate a 1D array using directional axis.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ArrayNavigate<T> : ArrayNavigate
  {

    T[] Array;
    int ElementCount { get { return Array.Length - 1; } }
    int ElementIndex = 0;

    public ArrayNavigate()
    {
      ElementIndex = 0;
    }

    public ArrayNavigate(T[] array)
    {
      Array = array;
      ElementIndex = 0;
    }

    /// <summary>
    /// Sets an updated array to use for navigation
    /// </summary>
    /// <param name="array"></param>
    public void Set(T[] array)
    {
      Array = array;
      ElementIndex = 0;
    }

    /// <summary>
    /// Retrieves the first element in the array.
    /// </summary>
    /// <returns></returns>
    public T First()
    {
      return Array[0];
    }

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T Last()
    {
      return Array[ElementCount];
    }

    /// <summary>
    /// Updates the current index to point at the given element
    /// </summary>
    /// <param name="element"></param>
    public bool UpdateIndex(T element)
    {
      // Look for the element in the array
      var index = 0;
      foreach(var e in Array)
      {
        // The element was found
        if (e.Equals(element))
        {
          ElementIndex = index;
          return true;
        }
        index++;
      }

      return false;
    }

    /// <summary>
    /// Retrieves a random element in the array.
    /// </summary>
    /// <returns></returns>
    public T Random()
    {
      var randomIndex = UnityEngine.Random.Range(0, ElementCount);
      return Array[randomIndex];
    }

    public T Navigate(Direction dir)
    {
      if (dir == Direction.Left || dir == Direction.Up)
      {
        if (ElementIndex < ElementCount)
          ElementIndex++;
      }
      else if (dir == Direction.Right || dir == Direction.Down)
      {
        if (ElementIndex != 0)
          ElementIndex--;
      }

      return Array[ElementIndex];
    }
  }






}
