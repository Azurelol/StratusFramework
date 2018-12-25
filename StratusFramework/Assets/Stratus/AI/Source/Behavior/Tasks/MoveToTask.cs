using UnityEngine;
using Stratus;
using System;
using Stratus.Types;

namespace Stratus
{
  namespace AI
  {
    public class MoveToTask : TargetedTask<Vector3> 
    {
      public override string description { get; } = "Moves the agent to the target location";

      protected override Vector3 GetTargetPosition(Vector3 target) => target;


      protected override Status OnTargetActionUpdate(Agent agent, Vector3 target)
      {      
        return Status.Success;
      }

      protected override void OnTargetActionStart(Agent agent, Vector3 target)
      {
        
      }

      protected override void OnTargetActionEnd(Agent agent, Vector3 target)
      {
        StratusDebug.Log($"Arrived at target");
      }
    }
  }

}