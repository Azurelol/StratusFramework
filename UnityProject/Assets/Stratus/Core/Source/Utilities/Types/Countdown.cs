using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace Utilities
  {
    /// <summary>
    /// Counts down from the specified amount of time. 
    /// From <i>n</i> amount of time to 0.0f;
    /// </summary>
    public class Countdown : Timer
    {
      public override float remaining => this.current;

      /// <summary>
      /// Constructor for the countdown.
      /// </summary>
      /// <param name="total">The total amount of time to countdown.</param>
      public Countdown(float total)
      {
        this.total = total;
        this.current = total;
      }

      /// <summary>
      /// Resets the countdown.
      /// </summary>
      protected override void OnReset()
      {
        this.current = this.total;
      }

      /// <summary>
      /// Sets the countdown's time
      /// </summary>
      /// <param name="time"></param>
      public override void Set(float time)
      {
        this.total = time;
        this.current = total;
      }

      /// <summary>
      /// Updates the timer by the specified delta time.
      /// </summary>
      /// <returns>True if is done, false otherwise</returns>
      public override bool Update(float dt)
      {
        if (this.current <= 0.0f)
        {
          this.current = 0f;
          Finish();
          return true;
        }

        this.current -= dt;
        return false;
      }
    }
  }

}