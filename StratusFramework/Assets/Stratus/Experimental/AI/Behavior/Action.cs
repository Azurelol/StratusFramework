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
      // Interface
      //------------------------------------------------------------------------/   
      protected abstract void OnActionStart();
      protected abstract void OnActionReset();
      protected abstract Status OnActionUpdate(float dt);
      protected abstract void OnActionEnd();

      //------------------------------------------------------------------------/
      // Messages
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
        this.status = Status.Suspended;
        this.OnActionReset();
      }

    }

  }

}