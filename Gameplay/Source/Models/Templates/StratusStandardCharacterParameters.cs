using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay.Models
{
	public class StratusHitpointsParameter : StratusEventDrivenVariableAttribute<StratusHitpointsParameter>
	{
		public override string defaultLabel => "Hitpoints";
		public StratusHitpointsParameter(float value, float floor = 0, float ceiling = float.MaxValue) : base(value, floor, ceiling)
		{
		}

	}

	public abstract class StratusStandardCharacterParameters<CharacterType>
				: StratusCharacterParameters<CharacterType>
				where CharacterType : StratusCharacter
	{
		public StratusHitpointsParameter hitpoints { get; protected set; }
		protected abstract void OnSetStandard(CharacterType character);

		protected override void OnSet(CharacterType character)
		{
			hitpoints = GetHitpoints(character);
			hitpoints.onModified += OnAnyParameterChange;
			OnSetStandard(character);
		}

		protected abstract StratusHitpointsParameter GetHitpoints(CharacterType character);
	}

	public abstract class StratusStandardRPGParameters<CharacterType>
		: StratusStandardCharacterParameters<CharacterType>
		where CharacterType : StratusCharacter
	{
		public abstract StratusVariableAttribute attack { get; set; }
		public abstract StratusVariableAttribute defense { get; set; }

		protected virtual float ModifyIncomingHitpointDamage(float damage)
		{
			return damage - this.defense.current;
		}

	}

}