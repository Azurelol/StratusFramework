using UnityEngine;
using Stratus.Utilities;
using System;

namespace Stratus.AI
{
	/// <summary>
	/// An action that takes a specified amount of time to complete
	/// </summary>
	public abstract class StratusTimedTask : StratusAITask
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public enum Mode
		{
			Static,
			Symbol
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Tooltip("How long it takes to execute this action")]
		public float duration = 1f;

		protected StratusCountdown timer;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public float progress { get { return timer.normalizedProgress; } }

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		protected abstract void OnTimedActionStart(StratusAgent agent);
		protected abstract Status OnTimedActionUpdate(StratusAgent agent);
		protected abstract void OnTimedActionEnd(StratusAgent agent);

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		protected override void OnTaskStart(StratusAgent agent)
		{
			this.timer = new StratusCountdown(this.duration);
		}

		protected override Status OnTaskUpdate(StratusAgent agent)
		{
			// Update the progress timer. 
			bool isFinished = timer.Update(Time.deltaTime);
			// Update the action
			if (!isFinished)
			{
				var status = this.OnTimedActionUpdate(agent);
				return status;
				//if (status != Status.Success)
				//  return Status.Running;
			}

			// If the timer has finished, end the action
			return Status.Success;
		}

		protected override void OnTaskEnd(StratusAgent agent)
		{
			this.OnTimedActionEnd(agent);
		}

	}
}