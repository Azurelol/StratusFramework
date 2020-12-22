using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;
using Stratus.Gameplay;

namespace Stratus.Gameplay
{
	public interface IStratusCombatController
	{
		void Activate();
		void Deactivate();
		string faction { get; }
	}

	public interface IStratusCombatControllerBehaviour<CombatControllerType>
		where CombatControllerType : StratusCombatController
	{
		CombatControllerType combatController { get; }
	}


	/// <summary>
	/// The base class which acts as the driver for combatants
	/// </summary>
	public abstract class StratusCombatController : IStratusCombatController, IStratusLogger
	{
		//------------------------------------------------------------------------/
		// Event Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// Informs that this controller has just spawned
		/// </summary>
		public class SpawnEvent : StratusCombatControllerEvent
		{
			public SpawnEvent(StratusCombatController controller) : base(controller)
			{
			}
		}
		/// <summary>
		/// Informs that this controller has been despawned (for example, after the character was "killed")
		/// </summary>
		public class DespawnEvent : StratusCombatControllerEvent
		{
			public DespawnEvent(StratusCombatController controller) : base(controller)
			{
			}
		}

		/// <summary>
		///Base activation event
		/// </summary>
		public abstract class BaseActivationEvent : StratusCombatControllerEvent
		{
			public bool valid = false;
			public BaseActivationEvent(StratusCombatController controller) : base(controller)
			{
			}
		}


		/// <summary>
		/// Informs that this controller has become active, such after being spawned, or revived
		/// </summary>
		public class ActivateEvent : BaseActivationEvent
		{
			public ActivateEvent(StratusCombatController controller) : base(controller)
			{
			}
		}

		/// <summary>
		/// Informs that this controller has become inactive, such after being killed
		/// </summary>
		public class DeactivateEvent : BaseActivationEvent
		{
			public DeactivateEvent(StratusCombatController controller) : base(controller)
			{
			}
		}
		/// <summary>
		/// Sets the target for a controller
		/// </summary>
		public class TargetEvent : StratusEvent { public StratusCombatController target; }
		/// <summary>
		/// Resumes activity for this character.
		/// </summary>
		public class ResumeEvent : StratusEvent { }
		/// <summary>
		/// Ceases activity for this character.
		/// </summary>
		public class PauseEvent : StratusEvent { }

		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// The current state of the character
		/// </summary>
		public enum State
		{
			/// <summary>
			/// The character is taking no action
			/// </summary>
			Idle,
			/// <summary>
			/// The character is taking action
			/// </summary>
			Active,
			/// <summary>
			/// The character is unable to take action
			/// </summary>
			Inactive
		}

		public delegate void Callback();

		//------------------------------------------------------------------------/
		// Public Fields
		//------------------------------------------------------------------------/
		[Header("Debug")]
		/// <summary>
		/// Whether to print debug output
		/// </summary>
		[Tooltip("Whether to print debug output")]
		public bool logging = false;

		/// <summary>
		/// How this character is controlled
		/// </summary>
		public StratusAgentControlType control;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The current state of the character
		/// </summary>
		public State state { get; private set; }

		/// <summary>
		/// The faction the character this controller currently belongs to
		/// </summary>
		public abstract string faction { get; }

		/// <summary>
		/// The current modules for the combat controller
		/// </summary>
		public StratusCombatControllerModule[] modules { get; set; }

		/// <summary>
		/// The behaviour that this controller is associated with
		/// </summary>
		public StratusBehaviour behaviour { get; private set; }

		/// <summary>
		/// The gameobject the behaviour is on
		/// </summary>
		public GameObject gameObject => behaviour.gameObject;

		/// <summary>
		/// The transform for the GameObject
		/// </summary>
		public Transform transform => gameObject.transform;

		/// <summary>
		/// Whether the controller has been initialized
		/// </summary>
		public bool initialized { get; private set; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/  
		public event Action onActivate;
		public event Action onDeactivate;

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/        
		protected abstract void OnInitialize();
		protected abstract void OnDespawn();
		protected abstract void OnActivate();
		protected abstract void OnDeactivate();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public StratusCombatController(StratusBehaviour behaviour)
		{
			this.behaviour = behaviour;
		}

		public void Initialize()
		{
			OnInitialize();
			initialized = true;
			this.Log($"Initialized {this}");
			StratusScene.Dispatch<SpawnEvent>(new SpawnEvent(this));
		}

		public void Activate()
		{
			if (!initialized)
			{
				this.LogError("Cannot activate before being initialized");
			}

			// If already active
			if (state == State.Active)
			{
				this.LogError($"{this} is already active");
			}

			// Send the activation event
			ActivateEvent activation = new ActivateEvent(this);
			StratusScene.Dispatch<ActivateEvent>(activation);

			// If the activation was successful
			if (activation.valid)
			{
				OnActivate();
				this.Log("Activated");
				state = State.Active;
				onActivate?.Invoke();
			}
			else
			{
				this.LogError($"Failed to activate {this}");
			}
		}


		public void Deactivate()
		{
			if (state == State.Inactive)
			{
				this.LogError($"{this} is already inactive");
			}

			// Send the deactivation event
			DeactivateEvent e = new DeactivateEvent(this);
			StratusScene.Dispatch<DeactivateEvent>(e);

			// If the deactivation was succesful
			if (e.valid)
			{
				OnDeactivate();
				this.Log("Deactivated");
				state = State.Inactive;
				onDeactivate?.Invoke();
			}
			else
			{
				this.LogError($"Failed to deactivate {this}");
			}
		}

		public abstract StratusCombatTargetRelation GetTargetRelation(StratusCombatController target);
	}

	/// <summary> 
	/// Informs a change in status in this combat controller 
	/// </summary>
	public abstract class StratusCombatControllerEvent : StratusEvent
	{
		protected StratusCombatControllerEvent(StratusCombatController controller)
		{
			this.controller = controller;
		}

		public StratusCombatController controller { get; private set; }
	}

	/// <summary>
	/// The main use combat controller that uses a character
	/// </summary>
	/// <typeparam name="CharacterType"></typeparam>
	public abstract class StratusCombatController<CharacterType, ParameterType>
		: StratusCombatController, IStratusCombatParametrized<ParameterType>
		where CharacterType : StratusCharacter
		where ParameterType : IStratusCombatParameterModel<CharacterType>, new()
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The character used by this controller
		/// </summary>
		public CharacterType character { get; private set; }
		/// <summary>
		/// Paramters used by this controller, based on the character
		/// </summary>
		public ParameterType parameters { get; private set; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnParametersSet(ParameterType parameters);

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public event Action onCharacterSet;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusCombatController(StratusBehaviour behaviour, CharacterType character)
			: base(behaviour)
		{
			this.character = character;
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnInitialize()
		{
			SetCharacter();
			OnCharacterControllerInitialize();
		}

		protected abstract void OnCharacterControllerInitialize();
		protected abstract void OnCharacterSet();


		public override string ToString()
		{
			return character.ToString();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void SetCharacter()
		{
			if (character != null)
			{
				parameters = new ParameterType();
				parameters.Set(character);
				OnParametersSet(parameters);
				onCharacterSet?.Invoke();
				this.Log("Character has been set");
			}
			else
			{
				this.LogError("No character to set");
			}
		}


	}

}