using UnityEngine;
using Stratus.Utilities;
using System;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// An action that takes a specified amount of time to complete
    /// </summary>
    public abstract class TimedAction : Action 
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      [Tooltip("How long it takes to execute this action")]
      public float Speed = 1f;
      public float Progress { get { return ProgressTimer.normalizedProgress; } }
      protected Countdown ProgressTimer;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnTimedActionStart();
      protected abstract Status OnTimedActionUpdate(float dt);
      protected abstract void OnTimedActionEnd();

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected override void OnActionStart()
      {
        this.ProgressTimer = new Countdown(this.Speed);
      }

      protected override void OnActionReset()
      {
        this.ProgressTimer.Set(this.Speed);
      }

      protected override Status OnActionUpdate(float dt)
      {
        // Update the progress timer. 
        bool isFinished = ProgressTimer.Update(Time.deltaTime);
        // Update the action
        if (!isFinished)
        {
          var status = this.OnTimedActionUpdate(dt);
          if (status != Status.Success)
            return Status.Running;
        }
        
        // If the timer has finished, end the action
        return Status.Success;
      }

      protected override void OnActionEnd()
      {
        this.OnTimedActionEnd();
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
    }
  }

}