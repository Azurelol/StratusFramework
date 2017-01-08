/******************************************************************************/
/*!
@file   Utilities.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using System;


namespace Stratus
{
  public class ArrayNavigate
  {
    public enum Direction { Forward, Backward, Up, Down, Left, Right }
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



  /// <summary>
  /// A counter which is incremented one at a time.
  /// </summary>
  [Serializable]
  public struct Counter
  {
    public int Total;
    public int Current;

    public bool IsFull
    {
      get
      {
        if (Current == Total)
          return true;
        return false;
      }
    }

    public float Percentage
    {
      get
      {
        return ( (float)Current / (float)Total) * 100.0f;
      }
    }

    public string Print { get { return Current + "/" + Total; } }

    public Counter(int total)
    {
      Total = total;
      Current = 0;
    }

    /// <summary>
    /// Increments this counter
    /// </summary>
    /// <returns>True if the counter is full, false otherwise</returns>
    public bool Increment()
    {
      if (IsFull)
        return true;

      Current++;

      if (IsFull)
        return true;

      return false;
    }

  }


}
