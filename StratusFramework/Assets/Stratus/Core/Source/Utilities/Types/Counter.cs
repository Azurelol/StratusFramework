/******************************************************************************/
/*!
@file   Counter.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;

namespace Stratus
{
  /// <summary>
  /// A counter of items, which is incremented one at a time.
  /// </summary>
  [Serializable]
  public struct Counter
  {
    /// <summary>
    /// The upper bound of this counter
    /// </summary>
    public int total { get; private set; }

    /// <summary>
    /// The current tally of this counter
    /// </summary>
    public int current { get; private set; }

    /// <summary>
    /// Whether the counter has been filled
    /// </summary>
    public bool isFull
    {
      get
      {
        if (current == total)
          return true;
        return false;
      }
    }

    /// <summary>
    /// How filled is this counter as a value between 0 and 100
    /// </summary>
    public float percentage
    {
      get
      {
        return ((float)current / (float)total) * 100.0f;
      }
    }

    /// <summary>
    /// A string value showing completion of this counter
    /// </summary>
    public string completion { get { return current + "/" + total; } }

    public Counter(int total)
    {
      this.total = total;
      current = 0;
    }

    /// <summary>
    /// Increments this counter
    /// </summary>
    /// <returns>Returns true if the counter is full, false otherwise</returns>
    public bool Increment()
    {
      if (isFull)
        return true;

      current++;

      if (isFull)
        return true;

      return false;
    }

    /// <summary>
    /// Resets the counter
    /// </summary>
    public void Reset()
    {
      current = 0;
    }

  }


}
