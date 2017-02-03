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
  namespace Utilities
  {
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
          return ((float)Current / (float)Total) * 100.0f;
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


}
