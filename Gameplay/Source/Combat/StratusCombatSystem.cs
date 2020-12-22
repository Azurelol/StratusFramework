using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Stratus.Gameplay
{
	/// <summary>
	/// The base class that drives the logic for the game's combat system.
	/// </summary>
	public abstract class StratusCombatSystem : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public class ReferenceEvent : StratusEvent { public StratusCombatSystem System; }
		/// <summary>
		/// Informs that the combat system has finished its initialization routine
		/// </summary>
		public class InitializedEvent : StratusEvent { }
		/// <summary>
		/// Informs that combat has been resolved
		/// </summary>
		public class ResolveEvent : StratusEvent { }
		/// <summary>
		/// Informs that combat has been won by the player 
		/// </summary>
		public class VictoryEvent : StratusEvent { public StratusCombatEncounter Encounter; }
		/// <summary>
		/// Informs that the player has been defeated 
		/// </summary>
		public class DefeatEvent : StratusEvent { public StratusCombatEncounter Encounter; }
		/// <summary>
		// Informs that combat is to be retried/ 
		/// </summary>
		public class RetryEvent : StratusEvent { public StratusCombatEncounter Encounter; }

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public bool debug = false;
		/// <summary>
		/// Whether combat has been resolved.
		/// </summary>
		protected bool IsResolved = false;

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		// Combatants
		protected abstract void OnConfigureCombatController(StratusCombatController CombatController);
		protected abstract void OnCombatControllerSpawn(StratusCombatController controller);
		protected abstract void OnCombatControllerDeath(StratusCombatController controller);
		// System
		protected abstract void OnCombatSystemInitialize();
		protected abstract void OnCombatSystemSubscribe();
		protected abstract void OnCombatSystemTerminate();
		protected abstract void OnCombatSystemUpdate();
		// Combat
		protected abstract void OnCombatStart();
		protected abstract void OnCombatRetry();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			this.Subscribe();
			this.OnCombatSystemSubscribe();
			this.OnCombatSystemInitialize();

			// Announce that the combat system has finished initializing
			StratusScene.Dispatch<InitializedEvent>(new InitializedEvent());
		}

		/// <summary>
		/// Updates the battle system every frame.
		/// </summary>
		void FixedUpdate()
		{
			if (IsResolved)
				return;

			this.OnCombatSystemUpdate();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Subscribe to events.
		/// </summary>
		protected virtual void Subscribe()
		{
			StratusScene.Connect<StratusCombatController.SpawnEvent>(this.OnCombatControllerSpawnEvent);
			StratusScene.Connect<StratusCombatController.DeactivateEvent>(this.OnCombatControllerDeathEvent);
			StratusScene.Connect<RetryEvent>(this.OnCombatRetryEvent);
		}

		/// <summary>
		/// Registers the CombatControllers depending on their given faction.
		/// </summary>
		/// <param name="e"></param>
		void OnCombatControllerSpawnEvent(StratusCombatController.SpawnEvent e)
		{
			OnCombatControllerSpawn(e.controller);
		}

		/// <summary>
		/// Received when a combat controller becomes inactive (such as being knocked out)
		/// </summary>
		/// <param name="e">The controller which has gone inactive. </param>
		void OnCombatControllerDeathEvent(StratusCombatController.DeactivateEvent e)
		{
			OnCombatControllerDeath(e.controller);
		}

		void OnCombatRetryEvent(RetryEvent e)
		{
			this.OnCombatRetry();
		}

		protected void StartCombat()
		{
			//Gamestate.Change(Gamestate.State.Combat);
			StratusScene.Dispatch<StratusCombat.StartedEvent>(new StratusCombat.StartedEvent());
		}

		protected void EndCombat()
		{
			//Gamestate.Revert();
			StratusScene.Dispatch<StratusCombat.EndedEvent>(new StratusCombat.EndedEvent());
		}
	}

	public enum StratusCombatState
	{
		Setup,
		Started,
		Ended
	}

	public enum StratusCombatResolutionResult
	{
		Ongoing,
		Victory,
		Defeat
	}

	public abstract class StratusCombatResolutionPredicate : IStratusLogger
	{
		public string label;

		public virtual string title => label;
		public abstract string description { get; }

		private Func<StratusCombatResolutionResult> isResolved { get; set; }

		public StratusCombatResolutionPredicate(string label)
		{
			this.label = label;
			this.isResolved = isResolved;
		}

		public abstract StratusCombatResolutionResult IsResolved(IStratusCombatSystem system);

		public override string ToString()
		{
			return label;
		}
	}
	public abstract class StratusCombatResolutionPredicate<CombatSystem> 
		: StratusCombatResolutionPredicate
		where CombatSystem : class, IStratusCombatSystem
	{
		public StratusCombatResolutionPredicate(string label) : base(label)
		{
		}

		public override StratusCombatResolutionResult IsResolved(IStratusCombatSystem system)
		{
			return OnIsResolved(system as CombatSystem);
		}

		protected abstract StratusCombatResolutionResult OnIsResolved(CombatSystem system);
	}

	public interface IStratusCombatSystem
	{
		void StartCombat(StratusCombatResolutionPredicate predicate);
		void EndCombat(StratusCombatResolutionResult resolution);
	}

	public abstract class StratusCombatResults
	{
		public StratusCombatResolutionResult resolution;

		public int unitsSpawned;
		public int unitsDefeated;

		public int playerUnitsSpawned;
		public int playerUnitsDefeated;

		public int enemyUnitsSpawned;
		public int enemyUnitsDefeated;
	}

	public abstract class StratusCombatSystem<T, CombatController, CombatResults> 
		: StratusSingletonBehaviour<T>, IStratusCombatSystem

		where T : StratusCombatSystem<T, CombatController, CombatResults>
		where CombatController : class, IStratusCombatController
		where CombatResults : StratusCombatResults, new()
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// Informs that combat has been resolved
		/// </summary>
		public class ResolveEvent : StratusEvent 
		{
		}
		/// <summary>
		/// Combat has started
		/// </summary>
		public class StartedEvent : StratusEvent 
		{
			public StratusCombatResolutionPredicate predicate;

			public StartedEvent(StratusCombatResolutionPredicate predicate)
			{
				this.predicate = predicate;
			}
		}

		/// <summary>
		/// Combat has ended
		/// </summary>
		public class EndedEvent : ResolveEvent
		{
			public CombatResults results { get; private set; }
			public StratusCombatResolutionResult resolution => results.resolution;

			public EndedEvent(CombatResults results)
			{
				this.results = results;
			}

		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private List<CombatController> _controllers = new List<CombatController>();
		private List<CombatController> _queuedControllers = new List<CombatController>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this system is being debugged currently
		/// </summary>
		public virtual bool debug => true;
		/// <summary>
		/// The current combat state
		/// </summary>
		public StratusCombatState state
		{
			get => _state;
			set
			{
				if (value != _state)
				{
					_state = value;
					onStateChanged?.Invoke(value);
				}
			}
		}
		private StratusCombatState _state;
		/// <summary>
		/// All active combat controllers
		/// </summary>
		protected CombatController[] controllers => _controllers.ToArray();
		/// <summary>
		/// The number of active combat controllers
		/// </summary>
		public int controllerCount => _controllers.Count;
		/// <summary>
		/// Decides how combat is resolved
		/// </summary>
		public StratusCombatResolutionPredicate resolutionPredicate { get; private set; }
		/// <summary>
		/// When combat starts, collects all relevant combat data
		/// </summary>
		public CombatResults results { get; private set; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public event Action<StratusCombatState> onStateChanged;
		public event Action<CombatController> onControllerAdded;
		public event Action<CombatController> onControllerRemoved;

		//------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------/
		protected abstract void OnCombatSystemInitialize();
		protected abstract void OnCombatSystemTerminate();

		protected abstract void OnStartCombat(CombatController[] controllers);
		protected abstract void OnEndCombat();
		protected abstract void OnCombatRetry();

		protected abstract void OnControllerAdded(CombatController combatController);
		protected abstract void OnControllerRemoved(CombatController combatController);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			state = StratusCombatState.Setup;
			OnCombatSystemInitialize();
			StratusScene.Connect<StratusCombatController.SpawnEvent>(OnCombatControllerSpawnEvent);
			//Scene.Connect<StratusCombatController.DespawnEvent>(OnCombatControllerDespawnEvent);
			StratusScene.Connect<StratusCombatController.ActivateEvent>(OnCombatControllerActivateEvent);
			StratusScene.Connect<StratusCombatController.DeactivateEvent>(OnCombatControllerDeactivateEvent);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void StartCombat(StratusCombatResolutionPredicate predicate)
		{
			state = StratusCombatState.Started;

			this.results = new CombatResults();
			this.resolutionPredicate = predicate;

			AddQueuedControllers();
			//ActivateControllers();
			OnStartCombat(controllers);

			this.Log($"Combat started with predicate: {resolutionPredicate}");
			StratusScene.Dispatch<StartedEvent>(new StartedEvent(predicate));
		}

		public void EndCombat(StratusCombatResolutionResult resolution)
		{
			state = StratusCombatState.Ended;

			OnEndCombat();
			this.Log($"Combat ended: {resolutionPredicate}");
			results.resolution = resolution;
			StratusScene.Dispatch<EndedEvent>(new EndedEvent( results));
		}

		public StratusCombatResolutionResult IsResolved()
		{
			if (state != StratusCombatState.Started)
			{
				throw new Exception("Combat not yet started");
			}

			return resolutionPredicate.IsResolved(this);
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// When a controller is initialized, we add it or queue it to be added
		/// when combat starts
		/// </summary>
		/// <param name="e"></param>
		private void OnCombatControllerSpawnEvent(StratusCombatController.SpawnEvent e)
		{
			CombatController controller = e.controller as CombatController;
			if (controller != null)
			{
				Add(controller);
			}
		}

		//private void OnCombatControllerDespawnEvent(StratusCombatController.DespawnEvent e)
		//{
		//	CombatController controller = e.controller as CombatController;
		//	if (controller != null)
		//	{
		//		//Remove(controller);
		//	}
		//}

		/// <summary>
		/// When a controller requests to be activated, it is valid only when combat has already started
		/// </summary>
		/// <param name="e"></param>
		private void OnCombatControllerActivateEvent(StratusCombatController.ActivateEvent e)
		{
			CombatController controller = e.controller as CombatController;
			if (controller != null)
			{
				if (state == StratusCombatState.Started)
				{
					e.valid = true;
				}
				else
				{
					this.LogError($"Cannot active {controller} while combat not started");
				}
			}
		}

		/// <summary>
		/// When a controller requests to be deactivated, it is valid only when combat has already started
		/// </summary>
		/// <param name="e"></param>
		private void OnCombatControllerDeactivateEvent(StratusCombatController.DeactivateEvent e)
		{
			CombatController controller = e.controller as CombatController;
			if (controller != null)
			{
				if (state == StratusCombatState.Started)
				{
					e.valid = true;
					Remove(controller);
				}
				else
				{
					this.LogError($"Cannot deactivate {controller} while combat not started");
				}
			}
		}

		//------------------------------------------------------------------------/
		// Controllers
		//------------------------------------------------------------------------/
		public void Add(CombatController controller)
		{
			if (state == StratusCombatState.Setup)
			{
				this.Log($"Queuing controller {controller} to be added when combat starts...");
				_queuedControllers.Add(controller);
				return;
			}

			_controllers.Add(controller);
			OnControllerAdded(controller);
			onControllerAdded?.Invoke(controller);
			controller.Activate();

			results.unitsSpawned++;
		}		

		public void Remove(CombatController controller)
		{
			_controllers.Remove(controller);
			OnControllerRemoved(controller);
			onControllerRemoved?.Invoke(controller);


			results.unitsDefeated++;
		}


		private void AddQueuedControllers()
		{
			foreach(var controller in _queuedControllers)
			{
				Add(controller);
			}
			_queuedControllers.Clear();
		}

		//private void ActivateControllers()
		//{
		//	this.Log("Now activating all controllers");
		//	foreach (var controller in controllers)
		//	{
		//		controller.Activate();
		//	}
		//}
	}

}