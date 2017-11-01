/******************************************************************************/
/*!
@file   Utilities.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;
using UnityEngine;

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
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The array being used
    /// </summary>
    private T[] Array;

    /// <summary>
    /// The amount of 0-indexed elements in the array
    /// </summary>
    private int indexSize { get { return Array.Length - 1; } }

    /// <summary>
    /// The current index
    /// </summary>
    private int currentIndex = 0;

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T first => Array[0];

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T current => Array[currentIndex];

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T last => Array[indexSize];

    /// <summary>
    /// Retrieves a random element in the array.
    /// </summary>
    /// <returns></returns>
    public T random
    {
      get
      {
        var randomIndex = UnityEngine.Random.Range(0, indexSize);
        return Array[randomIndex];
      }
    }

    /// <summary>
    /// Function to invoke once the index of this array has changed
    /// </summary>
    public System.Action<T> onIndexChanged { get; set; }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public ArrayNavigate()
    {
      currentIndex = 0;
    }

    public ArrayNavigate(T[] array)
    {
      Array = array;
      currentIndex = 0;
    }

    /// <summary>
    /// Sets an updated array to use for navigation
    /// </summary>
    /// <param name="array"></param>
    public void Set(T[] array)
    {
      Array = array;
      currentIndex = 0;
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
          currentIndex = index;
          return true;
        }
        index++;
      }

      return false;
    }

    /// <summary>
    /// Navigates along the array in a given direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public T Navigate(Direction dir)
    {
      if (dir == Direction.Right || dir == Direction.Up)
      {
        if (currentIndex < indexSize)
        {
          currentIndex++;          
          onIndexChanged?.Invoke(current);
        }
      }
      else if (dir == Direction.Left || dir == Direction.Down)
      {
        if (currentIndex != 0)
        {
          currentIndex--;
          onIndexChanged?.Invoke(current);
        }
      }

      return Array[currentIndex];
    }
  }






}
