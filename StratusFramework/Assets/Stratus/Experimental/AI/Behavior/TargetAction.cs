using UnityEngine;
using Stratus;
using System;
using Stratus.Types;

namespace Stratus
{
  namespace AI
  {
    public abstract class TargetAction<TargetType> : Action
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The symbol used in the blackboard for this target
      /// </summary>
      [Tooltip("The symbol used in the blackboard for this target")]
      public Symbol.Reference Symbol;
      /// <summary>
      /// The range at which this action needs to be within the target
      /// </summary>
      [Tooltip("The range at which this action needs to be within the target")]
      [Range(0f, 10f)]
      public float range = 2f;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/ 
      /// <summary>
      /// The current target of this action
      /// </summary>
      protected abstract TargetType target { get; }
      /// <summary>
      /// Whether there's currently a valid target for this action
      /// </summary>
      protected bool hasTarget { get { return target != null; } }
      /// <summary>
      /// The current position of the target
      /// </summary>
      protected abstract Vector3 targetPosition { get; }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnTargetActionStart();
      protected abstract Status OnTargetActionUpdate(float dt);
      protected abstract void OnTargetActionEnd();

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/ 
      protected override void OnActionStart()
      {
        this.Approach();
        this.OnTargetActionStart();
      }

      protected override Status OnActionUpdate(float dt)
      {
        // If not within range of the target, approach it
        if (!IsWithinRange())
        {
          // If the target has been destroyed, cancel this action
          // If something happened to the target, replan
          if (!hasTarget)
          {
            this.agent.gameObject.Dispatch<CanceledEvent>(new CanceledEvent());
            return Status.Failure;
          }

          // If there's a valid target, approach it
          //this.Approach();
          return Status.Running;
        }

        // If it's in range, perform the underlying action
        return OnTargetActionUpdate(dt);
      }

      protected override void OnActionEnd()
      {
        this.OnTargetActionEnd();
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/ 
      /// <summary>
      /// Checks whether the agent is within range of its target
      /// </summary>
      /// <returns></returns>
      bool IsWithinRange()
      {
        return Library.CheckRange(this.agent.transform, this.targetPosition, this.range);
      }

      /// <summary>
      /// Approaches the current target of this action
      /// </summary>
      void Approach()
      {
        if (!hasTarget)
          return;

        //if (this.Agent.tar)
        this.agent.MoveTo(this.targetPosition);        
      }

    } 
  } 
}
