using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	public interface IStratusFPSCombatController : IStratusCombatController
	{
		bool jumping { get; }
		bool crouched { get; }
	}

	/// <summary>
	/// A combat controller for FPS 
	/// </summary>
	/// <typeparam name="CharacterType"></typeparam>
	/// <typeparam name="ParameterType"></typeparam>
	public abstract class StratusFPSCombatController<CharacterType, ParameterType>
		: StratusCombatController<CharacterType, ParameterType>,
		IStratusFPSCombatController

		where CharacterType : StratusCharacter
		where ParameterType : IStratusCombatParameterModel<CharacterType>, new()
	{
		public override string faction { get; }
		public bool jumping { get; protected set; }
		public bool crouched { get; protected set; }

		protected StratusFPSCombatController(StratusBehaviour behaviour, CharacterType character) : base(behaviour, character)
		{

		}

		protected override void OnActivate()
		{
		}

		protected override void OnCharacterControllerInitialize()
		{
		}

		protected override void OnCharacterSet()
		{
		}

		protected override void OnDeactivate()
		{
		}

		protected override void OnDespawn()
		{
		}

		protected override void OnParametersSet(ParameterType parameters)
		{
		}
	}

}