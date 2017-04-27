/******************************************************************************/
/*!
@file   ActionOwner.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

/// <summary>
/// An ActionOwner is a container of all actions a particular GameObject
/// has.They propagate updates to all actions attached to it.
/// </summary>
namespace Stratus
{
  public class ActionsOwner : ActionGroup
  {
    public GameObject Owner;

    public ActionsOwner(GameObject owner, UpdateMode mode = UpdateMode.Normal) : base(mode)
    {
      this.Owner = owner;
      this.Type = "ActionsOwner";
      if (Actions.Debugging) Trace.Script("Owner = '" + this.Owner + "'");
    }

    ~ActionsOwner() { }
    
    /// <summary>
    /// Updates a GameObject's actions, updating all the actions one tier below
    /// in parallel.
    /// </summary>
    /// <param name="dt">The time to be updated.</param>
    /// <returns>How much time was consumed while updating.</returns>
    public override float Update(float dt)
    {
      Migrate();
      return base.Update(dt);

      //var mostTimeElapsed = 0.0f;
      //
      //// In an ActionGroup, every action is updated in parallel, given the same 
      //// time slice.
      //foreach (var action in this.ActiveActions)
      //{
      //  // Every action consumes time from the time slice given (dt)
      //  var timeElapsed = action.Update(dt);
      //  // If this action took longer than the previous action, it is the new maximum
      //  if (timeElapsed > mostTimeElapsed)
      //    mostTimeElapsed = timeElapsed;
      //}
      //
      //// Sweep inactive actions
      //this.Sweep();
      //
      //
      //return mostTimeElapsed;
    }
  }
}