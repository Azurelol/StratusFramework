/******************************************************************************/
/*!
@file   CombatControllerEvents.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype
{
  public abstract partial class CombatController : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Event Declarations
    //------------------------------------------------------------------------/

    // Actions

    /// <summary> Informs a change in status in this combat controller </summary>
    public abstract class CombatControllerEvent : Stratus.Event { public CombatController Controller; }    
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
    public class ActionSelectedEvent : CombatControllerEvent { public CombatAction Action; }
    
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
      public State NextState;
      public ChangeStateEvent(State nextState) { NextState = nextState; }
    }
    public class StateChangedEvent : CombatControllerEvent
    {
      public State CurrentState;
      public StateChangedEvent(State currState) { CurrentState = currState; }
    }

    /// <summary>
    /// Changes the control mode for this controller 
    /// </summary>
    public class ChangeControlModeEvent : Stratus.Event { public Character.ControlMode Mode; }
    /// <summary>
    /// Sets the target for a controller
    /// </summary>
    public class TargetEvent : Stratus.Event { public CombatController Target; }
    /// <summary>
    /// Resumes activity for this character.
    /// </summary>
    public class ResumeEvent : Stratus.Event { }
    /// <summary>
    /// Ceases activity for this character.
    /// </summary>
    public class PauseEvent : Stratus.Event { }

    // Resources
    public abstract class CheatEvent : Stratus.Event { public bool Flag; }
    /// <summary>
    /// Signals that the character should ignore damage events
    /// </summary>
    public class InvulnerabilityEvent : CheatEvent { }
    /// <summary>
    /// Signals the character's stamina should be immediately replenished
    /// </summary>
    public class UnlimitedStaminaEvent : CheatEvent { }
    /// <summary>
    /// Signals that the character's health should be fully restored
    /// </summary>
    public class FullRestoreEvent : Stratus.Event { }

    //------------------------------------------------------------------------/
    // Health
    //------------------------------------------------------------------------/
    public abstract class ModifyHealthEvent : Stratus.Event
    {
      /// <summary>
      /// The source that is modifying this character's health
      /// </summary>
      public CombatController source;
      /// <summary>
      /// By how much to modify the character's health
      /// </summary>
      public float value;
    }
    
    public class DamageEvent : ModifyHealthEvent
    {
      public int penetration;
    }
    public class HealEvent : ModifyHealthEvent {}

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
      public HealthModifiedEvent(CombatController controller) { Controller = controller; }
    }

    //------------------------------------------------------------------------/
    // Stamina
    //------------------------------------------------------------------------/        
    public abstract class ModifyStaminaEvent : Stratus.Event { public float Value; }
    public class ConsumeStaminaEvent : ModifyStaminaEvent {}
    public class GainStaminaEvent : ModifyStaminaEvent { }
    public class StaminaModifiedEvent : Stratus.Event { }
    
    // Status events: Conditions applied to the character
    
    /// <summary>
    /// Applies a status to the character.
    /// </summary>
    public class StatusEvent : Stratus.Event { public Status.Instance Status; }
    /// <summary>
    /// Removes a status from the character.
    /// </summary>
    public class StatusEndedEvent : Stratus.Event { public Status.Instance Status; }
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
    void Subscribe()
    {
      Scene.Connect<CombatSystem.TimeStepEvent>(this.OnTimeStepEvent);
      this.gameObject.Connect<ChangeStateEvent>(this.OnStateChangeEvent);
      // Combat actions
      //this.gameObject.Connect<SelectActionEvent>(this.OnSelectActionEvent);
      //this.gameObject.Connect<ActionSelectedEvent>(this.OnActionSelectedEvent);
      this.gameObject.Connect<CombatAction.QueueEvent>(this.OnCombatActionQueueEvent);
      this.gameObject.Connect<CombatAction.StartedEvent>(this.OnCombatActionStartedEvent);
      this.gameObject.Connect<CombatAction.ApproachEvent>(this.OnCombatActionApproachEvent);
      this.gameObject.Connect<CombatAction.TriggerEvent>(this.OnCombatActionTriggerEvent);
      this.gameObject.Connect<CombatAction.ExecuteEvent>(this.OnCombatActionExecuteEvent);
      this.gameObject.Connect<CombatAction.EndedEvent>(this.OnCombatActionEndedEvent);
      this.gameObject.Connect<CombatAction.CancelEvent>(this.OnCombatActionCancelEvent);
      this.gameObject.Connect<CombatAction.DelayEvent>(this.OnCombatActionDelayEvent);
      this.gameObject.Connect<CombatAction.FailedEvent>(this.OnCombatActionFailedEvent);
      // Damage events
      this.gameObject.Connect<DamageEvent>(this.OnDamageEvent);
      this.gameObject.Connect<HealEvent>(this.OnHealEvent);
      this.gameObject.Connect<FullRestoreEvent>(this.OnRestoreEvent);
      // Stamina
      this.gameObject.Connect<GainStaminaEvent>(this.OnGainStaminaEvent);
      this.gameObject.Connect<ConsumeStaminaEvent>(this.OnConsumeStaminaEvent);
      // Control events
      this.gameObject.Connect<TargetEvent>(this.OnTargetEvent);
      this.gameObject.Connect<PauseEvent>(this.OnPauseEvent);
      this.gameObject.Connect<ResumeEvent>(this.OnResumeEvent);
      this.gameObject.Connect<InterruptEvent>(this.OnInterruptEvent);
      // Effects events
      this.gameObject.Connect<StatusEvent>(this.OnStatusEvent);
      this.gameObject.Connect<PersistentEffect.ApplyEvent>(this.OnPersistentEffectApplyEvent);
      // States
      this.gameObject.Connect<InvulnerabilityEvent>(this.OnInvulnerabilityEvent);
      this.gameObject.Connect<UnlimitedStaminaEvent>(this.OnUnlimitedStaminaEvent);
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
      this.currentState = e.NextState;
    }

    /// <summary>
    /// Received when the target for this combat controller has been updated.
    /// </summary>
    /// <param name="e"></param>
    void OnTargetEvent(TargetEvent e)
    {
      this.currentTarget = e.Target;
    }

    /// <summary>
    /// Received when a status is to be applied to this combat controller.
    /// </summary>
    /// <param name="e"></param>
    void OnStatusEvent(StatusEvent e)
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
      //Trace.Script("Invulnerable = " + e.Invulnerable, this);
      this.isInvulnerable = e.Flag;
      if (this.isInvulnerable)
        this.defense.SetModifier(1000);
      else
        this.defense.ClearModifiers();
    }

    void OnUnlimitedStaminaEvent(UnlimitedStaminaEvent e)
    {
      this.hasUnlimitedStamina = e.Flag;
    }


    //------------------------------------------------------------------------/
    // Events: Combat Actions
    //------------------------------------------------------------------------/
    /// <summary>
    /// Queues an action. At the moment we only run one at a time!
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionQueueEvent(CombatAction.QueueEvent e)
    {
      currentAction = e.Action;
    }


    /// <summary>
    /// Received when an action requires this controller to approach
    /// its target
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionApproachEvent(CombatAction.ApproachEvent e)
    {
      OnActionApproach(e.Action);
    }    



    ///// <summary>
    ///// Received when this CombatController is ready to act.
    ///// </summary>
    ///// <param name="e"></param>
    //void OnSelectActionEvent(SelectActionEvent e)
    //{      
    //  //this.OnSelectAction();
    //}

    /// <summary>
    /// Received when a combat action has been assigned and has started running.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionStartedEvent(CombatAction.StartedEvent e)
    {
      //CurrentAction = e.Action;
      // Inform the space that we have started an action. This will resume its updating.
      this.OnActionStarted(currentAction);
      Scene.Dispatch<CombatAction.StartedEvent>(e);
    }

    /// <summary>
    /// Received when a combat action is ready to be triggered.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionTriggerEvent(CombatAction.TriggerEvent e)
    {
      // Inform the ATB combat system we are now triggering an action. This will stop its updating.
      Scene.Dispatch<CombatAction.TriggerEvent>(e);
      
      // Execute the action. In the future add some QTEs or whatever here?
      e.Action.Execute();

      // Resolve trigger (if there's one)
      //if (e.Action is CombatActionSkill)
      //{
      //  //var skillAction 
      //}

    }

    /// <summary>
    /// Received when a combat action is being executed/
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionExecuteEvent(CombatAction.ExecuteEvent e)
    {
      // If we were in the middle of executing when combat was resolved (victory/defeat),
      // this will be null!
      //if (this.CurrentAction == null)
      //  return;
      //
      // Inform the ATB combat system we are done executing an action. This will resume its updating.
      //Scene.Dispatch<CombatAction.ExecuteEvent>(e);

      //if (e.Result == CombatTrigger.Result.Success)
      //{
      //  this.CurrentAction.Execute();
      //}
      //else
      //{
      //  this.CurrentAction.Cancel();
      //  Trace.Script("Trigger failed!", this);
      //}

    }

    /// <summary>
    /// Received when a combat action has ended. This will reset timers?
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionEndedEvent(CombatAction.EndedEvent e)
    {
      this.OnActionEnded(e.Action);

      // Inform the combat system we are done executing an action. This will resume its updating.
      Scene.Dispatch<CombatAction.EndedEvent>(e);
    }

    /// <summary>
    /// Received when a combat action is supposed to be cancelled. 
    /// (Such as by an interrupt attack)?
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionCancelEvent(CombatAction.CancelEvent e)
    {
      this.Cancel();
    }

    void OnCombatActionDelayEvent(CombatAction.DelayEvent e)
    {
      this.OnActionDelay(e.Delay);
    }

    /// <summary>
    /// Received when a combat action failed to be constructed.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionFailedEvent(CombatAction.FailedEvent e)
    {
      this.OnActionFailed();
    }

    //------------------------------------------------------------------------/
    // Events: Damage, Health, Stamina
    //------------------------------------------------------------------------/
    /// <summary>
    /// Received when the combat controller receives damage.
    /// </summary>
    /// <param name="e"></param>
    void OnDamageEvent(DamageEvent e)
    {
      Trace.Script("Received damage event", this);

      if (!isReceivingDamage)
      {
        if (logging) Trace.Script("Not receiving damage!", this);
        return;
      }

      this.OnDamage(e.value);
    }

    /// <summary>
    /// Received when the combat controller receives a heal.
    /// </summary>
    /// <param name="e"></param>
    void OnHealEvent(HealEvent e)
    {
      this.health.Add(e.value);
      //Trace.Script("Healh now at " + Health.Current, this);
      // Announce that health has been modified
      this.gameObject.Dispatch<HealthModifiedEvent>(new HealthModifiedEvent(this));
    }
    
    /// <summary>
    /// Received when the character has consumed stamina
    /// </summary>
    /// <param name="e"></param>
    void OnConsumeStaminaEvent(ConsumeStaminaEvent e)
    {
      this.ConsumeStamina(e.Value);
    }

    /// <summary>
    /// Received when the character has gained stamina
    /// </summary>
    /// <param name="e"></param>
    void OnGainStaminaEvent(GainStaminaEvent e)
    {
      this.GainStamina(e.Value);
    }    
    
    /// <summary>
    /// Received when a character's health and stamina should be fully restored
    /// </summary>
    /// <param name="e"></param>
    void OnRestoreEvent(FullRestoreEvent e)
    {
      this.Restore();
    }


  }

}