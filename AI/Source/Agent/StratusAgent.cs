using System;

using UnityEngine;

namespace Stratus.AI
{
	/// <summary>
	/// In artificial intelligence, an intelligent agent (IA) is an autonomous entity 
	/// which observes through sensors and acts upon an environment using actuators 
	/// and directs its activity towards achieving goals.
	/// </summary>
	public partial class StratusAgent : StratusManagedBehaviour, IStratusDebuggable
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// The agent's base states
		/// </summary>
		public enum State
		{
			/// <summary>
			/// The agent is idle, performing no actions
			/// </summary>
			Idle,
			/// <summary>
			/// The agent is moving towards its target destination
			/// </summary>
			Moving,
			/// <summary>
			/// The agent is performing an action
			/// </summary>
			Action
		}

		public enum Control
		{
			Manual,
			Automatic
		}

		/// <summary>
		/// Base class for all status events
		/// </summary>
		public abstract class BaseEvent : StratusEvent
		{
			protected BaseEvent(StratusAgent agent)
			{
				this.agent = agent;
			}

			public StratusAgent agent { get; set; }
		}

		/// <summary>
		/// Signals that the agent has spawned
		/// </summary>
		public class SpawnEvent : BaseEvent
		{
			public SpawnEvent(StratusAgent agent) : base(agent)
			{
			}
		}

		/// <summary>
		/// Signals the agent to start considering its next action
		/// </summary>
		public class AssessEvent : BaseEvent
		{
			public AssessEvent(StratusAgent agent) : base(agent)
			{
			}
		}

		/// <summary>
		/// Signals to the agent that it should be disabled for a set amount of time
		/// </summary>
		public class DisableEvent : StratusEvent
		{
			public float duration = 0f;

			public DisableEvent(float duration) 
			{
				this.duration = duration;
			}
		}

		/// <summary>
		/// Signals that the agent should stop its current action
		/// </summary>
		public class StopEvent : StratusEvent { }

		//------------------------------------------------------------------------/
		// Fields: Public
		//------------------------------------------------------------------------/
		[Header("Status")]
		/// <summary>
		/// How the agent is currently controlled
		/// </summary>
		public Control control = Control.Automatic;
		/// <summary>
		/// The collection of behaviors to run on this agent (a behavior system such as a BT, Planner, etc)
		/// </summary>
		public StratusBehaviorSystem behavior;
		/// <summary>
		/// Whether this agent is active
		/// </summary>
		public bool active = true;
		/// <summary>
		/// Whether we are debugging the agent
		/// </summary>
		public bool debug = false;

		//------------------------------------------------------------------------/
		// Properties: Public 
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this agent is being driven by a behaviour system
		/// </summary>
		public bool isAutomatic => this.control == Control.Automatic;
		/// <summary>
		/// The current state of this agent
		/// </summary>
		public State currentState { get; protected set; }
		/// <summary>
		/// If there's a behavior set for this agent
		/// </summary>
		public bool hasBehavior => this.behavior != null;
		/// <summary>
		/// The blackboard this agent is using
		/// </summary>
		public StratusBlackboard blackboard => this.behavior.blackboard;

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		protected virtual void OnAgentAwake() { }
		protected virtual void OnAgentDestroy() { }
		protected virtual void OnAgentStart() { }
		protected virtual void OnAgentUpdate() { }
		protected virtual void OnAgentStop() { }
		protected virtual void OnTargetAgent(StratusAgent agent) { }
		protected virtual void OnAgentPause() { }
		protected virtual void OnAgentResume() { }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public event Action onPause;
		public event Action onResume;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnManagedAwake()
		{
			this.Subscribe();
			this.OnAgentAwake();
			StratusScene.Dispatch<SpawnEvent>(new SpawnEvent(this));
		}

		protected override void OnManagedStart()
		{
			this.OnAgentStart();
			this.currentState = State.Idle;
			if (this.hasBehavior)
			{
				this.behavior = StratusBehaviorSystem.InitializeSystemInstance(this, this.behavior);
			}
		}

		protected override void OnManagedDestroy()
		{
			this.OnAgentDestroy();
		}

		protected override void OnManagedUpdate()
		{
			if (!this.active)
			{
				return;
			}

			this.OnAgentUpdate();
			if (this.hasBehavior && isAutomatic)
			{
				this.behavior.UpdateSystem();
			}
		}

		void IStratusDebuggable.Toggle(bool debug)
		{
			this.debug = debug;
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Subscribes to events
		/// </summary>
		protected virtual void Subscribe()
		{
			this.gameObject.Connect<DisableEvent>(this.OnDisableEvent);
			this.gameObject.Connect<StopEvent>(this.OnStopEvent);
		}

		private void OnDisableEvent(DisableEvent e)
		{
			this.Disable(e.duration);
		}

		private void OnStopEvent(StopEvent e)
		{
			this.Stop();
		}

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/
		/// <summary>
		/// Targets the given agent, performing the default action on it
		/// </summary>
		/// <param name="agent"></param>
		public void Target(StratusAgent agent)
		{
			this.OnTargetAgent(agent);

		}

		/// <summary>
		/// Stops all of the agent's current actions
		/// </summary>
		public void Stop()
		{
			if (this.debug)
			{
				StratusDebug.Log("The agent has been stopped.", this);
			}

			this.OnAgentStop();
			this.StopAllCoroutines();
		}

		/// <summary>
		/// Pauses this agent, stopping its AI routines and navigation
		/// </summary>
		public void Pause()
		{
			if (this.debug)
			{
				StratusDebug.Log("Paused", this);
			}

			this.active = false;


			this.OnAgentPause();
		}

		/// <summary>
		/// Resumes the AI routines and navigation for this agent
		/// </summary>
		public void Resume()
		{
			if (this.debug)
			{
				StratusDebug.Log("Resumed", this);
			}

			this.active = true;
			this.OnAgentResume();
		}

		/// <summary>
		/// Disables this agent's behaviour temporarily
		/// </summary>
		/// <param name="duration"></param>
		public void Disable(float duration)
		{
			this.active = false;
			this.Stop();
			StratusActionSet seq = StratusActions.Sequence(this);
			StratusActions.Call(seq, () => { this.active = true; });
		}


	}
}