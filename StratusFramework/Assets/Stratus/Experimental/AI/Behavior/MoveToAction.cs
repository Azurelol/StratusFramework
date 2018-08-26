using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace AI
  {
    public class MoveToAction : TargetAction<Vector3>
    {
      public override string description { get; } = "Moves the agent to the target location";

      protected override Vector3 target => agent.blackboard.locals.GetValue<Vector3>(Symbol.key);

      protected override Vector3 targetPosition => target;

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
        Trace.Script("Reached the target! ", this.agent);
        return Status.Success; 
      }
    }
  }

}