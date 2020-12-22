using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stratus;

namespace Stratus.Gameplay
{
	public abstract class StratusSplashEffectAttribute<Parameters> : StratusCombatEffect<Parameters>
		where Parameters : IStratusCombatParameterModel
	{
		public StratusCombatTarget targeting = StratusCombatTarget.Enemy;
		public float radius = 3.0f;

		protected abstract void OnSplash<CombatController>(CombatController caster, CombatController target);

		protected override void OnApply<CombatController>(CombatController user, CombatController target)
		{
			// Find eligible targets within range of the target
			var targetsWithinRange = FindTargets(user, targeting, this.radius);
			foreach (var targetWithinRange in targetsWithinRange)
			{
				var distFromAlly = Vector3.Distance(target.transform.localPosition, targetWithinRange.transform.localPosition);
				if (distFromAlly <= this.radius)
				{
					this.OnSplash(user, targetWithinRange);
				}
			}
		}

		protected abstract CombatController[] FindTargets<CombatController>(CombatController source, StratusCombatTarget parameters, float radius);
	}

}