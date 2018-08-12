/******************************************************************************/
/*!
@file   CombatControllerRoutines.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  public abstract partial class CombatController : MonoBehaviour
  {
    /// <summary>
    /// Advances time for the combat controller
    /// </summary>
    /// <param name="step"></param>
    public void TimeStep(float step)
    {
      // Update all skills
      skills.Update(step);
      // Update all effects
      effects.Update(step);
      // Regain stamina, if not at maximum      
      this.GainStamina(step);     

      // If inactive/incapacitated, do nothing else
      if (this.currentState == State.Stunned ||
          this.currentState == State.Inactive)
        return;      

      // Called on the subclass
      this.OnTimeStep(step);
    }

    /// <summary>
    /// Queues the given action.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Queue(CombatAction action)
    {
      // Request the controller to attack the given target
      var queuedAction = new CombatAction.QueueEvent();
      queuedAction.Action = action;
      this.gameObject.Dispatch<CombatAction.QueueEvent>(queuedAction);
    }

    /// <summary>
    /// Pauses the controller, cancelling the current action
    /// if there's any, and preventing it from taking action.
    /// </summary>
    void Pause()
    {
      this.Cancel();
      this.currentState = State.Stunned;
    }
        
    /// <summary>
    /// Cancels the controller's current action.
    /// </summary>
    void Cancel()
    {
      if (this.currentAction == null)
        return;

      //Trace.Script("Cancelling the current action!", this);
      this.currentAction.Cancel();
      this.OnActionCanceled();
      this.currentAction = null;
    }

    /// <summary>
    /// Delays the controller's current action
    /// </summary>
    /// <param name="delay"></param>
    void Delay(float delay)
    {
      this.OnActionDelay(delay);
    }
    
    /// <summary>
    /// Interrupt's the character's current action
    /// </summary>
    /// <param name="duration"></param>
    void Interrupt(float duration)
    {
      this.Cancel();
    }

    /// <summary>
    /// Resumes the controller's activity
    /// </summary>
    void Resume()
    {
      this.currentState = State.Active;
      AnnounceStatus<ActiveEvent>();
    }

    /// <summary>
    /// Called upon when this combat controller has been incapacitated.
    /// </summary>
    void Incapacitate()
    {
      //Trace.Script("Incapacitated!", this);
      this.currentState = State.Inactive;
      this.Cancel();
      AnnounceStatus<DeathEvent>();
      this.OnIncapacitate();
    }

    /// <summary>
    /// Restores this character's health and removes all negative effects
    /// </summary>
    public void Restore()
    {
      float heal = this.health.maximum;
      //Trace.Script("Sending heal event with value of " + heal);
      var healEvent = new CombatController.HealEvent();
      healEvent.value = heal;
      this.gameObject.Dispatch<CombatController.HealEvent>(healEvent);

      //var staminaEvent = new CombatController.GainStaminaEvent();
      this.stamina.Reset();

    }

    /// <summary>
    /// Revives this character, restoring its health and setting it back to active
    /// </summary>
    public void Revive()
    {
      this.Restore();
      this.currentState = State.Active;
      AnnounceStatus<ReviveEvent>();
    }

    /// <summary>
    /// Perform any cleanup operations on this controller.
    /// </summary>
    public void Reset()
    {
      //Trace.Script("Resetting!", this);
      this.currentAction = null;
      //AnnounceStatus<InactiveEvent>();
    }

    /// <summary>
    /// Imports the attributes, skills, etc for this controller from a 
    /// registered character.
    /// </summary>
    /// <param name="characterData"></param>
    public void Import(Character characterData, bool loadModel = true)
    {
      this.Character = characterData;
      
      // Import model
      if (loadModel)
      {
        //this.gameObject.GetComponent<CharacterModel>().Load(Character.Model, Character.Skin);
      }

      // Import combat stats
      this.health = new Attribute(Character.Attributes.statistics.health);
      this.stamina = new Attribute(Character.Attributes.statistics.stamina);
      this.attack = new Attribute(Character.Attributes.statistics.attack);
      this.defense = new Attribute(Character.Attributes.statistics.defense);
      this.speed = new Attribute(Character.Attributes.statistics.speed);
      this.range = new Attribute(Character.Attributes.statistics.attack);
      
      // Import skills
      if (this.Character.Attack == null)
      {
        Trace.Error("Default attack skill not set!", this);
        throw new Exception();
      }
      skills.SetAttack(this.Character.Attack);
      foreach (var skill in this.Character.Skills)
      {
        skills.Add(skill);
      }
    }

    /// <summary>
    /// Imports the attributes, skills, etc for this CombatController from a 
    /// registered character.
    /// </summary>
    public void Import()
    {
      Import(this.Character, false);
    }

    /// <summary>
    /// Gains stamina
    /// </summary>
    /// <param name="value"></param>
    private void GainStamina(float value)
    {
      // Regain stamina, if not at maximum
      if (stamina.isAtMaximum)
        return;

      // Do not regain stamina while the stamina recovery is on cooldown
      if (this.staminaRecoveryCooldown.isActive)
        return;

      //Trace.Script("Gaining " + value + " stamina", this);
      this.stamina.Add(value);
      this.gameObject.Dispatch<StaminaModifiedEvent>(new StaminaModifiedEvent());
    }

    /// <summary>
    /// Consumes stamina. Whenever it does, reset the stamina recovery timer.
    /// </summary>
    /// <param name="value"></param>
    private void ConsumeStamina(float value)
    {
      if (hasUnlimitedStamina)
        return;
      
      this.stamina.Reduce(value);
      this.staminaRecoveryCooldown.Activate();
      this.gameObject.Dispatch<StaminaModifiedEvent>(new StaminaModifiedEvent());
    }

    /// <summary>
    /// Announces a change of state
    /// </summary>
    /// <typeparam name="Event"></typeparam>
    void AnnounceStatus<Event>() where Event : CombatControllerEvent, new()
    {
      var e = new Event();
      e.Controller = this;
      this.gameObject.Dispatch<StateChangedEvent>(new StateChangedEvent(this.currentState));
      this.gameObject.Dispatch<Event>(e);
      Scene.Dispatch<Event>(e);
    }


    /// <summary>
    /// Constructs the CombatController.
    /// </summary>
    /// <param name="character">A reference to the character to control.</param>
    /// <param name="mode">Its starting control mode.</param>
    /// <returns></returns>
    public static CombatController Construct(Character character)
    {
      var CombatControllerObj = Instantiate(Resources.Load("Combat/CombatController")) as GameObject;
      CombatControllerObj.name = character.name;
      var controlModeEvent = new ChangeControlModeEvent();
      controlModeEvent.Mode = character.Mode;
      CombatControllerObj.Dispatch<ChangeControlModeEvent>(controlModeEvent);
      CombatControllerObj.GetComponent<CombatController>().Import(character);
      return CombatControllerObj.GetComponent<CombatController>();
    }    

    /// <summary>
    /// Handles damage logic
    /// </summary>
    /// <param name="value"></param>
    void OnDamage(float value)
    {
      // Calculate damage after defense
      var damage = value - this.defense.maximum;

      // If there is damage to be taken
      if (damage > 0)
      {        
        float healthPercentageLost = this.health.Reduce(damage);
        var damageReceived = new DamageReceivedEvent();
        damageReceived.damage = damage;
        damageReceived.healthPercentageLost = healthPercentageLost;
        this.gameObject.Dispatch<DamageReceivedEvent>(damageReceived);

        // If the damage is exceeded 
        if (this.health.current <= 0f)
        {
          this.Incapacitate();
        }
      }
      // If no damage was taken, play a BLOCKED animation?
      else
      {
        //Trace.Script("Damage blocked!", this);        
        this.gameObject.Dispatch<DamageBlockedEvent>(new DamageBlockedEvent());
      }


      if (this.logging)
        Trace.Script("Received " + damage + " damage! Now at " + this.health.current + " health!", this);

      // Announce that health has been modified
      this.gameObject.Dispatch<HealthModifiedEvent>(new HealthModifiedEvent(this));
    }

  }
}