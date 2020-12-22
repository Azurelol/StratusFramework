using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.AI
{
	public class StratusLogTask : StratusAITask
	{
		public string message;
		public override string description => "Prints a message to the console";

		protected override void OnTaskEnd(StratusAgent agent)
		{
			StratusDebug.Log(message);
		}

		protected override void OnTaskStart(StratusAgent agent)
		{
		}

		protected override Status OnTaskUpdate(StratusAgent agent)
		{
			return Status.Success;
		}
	}



}
