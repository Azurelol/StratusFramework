using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.AI
{
	/// <summary>
	/// Bases its condition on wheher its loop counter has exceeded
	/// </summary>
	public class StratusLoopDecorator : PostExecutionRepeatingDecorator
	{
		public int counter = 3;
		public StratusCounter currentCounter { get; set; }

		public override string description => "Bases its condition on wheher its loop counter has exceeded";

		protected override void OnDecoratorStart(Arguments args)
		{
			this.currentCounter = new StratusCounter(this.counter);
		}

		protected override bool OnRepeatingDecoratorChildEnded(Arguments args, Status status)
		{
			if (status == Status.Failure)
				return false;

			this.currentCounter.Increment();
			return !this.currentCounter.isAtLimit;
		}
	}

}