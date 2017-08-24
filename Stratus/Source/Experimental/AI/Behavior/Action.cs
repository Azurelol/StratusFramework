/******************************************************************************/
/*!
@file   PlanAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus.Utilities;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Also known as a leaf node, an action represents any concrete action
    /// an agent can make (such as moving to a location, attacking a target, etc)
    /// </summary>
    public abstract class Action : Behavior
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/

      //------------------------------------------------------------------------/
      // Inheritance
      //------------------------------------------------------------------------/   
      protected abstract void OnActionStart();
      protected abstract void OnActionReset();
      protected abstract Status OnActionUpdate(float dt);
      protected abstract void OnActionEnd();

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected override void OnStart()
      {
        this.OnActionStart();
      }

      protected override Status OnUpdate(float dt)
      {
        var status = OnActionUpdate(dt);
                
        // If the action has finished, end it
        if (status == Status.Success)
          this.End();

        return status;
      }

      protected override void OnEnd()
      {
        this.OnActionEnd();
      }      

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Resets all values to this action, making it execute again from the beginning.
      /// </summary>
      public void Reset()
      {
        this.Active = false;
        this.CurrentStatus = Status.Suspended;
        this.OnActionReset();
      }

      /// <summary>
      /// Executes this action.
      /// </summary>
      protected virtual void Execute()
      {
        //Trace.Script(Description + " : Casting action...", this);
      }
            

      






    }

  }

}