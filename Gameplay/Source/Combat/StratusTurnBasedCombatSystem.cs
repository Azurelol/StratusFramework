using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Stratus.Gameplay
{
	public enum StratusTurnOrderType
	{
		Phases,
		Interleaved
	}

	public interface IStratusTurnBasedCombatSystem
		: IStratusCombatSystem
	{
		int turn { get; }
		StratusTurnOrderType turnOrderType { get; }
		bool IsFactionRemaining(string faction);
	}

	public abstract class StratusTurnBasedCombatResolutionPredicate<CombatSystem>
		: StratusCombatResolutionPredicate<CombatSystem>
		where CombatSystem : class, IStratusTurnBasedCombatSystem
	{

		public abstract string playerFaction { get; }
		public abstract string[] mandatoryCharacters { get; }

		public bool hasMandatoryCharacters => mandatoryCharacters.IsValid();

		public StratusTurnBasedCombatResolutionPredicate(string label) : base(label)
		{
		}
	}

	public class StratusTurnBasedCombatResults : StratusCombatResults
	{
		public int turns;
	}

	public abstract class StratusTurnBasedCombatSystem<T, CombatController, CombatResults>
		: StratusCombatSystem<T, CombatController, CombatResults>,
		IStratusTurnBasedCombatSystem

		  where T : StratusCombatSystem<T, CombatController, CombatResults>
		  where CombatController : class, IStratusTurnBasedCombatController
		  where CombatResults : StratusTurnBasedCombatResults, new()
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Base class for events signaling turn changes
		/// </summary>
		public abstract class TurnEvent : StratusEvent
		{
			public int turn;
		}

		/// <summary>
		/// Informs that a combat turn has passed
		/// </summary>
		public class TurnStartedEvent : TurnEvent
		{
			public Round[] rounds { get; set; }
		}

		/// <summary>
		/// Informs that a combat turn has passed
		/// </summary>
		public class TurnEndedEvent : TurnEvent
		{
		}

		public class Round
		{
			public string faction { get; private set; }
			public Action<Round> onFinished { get; private set; }

			public Round(string faction, Action<Round> onFinished)
			{
				this.onFinished = onFinished;
				this.faction = faction;
			}

			public void Return()
			{
				onFinished?.Invoke(this);
			}
		}

		/// <summary>
		/// A request to inform a group that it can now act
		/// </summary>
		public class TurnCommandRequestEvent : StratusEvent
		{
			public TurnCommandRequestEvent(Round turnControl)
			{
				this.turnControl = turnControl;
			}

			public Round turnControl { get; private set; }

		}

		public class RoundTakenEvent : StratusEvent
		{

		}

		public abstract class CommandHandler : IStratusLogger
		{
			public abstract string faction { get; }

			public void ReceiveControl(Action onFinished)
			{
				(this).Log($"{this.faction} received control");
				OnReceiveControl(onFinished);
			}
			protected abstract void OnReceiveControl(Action onFinished);
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public int turn
		{
			get => results.turns;
			private set
			{
				results.turns = value;
				onTurnChanged?.Invoke(value);
			}
		}
		public abstract StratusTurnOrderType turnOrderType { get; }
		public HashSet<CombatController> readyControllers { get; private set; }
		public Dictionary<string, HashSet<CombatController>> controllersByFaction { get; private set; }
		public bool hasActiveControllers => readyControllers.Count > 0;
		public bool hasControllers => controllerCount > 0;
		public int numberOfFactions => controllersByFaction.Count;
		private Queue<Round> turnOrderByFaction { get; set; }
		public Round[] rounds => this.turnOrderByFaction.ToArray();
		private List<string> controllerFactionNames { get; set; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public event Action<int> onTurnChanged;
		public event Action onTurnStarted;
		public event Action onTurnEnded;

		//------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------/
		protected abstract void OnTurnBasedCombatSystemInitialize();
		protected abstract void OnTurnStarted();
		protected abstract void OnTurnEnded();
		protected abstract void OnTurnBasedCombatStarted();
		protected abstract void OnTurnBasedCombatEnded();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnCombatSystemInitialize()
		{
			readyControllers = new HashSet<CombatController>();
			controllersByFaction = new Dictionary<string, HashSet<CombatController>>();
			turnOrderByFaction = new Queue<Round>();
			controllerFactionNames = new List<string>();

			StratusScene.Connect<StratusCombatControllerTurnTakenEvent>(this.OnStratusCombatTurnTakenEvent);

			OnTurnBasedCombatSystemInitialize();
		}

		protected override void OnCombatSystemTerminate()
		{
		}

		protected override void OnCombatRetry()
		{
		}

		protected override void OnStartCombat(CombatController[] controllers)
		{
			turn = 0;
			StartTurn();
		}

		protected override void OnEndCombat()
		{
			OnTurnBasedCombatEnded();
			Clear();
		}

		private void Clear()
		{
			controllersByFaction.Clear();
			turnOrderByFaction.Clear();
			controllerFactionNames.Clear();
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void OnStratusCombatTurnTakenEvent(StratusCombatControllerTurnTakenEvent e)
		{
			CombatController controller = e.controller as CombatController;
			if (controller != null)
			{
				NotifyTurnEnded(controller);
			}
		}

		//------------------------------------------------------------------------/
		// Methods: Turn
		//------------------------------------------------------------------------/
		public StratusValidation StartTurn(bool force = false)
		{
			if (!hasControllers)
			{
				this.LogError("No controllers present. Cannot start turn");
				return false;
			}

			turn++;
			this.Log($"Turn {this.turn} started for {controllers.Length} controllers");
			foreach (var controller in controllers)
			{
				readyControllers.Add(controller);
				controller.StartTurn();
			}

			GenerateTurnOrder();
			TurnStartedEvent e = new TurnStartedEvent();
			e.turn = this.turn;
			e.rounds = this.rounds;
			StratusScene.Dispatch<TurnStartedEvent>(e);
			onTurnStarted?.Invoke();
			RequestNextCommand();
			OnTurnStarted();
			return true;
		}

		public bool EndTurn(bool force)
		{
			if (hasActiveControllers && !force)
			{
				this.LogWarning("Cannot end turn while there are active controllers remaining");
				return false;
			}

			this.Log($"Turn {this.turn} ended");
			OnTurnEnded();
			StratusScene.Dispatch<TurnEndedEvent>(new TurnEndedEvent() { turn = this.turn });
			onTurnEnded?.Invoke();

			StartTurn();
			return true;
		}

		public void NotifyTurnEnded(CombatController controller)
		{
			this.Log($"Turn ended for {controller}");
			readyControllers.Remove(controller);
			StratusScene.Dispatch<RoundTakenEvent>(new RoundTakenEvent());

			StratusCombatResolutionResult resolution = IsResolved();
			switch (resolution)
			{
				case StratusCombatResolutionResult.Ongoing:
					if (!hasActiveControllers)
					{
						EndTurn(false);
					}
					else
					{
						this.Log("There's still active controllers?");
					}
					break;

				case StratusCombatResolutionResult.Victory:
				case StratusCombatResolutionResult.Defeat:
					EndCombat(resolution);
					break;
			}

		}

		public bool IsFactionRemaining(string faction)
		{
			return controllersByFaction.ContainsKey(faction);
		}

		//------------------------------------------------------------------------/
		// Turn Order
		//------------------------------------------------------------------------/
		private void GenerateTurnOrder()
		{
			this.Log($"Generating turn order for n={this.numberOfFactions}  groups for type: {this.turnOrderType}");
			turnOrderByFaction.Clear();

			switch (this.turnOrderType)
			{
				case StratusTurnOrderType.Phases:
					foreach (KeyValuePair<string, HashSet<CombatController>> group in controllersByFaction)
					{
						foreach (CombatController controller in group.Value)
						{
							Round control = new Round(controller.faction, OnTurnActionTaken);
							turnOrderByFaction.Enqueue(control);
						}
					}
					break;

				case StratusTurnOrderType.Interleaved:

					// For every controller, create 1 turn action
					for (int i = 0; i < controllerCount; ++i)
					{
						// Pick a random group
						int g = UnityEngine.Random.Range(0, controllerFactionNames.Count - 1);
						string group = controllerFactionNames[g];
						Round control = new Round(group, OnTurnActionTaken);
						turnOrderByFaction.Enqueue(control);
					}
					break;
			}
		}

		private void OnTurnActionTaken(Round command)
		{
			this.Log($"Turn action taken by {command.faction}");
			RequestNextCommand();
		}

		private void RequestNextCommand()
		{
			if (turnOrderByFaction.Count == 0)
			{
				this.LogWarning("No further actions can be taken this turn");
				EndTurn(false);
				return;
			}

			Round control = turnOrderByFaction.Dequeue();
			TurnCommandRequestEvent e = new TurnCommandRequestEvent(control);
			StratusScene.Dispatch<TurnCommandRequestEvent>(e);
			this.Log($"Submitting command request to group {control.faction}");
		}

		//------------------------------------------------------------------------/
		// Methods: Debug
		//------------------------------------------------------------------------/
		public string DescribeControllersByFaction()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var faction in controllersByFaction)
			{
				sb.AppendLine($"{faction.Key}:");
				foreach (var controller in faction.Value)
				{
					sb.AppendLine($"-{controller}");
				}
			}
			return sb.ToString();
		}

		//------------------------------------------------------------------------/
		// Methods: Factions
		//------------------------------------------------------------------------/
		public bool IsPlayerFaction(CombatController combatController)
		{
			return combatController.playerRelation == StratusCombatTargetRelation.Self;
		}

		//------------------------------------------------------------------------/
		// Methods: Controllers
		//------------------------------------------------------------------------/
		protected override void OnControllerAdded(CombatController combatController)
		{
			if (!controllersByFaction.ContainsKey(combatController.faction))
			{
				this.Log($"Added combat controller faction: {combatController.faction}");
				controllerFactionNames.Add(combatController.faction);
				controllersByFaction.Add(combatController.faction, new HashSet<CombatController>());
			}

			this.Log($"Adding controller {combatController.faction} : {combatController}");
			controllersByFaction[combatController.faction].Add(combatController);
			switch (combatController.playerRelation)
			{
				case StratusCombatTargetRelation.Self:
					results.playerUnitsSpawned++;
					break;
				case StratusCombatTargetRelation.Friendly:
					break;
				case StratusCombatTargetRelation.Neutral:
					break;
				case StratusCombatTargetRelation.Hostile:
					results.enemyUnitsSpawned++;
					break;
				default:
					break;
			}
			results.unitsSpawned++;
		}

		protected override void OnControllerRemoved(CombatController combatController)
		{
			this.Log($"Removing controller {combatController.faction} : {combatController}");
			controllersByFaction[combatController.faction].Remove(combatController);
			if (controllersByFaction[combatController.faction].Count == 0)
			{
				this.Log($"Removing combat controller faction: {combatController.faction}");
				controllersByFaction.Remove(combatController.faction);
				controllerFactionNames.Remove(combatController.faction);
			}
			switch (combatController.playerRelation)
			{
				case StratusCombatTargetRelation.Self:
					results.playerUnitsDefeated++;
					break;
				case StratusCombatTargetRelation.Friendly:
					break;
				case StratusCombatTargetRelation.Neutral:
					break;
				case StratusCombatTargetRelation.Hostile:
					results.enemyUnitsDefeated++;
					break;
			}
			results.unitsDefeated++;
		}
	}
}