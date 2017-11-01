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
using System;

namespace Stratus
{
  /// <summary>
  /// Invokes a function immediately
  /// </summary>
  public class ActionCall : Action
  {
    public delegate void Delegate();
    Delegate DelegateInstance;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="deleg">A provided function which to invoke.</param>
    public ActionCall(Delegate deleg) : base("ActionCall")
    {
      this.DelegateInstance = deleg;
    }
    
    /// <summary>
    /// Updates the action
    /// </summary>
    /// <param name="dt">The delta time.</param>
    /// <returns>How much time was consumed during this action.</returns>
    public override float Update(float dt)
    {
      if (Tracing)
        Trace.Script("#" + this.id + ": Calling function '" + DelegateInstance.Method.Name + "'");
      
      DelegateInstance.DynamicInvoke();
      this.isFinished = true;

      if (Tracing)
        Trace.Script("#" + this.id + ": Finished!");

      return 0.0f;
    }
  }


  /// <summary>
  /// Invokes a function immediately
  /// </summary>
  public class ActionCall<T> : Action
  {
    public delegate void Delegate(T arg);
    Delegate DelegateInstance;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="deleg">A provided function which to invoke.</param>
    public ActionCall(Delegate deleg) : base("ActionCall")
    {
      this.DelegateInstance = deleg;
    }    

    /// <summary>
    /// Updates the action
    /// </summary>
    /// <param name="dt">The delta time.</param>
    /// <returns>How much time was consumed during this action.</returns>
    public override float Update(float dt)
    {
      if (Tracing)
        Trace.Script("#" + this.id + ": Calling function '" + DelegateInstance.Method.Name + "'");

      // If the target was destroyed in the meantime...
      if (DelegateInstance.Target == null)
        return 0f;

      DelegateInstance.DynamicInvoke();
      this.isFinished = true;

      if (Tracing)
        Trace.Script("#" + this.id + ": Finished!");

      return 0.0f;
    }
  }

}