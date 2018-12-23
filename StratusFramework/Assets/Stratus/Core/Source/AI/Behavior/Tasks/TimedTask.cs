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
    public abstract class TimedTask : Task
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      public enum Mode
      {
        Static,
        Symbol
      }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      [Tooltip("How long it takes to execute this action")]
      public float duration = 1f;

      protected Countdown timer;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public float progress { get { return timer.normalizedProgress; } }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnTimedActionStart(Agent agent);
      protected abstract Status OnTimedActionUpdate(Agent agent);
      protected abstract void OnTimedActionEnd(Agent agent);

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected override void OnTaskStart(Agent agent)
      {
        this.timer = new Countdown(this.duration);
      }

      protected override Status OnTaskUpdate(Agent agent)
      {
        // Update the progress timer. 
        bool isFinished = timer.Update(Time.deltaTime);
        // Update the action
        if (!isFinished)
        {
          var status = this.OnTimedActionUpdate(agent);
          return status;
          //if (status != Status.Success)
          //  return Status.Running;
        }

        // If the timer has finished, end the action
        return Status.Success;
      }

      protected override void OnTaskEnd(Agent agent)
      {
        this.OnTimedActionEnd(agent);
      }

    }
  }

}