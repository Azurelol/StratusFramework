/******************************************************************************/
/*!
@file   ActionGroup.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class An ActionSequence is a type of set that updates all its actions
         and children in sequence, depleting its time slice as it updates
         each.
  */
  /**************************************************************************/
  public class ActionGroup : ActionSet
  {
    public ActionGroup() : base("ActionGroup") { }

    /**************************************************************************/
    /*!
    @brief  Updates an ActionGroup, by updating the actions in the group in
            parallel.
    @param  dt The time to be updated.
    @return How much time was consumed while updating.
    */
    /**************************************************************************/
    public override float Update(float dt)
    {
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
