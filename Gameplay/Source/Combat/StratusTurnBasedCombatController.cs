using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	public interface IStratusTurnBasedCombatController : IStratusCombatController
	{
		bool moved { get; }
		bool canAct { get; }
		void StartTurn();
		void EndTurn();
		StratusCombatTargetRelation playerRelation { get; }

		event Action onTurnEnded;
	}

	/// <summary>
	/// Base class for turn-based combat actions
	/// </summary>
	/// <typeparam name="CombatController"></typeparam>
	public abstract class StratusTurnBasedAction<CombatController> 
		: StratusUndoAction
		where CombatController : IStratusTurnBasedCombatController
	{
		protected StratusTurnBasedAction(CombatController source)
		{
			this.source = source;
		}

		public CombatController source { get; private set; }
	}

	/// <summary>
	/// Base class for turn-based combat actions
	/// </summary>
	/// <typeparam name="CombatController"></typeparam>
	public abstract class StratusTurnBasedAction 
		: StratusTurnBasedAction<IStratusTurnBasedCombatController>
	{
		public StratusTurnBasedAction(IStratusTurnBasedCombatController source) : base(source)
		{
		}
	}

	/// <summary>
	/// Makes the character wait, ending its turn
	/// </summary>
	/// <typeparam name="CombatController"></typeparam>
	public class StratusTurnBasedWaitAction : StratusTurnBasedAction
	{
		public StratusTurnBasedWaitAction(IStratusTurnBasedCombatController source) 
			: base(source)
		{
		}

		protected override void OnExecute(Action onFinished = null)
		{
			source.EndTurn();
			onFinished?.Invoke();
		}

		protected override void OnUndo(Action onFinished = null)
		{
			source.StartTurn();
			onFinished?.Invoke();
		}
	}

	/// <summary>
	/// Kills the character
	/// </summary>
	/// <typeparam name="CombatController"></typeparam>
	public class StratusTurnBasedKillAction : StratusTurnBasedAction
	{
		public StratusTurnBasedKillAction(IStratusTurnBasedCombatController source)
			: base(source)
		{
		}

		protected override void OnExecute(Action onFinished = null)
		{
			source.Deactivate();
			onFinished?.Invoke();
		}

		protected override void OnUndo(Action onFinished = null)
		{
			source.Activate();
			onFinished?.Invoke();
		}
	}

	/// <summary>
	/// Makes the character wait, ending its turn
	/// </summary>
	/// <typeparam name="CombatController"></typeparam>
	public class StratusTurnBasedTargetAction<CombatController>
		: StratusTurnBasedAction<CombatController>
		where CombatController : class, IStratusTurnBasedCombatController
	{
		public delegate void TargetAction(CombatController source, CombatController target, Action onFinished = null);

		public CombatController target { get; private set; }
		private TargetAction onExecute { get; set; }
		private TargetAction onUndo{ get; set; }

		public StratusTurnBasedTargetAction(CombatController source, 
			CombatController target, TargetAction onExecute, TargetAction onUndo) 
			: base(source)
		{
			this.target = target;
			this.onExecute = onExecute;
			this.onUndo = onUndo;
		}
		
		protected override void OnExecute(Action onFinished = null)
		{
			onExecute.Invoke(source, target, onFinished);
		}

		protected override void OnUndo(Action onFinished = null)
		{
			onUndo.Invoke(source, target, onFinished);
		}

	}

	public interface IStratusGridTurnBasedCombatController : IStratusTurnBasedCombatController
	{
		Vector2Int movementRange { get; }
		Vector2Int attackRange { get; }
		Vector2Int interactionRange { get; }
	}

	/// <summary>
	/// Signals that the combat controller has taken its turn
	/// </summary>
	public class StratusCombatControllerTurnTakenEvent : StratusCombatControllerEvent
	{
		public StratusCombatControllerTurnTakenEvent(StratusCombatController controller) : base(controller)
		{
		}
	}

	/// <summary>
	/// A combat controller for a 2D grid turn based combat system
	/// </summary>
	/// <typeparam name="CharacterType"></typeparam>
	public abstract class StratusGridTurnbasedCombatController<CharacterType, ParameterType>
		: StratusCombatController<CharacterType, ParameterType>,
		IStratusGridTurnBasedCombatController

		where CharacterType : StratusCharacter
		where ParameterType : IStratusCombatParameterModel<CharacterType>, new()
	{

		public bool moved { get; set; }
		public int remainingActions { get; set; }
		public bool canAct => remainingActions > 0;
		public virtual int defaultActionCount => 1;

		public Vector2Int position => positionFunction();
		public abstract Vector2Int movementRange { get; }
		public abstract Vector2Int attackRange { get; }
		public abstract Vector2Int interactionRange { get; }
		public abstract StratusCombatTargetRelation playerRelation { get; }

		public event Action onTurnEnded;
		public event Action onActionsEnded;
		public event Action onActionsReset;

		private Func<Vector2Int> positionFunction;

		protected abstract void OnStartTurn();
		protected abstract void OnEndTurn();

		public StratusGridTurnbasedCombatController(StratusBehaviour behaviour,
			CharacterType character) : base(behaviour, character)
		{
		}

		protected override void OnCharacterControllerInitialize()
		{
			EndActions();
		}

		public void StartTurn()
		{
			this.Log("Starting turn");
			ResetActions();
		}

		public void EndTurn()
		{
			this.Log("Ending turn");
			EndActions();
			behaviour.InvokeNextFrame(NotifyCombatSystemTurnEnded);
			onTurnEnded?.Invoke();
		}

		protected override void OnActivate()
		{

		}

		protected override void OnDeactivate()
		{
			EndTurn();
		}

		private void ResetActions()
		{
			moved = false;
			remainingActions = defaultActionCount;
			onActionsReset?.Invoke();
		}

		protected void ConsumeAction()
		{
			remainingActions--;
			if (!canAct)
			{
				EndTurn();
			}
		}

		private void EndActions()
		{
			remainingActions = 0;
			onActionsEnded?.Invoke();
		}

		protected virtual void NotifyCombatSystemTurnEnded()
		{
			StratusScene.Dispatch<StratusCombatControllerTurnTakenEvent>(new StratusCombatControllerTurnTakenEvent(this));
		}

		public StratusTurnBasedWaitAction WaitAction()
		{
			return new StratusTurnBasedWaitAction(this);
		}

		public StratusTurnBasedKillAction KillAction()
		{
			return new StratusTurnBasedKillAction(this);
		}
	}
}