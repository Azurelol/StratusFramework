using UnityEngine;
using Stratus;
using System;
using Stratus.Types;

namespace Stratus.AI
{
	public abstract class StratusTargetedTask<TargetType> : StratusAITask
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public StratusBlackboard.Reference<TargetType> targetSymbol = new StratusBlackboard.Reference<TargetType>();
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

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		protected abstract void OnTargetActionStart(StratusAgent agent, TargetType target);
		protected abstract Status OnTargetActionUpdate(StratusAgent agent, TargetType target);
		protected abstract void OnTargetActionEnd(StratusAgent agent, TargetType target);
		protected abstract Vector3 GetTargetPosition(TargetType target);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/  
		protected override void OnTaskStart(StratusAgent agent)
		{
			TargetType target = GetTarget(agent);
			this.OnTargetActionStart(agent, target);
		}

		protected override Status OnTaskUpdate(StratusAgent agent)
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

		protected override void OnTaskEnd(StratusAgent agent)
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
		protected bool IsWithinRange(StratusAgent agent, Vector3 targetPosition)
		{
			return StratusDetection.CheckRange(agent.transform, targetPosition, this.range);
		}

		/// <summary>
		/// Attemps to approach the current target of this action
		/// </summary>
		protected bool Approach(StratusAgent agent, Vector3 targetPosition)
		{
			return StratusAgentNavigation.DispatchMoveToEvent(agent, targetPosition);
		}

		TargetType GetTarget(StratusAgent agent) => this.targetSymbol.GetValue(agent.blackboard, agent.gameObject);

	}
}
