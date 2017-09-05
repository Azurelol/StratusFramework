/******************************************************************************/
/*!
@file   Agent.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stratus.Utilities;
using UnityEngine.AI;
using Stratus.Types;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// In artificial intelligence, an intelligent agent (IA) is an autonomous entity 
    /// which observes through sensors and acts upon an environment using actuators 
    /// and directs its activity towards achieving goals.
    /// </summary>
    public abstract partial class Agent : MonoBehaviour
    {
      public enum State
      {
        Idle,
        Moving,
        Engaged,
        Acting,
        Inactive
      }

      //------------------------------------------------------------------------/
      // Fields: Public
      //------------------------------------------------------------------------/
      [Header("Status")]
      /// <summary>
      /// Whether this agent is active
      /// </summary>
      public bool active = true;
      /// <summary>
      /// Whether we are debugging the agent
      /// </summary>
      public bool logging = false;

      //------------------------------------------------------------------------/
      // Properties: Public 
      //------------------------------------------------------------------------/
      /// <summary>
      /// The NavMeshAgent component used by this agent
      /// </summary>
      public NavMeshAgent navigation { get; private set; }
      /// <summary>
      /// Whether the agent is currently targetable
      /// </summary>
      public bool targetable { get; private set; } = true;       
      /// <summary>
      /// The blackboard this agent is using
      /// </summary>
      public abstract Blackboard blackboard { get; }
      /// <summary>
      /// The agent's current target
      /// </summary>
      public Agent target { get; protected set; }
      /// <summary>
      /// A list of all the agents engaged to this one
      /// </summary>
      public Agent[] engagements { get { return currentEngagements.ToArray(); } }
      /// <summary>
      /// The current state of this agent
      /// </summary>
      public State currentState { get; protected set; }
      /// <summary>
      /// Whether the agent is currently moving
      /// </summary>
      public bool isMoving { get { return navigation.hasPath; } }
      /// <summary>
      /// The sensor this agent is using
      /// </summary>
      public Sensor sensor { get; protected set; }      
      /// <summary>
      /// A renderer used for debugging purposes
      /// </summary>
      protected LineRenderer lineRenderer { get; set; }
      /// <summary>
      /// The rigidbody component used by this component
      /// </summary>
      public new Rigidbody rigidbody { get; private set; }

      //------------------------------------------------------------------------/
      // Fields: Private
      //------------------------------------------------------------------------/

      /// <summary>
      /// The currently running steering routine
      /// </summary>
      private IEnumerator steeringRoutine;
      /// <summary>
      /// The list of targets currently engaged to this agent
      /// </summary>
      private List<Agent> currentEngagements = new List<Agent>();

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnStart();
      protected abstract void OnSubscribe();
      protected abstract void OnUpdate();
      protected virtual void OnEngage(Agent target) { }
      protected virtual void OnDisengage() { }
      protected virtual void OnDisengaged(Agent agent) {}
      protected virtual void OnCombatEnter() { }
      protected virtual void OnCombatExit() { }
      protected abstract void OnDeath();
      protected virtual void OnInteractScan(bool hasFoundInteractions) { }
      protected virtual bool OnMoveTo(NavMeshPath path) { return false; }
      protected virtual void OnMovementStarted() {}
      protected virtual void OnMovementEnded() {}


      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes this agent
      /// </summary>
      void Start()
      {
        if (this.logging) this.AddLineRenderer();

        // Cache the main components, ho!
        this.navigation = GetComponent<NavMeshAgent>(); ;
        this.sensor = GetComponent<Sensor>();
        this.rigidbody = GetComponent<Rigidbody>();

        this.Subscribe();
        this.OnStart();
        currentState = State.Idle;
      }

      /// <summary>
      /// Updates this agent
      /// </summary>
      void FixedUpdate()
      {
        if (!active)
          return;

        //if (this.logging)
        //  this.Debug();

        this.OnUpdate();
      }

      private void OnDisable()
      {
        if (this.steeringRoutine != null)
        {
          //Trace.Script("Stopping steering!", this);
          StopCoroutine(this.steeringRoutine);
        }
      }

      private void OnEnable()
      {
        if (this.steeringRoutine != null)
        {
          //Trace.Script("Resuming steering!", this);
          this.navigation.enabled = true;
          StartCoroutine(this.steeringRoutine);
        }
      }


      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/
      /// <summary>
      /// Subscribes to events
      /// </summary>
      protected virtual void Subscribe()
      {
        this.gameObject.Connect<InteractEvent>(this.OnInteractEvent);
        this.gameObject.Connect<Sensor.InteractScanResultEvent>(this.OnInteractScanResultEvent);
        this.gameObject.Connect<DeathEvent>(this.OnDeathEvent);
        this.gameObject.Connect<MoveToEvent>(this.OnMoveToEvent);
        this.gameObject.Connect<EngageTargetEvent>(this.OnEngageTargetEvent);
        this.gameObject.Connect<EngagedEvent>(this.OnEngagedEvent);
        this.gameObject.Connect<DisengagedEvent>(this.OnDisengagedEvent);
        this.gameObject.Connect<SwitchTargetEvent>(this.OnSwitchTargetEvent);
        this.gameObject.Connect<DisableEvent>(this.OnDisableEvent);
        this.gameObject.Connect<StopEvent>(this.OnStopEvent);
        this.OnSubscribe();
      }

      /// <summary>
      /// Received when this agent has been marked as dead
      /// </summary>
      /// <param name="e"></param>
      void OnDeathEvent(DeathEvent e)
      {
        targetable = false;
        if (this.logging) Trace.Script("This agent has been killed.", this);
        this.Stop();
        this.OnDeath();
      }
      void OnInteractEvent(Agent.InteractEvent e)
      {
        if (this.sensor.closestInteractive)
        {
          if (this.logging) Trace.Script("Interacting!", this);
          var interactEvent = new Agent.InteractEvent();
          interactEvent.Object = this.gameObject;
          this.sensor.closestInteractive.gameObject.Dispatch<Agent.InteractEvent>(interactEvent);
        }
      }

      void OnInteractScanResultEvent(Sensor.InteractScanResultEvent e)
      {
        this.OnInteractScan(e.HasFoundInteractions);
      }

      void OnMoveToEvent(MoveToEvent e)
      {
        MoveTo(e.Point);
      }

      void OnEngageTargetEvent(EngageTargetEvent e)
      {
        this.target = e.Target;
        this.OnEngage(e.Target);
      }

      void OnSwitchTargetEvent(SwitchTargetEvent e)
      {
        //Trace.Script("Now switching target to " + e.Target);
        this.target = e.Target;
      }

      void OnEngagedEvent(EngagedEvent e)
      {
        //Trace.Script(e.Agent + " is engaged to me!", this);
        currentEngagements.Add(e.Agent);
        // This agent has just entered combat
        if (currentEngagements.Count == 1)
          this.OnCombatEnter();
      }

      void OnDisengagedEvent(DisengagedEvent e)
      {
        //Trace.Script(e.Agent + " has disengaged from me!", this);
        OnDisengaged(e.Agent);

        // This agent is no longer in combat
        if (currentEngagements.Count == 0)
          this.OnCombatExit();

        currentEngagements.Remove(e.Agent);
      }

      void OnDisableEvent(DisableEvent e)
      {
        this.Disable(e.Duration);
        //this.Interrupt(e.Duration);
      }

      void OnStopEvent(StopEvent e)
      {
        this.Stop();
      }

      //------------------------------------------------------------------------/
      // Methods: Public
      //------------------------------------------------------------------------/
      /// <summary>
      /// Moves this agent to the target point
      /// </summary>
      /// <param name="point"></param>
      public void MoveTo(Vector3 point)
      {
        if (!enabled)
          return;
        // Reset the current path
        navigation.ResetPath();

        // Calculate a path to the point
        NavMeshPath path = new NavMeshPath();
        if (navigation.CalculatePath(point, path) && path.status == NavMeshPathStatus.PathComplete)
        {
          //if (this.logging) PrintPath(path);
          // If the path is not modified by a subclass, use it
          if (!this.OnMoveTo(path))
            navigation.SetPath(path);
        }
        else
        {
          navigation.SetDestination(transform.localPosition);

          if (this.logging)
            Trace.Script("Can not move to that position!", this);
        }
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
        if (logging)
          Trace.Script("Will now approach " + target.name, this);

        // Previously enabled was below steering routine?
        if (!enabled)
          return;

        if (steeringRoutine != null)
          StopCoroutine(steeringRoutine);

        steeringRoutine = ApproachTargetRoutine(target, speed, acceleration, stoppingDistance, angle);
        StartCoroutine(steeringRoutine);
      }


      /// <summary>
      /// Engages the specified target
      /// </summary>
      /// <param name="target"></param>
      public void Engage(Agent target)
      {
        if (logging)
          Trace.Script("Now engaging " + target.name, this);

        if (this.target)
          Disengage();

        // Inform the target that it's been engaged on
        this.target = target;
        SignalEngagement<EngagedEvent>(this.target);

        this.OnEngage(target);
        this.currentState = State.Engaged;
      }

      /// <summary>
      /// Disengages from the current target, stopping navigation and setting the state to idle
      /// </summary>
      public void Disengage()
      {
        if (logging)
          Trace.Script("Disengaging!", this);

        // Inform that we have disengaged from the target
        if (this.target)
          SignalEngagement<DisengagedEvent>(this.target);
        this.target = null;

        if (this.navigation.isOnNavMesh)
        {
          this.navigation.isStopped = true;
          this.navigation.ResetPath();
        }

        this.OnDisengage();
        this.currentState = State.Idle;
      }

      /// <summary>
      /// Stops all of the agent's current actions
      /// </summary>
      public void Stop()
      {
        if (this.logging)
          Trace.Script("The agent has been stopped.", this);

        this.Disengage();
        this.StopAllCoroutines();
        //if (SteeringRoutine != null) StopCoroutine(SteeringRoutine);
      }

      /// <summary>
      /// Pauses this agent, stopping its AI routines and navigation
      /// </summary>
      public void Pause()
      {
        if (logging)
          Trace.Script("Paused", this);
        this.active = false;
        this.navigation.isStopped = true;
        this.navigation.enabled = false;
      }


      /// <summary>
      /// Resumes the AI routines and navigation for this agent
      /// </summary>
      public void Resume()
      {
        if (logging)
          Trace.Script("Resumed", this);
        this.active = true;
        this.navigation.enabled = true;
        this.navigation.isStopped = false;
      }

      /// <summary>
      /// Disables this agent's behaviour temporarily
      /// </summary>
      /// <param name="duration"></param>
      public void Disable(float duration)
      {
        this.active = false;
        this.Stop();
        var seq = Actions.Sequence(this);
        Actions.Call(seq, () => { this.active = true; });
      }

      /// <summary>
      /// Informs the enemy agent that it has been engaged upon by this agent
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="target"></param>
      public void SignalEngagement<T>(Agent target) where T : EngagementEvent, new()
      {
        var e = new T();
        e.Agent = this;
        target.gameObject.Dispatch<T>(e);
        Scene.Dispatch<T>(e);
      }

      /// <summary>
      /// Signals that this agent has died
      /// </summary>
      protected void SignalDeath()
      {
        this.gameObject.Dispatch<DeathEvent>(new DeathEvent(this));
      }
    }
  } 
}