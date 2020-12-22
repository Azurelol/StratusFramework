using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Stratus.AI
{
	/// <summary>
	/// Handles navigation for a stratus agent
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent))]
	public class StratusAgentNavigation : StratusAgentComponent
	{
		//------------------------------------------------------------------------/
		// Declaration
		//------------------------------------------------------------------------/
		/// <summary>
		/// Used to temporarily set, then revert navigation settings
		/// </summary>
		public struct NavigationSettings
		{
			private NavMeshAgent navigation;
			private float previousSpeed;
			private float previousAcceleration;
			private float previousStoppingDistance;

			public NavigationSettings(NavMeshAgent agent)
			{
				navigation = agent;
				previousSpeed = navigation.speed;
				previousAcceleration = navigation.acceleration;
				previousStoppingDistance = navigation.stoppingDistance;
			}

			public void Set(float speed, float acceleration, float stoppingDistance)
			{
				this.navigation.speed = speed;
				this.navigation.acceleration = acceleration;
				this.navigation.stoppingDistance = stoppingDistance;
			}

			public void Revert()
			{
				this.navigation.speed = previousSpeed;
				this.navigation.acceleration = previousAcceleration;
				this.navigation.stoppingDistance = previousStoppingDistance;
			}
		}

		/// <summary>
		/// Signals the agent to move to a specified position
		/// </summary>
		public class MoveEvent : StratusEvent
		{
			public StratusPositionField position { get; set; }
			public bool accepted { get; internal set; }

			public MoveEvent(Vector3 point)
			{
				this.position = new StratusPositionField(point);
			}
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether we are debugging the agent
		/// </summary>
		public bool debug = false;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/


		/// <summary>
		/// The NavMeshAgent component used by this agent
		/// </summary>
		public NavMeshAgent navigation { get; private set; }

		/// <summary>
		/// Whether the agent is currently moving
		/// </summary>
		public bool isMoving => this.navigation.hasPath;

		/// <summary>
		/// The currently running steering routine
		/// </summary>
		private IEnumerator steeringRoutine { get; set; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected virtual bool OnAgentMoveTo(NavMeshPath path) { return false; }
		protected virtual void OnAgentMovementStarted() { }
		protected virtual void OnAgentMovementEnded() { }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnManagedAwake()
		{
			base.OnManagedAwake();
			this.navigation = this.GetComponent<NavMeshAgent>();
			agent.onPause += this.OnAgentPause;
			agent.onResume += this.OnAgentResume;
			this.gameObject.Connect<MoveEvent>(this.OnMoveToEvent);
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
			if (this.steeringRoutine != null)
			{
				this.navigation.enabled = true;
				this.StartCoroutine(this.steeringRoutine);
			}
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void OnAgentResume()
		{
			this.navigation.enabled = true;
			if (this.navigation.isOnNavMesh)
			{
				this.navigation.isStopped = false;
			}
		}

		private void OnAgentPause()
		{
			if (this.navigation.isOnNavMesh)
			{
				this.navigation.isStopped = true;
			}
			this.navigation.enabled = false;
		}

		private void OnMoveToEvent(MoveEvent e)
		{
			e.accepted = this.MoveTo(e.position);
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		public static bool DispatchMoveToEvent(StratusAgent agent, Vector3 position)
		{
			MoveEvent e = new MoveEvent(position);
			agent.gameObject.Dispatch(e);
			return e.accepted;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
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
		/// Attempts to approach the target until it gets within range
		/// </summary>
		private IEnumerator ApproachTargetRoutine(Transform target, float speed, float acceleration, float stoppingDistance, float angle)
		{
			// Store, then set the new settings
			var navSettings = new NavigationSettings(this.navigation);
			navSettings.Set(speed, acceleration, stoppingDistance);

			//Trace.Script("Will now approach " + target.name + " up until " + stoppingDistance + " units at " + speed + " speed!", this);
			this.OnAgentMovementStarted();

			// While we are not within range of the target, keep making paths to it
			while (Vector3.Distance(transform.position, target.position) > stoppingDistance)
			{
				//Trace.Script("Current distance = " + dist);
				this.MoveTo(target.transform.position);

				yield return new WaitForFixedUpdate();
			}

			//Trace.Script("Approached " + target.name, this);
			// Now that we are in range of the target, let's revert to the old settings and stop moving
			this.navigation.isStopped = true;
			navSettings.Revert();
			this.steeringRoutine = null;

			this.OnAgentMovementEnded();
		}


		/// <summary>
		/// Moves this agent through a list of points.
		/// </summary>
		protected IEnumerator FollowPathRoutine(Vector3[] points, float speed, float acceleration, float stoppingDistance)
		{
			// Store, then set the new settings
			var navSettings = new NavigationSettings(this.navigation);
			navSettings.Set(speed, acceleration, stoppingDistance);

			this.OnAgentMovementStarted();

			IEnumerator drawRoutine = null;
			if (this.debug)
			{
				drawRoutine = DrawPathRoutine(points, Color.red, Color.yellow);
				StartCoroutine(drawRoutine);
			}

			// Now travel the path
			foreach (var point in points)
			{
				StratusDebug.Log("Moving to next point: " + point, this);
				while (Vector3.Distance(transform.position, point) > stoppingDistance)
				{
					this.navigation.SetDestination(point);
					yield return new WaitForFixedUpdate();
				}
			}

			StratusDebug.Log("Finished the path!");
			if (this.debug)
			{
				StopCoroutine(drawRoutine);
			}


			this.OnAgentMovementEnded();
			this.navigation.isStopped = true;
			navSettings.Revert();



		}

		private IEnumerator DrawPathRoutine(Vector3[] points, Color starting, Color ending)
		{
			while (true)
			{
				//Trace.Script("Drawing points!");
				foreach (var point in points)
				{
					UnityEngine.Debug.DrawRay(point, Vector3.up, Color.green);
				}
				yield return new WaitForFixedUpdate();
			}
		}
	}
}