/******************************************************************************/
/*!
@file   ActionSequence.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  /// <summary>
  /// An ActionSequence is a type of set that updates all its actions
  /// and children in sequence, depleting its time slice as it updates
  /// each.
  /// </summary>
  public class ActionSequence : ActionSet
  {
    public ActionSequence(TimeScale mode = TimeScale.Delta) : base("ActionSequence", mode) {}

    /// <summary>
    /// Updates an ActionSequence, by updating the actions in the sequence
    /// sequentially.
    /// </summary>
    /// <param name="dt">The time to be updated</param>
    /// <returns>How much time was consumed while updating</returns>
    public override float Update(float dt)
    {
      Migrate();

      var timeLeft = dt;
      foreach(var action in this.ActiveActions)
      {
        // If an action is inactive, stop the sequence (since its blocking)
        if (action.isActive)
          break;

        // Every action consumes time from the time slice given (dt)
        timeLeft -= action.Update(dt);
        // If the action was completed (Meaning there is time remaining
        // after it was updated, then it will be cleared on the next frame!
        if (timeLeft <= 0)
          break;
      }

      // Sweep all inactive actions
      this.Sweep();

      return dt - timeLeft;
    }
  }

}
