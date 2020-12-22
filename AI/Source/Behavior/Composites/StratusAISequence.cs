using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Stratus.AI
{
	/// <summary>
	/// Executes each of the child behaviors in sequence
	/// until all of the children have executed successfully
	/// or until one of the child behaviors fail
	/// </summary>
	public class StratusAISequence : StratusAIComposite
	{
		public IEnumerator<StratusAIBehavior> childrenEnumerator { get; private set; }
		public override StratusAIBehavior currentChild => childrenEnumerator.Current;

		public override string description
		{
			get
			{
				return "Executes each of the child behaviors in sequence until all of the children " +
					   "have executed successfully or until one of the child behaviors fail";
			}
		}

		protected override void OnCompositeStart(Arguments args)
		{
			this.childrenEnumerator = children.GetEnumerator();
		}

		protected override bool OnCompositeSetNextChild(Arguments args)
		{
			bool valid = this.childrenEnumerator.MoveNext();
			if (valid)
			{
				this.currentChild.Start(args, this.OnCompositeChildEnded);
			}
			return valid;
		}

		protected override bool OnCompositeChildEnded(Arguments args, Status status)
		{
			if (status == Status.Failure)
			{
				this.End(args, Status.Failure);
				return true;
			}
			else if (!this.OnCompositeSetNextChild(args))
			{
				this.End(args, Status.Success);
			}
			return true;
		}

	}
}