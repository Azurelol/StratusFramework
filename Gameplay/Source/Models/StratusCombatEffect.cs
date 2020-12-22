using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Stratus.Gameplay
{
	public interface IStratusCombatEffectModel
	{
	}

	//public interface IStratusCharacterQueryable<Query>
	//	where Query : Enum
	//{
	//	float GetValue(Query query);
	//}

	[Serializable]
	public abstract class StratusCombatEffect<Parameters> : IStratusCombatEffectModel
		where Parameters : IStratusCombatParameterModel
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// If this effect has any modifiers, such that it affects additional targets, etc
		/// </summary>
		public enum TargetingModifier
		{
			None,
			Self,
			Allies,
			Enemies,
			All
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public TargetingModifier modifier = TargetingModifier.None;
		public bool persistent = false;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public Type type
		{
			get
			{
				if (_type == null)
					_type = this.GetType();
				return _type;
			}
		}
		private Type _type;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnApply<CombatController>(CombatController user, CombatController target)
			where CombatController : StratusCombatController, IStratusCombatParametrized<Parameters>;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
			/// <summary>
			/// Applies this effect from the caster to the target.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="target"></param>
		public void Apply<CombatController>(CombatController source, CombatController target)
			where CombatController : StratusCombatController, IStratusCombatParametrized<Parameters>
		{

			// Otherwise, apply it right away
			switch (this.modifier)
			{
				case TargetingModifier.None:
					OnApply(source, target);
					break;
				case TargetingModifier.Self:
					OnApply(source, source);
					break;
				case TargetingModifier.Allies:
					throw new NotImplementedException();
				case TargetingModifier.Enemies:
					throw new NotImplementedException();

			}
		}
	}
}