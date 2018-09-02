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
    public abstract class TimedAction : Task
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      [Tooltip("How long it takes to execute this action")]
      public float speed = 1f;
      public float progress { get { return progressTimer.normalizedProgress; } }
      protected Countdown progressTimer;

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
        this.progressTimer = new Countdown(this.speed);
      }

      protected override Status OnTaskUpdate(Agent agent)
      {
        // Update the progress timer. 
        bool isFinished = progressTimer.Update(Time.deltaTime);
        // Update the action
        if (!isFinished)
        {
          var status = this.OnTimedActionUpdate(agent);
          if (status != Status.Success)
            return Status.Running;
        }

        // If the timer has finished, end the action
        return Status.Success;
      }

      protected override void OnTaskEnd(Agent agent)
      {
        this.OnTimedActionEnd(agent);
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
    }
  }

}