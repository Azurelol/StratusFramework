using UnityEngine;
using Stratus;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Represents an action taken by the player
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ValueEvent<ValueType>
	{
		/// <summary>
		/// If an input has been accepted, and is legal, represents the beginning of an action
		/// </summary>
		public class GainEvent : Stratus.StratusEvent { public ValueType value; }
		/// <summary>
		/// If an input has been accepted, and is legal, represents the beginning of an action
		/// </summary>
		public class LossEvent : Stratus.StratusEvent { public ValueType value; }
	}

	/// <summary>
	/// Provides major definitions for combat within the framework
	/// </summary>
	public static class StratusCombat
	{
		//------------------------------------------------------------------------/
		// Events: State
		//------------------------------------------------------------------------/
		/// <summary>
		/// Base class for all combat events
		/// </summary>
		public abstract class BaseCombatEvent : StratusEvent 
		{ 
		}
		/// <summary>
		/// Combat has started
		/// </summary>
		public class StartedEvent : StratusEvent 
		{ 
			public StratusCombatEncounter Encounter; 
		}
		/// <summary>
		/// Combat has ended
		/// </summary>
		public class EndedEvent : StratusEvent { }
	}

}