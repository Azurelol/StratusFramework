/******************************************************************************/
/*!
@file   ActionTrace.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionTrace 
  */
  /**************************************************************************/
  public class ActionTrace : Action
  {
    MonoBehaviour Object;
    object Message;
    
    /**************************************************************************/
    /*!
    @brief ActionTrace constructor
    @param duration How long this Action should delay for.
    */
    /**************************************************************************/
    public ActionTrace(object message, MonoBehaviour obj = null) : base("ActionCall")
    {
      this.Message = message;
      Object = obj;
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
      Trace.Script(Message, this.Object);
      this.isFinished = true;

      if (Actions.debug)
        Debug.Log("#" + this.id + ": Finished!");

      return 0.0f;
    }

  }

}