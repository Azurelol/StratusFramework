using UnityEngine;
using Stratus;
using System;
using Stratus.Types;

namespace Stratus
{
  namespace AI
  {
    public abstract class TargetedTask<TargetType> : Task
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      public Blackboard.Reference<TargetType> targetSymbol = new Blackboard.Reference<TargetType>();
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
      /// Whether the target is currently being approached
      /// </summary>
      public bool isApproaching { get; private set; }

      ///// <summary>
      ///// The current target of this action
      ///// </summary>
      //protected abstract TargetType target { get; }
      ///// <summary>
      ///// Whether there's currently a valid target for this action
      ///// </summary>
      //protected bool hasTarget { get { return target != null; } }
      ///// <summary>
      ///// The current position of the target
      ///// </summary>
      //protected abstract Vector3 targetPosition { get; }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnTargetActionStart(Agent agent, TargetType target);
      protected abstract Status OnTargetActionUpdate(Agent agent, TargetType target);
      protected abstract void OnTargetActionEnd(Agent agent, TargetType target);
      protected abstract Vector3 GetTargetPosition(TargetType target);

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/  
      protected override void OnTaskStart(Agent agent)
      {
        TargetType target = GetTarget(agent);
        this.OnTargetActionStart(agent, target);
      }

      protected override Status OnTaskUpdate(Agent agent)
      {
        // If not within range of the target, approach it
        TargetType target = GetTarget(agent);
        Vector3 targetPosition = this.GetTargetPosition(target);


        if (!IsWithinRange(agent, targetPosition))
        {
          if (!isApproaching)
          {
            bool canApproach = this.Approach(agent, targetPosition);
            if (canApproach)
              this.isApproaching = true;
            else
              return Status.Failure;
          }

          // If there's a valid target, approach it
          //this.Approach();
          return Status.Running;
        }

        // If it's in range, perform the underlying action
        this.isApproaching = false;
        return OnTargetActionUpdate(agent, target);
      }

      protected override void OnTaskEnd(Agent agent)
      {        
        TargetType target = GetTarget(agent);
        this.OnTargetActionEnd(agent, target);
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/ 
      /// <summary>
      /// Checks whether the agent is within range of its target
      /// </summary>
      /// <returns></returns>
      protected bool IsWithinRange(Agent agent, Vector3 targetPosition)
      {        
        return Library.CheckRange(agent.transform, targetPosition, this.range);
      }

      /// <summary>
      /// Attemps to approach the current target of this action
      /// </summary>
      protected bool Approach(Agent agent, Vector3 targetPosition)
      {
        return agent.MoveTo(targetPosition);        
      }

      TargetType GetTarget(Agent agent) => this.targetSymbol.GetValue(agent.blackboard, agent.gameObject);



    } 
  } 
}
