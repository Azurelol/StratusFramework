using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.AI
{
	public class StratusWaitTask : StratusTimedTask
	{
		public override string description { get; } = "Wait until the wait time is finished";

		protected override void OnTimedActionEnd(StratusAgent agent)
		{
		}

		protected override void OnTimedActionStart(StratusAgent agent)
		{
		}

		protected override Status OnTimedActionUpdate(StratusAgent agent)
		{
			return Status.Running;
		}
	}

}