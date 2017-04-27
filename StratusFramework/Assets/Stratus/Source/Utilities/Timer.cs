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
    public abstract class BaseTimer
    {
      public abstract bool Update(float dt);
    }

    public abstract class Timer : BaseTimer
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      public delegate void Callback();

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// Returns the maximum duration for this timer
      /// </summary>
      public float Total { get { return Total_; } }
      /// <summary>
      /// Returns the currently elapsed time for this timer
      /// </summary>
      public float Current { get { return Current_; } }
      /// <summary>
      /// Whether this timer has finished running
      /// </summary>
      public bool IsFinished { get { return Finished_; } }
      /// <summary>
      /// The current progress in this timer as a percentage value ranging from 0 to 1.
      /// </summary>
      public float Progress { get { if (Total == 0.0f) return 0.0f; return (Current / Total); } }
      /// <summary>
      /// The current progress in this timer as a percentage value ranging from 0 to 100.
      /// </summary>
      public float Percentage { get { if (Total == 0.0f) return 0.0f; return (Current / Total) * 100.0f; } }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      protected float Total_;
      protected float Current_;
      protected bool Finished_ = false;
      /// <summary>
      /// The callback function for when this timer finishes
      /// </summary>
      Callback OnFinished;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      public abstract void Set(float time);
      protected abstract void OnReset();
            
      //------------------------------------------------------------------------/
      // Ro
      //------------------------------------------------------------------------/
      /// <summary>
      /// Finishes the timer
      /// </summary>
      public void Finish()
      {
        Finished_ = true;
        if (this.OnFinished != null) this.OnFinished();
      }

      /// <summary>
      /// Sets a callback function to be called when this timer finishes
      /// </summary>
      /// <param name="onFinished"></param>
      public void SetCallback(Callback onFinished)
      {
        OnFinished = onFinished;
      }

      /// <summary>
      /// Resets the timer
      /// </summary>
      public void Reset()
      {
        Finished_ = false;
        this.OnReset();
      }

      /// <summary>
      /// Updates the timer by the default delta time (Time.deltaTime)
      /// </summary>
      /// <returns>True if is done, false otherwise</returns>
      public bool Update()
      {
        return Update(Time.deltaTime);
      }

      /// <summary>
      /// Updates the timer, invoking the provided callback upon completion, then resets the timer.
      /// </summary>
      /// <param name="dt"></param>
      public void AutomaticUpdate(float dt)
      {
        Update(dt);
        if (this.IsFinished)
        {
          //if (this.OnFinished == null)
          //  Trace.Error("The provided callback function was not set!");
          //this.OnFinished();
          this.Reset();
        }
      }


    }

    /// <summary>
    /// Counts up to the specified amount of time, starting from 0.0f.
    /// </summary>
    public class Stopwatch : Timer
    {
      /// <summary>
      /// Constructor for the countdown.
      /// </summary>
      /// <param name="total">The total amount of time to countdown.</param>
      public Stopwatch(float total)
      {
        Total_ = total;
        Current_ = 0.0f;
      }

      /// <summary>
      /// Resets the countdown.
      /// </summary>
      protected override void OnReset()
      {
        Current_ = 0.0f;
        Finished_ = false;
      }

      /// <summary>
      /// Sets the timer
      /// </summary>
      /// <param name="total"></param>
      public override void Set(float total)
      {
        Total_ = total;
        Current_ = 0.0f;
      }

      /// <summary>
      /// Updates the timer by the specified delta time.
      /// </summary>
      /// <returns>True if is done, false otherwise</returns>
      public override bool Update(float dt)
      {
        if (Current_ >= Total_)
        {
          Current_ = Total_;
          Finish();
          return true;
        }

        Current_ += dt;
        return false;
      }
    }


    /// <summary>
    /// Counts down from the specified amount of time. 
    /// From <i>n</i> amount of time to 0.0f;
    /// </summary>
    public class Countdown : Timer
    {
      /// <summary>
      /// Constructor for the countdown.
      /// </summary>
      /// <param name="total">The total amount of time to countdown.</param>
      public Countdown(float total)
      {
        Total_ = total;
        Current_ = total;
      }

      /// <summary>
      /// Resets the countdown.
      /// </summary>
      protected override void OnReset()
      {
        Current_ = Total_;
      }

      /// <summary>
      /// Sets the countdown's time
      /// </summary>
      /// <param name="time"></param>
      public override void Set(float time)
      {
        Total_ = time;
        Current_ = Total;
      }

      /// <summary>
      /// Updates the timer by the specified delta time.
      /// </summary>
      /// <returns>True if is done, false otherwise</returns>
      public override bool Update(float dt)
      {
        if (Current_ <= 0.0f)
        {
          Current_ = 0f;
          Finish();
          return true;
        }

        Current_ -= dt;
        return false;
      }
    }

    public class Cooldown : BaseTimer
    {
      /// <summary>
      /// Whether this cooldown timer is currently ticking
      /// </summary>
      public bool IsActive { get { return Active; } }
      public float Current { get { return Countdown.Current; } }

      private Countdown Countdown;
      private bool Active;

      public Cooldown(float cooldownPeriod, bool startOnCooldown = false)
      {
        Active = startOnCooldown;
        Countdown = new Countdown(cooldownPeriod);
        //if (!Application.isEditor && Timers.Instance != null)
        //  Timers.Add(this);
      }

      ~Cooldown()
      {
        //if (!Application.isEditor && Timers.Instance != null)
        //  Timers.Remove(this);
      }

      /// <summary>
      /// Triggers this cooldown
      /// </summary>
      public void Activate()
      {
        Active = true;
        Countdown.Reset();
      }
      
      public override bool Update(float dt)
      {
        if (Countdown.Update(dt))
        {
          Active = false;
          return true;
        }

        return false;        
      }
      
    }


  }

}
