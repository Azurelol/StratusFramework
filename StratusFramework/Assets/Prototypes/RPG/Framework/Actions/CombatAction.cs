/******************************************************************************/
/*!
@file   CombatAction.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using Stratus.AI;

namespace Prototype
{
  /// <summary>
  /// An action taken in combat.
  /// </summary>
  public abstract class CombatAction
  {
    //----------------------------------------------------------------------/
    // Declarations
    //----------------------------------------------------------------------/
    /// <summary>
    /// Specific timings for the action's transitions
    /// </summary>
    [Serializable]
    public class Timings
    {
      /// <summary>
      /// How long to wait before the action starts casting
      /// </summary>
      [Tooltip("How long to wait before the action starts casting")]
      [Range(0f, 1.5f)]
      public float Start = 0.1f;
      /// <summary>
      /// How long this action will take to cast
      /// </summary>
      [Range(0f, 10f)]
      [Tooltip("How long it will take this action to cast")]
      public float Cast = 0.5f;
      /// <summary>
      /// How long to wait before the action is executed
      /// </summary>
      [Range(0f, 1.5f)]
      [Tooltip("How long to wait before the action starts executing.")]
      public float Trigger = 0.0f;
      /// <summary>
      /// How long it takes to execute the action
      /// </summary>
      [Range(0f, 1.5f)]
      [Tooltip("How long it takes to execute the action")]
      public float Execute = 0.25f;
      /// <summary>
      /// How long to wait before the action is ended
      /// </summary>
      [Range(0f, 1.5f)]
      [Tooltip("How long to wait before the action is ended")]
      public float End = 0.25f;
    }

    /// <summary>
    /// The phases, from beginning to end, of a combat action
    /// </summary>
    public enum Phase
    {
      /// <summary>
      /// The action has been selected and will start casting once it gets in range of the target
      /// </summary>
      Queue,
      /// <summary>
      /// The action has started
      /// </summary>
      Started,
      /// <summary>
      /// The action is now casting
      /// </summary>
      Casting,
      /// <summary>
      /// The action is ready to be executed
      /// </summary>
      Trigger,
      /// <summary>
      /// The action is being executed
      /// </summary>
      Execute,
      /// <summary>
      /// The action has ended
      /// </summary>
      Ended
    }

    //----------------------------------------------------------------------/
    // Events
    //----------------------------------------------------------------------/
    public abstract class CombatActionEvent : Stratus.Event { public CombatAction Action; }
    /// <summary>
    /// Signals that an action has been queued up
    /// </summary>
    public class QueueEvent : CombatActionEvent { }
    /// <summary>
    /// When an action should be selected by the combat controller
    /// </summary>
    public class SelectEvent : CombatActionEvent { }
    /// <summary>
    /// When the action has started casting
    /// </summary>
    public class StartedEvent : CombatActionEvent { }
    /// <summary>
    /// When the action needs the agent to approach the target to within a specified range.
    /// </summary>
    public class ApproachEvent : CombatActionEvent
    {
      public CombatController Target;
      public float Range;
    }
    /// <summary>
    /// Signals that an action is ready to be executed
    /// </summary>
    public class TriggerEvent : CombatActionEvent { }
    /// <summary>
    /// Signals that an action is executing
    /// </summary>
    public class ExecuteEvent : CombatActionEvent { public CombatTrigger.Result Result; }
    /// <summary>
    /// Signals that an action has finished executing
    /// </summary>
    public class EndedEvent : CombatActionEvent { }
    /// <summary>
    /// Cancels the action
    /// </summary>
    public class CancelEvent : CombatActionEvent { }
    /// <summary>
    /// Delays an action for a specified amount of time
    /// </summary>
    public class DelayEvent : CombatActionEvent { public float Delay; }
    /// <summary>
    /// When an action failed to be created.
    /// </summary>
    public class FailedEvent : CombatActionEvent { }
    /// <summary>
    /// Reassess what action to take
    /// </summary>
    public class ReassessEvent : CombatActionEvent { }
    //----------------------------------------------------------------------/
    // Properties
    //----------------------------------------------------------------------/

    /// <summary>
    /// Specific timers for this action's transitions
    /// </summary>
    public Timings Timers;
    /// <summary>
    /// At what range from the target should this action take place 
    /// </summary>
    public float Range = 3.0f;
    /// <summary>
    /// The caster of this action
    /// </summary>
    public CombatController User;
    /// <summary>
    /// The target of this action
    /// </summary>
    public CombatController Target;

    //------------------------------------------------------------------------/
    /// <summary>
    /// How much of the cast has progressed, as a percentage  
    /// </summary>
    public float castProgress
    {
      get
      {
        if (Timers.Cast == 0.0f) return 1.0f;
        return castTimer / Timers.Cast;
      }
    }

    /// <summary>
    /// The name of this action
    /// </summary>
    public abstract string name { get; }

    /// <summary>
    /// Description of the action 
    /// </summary>
    public abstract string description { get; }
    
    //----------------------------------------------------------------------/
    // Fields
    //----------------------------------------------------------------------/
    /// <summary>
    /// The current phase this action is on
    /// </summary>
    public Phase currentPhase { get; private set; }
    /// <summary>
    /// Whether the action has finished executing.
    /// </summary>
    public bool isFinished { get; private set; }
    /// <summary>
    /// Whether the user of this action is within range 
    /// </summary>
    private bool isWithinRange = false;
    /// <summary>
    /// How much time has spent casting the action
    /// </summary>
    protected float castTimer = 0.0f;
    /// <summary>
    /// The sequence currently being played for the active phase
    /// </summary>
    private ActionSet currentPhaseSequence;

    //----------------------------------------------------------------------/
    // Interface
    //----------------------------------------------------------------------/
    protected abstract void OnStart(CombatController user, CombatController target);
    protected abstract void OnCasting(CombatController user, CombatController target, float step);
    protected abstract void OnTrigger(CombatController user, CombatController target);
    protected abstract void OnExecute(CombatController user, CombatController target);
    protected abstract void OnEnd();
    protected abstract void OnCancel();
    //protected abstract void OnCast(CombatController user, CombatController target, float step);

    //----------------------------------------------------------------------/
    // Methods
    //----------------------------------------------------------------------/
    /// <summary>
    /// CombatAction constructor.
    /// </summary>
    /// <param name="user">The controller who will be running this action.</param>
    /// <param name="target">The target this action will be acted upon.</param>
    /// <param name="castTime">The cast time of this action.</param>
    /// <param name="range">The range of this action.</param>
    /// <param name="duration"></param>
    public CombatAction(CombatController user, CombatController target, float range, Timings timings)
    {
      this.Initialize(user, target, range, timings);
    }

    /// <summary>
    /// Initializes the action, configuring it for use.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="castTime"></param>
    /// <param name="range"></param>
    /// <param name="duration"></param>
    public void Initialize(CombatController user, CombatController target, float range, Timings timings)
    {
      this.User = user;
      this.Target = target;
      this.Range = range;
      this.Timers = timings;

      // Inform the user that we have picked this target
      var targetEvent = new CombatController.TargetEvent();
      targetEvent.Target = target;
      user.gameObject.Dispatch<CombatController.TargetEvent>(targetEvent);

      // Inform the user to approach the target
      var approachEvent = new ApproachEvent();
      approachEvent.Action = this;
      approachEvent.Target = this.Target;
      approachEvent.Range = this.Range;
      user.gameObject.Dispatch<ApproachEvent>(approachEvent);

      currentPhase = Phase.Queue;
    }

    /// <summary>
    /// Updates this CombatAction.
    /// </summary>
    /// <param name="timeStep">How much to update the action</param>
    public void Update(float timeStep)
    {
      if (isFinished)
        return;

      // Check whether we are in range of the target 
      if (!isWithinRange)
      {
        if (Library.CheckRange(User.transform, Target.transform, this.Range))
        {
          isWithinRange = true;
          this.Start(User, Target);
          //this.User.gameObject.Dispatch<Movement.EndedEvent>(new Movement.EndedEvent());
        }
      }

      // Once within range of the target, start casting the action
      else
      {
        //Cast(this.User, this.Target, timeStep);
        switch (this.currentPhase)
        {
          case Phase.Casting:
            Cast(this.User, this.Target, timeStep);
            break;
          case Phase.Trigger:
            break;
        }
      }

    }

    /// <summary>
    /// After getting in range of the target, starts the action. It will begin casting it.
    /// </summary>
    /// <param name="user">The controller who will be running this action.</param>
    /// <param name="target"></param>
    protected void Start(CombatController user, CombatController target)
    {
      currentPhase = Phase.Started;
      //Trace.Script("Starting", user);

      // Called the first time the action is about to start casting
      //this.OnStart(user, target);
      // Inform the controller the the action has started casting
      this.currentPhaseSequence = Actions.Sequence(this.User);
      Actions.Call(this.currentPhaseSequence, () => Inform<StartedEvent>());
      Actions.Delay(this.currentPhaseSequence, Timers.Start);
      Actions.Call(this.currentPhaseSequence, ()=>this.OnStart(user, target));
      Actions.Call(this.currentPhaseSequence, () => 
      {
        currentPhase = Phase.Casting;
      });
      //Actions.Property(seq, () => this.CurrentPhase, Phase.Casting, 0f, Ease.Linear);
    }

    /// <summary>
    /// Continuously casts the current action. Once it has finished casting, it will be triggered.
    /// </summary>
    /// <param name="user">The controller who will be running this action.</param>
    /// <param name="target">The target this action will be acted upon.</param>
    /// <param name="timeStep">How much will the cast be progressed</param>
    protected virtual void Cast(CombatController user, CombatController target, float timeStep)
    {
      // Update the timer
      castTimer += timeStep;

      //Trace.Script("Casting action , cast timer = " + this.CastTimer, user);
      // Once we are done casting the current action, execute it! It will first
      // animate it and check for a trigger.
      // Once the trigger has run, it will be calling this action's 'Execute' method
      if (castTimer >= Timers.Cast)
      {
        this.Trigger(user, target);
      }
      // If the skill is still casting,...
      else
      {
        OnCasting(user, target, timeStep);
      }
    }

    /// <summary>
    /// Triggers this action, signaling that it is ready to be executed.
    /// </summary>
    /// <param name="user"></param>
    public void Trigger(CombatController user, CombatController target)
    {
      currentPhase = Phase.Trigger;

      // Inform the skill is ready to be triggered
      this.currentPhaseSequence = Actions.Sequence(this.User);
      Actions.Call(this.currentPhaseSequence, () => Inform<TriggerEvent>(), Timers.Trigger);
      // The action is now finished updating
      isFinished = true;
      // Invoke the trigger
      this.OnTrigger(user, target);
    }

    /// <summary>
    /// Begins executing of this action. After it is executed, it will be considered ended.
    /// </summary>
    public void Execute()
    {
      currentPhase = Phase.Execute;
      Trace.Script("Now executing", User);
      this.currentPhaseSequence = Actions.Sequence(this.User);
      Actions.Delay(this.currentPhaseSequence, this.Timers.Execute);
      Actions.Call(this.currentPhaseSequence, () => this.OnExecute(this.User, this.Target));
      Actions.Call(this.currentPhaseSequence, () => Inform<ExecuteEvent>());
      Actions.Call(this.currentPhaseSequence, () => this.End(this.User));
    }

    /// <summary>
    /// Announces the end of this action.
    /// </summary>
    /// <param name="user"></param>
    protected void End(CombatController user)
    {
      currentPhase = Phase.Ended;

      this.OnEnd();
      this.currentPhaseSequence = Actions.Sequence(this.User);
      Actions.Call(this.currentPhaseSequence, () => Inform<EndedEvent>(), Timers.End);
    }

    /// <summary>
    /// Delays the cast time of this action.
    /// </summary>
    /// <param name="delay">How much to delay this action.</param>
    public void Delay(float delay)
    {
      //Trace.Script("Delayed by " + delay, this.User);
      this.castTimer -= delay;
      if (this.castTimer < 0.0f) this.castTimer = 0.0f;
    }

    /// <summary>
    /// Cancels the current action.
    /// </summary>
    public void Cancel()
    {      
      Trace.Script("Action cancelled!", User);
      if (this.currentPhaseSequence != null)
        this.currentPhaseSequence.Cancel();
      this.OnCancel();
      this.End(this.User);
    }

    /// <summary>
    /// Inform the combat controller of the current phase in the action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void Inform<T>() where T : CombatAction.CombatActionEvent, new()
    {
      var actionEvent = new T();
      actionEvent.Action = this;
      this.User.gameObject.Dispatch<T>(actionEvent);
    }
    
    

  }

}