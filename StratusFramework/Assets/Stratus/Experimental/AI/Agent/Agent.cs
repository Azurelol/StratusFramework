using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stratus.Utilities;
using UnityEngine.AI;
using Stratus.Types;
using Stratus.Interfaces;

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
    public abstract class StatusEvent : Stratus.Event
    {
      public Agent agent { get; internal set; }
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
    public class DisableEvent : Stratus.Event
    {
      public float duration = 0f;
      public DisableEvent(float duration) { this.duration = duration; }
    }
    /// <summary>
    /// Signals that the agent should stop its current action
    /// </summary>
    public class StopEvent : Stratus.Event { }

    /// <summary>
    /// Signals the agent to move to a specified position
    /// </summary>
    public class MoveEvent : StatusEvent
    {
      public PositionField position;
      public MoveEvent(Vector3 point) { position.Set(point); }
    }

    /// <summary>
    /// Signals the agent that the given object can be interacted with. 
    /// </summary>
    public class InteractionAvailableEvent : Stratus.Event
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
    public Control control;
    /// <summary>
    /// The blackboard this agent is using
    /// </summary>
    public Blackboard blackboard => behavior.blackboard;
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
    public bool isMoving { get { return navigation.hasPath; } }
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
    protected virtual void OnAgentAwake() {}
    protected virtual void OnAgentDestroy() {}
    protected virtual void OnAgentStart() {}
    protected virtual void OnAgentUpdate() {}
    protected virtual void OnAgentStop() {}
    protected virtual void OnTargetAgent(Agent agent) => this.MoveTo(agent.transform.position);
    protected virtual void OnInteractScan(bool hasFoundInteractions) { }
    protected virtual bool OnAgentMoveTo(NavMeshPath path) { return false; }
    protected virtual void OnAgentMovementStarted() { }
    protected virtual void OnAgentMovementEnded() { }
    protected virtual void OnAgentPause() { }
    protected virtual void OnAgentResume() { }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected internal override void OnManagedAwake()
    {
      // Cache the main components, ho!
      this.navigation = GetComponent<NavMeshAgent>(); ;
      this.sensor = GetComponent<Sensor>();
      this.rigidbody = GetComponent<Rigidbody>();
      this.Subscribe();

      // Inform the agent is up
      this.OnAgentAwake();
      Scene.Dispatch<SpawnEvent>(new SpawnEvent() { agent = this });
    }

    protected internal override void OnManagedStart()
    {
      if (hasBehavior)
        this.behavior = BehaviorSystem.InitializeSystemInstance(this, this.behavior);      

      this.OnAgentStart();
      currentState = State.Idle;
    }

    protected internal override void OnManagedDestroy()
    {
      this.OnAgentDestroy();
    }

    protected internal override void OnManagedUpdate()
    {
      if (!this.active)
        return;

      if (hasBehavior && isAutomatic)
        this.behavior.UpdateSystem();

      this.OnAgentUpdate();
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
      this.navigation = GetComponent<NavMeshAgent>(); ;
      this.sensor = GetComponent<Sensor>();
      this.rigidbody = GetComponent<Rigidbody>();

      if (this.steeringRoutine != null)
      {
        //Trace.Script("Resuming steering!", this);
        this.navigation.enabled = true;
        StartCoroutine(this.steeringRoutine);
      }
    }

    void Debuggable.Toggle(bool toggle)
    {
      debug = toggle;
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

    void OnInteractEvent(Sensor.InteractEvent e)
    {
      if (this.sensor.closestInteractable)
      {
        if (this.debug) Trace.Script("Interacting!", this);
        var interactEvent = new Sensor.InteractEvent();
        interactEvent.sensor = this.sensor;
        this.sensor.closestInteractable.gameObject.Dispatch<Sensor.InteractEvent>(interactEvent);
      }
    }

    void OnInteractScanResultEvent(Sensor.DetectionResultEvent e)
    {
      this.OnInteractScan(e.hasFoundInteractions);
    }

    void OnMoveToEvent(MoveEvent e)
    {
      MoveTo(e.position);
      e.agent = this;
      Scene.Dispatch<MoveEvent>(e);
    }

    void OnDisableEvent(DisableEvent e)
    {
      this.Disable(e.duration);
    }

    void OnStopEvent(StopEvent e)
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
      if (!enabled)
        return false;

      if (navigation.destination == point)
        return false;
            

      // Reset the current path
      navigation.ResetPath();

      // Calculate a path to the point
      NavMeshPath path = new NavMeshPath();
      if (navigation.CalculatePath(point, path) && path.status == NavMeshPathStatus.PathComplete)
      {
        //if (this.logging) PrintPath(path);
        // If the path is not modified by a subclass, use it
        if (!this.OnAgentMoveTo(path))
          navigation.SetPath(path);
      }
      else
      {
        navigation.SetDestination(transform.localPosition);

        if (this.debug)
          Trace.Script("Can not move to that position!", this);
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
      if (debug)
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
    /// Stops all of the agent's current actions
    /// </summary>
    public void Stop()
    {
      if (this.debug)
        Trace.Script("The agent has been stopped.", this);

      
      this.OnAgentStop();
      this.StopAllCoroutines();
    }

    /// <summary>
    /// Pauses this agent, stopping its AI routines and navigation
    /// </summary>
    public void Pause()
    {
      if (debug)
        Trace.Script("Paused", this);
      this.active = false;

      if (this.navigation.isOnNavMesh)
        this.navigation.isStopped = true;
      this.navigation.enabled = false;
      OnAgentPause();
    }

    /// <summary>
    /// Resumes the AI routines and navigation for this agent
    /// </summary>
    public void Resume()
    {
      if (debug)
        Trace.Script("Resumed", this);
      this.active = true;
      this.navigation.enabled = true;
      if (this.navigation.isOnNavMesh)
        this.navigation.isStopped = false;
      OnAgentResume();
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




  }
}