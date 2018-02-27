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
  /// <summary>
  /// Base class for the array navigator
  /// </summary>
  public class ArrayNavigatorBase
  {
    public enum Direction { Up, Down, Left, Right }
  }
  
  /// <summary>
  /// Provides a generic way to navigate a 1D array using directional axis.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ArrayNavigator<T> : ArrayNavigatorBase
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T current => array[currentIndex];

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T first => array[0];

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T previous => array[previousIndex];

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T last => array[indexSize];

    /// <summary>
    /// Retrieves a random element in the array.
    /// </summary>
    /// <returns></returns>
    public T random
    {
      get
      {
        var randomIndex = UnityEngine.Random.Range(0, indexSize);
        return array[randomIndex];
      }
    }

    /// <summary>
    /// Function to invoke once the index of this array has changed
    /// </summary>
    public System.Action<T> onIndexChanged { get; set; }

    /// <summary>
    /// Whether on navigating to the end of the array, we loop around
    /// </summary>
    public bool loop { get; set; }

    /// <summary>
    /// The amount of 0-indexed elements in the array
    /// </summary>
    private int indexSize { get { return array.Length - 1; } }
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/

    /// <summary>
    /// The array being used
    /// </summary>
    private T[] array;

    /// <summary>
    /// The current index
    /// </summary>
    private int currentIndex = 0;

    /// <summary>
    /// The current index
    /// </summary>
    private int previousIndex = 0;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public ArrayNavigator()
    {
      currentIndex = 0;
    }

    public ArrayNavigator(T[] array, bool loop = false)
    {
      this.array = array;
      this.loop = loop;
      this.currentIndex = 0;
    }

    /// <summary>
    /// Sets an updated array to use for navigation
    /// </summary>
    /// <param name="array"></param>
    public void Set(T[] array)
    {
      this.array = array;
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
      foreach(var e in array)
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
          previousIndex = currentIndex;
          currentIndex++;          
          onIndexChanged?.Invoke(current);
        }
        else if (loop)
        {
          Trace.Script("Looping around");
          previousIndex = currentIndex;
          currentIndex = 0;
          onIndexChanged?.Invoke(current);
        }
      }
      else if (dir == Direction.Left || dir == Direction.Down)
      {
        if (currentIndex != 0)
        {
          previousIndex = currentIndex;
          currentIndex--;
          onIndexChanged?.Invoke(current);
        }
        else if (loop)
        {
          previousIndex = currentIndex;
          currentIndex = indexSize;
          onIndexChanged?.Invoke(current);
        }
      }

      return array[currentIndex];
    }
  }






}
