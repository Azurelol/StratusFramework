using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	/// <summary>
	///  An abstract template for setting up a Gamestate class for your project
	/// </summary>
	/// <typeparam name="State"></typeparam>
	public abstract class StratusGamestate<State> : StratusSingleton<StratusGamestate<State>>  
		where State : Enum
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public class StateChangeEvent : StratusEvent
		{
			public State state;
			public StateChangeEvent(State state) { this.state = state; }
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The current gamestate
		/// </summary>
		public static State current { get; private set; }

		/// <summary>
		/// The previous gamestate
		/// </summary>
		public static State previous { get; private set; }

		/// <summary>
		/// What the initial state should be 
		/// </summary>
		protected abstract State initialState { get; }

		/// <summary>
		/// Whether we are debugging gamestates
		/// </summary>
		protected virtual bool debug => false;

		/// <summary>
		/// All event handlers
		/// </summary>
		private static List<StateEventHandler> handlers = new List<StateEventHandler>();

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Invoked whenever the state has changed
		/// </summary>
		public static event Action<State> onChanged;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnInitialize()
		{
			current = initialState;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Changes the gamestate
		/// </summary>
		/// <param name="state">The new state</param>
		public static bool Set(State state)
		{
			if (current.Equals(state))
			{				
				return false;
			}

			previous = current;
			current = state;
			StratusDebug.Log($"State set to {state}");
			Announce();
			return true;
		}

		/// <summary>
		/// Announce the current state
		/// </summary>
		public static void Announce()
		{
			StratusScene.Dispatch<StateChangeEvent>(new StateChangeEvent(current));
			onChanged?.Invoke(current);
			InformHandlers();
		}

		/// <summary>
		/// Reverts to the previous state
		/// </summary>
		public static void Revert()
		{
			Set(previous);
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		/// <summary>
		/// Informs all handlers of the current state
		/// </summary>
		private static void InformHandlers()
		{
			foreach (StateEventHandler handler in handlers)
			{
				handler.Inform(current);
			}
		}

		//------------------------------------------------------------------------/
		// Subclasses
		//------------------------------------------------------------------------/
		/// <summary>
		/// Handles state changes
		/// </summary>
		public class StateEventHandler
		{
			//------------------------------------------------------------------------/
			// Declarations
			//------------------------------------------------------------------------/
			public delegate void Callback();
			public delegate void ToggleCallback(bool isAtState);

			//------------------------------------------------------------------------/
			// Fields
			//------------------------------------------------------------------------/
			/// <summary>
			/// The class instance  this handler belongs to
			/// </summary>
			private MonoBehaviour parent;

			/// <summary>
			/// The goal state for this handler
			/// </summary>
			private State goal;

			/// <summary>
			/// Function called when entering this state
			/// </summary>
			private Callback onEnterState;

			/// <summary>
			/// Function called when exiting this state
			/// </summary>
			private Callback onExitState;

			/// <summary>
			/// Function called when the state has changed
			/// </summary>
			private ToggleCallback onStateChange;

			/// <summary>
			/// Whether this handle is currently at the goal state
			/// </summary>
			private bool isAtGoalState = false;
			/// <summary>
			/// Whether to print state changes
			/// </summary>
			public bool logging = false;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="state">The desired goal state for this handler</param>
			/// <param name="onEnter">The function to be called when the goal state has been entered</param>
			/// <param name="onExit">The function to be called when the goal state has been exited</param>
			public StateEventHandler(State state, MonoBehaviour parent = null, bool tracing = false)
			{
				this.goal = state;
				this.parent = parent;
				this.logging = tracing;
				handlers.Add(this);

			}
			/// <summary>
			/// Sets callbacks for when the target goal state has been entered and exited
			/// </summary>
			/// <param name="onEnter">The function which will be invoked when the target goal state has been entered</param>
			/// <param name="onExit">The function which will be invoked when the target goal state has been exited</param>
			public void Set(Callback onEnter, Callback onExit)
			{
				this.onEnterState = onEnter;
				this.onExitState = onExit;
				this.Inform(current);
			}

			/// <summary>
			/// Sets a callback for a function that will receive a bool signaling whether the target goal state has been reached
			/// </summary>
			/// <param name="onStateChange"></param>
			public void Set(ToggleCallback onStateChange, bool isCalledImmediately = true)
			{
				this.onStateChange = onStateChange;
				if (isCalledImmediately)
				{
					this.Inform(current);
				}
			}

			/// <summary>
			/// Informs the handler of state changes
			/// </summary>
			/// <param name="state"></param>
			internal void Inform(State state)
			{
				//if (Tracing)
				//  Trace.Script("Goal = " + Goal + ", State = " + state, Parent);
				// If the state has been changed to the goal state and we are not there currently...
				if (state.Equals(this.goal) && !this.isAtGoalState)
				{
					if (this.logging)
					{
						StratusDebug.Log("Now at goal state '" + this.goal.ToString() + "'", this.parent);
					}

					this.isAtGoalState = true;
					if (this.onEnterState != null)
					{
						this.onEnterState();
					}
					else
					{
						this.onStateChange?.Invoke(this.isAtGoalState);
					}
				}
				// If we were at the goal state and the state has been changed to another...
				else if (state.Equals(this.goal) == false && this.isAtGoalState)
				{
					this.isAtGoalState = false;
					if (this.onExitState != null)
					{
						if (this.logging)
						{
							StratusDebug.Log("Now exiting state '" + state.ToString() + "'", this.parent);
						}

						this.onExitState();
					}
					else if (this.onStateChange != null)
					{
						if (this.logging)
						{
							StratusDebug.Log("Not at goal state '" + this.goal.ToString() + "', flipping state to " + this.isAtGoalState, this.parent);
						}

						this.onStateChange(this.isAtGoalState);
					}
				}
			}

			/// <summary>
			/// Terminates this statehandler
			/// </summary>
			public void Shutdown()
			{
				handlers.Remove(this);
			}

		}

	}

}