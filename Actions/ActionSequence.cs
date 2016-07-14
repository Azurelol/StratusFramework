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
  /**************************************************************************/
  /*!
  @class An ActionSequence is a type of set that updates all its actions
         and children in sequence, depleting its time slice as it updates
         each.
  */
  /**************************************************************************/
  public class ActionSequence : ActionSet
  {
    public ActionSequence() : base("ActionSequence") {}

    /**************************************************************************/
    /*!
    @brief  Updates an ActionSequence, by updating the actions in the sequence
            sequentially.
    @param  dt The time to be updated.
    @return How much time was consumed while updating.
    */
    /**************************************************************************/
    public override float Update(float dt)
    {
      Migrate();

      var timeLeft = dt;
      foreach(var action in this.ActiveActions)
      {
        // Every action consumes time from the time slice given (dt)
        timeLeft -= action.Update(dt);
        // If the action was completed (Meaning there is time remaining
        // after it was updated, then it will be cleared on the next frame!
        if (timeLeft <= 0)
          break;
      }

      // Sweep all inactive actions
      this.Clear();

      return dt - timeLeft;
    }
  }

}
