using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace AI
  {
    public class MoveToAction : TargetAction<Vector3>
    {

      public override string Description
      {
        get
        {
          return "Moves the agent to the target location";
        }
      }

      protected override Vector3 Target
      {
        get
        {
          return Agent.blackboard.Locals.GetValue<Vector3>(Symbol.Key);
        }
      }

      protected override Vector3 TargetPosition
      {
        get
        {
          return Target;
        }
      }

      protected override void OnActionReset()
      {        
      }

      protected override void OnTargetActionEnd()
      {
        
      }

      protected override void OnTargetActionStart()
      {
        
      }

      protected override Status OnTargetActionUpdate(float dt)
      {
        Trace.Script("Reached the target! ", this.Agent);
        return Status.Success; 
      }
    }
  }

}