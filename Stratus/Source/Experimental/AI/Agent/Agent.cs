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
      public bool Active = true;
      /// <summary>
      /// Whether we are debugging the agent
      /// </summary>
      bool IsDebugging = false;

      //------------------------------------------------------------------------/
      // Properties: Public 
      //------------------------------------------------------------------------/
      /// <summary>
      /// The blackboard this agent is using
      /// </summary>
      public abstract Blackboard Blackboard { get; }
      /// <summary>
      /// The agent's current target
      /// </summary>
      public Agent Target { get; protected set; }
      /// <summary>
      /// A list of all the agents engaged to this one
      /// </summary>
      public Agent[] Engagements { get { return CurrentEngagements.ToArray(); } }
      /// <summary>
      /// The current state of this agent
      /// </summary>
      public State CurrentState { get; protected set; }
      /// <summary>
      /// Whether the agent is currently moving
      /// </summary>
      public bool IsMoving { get { return Navigation.hasPath; } }
      /// <summary>
      /// The sensor this agent is using
      /// </summary>
      public Sensor Sensor { get; protected set; }      
      /// <summary>
      /// A renderer used for debugging purposes
      /// </summary>
      protected LineRenderer LineRenderer { get; set; }

      //------------------------------------------------------------------------/
      // Fields: Private
      //------------------------------------------------------------------------/
      /// <summary>
      /// The currently running steering routine
      /// </summary>
      private IEnumerator SteeringRoutine;
      /// <summary>
      /// The list of targets currently engaged to this agent
      /// </summary>
      private List<Agent> CurrentEngagements = new List<Agent>();

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnStart();
      protected abstract void OnSubscribe();
      protected abstract void OnUpdate();
      protected virtual void OnEngage(Agent target) { }
      protected virtual void OnDisengage() { }
      protected virtual void OnCombatEnter() { }
      protected virtual void OnCombatExit() { }
      protected abstract void OnDeath();
      protected virtual void OnInteractScan(bool hasFoundInteractions) { }
      protected virtual bool OnMoveTo(NavMeshPath path) { return false; }
      protected virtual void OnMovementStarted() {}
      protected virtual void OnMovementEnded() {}
      public NavMeshAgent Navigation { get { return this.gameObject.GetComponent<NavMeshAgent>(); } }

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes this agent
      /// </summary>
      void Start()
      {
        if (this.IsDebugging) this.AddLineRenderer();
        this.Sensor = GetComponent<Sensor>();
        this.Subscribe();
        this.OnStart();
        CurrentState = State.Idle;
      }

      /// <summary>
      /// Updates this agent
      /// </summary>
      void FixedUpdate()
      {
        if (!Active)
          return;

        if (this.IsDebugging)
          this.Debug();

        this.OnUpdate();
      }

      private void OnDisable()
      {
        if (this.SteeringRoutine != null)
        {
          Trace.Script("Stopping steering!", this);
          StopCoroutine(this.SteeringRoutine);
        }
      }

      private void OnEnable()
      {
        if (this.SteeringRoutine != null)
        {
          Trace.Script("Resuming steering!", this);
          this.Navigation.enabled = true;
          StartCoroutine(this.SteeringRoutine);
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
        if (this.IsDebugging) Trace.Script("This agent has been killed.", this);
        this.Stop();
        this.OnDeath();
      }
      void OnInteractEvent(Agent.InteractEvent e)
      {
        if (this.Sensor.ClosestInteractive)
        {
          if (this.IsDebugging) Trace.Script("Interacting!", this);
          var interactEvent = new Agent.InteractEvent();
          interactEvent.Object = this.gameObject;
          this.Sensor.ClosestInteractive.gameObject.Dispatch<Agent.InteractEvent>(interactEvent);
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
        this.Target = e.Target;
        this.OnEngage(e.Target);
      }

      void OnSwitchTargetEvent(SwitchTargetEvent e)
      {
        //Trace.Script("Now switching target to " + e.Target);
        this.Target = e.Target;
      }

      void OnEngagedEvent(EngagedEvent e)
      {
        //Trace.Script(e.Agent + " is engaged to me!", this);
        CurrentEngagements.Add(e.Agent);
        // This agent has just entered combat
        if (CurrentEngagements.Count == 1)
          this.OnCombatEnter();
      }

      void OnDisengagedEvent(DisengagedEvent e)
      {
        //Trace.Script(e.Agent + " has disengaged from me!", this);
        CurrentEngagements.Remove(e.Agent);
        // This agent is no longer in combat
        if (CurrentEngagements.Count == 0)
          this.OnCombatExit();

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
        Navigation.ResetPath();

        // Calculate a path to the point
        NavMeshPath path = new NavMeshPath();
        if (Navigation.CalculatePath(point, path) && path.status == NavMeshPathStatus.PathComplete)
        {
          if (this.IsDebugging) PrintPath(path);
          // If the path is not modified by a subclass, use it
          if (!this.OnMoveTo(path))
            Navigation.SetPath(path);
        }
        else
        {
          Navigation.SetDestination(transform.localPosition);

          if (this.IsDebugging)
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
        if (SteeringRoutine != null) StopCoroutine(SteeringRoutine);
        SteeringRoutine = ApproachTargetRoutine(target, speed, acceleration, stoppingDistance, angle);
        if (!enabled) return;
        StartCoroutine(SteeringRoutine);
      }


      /// <summary>
      /// Engages the specified target
      /// </summary>
      /// <param name="target"></param>
      public void Engage(Agent target)
      {
        //if (IsDebugging)
        Trace.Script("Now engaging " + target.name, this);

        if (this.Target)
          Disengage();

        // Inform the target that it's been engaged on
        this.Target = target;
        SignalEngagement<EngagedEvent>(this.Target);

        this.OnEngage(target);
        this.CurrentState = State.Engaged;
      }

      /// <summary>
      /// Disengages from the current target, stopping navigation and setting the state to idle
      /// </summary>
      public void Disengage()
      {
        if (IsDebugging)
          Trace.Script("Disengaging!", this);

        // Inform that we have disengaged from the target
        if (this.Target)
          SignalEngagement<DisengagedEvent>(this.Target);
        this.Target = null;

        if (this.Navigation.isOnNavMesh)
        {
          this.Navigation.isStopped = true;
          this.Navigation.ResetPath();
        }

        this.OnDisengage();
        this.CurrentState = State.Idle;
      }

      /// <summary>
      /// Stops all of the agent's current actions
      /// </summary>
      public void Stop()
      {
        if (this.IsDebugging)
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
        //Trace.Script("Pausing agent!");
        this.Active = false;
        this.Navigation.isStopped = true;
      }


      /// <summary>
      /// Resumes the AI routines and navigation for this agent
      /// </summary>
      public void Resume()
      {
        this.Active = true;
        this.Navigation.isStopped = false;
      }

      /// <summary>
      /// Disables this agent's behaviour temporarily
      /// </summary>
      /// <param name="duration"></param>
      public void Disable(float duration)
      {
        this.Active = false;
        this.Stop();
        var seq = Actions.Sequence(this);
        Actions.Call(seq, () => { this.Active = true; });
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