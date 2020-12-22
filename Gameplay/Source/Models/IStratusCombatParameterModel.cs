using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Parameters determine how a character performs in combat
	/// </summary>
	public interface IStratusCombatParameterModel
	{
		/// <summary>
		/// A string description of the parameters
		/// </summary>
		string description { get; }
		/// <summary>
		/// Invoked whenever parameters are set
		/// </summary>
		event Action onSet;
	}

	/// <summary>
	/// Parameters determine how a character performs in combat
	/// </summary>
	public interface IStratusCombatParameterModel<CharacterType> : IStratusCombatParameterModel
		where CharacterType : StratusCharacter
	{
		/// <summary>
		/// Sets the parameters based on the attributes
		/// </summary>
		void Set(CharacterType character);
	}

	/// <summary>
	/// Defines a class that supports a parameter model
	/// </summary>
	/// <typeparam name="ParameterModel"></typeparam>
	public interface IStratusCombatParametrized<ParameterModel>
		where ParameterModel : IStratusCombatParameterModel
	{
		ParameterModel parameters { get; }
	}



	public abstract class StratusCharacterParameters<CharacterType>
		: IStratusCombatParameterModel<CharacterType>
		where CharacterType : StratusCharacter
		
	{
		public abstract string description { get; }

		public event Action onParameterChanged;
		public event Action onSet;
		protected abstract void OnSet(CharacterType character);

		public void Set(CharacterType character)
		{
			OnSet(character);
			onSet?.Invoke();
		}

		protected void NotifyParameterChange()
		{
			onParameterChanged?.Invoke();
		}

		protected void OnAnyParameterChange(float value)
		{
			NotifyParameterChange();
		}
	}

}