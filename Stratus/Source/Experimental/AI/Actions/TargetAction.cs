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
      //public class VariableTarget
      //{
      //  public Transform Transform;
      //  public GameObject GameObject;
      //  public Vector3 Position;
      //}

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
      public float Range = 2f;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/ 
      /// <summary>
      /// The current target of this action
      /// </summary>
      protected abstract TargetType Target { get; }
      /// <summary>
      /// Whether there's currently a valid target for this action
      /// </summary>
      protected bool HasTarget { get { return Target != null; } }
      /// <summary>
      /// The current position of the target
      /// </summary>
      protected abstract Vector3 TargetPosition { get; }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnTargetActionStart();
      protected abstract Status OnTargetActionUpdate(float dt);
      protected abstract void OnTargetActionEnd();

      //------------------------------------------------------------------------/
      // Message
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
          if (!HasTarget)
          {
            this.Agent.gameObject.Dispatch<CanceledEvent>(new CanceledEvent());
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
        return Library.CheckDistance(this.Agent.transform, this.TargetPosition, this.Range);
      }

      /// <summary>
      /// Approaches the current target of this action
      /// </summary>
      void Approach()
      {
        if (!HasTarget)
          return;

        //if (this.Agent.tar)
        this.Agent.MoveTo(this.TargetPosition);        
      }

    } 
  } 
}
