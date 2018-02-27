/******************************************************************************/
/*!
@file   Time.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace Utilities
  {   

    /// <summary>
    /// The base class for all timers
    /// </summary>
    public abstract class Timer 
    {
      /// <summary>
      /// A zero-argument for when a timer has finished (which depends on its type)
      /// </summary>
      public delegate void Callback();

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// Returns the maximum duration for this timer
      /// </summary>
      public float total { protected set; get; }
      /// <summary>
      /// Returns the currently elapsed time for this timer
      /// </summary>
      public float current { protected set; get; }
      /// <summary>
      /// The remaining duration on the timer before its complete
      /// </summary>
      public abstract float remaining { get; }
      /// <summary>
      /// Whether this timer has finished running
      /// </summary>
      public bool isFinished { protected set; get; }
      /// <summary>
      /// The current progress in this timer as a percentage value ranging from 0 to 1.
      /// </summary>
      public float normalizedProgress { get { if (total == 0.0f) return 0.0f; return (current / total); } }
      /// <summary>
      /// The current progress in this timer as a percentage value ranging from 0 to 100.
      /// </summary>
      public float progress { get { if (total == 0.0f) return 0.0f; return (current / total) * 100.0f; } }
      /// <summary>
      /// Whether this timer should automatically reset when it has finished
      /// </summary>
      public bool resetOnFinished { get; set; } = false;

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The callback function for when this timer finishes
      /// </summary>
      Callback onFinished;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      public abstract void Set(float time);
      public abstract bool Update(float dt);
      protected abstract void OnReset();
            
      //------------------------------------------------------------------------/
      // Ro
      //------------------------------------------------------------------------/
      /// <summary>
      /// Finishes the timer
      /// </summary>
      public void Finish()
      {
        if (!isFinished)
          this.onFinished?.Invoke();
        isFinished = true;
        if (resetOnFinished)
          Reset();
      }

      /// <summary>
      /// Sets a callback function to be called when this timer finishes
      /// </summary>
      /// <param name="onFinished"></param>
      public void SetCallback(Callback onFinished)
      {
        this.onFinished = onFinished;
      }

      /// <summary>
      /// Resets the timer
      /// </summary>
      public void Reset()
      {
        isFinished = false;
        this.OnReset();
      }

      /// <summary>
      /// Updates the timer by the default delta time (Time.deltaTime)
      /// </summary>
      /// <returns>True if is done, false otherwise</returns>
      public bool Update()
      {
        if (isFinished)
          return true;
        return Update(Time.deltaTime);
      }

      /// <summary>
      /// Updates the timer, invoking the provided callback upon completion, then resets the timer.
      /// </summary>
      /// <param name="dt"></param>
      public void AutomaticUpdate(float dt)
      {
        Update(dt);
        if (this.isFinished)
        {
          onFinished?.Invoke();
          this.Reset();
        }
      }
    }
     



  }

}
