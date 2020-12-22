using System.Collections;
using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Stratus.AI
{
	/// <summary>
	/// Services attach to Composite/Tasks nodes, and will execute at their defined frequency as long     
	/// as their branch is being executed. These are often used to make checks and to update the Blackboard. 
	/// These take the place of traditional Parallel nodes in other Behavior Tree systems
	/// </summary>
	public abstract class StratusAIService
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Tooltip("How long to wait between updates of the service")]
		public float interval;
		[Tooltip("An added random deviation between updates")]
		public float deviation;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		private StratusStopwatch timer { get; set; }
		private bool initialized { get; set; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnExecute(StratusAgent agent);

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/
		public void Execute(StratusAgent agent)
		{
			if (!this.initialized)
			{
				this.timer = new StratusStopwatch(interval + Random.Range(0, deviation));
				this.initialized = true;
			}

			this.timer.Update();
			if (this.timer.isFinished)
			{
				this.OnExecute(agent);
				this.timer.Reset();
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
	}

	public interface IServiceSupport
	{
		List<StratusAIService> services { get; }
	}


}