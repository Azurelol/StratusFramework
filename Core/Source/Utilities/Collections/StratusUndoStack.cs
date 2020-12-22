using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public interface IStratusUndoAction
	{
		void Undo(Action onFinished = null);
		void Execute(Action onFinished = null);
	}

	public abstract class StratusUndoAction : IStratusUndoAction
	{
		public Action onAny { get; set; }
		public string description { get; set; }

		public void Undo(Action onFinished = null)
		{
			OnUndo(onFinished);
			onAny?.Invoke();
		}

		public void Execute(Action onFinished = null)
		{
			OnExecute(onFinished);
			onAny?.Invoke();
		}

		public override string ToString()
		{
			return description;
		}

		protected abstract void OnUndo(Action onFinished = null);
		protected abstract void OnExecute(Action onFinished = null);		
	}

	public abstract class StratusUndoAction<T> : StratusUndoAction
		where T : class
	{
		public T target { get; private set; }

		public StratusUndoAction(T target)
		{
			this.target = target;
		}
	}

	public class StratusUndoTargetAction<T> : StratusUndoAction<T>
		where T : class
	{
		private Action<T> undo, redo;

		public StratusUndoTargetAction(T target, Action<T> undo, Action<T> redo)
			: base(target)
		{
			this.undo = undo;
			this.redo = redo;
		}

		protected override void OnExecute(Action onFinished = null)
		{
			undo.Invoke(target);
		}

		protected override void OnUndo(Action onFinished = null)
		{
			redo.Invoke(target);
		}
	}


	public class StratusUndoStack<ActionType> : IStratusLogger
		where ActionType : IStratusUndoAction
	{
		private Stack<ActionType> undoStack = new Stack<ActionType>();
		private Stack<ActionType> redoStack = new Stack<ActionType>();
		public bool debug { get; set; }

		public override string ToString()
		{
			return $"{undoStack.Count}/{redoStack.Count}";
		}

		public void Record(ActionType action)
		{
			undoStack.Push(action);
		}

		public void ExecuteAndRecord(ActionType action)
		{
			action.Execute();
			Record(action);
		}

		public bool Redo(Action onFinished = null)
		{
			ActionType action = redoStack.PopOrDefault();
			if (action == null)
			{
				return false;
			}

			if (debug)
			{
				this.Log($"Redoing {action}");
			}
			action.Execute(onFinished);
			undoStack.Push(action);
			return true;
		}

		public bool Undo(Action onFinished = null)
		{
			ActionType action = undoStack.PopOrDefault();
			if (action == null)
			{
				return false;
			}

			if (debug)
			{
				this.Log($"Undoing {action}");
			}
			action.Undo(onFinished);
			redoStack.Push(action);
			return true;
		}

		public void Clear()
		{
			undoStack.Clear();
			redoStack.Clear();
		}

	}

	public class StratusUndoStack : StratusUndoStack<StratusUndoAction>
	{
	}

	public abstract class StratusUndoActionManager<T> : StratusSingleton<T>
		where T : StratusUndoActionManager<T>
	{
		public StratusUndoStack actions { get; private set; }
		public abstract bool debug { get; }

		protected override void OnInitialize()
		{
			actions = new StratusUndoStack();
			actions.debug = this.debug;
		}

		public static void Execute(StratusUndoAction action, Action onFinished)
		{
			action.Execute(onFinished);
			Record(action);
		}

		public static void Record(StratusUndoAction action) => instance.actions.Record(action);

		public static bool Undo() => instance.actions.Undo();
		public static bool Redo() => instance.actions.Redo();

		public override string ToString() => actions.ToString();
	}

}