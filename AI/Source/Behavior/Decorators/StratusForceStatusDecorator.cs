using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.AI
{
	public class StratusForceStatusDecorator : PostExecutionDecorator
	{
		public enum ForcedStatus
		{
			Success,
			Failure
		}

		public ForcedStatus forcedStatus = ForcedStatus.Success;

		public override string description => "Forces a selected status once the child finishes";

		protected override bool OnChildEnded(Arguments args, Status status)
		{
			this.End(args, (forcedStatus == ForcedStatus.Success) ? Status.Success : Status.Failure);
			return true;
		}

	}

}