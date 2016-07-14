/******************************************************************************/
/*!
@file   ActionOwner.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

/**************************************************************************/
/*!
@class An ActionOwner is a container of all actions a particular GameObject
      has. They propagate updates to all actions attached to it.
*/
/**************************************************************************/
namespace Stratus
{
  public class ActionsOwner : ActionSet
  {
    public GameObject Owner;

    public ActionsOwner(GameObject owner) : base("ActionsOwner")
    {
      this.Owner = owner;
      if (Actions.Debugging) Trace.Script("Owner = '" + this.Owner + "'");
    }

    ~ActionsOwner() {}

    /**************************************************************************/
    /*!
    @brief Updates a GameObject's actions, updating all the actions one tier below
           in parallel.
    @param dt The time to be updated.
    @return How much time was consumed while updating.
    */
    /**************************************************************************/
    public override float Update(float dt)
    {
      Migrate();

      var mostTimeElapsed = 0.0f;

      // In an ActionGroup, every action is updated in parallel, given the same 
      // time slice.
      foreach (var action in this.ActiveActions)
      {
        // Every action consumes time from the time slice given (dt)
        var timeElapsed = action.Update(dt);
        // If this action took longer than the previous action, it is the new maximum
        if (timeElapsed > mostTimeElapsed)
          mostTimeElapsed = timeElapsed;
      }

      // Sweep inactive actions
      this.Clear();


      return mostTimeElapsed;
    }
  }
}