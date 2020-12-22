using UnityEngine;
using Stratus;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Applies the effect on a percentage-based chance.
	/// </summary>
	public abstract class ChanceEffectAttribute<Parameters> : StratusCombatEffect<Parameters>
		where Parameters : IStratusCombatParameterModel
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public float chance = 1.0f;

		//------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------/
		protected abstract void OnChance<CombatController>(CombatController caster, CombatController target);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		protected override void OnApply<CombatController>(CombatController user, CombatController target)
		{
			// Roll to see whether the effect should be applied or not
			if (Roll())
			{
				this.OnChance(user, target);
			}
		}

		protected virtual bool Roll()
		{
			var roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll >= (1.0f - this.chance))
			{
				return true;
			}
			return false;
		}

	}

}