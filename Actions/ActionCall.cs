/******************************************************************************/
/*!
@file   ActionCall.cs
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
  public class ActionCall : Action
  {
    public delegate void Delegate();
    Delegate DelegateInstance;
    //-------------------------------/
    public static bool Trace = false;

    /**************************************************************************/
    /*!
    @brief ActionDelay constructor
    @param deleg A provided function which to invoke.
    */
    /**************************************************************************/
    public ActionCall(Delegate deleg) : base("ActionCall")
    {
      this.DelegateInstance = deleg;
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
      if (Trace)
        Debug.Log("#" + this.ID + ": Calling function '" + DelegateInstance.Method.Name + "'");


      DelegateInstance.Invoke();
      this.Finished = true;

      if (Trace)
        Debug.Log("#" + this.ID + ": Finished!");

      return 0.0f;
    }
  }

}