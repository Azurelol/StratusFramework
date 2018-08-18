/******************************************************************************/
/*!
@file   CombatControllerEvents.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Altostratus
{
  public abstract partial class CombatController : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Event Declarations
    //------------------------------------------------------------------------/

    // Actions

    /// <summary> Informs a change in status in this combat controller </summary>
    public abstract class CombatControllerEvent : Stratus.Event { public CombatController controller; }    
    /// <summary>
    /// Informs that a combat controller is ready to act
    /// </summary>
    public class ReadyEvent : CombatControllerEvent { }
    /// <summary>
    ///  Orders a combat controller to select its action
    /// </summary>
    public class SelectActionEvent : CombatControllerEvent { }
    /// <summary>
    /// Informs that a combat controller is now starting its selected action
    /// </summary>
    public class ActionSelectedEvent : CombatControllerEvent { public CombatAction action; }
    
    // State

    /// <summary>
    /// Informs that this controller has just spawned
    /// </summary>
    public class SpawnEvent : CombatControllerEvent { }
    /// <summary>
    /// Informs that this controller has become active
    /// </summary>
    public class ActiveEvent : CombatControllerEvent { }
    /// <summary>
    /// Informs that this controller has become inactive
    /// </summary>
    public class DeathEvent : CombatControllerEvent { }
    /// <summary>
    /// Informs that this controller has been revived
    /// </summary>
    public class ReviveEvent : CombatControllerEvent { }
    /// <summary>
    /// Signals to the controller that it should forcefully change its state
    /// </summary>
    public class ChangeStateEvent : Stratus.Event
    {
      public State nextState;
      public ChangeStateEvent(State nextState) { this.nextState = nextState; }
    }
    public class StateChangedEvent : CombatControllerEvent
    {
      public State currentState;
      public StateChangedEvent(State currState) { currentState = currState; }
    }

    /// <summary>
    /// Changes the control mode for this controller 
    /// </summary>
    public class ChangeControlModeEvent : Stratus.Event { public Character.ControlMode mode; }
    /// <summary>
    /// Sets the target for a controller
    /// </summary>
    public class TargetEvent : Stratus.Event { public CombatController target; }
    /// <summary>
    /// Resumes activity for this character.
    /// </summary>
    public class ResumeEvent : Stratus.Event { }
    /// <summary>
    /// Ceases activity for this character.
    /// </summary>
    public class PauseEvent : Stratus.Event { }

    // Resources
    public abstract class CheatEvent : Stratus.Event { public bool flag; }
    /// <summary>
    /// Signals that the character should ignore damage events
    /// </summary>
    public class InvulnerabilityEvent : CheatEvent { }
    ///// <summary>
    ///// Signals the character's stamina should be immediately replenished
    ///// </summary>
    //public class UnlimitedStaminaEvent : CheatEvent { }


    //------------------------------------------------------------------------/
    // Damage Resolution
    //------------------------------------------------------------------------/
    /// <summary>
    /// Signals that this character blocked the most recent source of damage
    /// </summary>
    public class DamageBlockedEvent : Stratus.Event { }
    /// <summary>
    /// Signals that this character has just received the specified amount of damage
    /// </summary>
    public class DamageReceivedEvent : Stratus.Event
    {
      public CombatController source;
      public float damage;
      public float healthPercentageLost;
    }
    /// <summary>
    /// Signals that the health of this character was recently modified
    /// </summary>
    public class HealthModifiedEvent : CombatControllerEvent
    {
      public HealthModifiedEvent(CombatController controller) { base.controller = controller; }
    }

    //------------------------------------------------------------------------/
    // Stamina
    //------------------------------------------------------------------------/        
    // Status events: Conditions applied to the character    
    public class StatusEvent : Stratus.Event { public Status.Instance Status; }
    /// <summary>
    /// Applies a status to the character.
    /// </summary>
    public class StatusAppliedEvent : StatusEvent {}
    /// <summary>
    /// Removes a status from the character.
    /// </summary>
    public class StatusEndedEvent : StatusEvent {}
    /// <summary>
    /// Signals that the character's current action should be interrupted
    /// </summary>
    public class InterruptEvent : Stratus.Event
    {
      /// <summary>
      /// How long after being interrupted should the character remain unable
      /// to resume action
      /// </summary>
      [Range(0f, 1f)]
      public float Duration;
    } 

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    /// <summary>
    /// Subscribes to events.
    /// </summary>
    protected virtual void Subscribe()
    {
      Scene.Connect<CombatSystem.TimeStepEvent>(this.OnTimeStepEvent);
      this.gameObject.Connect<ChangeStateEvent>(this.OnStateChangeEvent);
      // Damage events
      this.gameObject.Connect<Combat.DamageEvent>(this.OnDamageEvent);
      this.gameObject.Connect<Combat.HealEvent>(this.OnHealEvent);
      this.gameObject.Connect<Combat.RestoreEvent>(this.OnRestoreEvent);
      // Control events
      this.gameObject.Connect<TargetEvent>(this.OnTargetEvent);
      this.gameObject.Connect<PauseEvent>(this.OnPauseEvent);
      this.gameObject.Connect<ResumeEvent>(this.OnResumeEvent);
      this.gameObject.Connect<InterruptEvent>(this.OnInterruptEvent);
      // Effects events
      this.gameObject.Connect<StatusAppliedEvent>(this.OnStatusEvent);
      this.gameObject.Connect<PersistentEffect.ApplyEvent>(this.OnPersistentEffectApplyEvent);
      // States
      this.gameObject.Connect<InvulnerabilityEvent>(this.OnInvulnerabilityEvent);
    }

    /// <summary>
    /// Received when time has moved forward in combat.
    /// </summary>
    /// <param name="e"></param>
    void OnTimeStepEvent(CombatSystem.TimeStepEvent e)
    {
      this.TimeStep(e.Step);
    }

    void OnPauseEvent(PauseEvent e)
    {
      this.Pause();
    }

    void OnResumeEvent(ResumeEvent e)
    {
      this.Resume();
    }

    void OnInterruptEvent(InterruptEvent e)
    {
      this.Interrupt(e.Duration);
    }

    void OnStateChangeEvent(ChangeStateEvent e)
    {
      this.currentState = e.nextState;
    }

    /// <summary>
    /// Received when the target for this combat controller has been updated.
    /// </summary>
    /// <param name="e"></param>
    void OnTargetEvent(TargetEvent e)
    {
      this.currentTarget = e.target;
    }

    /// <summary>
    /// Received when a status is to be applied to this combat controller.
    /// </summary>
    /// <param name="e"></param>
    void OnStatusEvent(StatusAppliedEvent e)
    {
      //Trace.Script("Now applying " + e.Status.Status.Name);
      effects.Add(e.Status);
    }

    void OnPersistentEffectApplyEvent(PersistentEffect.ApplyEvent e)
    {      
    }

    //------------------------------------------------------------------------/
    // Events: States
    //------------------------------------------------------------------------/
    /// <summary>
    /// Received when this combat controller is to be made invulnerable (or not)
    /// </summary>
    /// <param name="e"></param>
    void OnInvulnerabilityEvent(InvulnerabilityEvent e)
    {
      this.OnInvulnerable(e.flag);
    }    

    //------------------------------------------------------------------------/
    // Events: Damage, Health, Stamina
    //------------------------------------------------------------------------/
    /// <summary>
    /// Received when the combat controller receives damage.
    /// </summary>
    /// <param name="e"></param>
    void OnDamageEvent(Combat.DamageEvent e)
    {
      this.OnDamage(e.value);
    }

    /// <summary>
    /// Received when the combat controller receives a heal.
    /// </summary>
    /// <param name="e"></param>
    void OnHealEvent(Combat.HealEvent e)
    {
      this.OnHeal(e.value);

      // Announce that health has been modified
      this.gameObject.Dispatch<HealthModifiedEvent>(new HealthModifiedEvent(this));
    }    

    /// <summary>
    /// Received when a character's health and stamina should be fully restored
    /// </summary>
    /// <param name="e"></param>
    void OnRestoreEvent(Combat.RestoreEvent e)
    {
      this.OnRestore();
    }


  }

}