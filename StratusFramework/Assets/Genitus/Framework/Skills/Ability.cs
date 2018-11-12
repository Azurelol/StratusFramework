using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;

namespace Genitus
{
  public abstract class Ability
  {
    /// <summary>
    /// Base class for skill events
    /// </summary>
    public class BaseEvent : Stratus.StratusEvent
    {
      public Ability ability { get; set; }
    }

    /// <summary>
    /// A query to determine whether this skill can be used
    /// </summary>
    public class IsAvailabilityEvent : BaseEvent
    {
      public bool available { get; set; } = false;
    }

    /// <summary>
    /// Signals that the skill has been used
    /// </summary>
    public class UsedEvent : Stratus.StratusEvent
    {
      public Ability ability;
      public AudioClip clip;
    }

  }

  public abstract class Ability<T> : Ability
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Signals that this ability is ready to be used
    /// </summary>
    public class ActivateEvent : Stratus.StratusEvent
    {
      public float build;
    }

    /// <summary>
    /// Signals that the ability has hit a target
    /// </summary>
    public class HitEvent : Stratus.StratusEvent
    {
    }

    //------------------------------------------------------------------------/
    // Properties    
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this ability is enabled
    /// </summary>
    [Tooltip("Whether this ability is enabled")]
    public bool enabled = true;
    /// <summary>
    /// Whether this ability is currently being debugged
    /// </summary>
    public bool logging = false;

    [Header("Activation Settings")]
    /// <summary>
    /// The cost, in stamina, of the ability..
    /// </summary>
    [Tooltip("How much stamina the ability costs")]
    [Range(0.0f, 100.0f)]
    public float cost = 20.0f;
    /// <summary>
    /// Time required before the skill can be used again after activation
    /// </summary>
    [Tooltip("Time required before the skill can be used again after activation, in seconds")]
    [Range(0.0f, 5.0f)]
    public float cooldown = 1.0f;
    /// <summary>
    /// The sound effect played when this ability is activated
    /// </summary>
    [Tooltip("The audiovisual effects played when this ability is activated")]
    public AudioClip clip;

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// When this timer is active, the ability cannot be used
    /// </summary>
    private Cooldown cooldownTimer;
    /// <summary>
    /// Whether this ability is currently active
    /// </summary>
    public bool isActive { get; protected set; } = false;
    /// <summary>
    /// Checks whether this ability can be currently activate.
    /// It will check for whether there's enough stamina,
    /// and whether its off cooldown.
    /// </summary>
    /// <returns></returns>
    bool isReadyToBeUsed
    {
      get
      {
        throw new System.NotImplementedException();
        //if (!enabled)
        //{
        //  //Trace.Script("Ability disabled!");
        //  return false;
        //}

        //// If the caster does not have enough resources to cast it
        ////  owner.stamina.current < cost
        ////if (owner.IsAvailable(this))
        ////{
        ////  //Trace.Script("Not enough stamina to cast! ");
        ////  return false;
        ////}

        //// If it's on cooldown
        //if (cooldownTimer.isActive)
        //{
        //  //Trace.Script("Ability on cooldown!");
        //  return false;
        //}

        //if (!OnValidate())
        //{
        //  return false;
        //}


        //return true;
      }
    }

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    /// <summary>
    /// The display name for this ability
    /// </summary>
    public abstract string Name { get; }
    protected CombatController owner;
    protected abstract bool OnActivate(ActivateEvent e);
    protected abstract void OnCancel();
    protected virtual void OnInitialize() { }
    protected virtual bool OnValidate() { return true; }
    protected virtual void OnUpdate() { }

    //------------------------------------------------------------------------/
    // Routines
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes this ability.
    /// </summary>
    /// <param name="owner"></param>
    public void Initialize(CombatController owner)
    {
      this.owner = owner;
      cooldownTimer = new Cooldown(this.cooldown);
      this.OnInitialize();
    }

    /// <summary>
    /// Updates this ability.
    /// </summary>
    public void Update()
    {
      if (!this.enabled)
        return;

      cooldownTimer.Update(Time.deltaTime);
      this.OnUpdate();
    }

    /// <summary>
    /// Activates the ability, consuming stamina in the process.
    /// <returns>True if the activity was successfully activated, false otherwise.</returns>
    /// </summary>
    public bool Activate(ActivateEvent e)
    {
      // Check if there's enough stamina to use the ability
      if (!isReadyToBeUsed)
        return false;

      // If the ability was activated, consume stamina
      if (this.OnActivate(e))
      {
        // Put it on cooldown
        cooldownTimer.Activate();
        // Note that the ability has been activated
        owner.gameObject.Dispatch<UsedEvent>(new UsedEvent() { ability = this });
        // Play effects
        //AudioVisualEffect.Spawn(this.SFX, this.Owner.transform.position);
        return true;
      }

      // Ability could not be activated
      return false;
    }

    /// <summary>
    /// Abruptly stops this ability
    /// </summary>
    public void Cancel()
    {
      //Trace.Script("Cancelling " + Name);
      isActive = false;
      this.OnCancel();
    }

    //------------------------------------------------------------------------/
    // Subroutines
    //------------------------------------------------------------------------/

  }

}