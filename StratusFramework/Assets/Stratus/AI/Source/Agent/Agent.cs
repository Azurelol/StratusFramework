using System.Collections;
using Stratus.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Stratus.AI
{
	/// <summary>
	/// In artificial intelligence, an intelligent agent (IA) is an autonomous entity 
	/// which observes through sensors and acts upon an environment using actuators 
	/// and directs its activity towards achieving goals.
	/// </summary>
	public partial class Agent : ManagedBehaviour, Interfaces.Debuggable
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
		public abstract class StatusEvent : Stratus.StratusEvent
		{
			public Agent agent { get; set; }
		}
		/// <summary>
		/// Signals that the agent has spawned
		/// </summary>
		public class SpawnEvent : StatusEvent { }
		/// <summary>
		/// Signals the agent to start considering its next action
		/// </summary>
		public class AssessEvent : StatusEvent { }
		/// <summary>
		/// Signals to the agent that it should be disabled for a set amount of time
		/// </summary>
		public class DisableEvent : Stratus.StratusEvent
		{
			public float duration = 0f;
			public DisableEvent(float duration) { this.duration = duration; }
		}
		/// <summary>
		/// Signals that the agent should stop its current action
		/// </summary>
		public class StopEvent : Stratus.StratusEvent { }

		/// <summary>
		/// Signals the agent to move to a specified position
		/// </summary>
		public class MoveEvent : StatusEvent
		{
			public PositionField position;
			public MoveEvent(Vector3 point) { this.position.Set(point); }
		}

		/// <summary>
		/// Signals the agent that the given object can be interacted with. 
		/// </summary>
		public class InteractionAvailableEvent : Stratus.StratusEvent
		{
			public InteractableTrigger interactive;
			public string context;
		}


		//------------------------------------------------------------------------/
		// Fields: Public
		//------------------------------------------------------------------------/
		[Header("Status")]
		/// <summary>
		/// How the agent is currently controlled
		/// </summary>
		public Control control = Control.Automatic;
		/// <summary>
		/// The blackboard this agent is using
		/// </summary>
		public Blackboard blackboard => this.behavior.blackboard;
		/// <summary>
		/// The collection of behaviors to run on this agent (a behavior system such as a BT, Planner, etc)
		/// </summary>
		public BehaviorSystem behavior;
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
		/// The NavMeshAgent component used by this agent
		/// </summary>
		public NavMeshAgent navigation { get; private set; }
		/// <summary>
		/// The current state of this agent
		/// </summary>
		public State currentState { get; protected set; }
		/// <summary>
		/// Whether the agent is currently moving
		/// </summary>
		public bool isMoving => this.navigation.hasPath;
		/// <summary>
		/// The sensor this agent is using
		/// </summary>
		public Sensor sensor { get; protected set; }
		/// <summary>
		/// The rigidbody component used by this component
		/// </summary>
		public new Rigidbody rigidbody { get; private set; }
		/// <summary>
		/// If there's a behavior set for this agent
		/// </summary>
		public bool hasBehavior => this.behavior != null;

		//------------------------------------------------------------------------/
		// Fields: Private
		//------------------------------------------------------------------------/
		/// <summary>
		/// The currently running steering routine
		/// </summary>
		private IEnumerator steeringRoutine;

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		protected virtual void OnAgentAwake() { }
		protected virtual void OnAgentDestroy() { }
		protected virtual void OnAgentStart() { }
		protected virtual void OnAgentUpdate() { }
		protected virtual void OnAgentStop() { }
		protected virtual void OnTargetAgent(Agent agent)
		{
			this.MoveTo(agent.transform.position);
		}

		protected virtual void OnInteractScan(bool hasFoundInteractions) { }
		protected virtual bool OnAgentMoveTo(NavMeshPath path) { return false; }
		protected virtual void OnAgentMovementStarted() { }
		protected virtual void OnAgentMovementEnded() { }
		protected virtual void OnAgentPause() { }
		protected virtual void OnAgentResume() { }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnManagedAwake()
		{
			// Cache the main components, ho!
			this.navigation = this.GetComponent<NavMeshAgent>(); ;
			this.sensor = this.GetComponent<Sensor>();
			this.rigidbody = this.GetComponent<Rigidbody>();
			this.Subscribe();

			// Inform the agent is up
			this.OnAgentAwake();
			Scene.Dispatch<SpawnEvent>(new SpawnEvent() { agent = this });
		}

		protected override void OnManagedStart()
		{
			if (this.hasBehavior)
			{
				this.behavior = BehaviorSystem.InitializeSystemInstance(this, this.behavior);
			}

			this.OnAgentStart();
			this.currentState = State.Idle;
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

			if (this.hasBehavior && this.isAutomatic)
			{
				this.behavior.UpdateSystem();
			}

			this.OnAgentUpdate();
		}

		private void OnDisable()
		{
			if (this.steeringRoutine != null)
			{
				//Trace.Script("Stopping steering!", this);
				this.StopCoroutine(this.steeringRoutine);
			}
		}

		private void OnEnable()
		{
			this.navigation = this.GetComponent<NavMeshAgent>(); ;
			this.sensor = this.GetComponent<Sensor>();
			this.rigidbody = this.GetComponent<Rigidbody>();

			if (this.steeringRoutine != null)
			{
				//Trace.Script("Resuming steering!", this);
				this.navigation.enabled = true;
				this.StartCoroutine(this.steeringRoutine);
			}
		}

		void Debuggable.Toggle(bool toggle)
		{
			this.debug = toggle;
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Subscribes to events
		/// </summary>
		protected virtual void Subscribe()
		{
			this.gameObject.Connect<Sensor.InteractEvent>(this.OnInteractEvent);
			this.gameObject.Connect<Sensor.DetectionResultEvent>(this.OnInteractScanResultEvent);
			this.gameObject.Connect<MoveEvent>(this.OnMoveToEvent);
			this.gameObject.Connect<DisableEvent>(this.OnDisableEvent);
			this.gameObject.Connect<StopEvent>(this.OnStopEvent);
		}

		private void OnInteractEvent(Sensor.InteractEvent e)
		{
			if (this.sensor.closestInteractable)
			{
				if (this.debug)
				{
					StratusDebug.Log("Interacting!", this);
				}

				Sensor.InteractEvent interactEvent = new Sensor.InteractEvent
				{
					sensor = this.sensor
				};
				this.sensor.closestInteractable.gameObject.Dispatch<Sensor.InteractEvent>(interactEvent);
			}
		}

		private void OnInteractScanResultEvent(Sensor.DetectionResultEvent e)
		{
			this.OnInteractScan(e.hasFoundInteractions);
		}

		private void OnMoveToEvent(MoveEvent e)
		{
			this.MoveTo(e.position);
			e.agent = this;
			Scene.Dispatch<MoveEvent>(e);
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
		public void Target(Agent agent)
		{
			this.OnTargetAgent(agent);

		}

		/// <summary>
		/// Moves this agent to the target point
		/// </summary>
		/// <param name="point"></param>
		public bool MoveTo(Vector3 point)
		{
			if (!this.enabled)
			{
				return false;
			}

			if (this.navigation.destination == point)
			{
				return false;
			}


			// Reset the current path
			this.navigation.ResetPath();

			// Calculate a path to the point
			NavMeshPath path = new NavMeshPath();
			if (this.navigation.CalculatePath(point, path) && path.status == NavMeshPathStatus.PathComplete)
			{
				//if (this.logging) PrintPath(path);
				// If the path is not modified by a subclass, use it
				if (!this.OnAgentMoveTo(path))
				{
					this.navigation.SetPath(path);
				}
			}
			else
			{
				this.navigation.SetDestination(this.transform.localPosition);

				if (this.debug)
				{
					StratusDebug.Log("Can not move to that position!", this);
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Approaches the specified target
		/// </summary>
		/// <param name="target"></param>
		/// <param name="acceleration"></param>
		/// <param name="speed"></param>
		/// <param name="stoppingDistance"></param>
		/// <param name="angle"></param>
		public void ApproachTarget(Transform target, float stoppingDistance, float angle = 0f, float speed = 5f, float acceleration = 8f)
		{
			if (this.debug)
			{
				StratusDebug.Log("Will now approach " + target.name, this);
			}

			// Previously enabled was below steering routine?
			if (!this.enabled)
			{
				return;
			}

			if (this.steeringRoutine != null)
			{
				this.StopCoroutine(this.steeringRoutine);
			}

			this.steeringRoutine = this.ApproachTargetRoutine(target, speed, acceleration, stoppingDistance, angle);
			this.StartCoroutine(this.steeringRoutine);
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

			if (this.navigation.isOnNavMesh)
			{
				this.navigation.isStopped = true;
			}

			this.navigation.enabled = false;
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
			this.navigation.enabled = true;
			if (this.navigation.isOnNavMesh)
			{
				this.navigation.isStopped = false;
			}

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