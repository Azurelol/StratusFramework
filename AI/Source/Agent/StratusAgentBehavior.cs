//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Stratus.AI
//{
//	/// <summary>
//	/// Handles control of an agent through AI
//	/// </summary>
//	[RequireComponent(typeof(StratusAgent))]
//	public class StratusAgentBehavior : StratusAgentComponent
//	{
//		/// <summary>
//		/// The collection of behaviors to run on this agent (a behavior system such as a BT, Planner, etc)
//		/// </summary>
//		public StratusBehaviorSystem behavior;
//		/// <summary>
//		/// If there's a behavior set for this agent
//		/// </summary>
//		public bool hasBehavior => this.behavior != null;
//		/// <summary>
//		/// The blackboard this agent is using
//		/// </summary>
//		public StratusBlackboard blackboard => this.behavior.blackboard;

//		protected override void OnManagedStart()
//		{
//			if (this.hasBehavior)
//			{
//				this.behavior = StratusBehaviorSystem.InitializeSystemInstance(this.agent, this.behavior);
//			}
//		}

//		protected override void OnManagedUpdate()
//		{
//			base.OnManagedUpdate();
//			if (this.hasBehavior && agent.isAutomatic)
//			{
//				this.behavior.UpdateSystem();
//			}
//		}
//	}

//}