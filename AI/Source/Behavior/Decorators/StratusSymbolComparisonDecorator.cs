using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.AI
{
	public class StratusSymbolComparisonDecorator : PreExecutionDecorator
	{
		public enum Comparison
		{
			IsEqualTo,
			IsNotEqualTo
		}
		public override string description => "Compares the values of two blackboard symbols, allowing the execution of a node based on the result";

		public Comparison comparison;
		public StratusBlackboard.SymbolReference first = new StratusBlackboard.SymbolReference();
		public StratusBlackboard.SymbolReference second = new StratusBlackboard.SymbolReference();

		protected override bool OnDecoratorCanChildExecute(Arguments args)
		{
			object firstValue = this.GetSymbolValue(args, first);
			object secondValue = this.GetSymbolValue(args, second);

			bool allow = false;
			switch (comparison)
			{
				case Comparison.IsEqualTo:
					allow = firstValue.Equals(secondValue);
					break;
				case Comparison.IsNotEqualTo:
					allow = !firstValue.Equals(secondValue);
					break;
			}
			return allow;

		}


	}

}