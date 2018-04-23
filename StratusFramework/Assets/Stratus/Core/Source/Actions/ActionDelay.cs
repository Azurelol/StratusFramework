/******************************************************************************/
/*!
@file   ActionDelay.cs
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
  @class An ActionDelay is a type of action that blocks all actions behind it
  for a specified amount of time.
  */
  /**************************************************************************/
  public class ActionDelay : Action
  {
    public static bool Trace = false;

    /**************************************************************************/
    /*!
    @brief ActionDelay constructor
    @param duration How long this Action should delay for.
    */
    /**************************************************************************/
    public ActionDelay(float duration) : base("ActionDelay")
    {
      this.duration = duration;
    }

    /**************************************************************************/
    /*!
    @brief Updates the action
    @param dt The delta time.
    @return How much time was consumed during this action.
    */
    /**************************************************************************/
    public override float Update(float dt)
    {
      this.elapsed += dt;
      var timeLeft = this.duration - this.elapsed;

      if (this.elapsed >= this.duration)
      {
        this.isFinished = true;
        if (Trace)
          Debug.Log("Finished!");
      }
      else
      {
        if (Trace)
          Debug.Log("dt = '" + dt + "', timeLeft = '" + timeLeft + "'");
      }

      // I don't remember why this works @_@
      if (timeLeft < dt)
        return dt;
      else
        return timeLeft;
      
    }
  }

}

