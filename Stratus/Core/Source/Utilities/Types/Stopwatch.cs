using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace Utilities
  {
    /// <summary>
    /// Counts up to the specified amount of time, starting from 0.0f.
    /// </summary>
    public class Stopwatch : Timer 
    {
      public override float remaining => this.total - this.current;

      /// <summary>
      /// Constructor for the countdown.
      /// </summary>
      /// <param name="total">The total amount of time to countdown.</param>
      public Stopwatch(float total)
      {
        this.total = total;
        this.current = 0.0f;
      }

      /// <summary>
      /// Resets the countdown.
      /// </summary>
      protected override void OnReset()
      {
        this.current = 0.0f;
        this.isFinished = false;
      }

      /// <summary>
      /// Sets the timer
      /// </summary>
      /// <param name="total"></param>
      public override void Set(float total)
      {
        this.total = total;
        this.current = 0.0f;
      }

      /// <summary>
      /// Updates the timer by the specified delta time.
      /// </summary>
      /// <returns>True if is done, false otherwise</returns>
      public override bool Update(float dt)
      {
        if (this.current >= this.total)
        {
          this.current = this.total;
          Finish();
          return true;
        }

        this.current += dt;
        return false;
      }
    }
  }
}