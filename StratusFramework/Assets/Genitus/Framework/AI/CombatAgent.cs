using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using Stratus.AI;

namespace Genitus.AI
{
  /// <summary>
  /// Base class for all combat-enabled agents within the Genitus framework
  /// </summary>  
  public abstract class CombatAgent : Agent
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public enum CombatState
    {
      /// <summary>
      /// Agent's engaged in combat
      /// </summary>
      Active,
      /// <summary>
      /// Agent is not in combat
      /// </summary>
      Inactive
    }

    /// <summary>
    ///  Signals that the agent has entered combat
    /// </summary>
    public class EnterCombatEvent : StatusEvent { }
    /// <summary>
    /// Signals that the agent has ended combat
    /// </summary>
    public class ExitCombatEvent : StatusEvent { }

    /// <summary>
    /// Signals that the agent has died
    /// </summary>
    public class DeathEvent : StatusEvent { public float delay; }
    /// <summary>
    /// Signals that the agent has been revived
    /// </summary>
    public class ReviveEvent : StatusEvent {}

    /// <summary>
    /// Base class for all engagement events
    /// </summary>
    public abstract class EngagementEvent : StatusEvent {}
    /// <summary>
    /// Signals the the agent has engaged the target
    /// </summary>
    public class EngagedEvent : EngagementEvent {}
    /// <summary>
    /// Signals that the agent has disengaged from the target
    /// </summary>
    public class DisengagedEvent : EngagementEvent {}

    /// <summary>
    /// Base class for all targeting events
    /// </summary>
    public abstract class TargetingEvent : Stratus.Event { public Agent target; }
    /// <summary>
    /// Signals to the agent to engage the target in combat
    /// </summary>
    public class EngageTargetEvent : TargetingEvent {}

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The list of targets currently engaged to this agent
    /// </summary>
    private List<Agent> _engagements = new List<Agent>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// If the agent is acting, what its current state is
    /// </summary>
    public CombatState combatState { get; private set; }
    /// <summary>
    /// The agent's current target
    /// </summary>
    public Agent target { get; protected set; }
    /// <summary>
    /// Whether the agent is currently targetable
    /// </summary>
    public bool targetable { get; private set; } = true;
    /// <summary>
    /// A list of all the agents engaged to this one
    /// </summary>
    public Agent[] engagements { get { return _engagements.ToArray(); } }

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    protected abstract void OnCombatAgentAwake();
    protected virtual void OnCombatAgentStart() {}
    protected abstract void OnCombatEnter();
    protected abstract void OnCombatExit();
    protected abstract void OnEngage(Agent target);
    protected abstract void OnDisengage();
    protected abstract void OnDisengaged(Agent agent);
    protected abstract void OnCombatAgentDeath();
    protected abstract void OnCombatAgentRevive();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAgentAwake()
    { 
      this.gameObject.Connect<EngageTargetEvent>(this.OnEngageTargetEvent);
      this.gameObject.Connect<EngagedEvent>(this.OnEngagedEvent);
      this.gameObject.Connect<DisengagedEvent>(this.OnDisengagedEvent);
      this.gameObject.Connect<DeathEvent>(this.OnDeathEvent);
      this.gameObject.Connect<ReviveEvent>(this.OnReviveEvent);
      this.OnCombatAgentAwake();
    }

    protected override void OnAgentStart()
    {
      this.OnCombatAgentStart();
    }

    protected override void OnAgentDestroy()
    {
      
    }

    protected override void OnAgentUpdate()
    {
      
    }

    protected override void OnAgentStop()
    {
      this.Disengage();
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    void OnEngageTargetEvent(EngageTargetEvent e)
    {
      this.target = e.target;
      this.Engage(e.target);
    }
    
    void OnEngagedEvent(EngagedEvent e)
    {
      //Trace.Script(e.Agent + " is engaged to me!", this);
      _engagements.Add(e.agent);
      // This agent has just entered combat
      if (_engagements.Count == 1)
      {
        Scene.Dispatch<EnterCombatEvent>(new EnterCombatEvent());
        this.OnCombatEnter();
      }
    }

    void OnDisengagedEvent(DisengagedEvent e)
    {
      //Trace.Script(e.Agent + " has disengaged from me!", this);
      OnDisengaged(e.agent);

      // This agent is no longer in combat
      if (_engagements.Count == 0)
      {
        Scene.Dispatch<ExitCombatEvent>(new ExitCombatEvent());
        this.OnCombatExit();
      }
      _engagements.Remove(e.agent);
    }

    /// <summary>
    /// Received when this agent has been marked as dead
    /// </summary>
    /// <param name="e"></param>
    void OnDeathEvent(DeathEvent e)
    {
      targetable = false;
      if (this.debug) Trace.Script("This agent has been killed.", this);
      this.Stop();
      this.OnCombatAgentDeath();

      // Inform the scene
      e.agent = this;
      Scene.Dispatch<DeathEvent>(e);
    }

    void OnReviveEvent(ReviveEvent e)
    {
      this.OnCombatAgentRevive();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Engages the specified target
    /// </summary>
    /// <param name="target"></param>
    public void Engage(Agent target)
    {
      if (debug)
        Trace.Script("Now engaging " + target.name, this);

      if (this.target)
        Disengage();

      // Inform the target that it's been engaged on
      this.target = target;
      SignalEngagement<EngagedEvent>(this.target);

      this.OnEngage(target);
      this.combatState = CombatState.Active;
    }

    /// <summary>
    /// Disengages from the current target, stopping navigation and setting the state to idle
    /// </summary>
    public void Disengage()
    {
      if (debug)
        Trace.Script("Disengaging!", this);

      // Inform that we have disengaged from the target
      if (this.target)
      {
        SignalEngagement<DisengagedEvent>(this.target);
        this.target = null;
      }

      if (this.navigation.isOnNavMesh)
      {
        this.navigation.isStopped = true;
        this.navigation.ResetPath();
      }

      this.OnDisengage();
      this.combatState = CombatState.Inactive;
      this.currentState = State.Idle;
    }

    /// <summary>
    /// Informs the enemy agent that it has been engaged upon by this agent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    public void SignalEngagement<T>(Agent target) where T : EngagementEvent, new()
    {
      var e = new T();
      e.agent = this;
      target.gameObject.Dispatch<T>(e);
      Scene.Dispatch<T>(e);
    }
  }

  /// <summary>
  /// Base class for all combat-enabled agents within the Genitus framework,
  /// that specifies what combat controller it uses
  /// </summary>  
  public abstract class CombatAgent<CombatController> : CombatAgent
    where CombatController : Genitus.CombatController
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The controller this agent is driving
    /// </summary>
    public CombatController controller { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/


  }

}