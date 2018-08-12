/******************************************************************************/
/*!
@file   CombatController.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System.Linq;
using System;
using Stratus.Utilities;

namespace Prototype
{
  /// <summary>
  /// The base class which acts as the driver/vehicle for combatants.
  /// </summary>
  public abstract partial class CombatController : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Public Fields
    //------------------------------------------------------------------------/
    [Header("Debug")]
    /// <summary>
    /// Whether to print debug output
    /// </summary>
    [Tooltip("Whether to print debug output")]
    public bool logging = false;

    [Header("Configuration")]
    /// <summary>
    /// The character used by this controller
    /// </summary>
    [Tooltip("The character used by this controller")]
    public Character Character;

    //------------------------------------------------------------------------/
    // Private Fields
    //------------------------------------------------------------------------/
    private Cooldown staminaRecoveryCooldown { get; set; }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The skills available to this character
    /// </summary>
    public Skills skills { get; protected set; } = new Skills();
    /// <summary>
    /// The effects this character is currently under
    /// </summary>
    public Effects effects { get; protected set; } = new Effects();
    /// <summary>
    /// The current state of this character
    /// </summary>
    public State currentState { get; protected set; } = State.Active;    
    /// <summary>
    /// What party this character belogns to
    /// </summary>
    public Faction party => Character.Faction;
    /// <summary>
    /// The statistics of this character
    /// </summary>
    protected Attributes.Statistics statistics => Character.Attributes.statistics;
    /// <summary>
    /// This controller's current target
    /// </summary>
    public CombatController currentTarget { get; protected set; }   
    /// <summary>
    /// Whether this controller can currently receive damage
    /// </summary>
    public bool isReceivingDamage => !(isInvulnerable || this.health.current <= 0f);
    /// <summary>
    /// Whether this character is currently receiving damage events
    /// </summary>
    public bool isInvulnerable { get; protected set; }
    /// <summary>
    /// Whether this character is currently consuming no stamina
    /// </summary>
    public bool hasUnlimitedStamina { get; protected set; }
    /// <summary>
    /// The available skills to this character
    /// </summary>
    public Skill[] availableSkills { get; private set; }
    /// <summary>
    /// This controller's current action
    /// </summary>
    protected CombatAction currentAction { get; set; }
    /// <summary>
    /// The health of this character. When reached 0, the character is considered KO.
    /// </summary>
    public Attribute health { get; private set; }
    /// <summary>
    /// The stamina of this character, consumed when activating skills and abilities
    /// </summary>
    public Attribute stamina { get; private set; }
    /// <summary>
    /// Represents the innate damage bonus the character provides to his attacks
    /// </summary>
    public Attribute attack { get; private set; }
    /// <summary>
    /// Represents the innate defense bonus, reducing incoming damage
    /// </summary>
    public Attribute defense { get; private set; }
    /// <summary>
    /// Represents how quick this character can act
    /// </summary>
    public Attribute speed { get; private set; }
    /// <summary>
    /// Represents the range of this character's default attack
    /// </summary>
    public Attribute range { get; private set; }
    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/        
    /// <summary>
    /// Whether the target is currently performing an action.
    /// </summary>
    public abstract bool IsCasting { get; }
    protected abstract void OnControllerInitialize();
    protected abstract void OnTimeStep(float step);
    protected virtual void OnIncapacitate() {}
    // Actions
    //protected abstract void OnSelectAction();
    protected abstract void OnActionStarted(CombatAction action);
    protected abstract void OnActionApproach(CombatAction action);
    protected abstract void OnActionAnimated(CombatAction action);
    protected abstract void OnActionEnded(CombatAction action);
    protected virtual void OnActionCanceled() {}
    protected virtual void OnActionFailed() {}
    protected virtual void OnActionDelay(float delay) {}
    // Targeting
    //public abstract CombatController[] FindTargetsOfType(TargetingParameters type, State state = State.Active);
    //public abstract CombatController[] FindTargets(State state = State.Active);
    //public abstract CombatController[] FindTargetsOfType(TargetingParameters type, float radius);

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the CombatController.
    /// </summary>
    void Start()
    {
      // If data has already been set, import it on start up
      if (this.Character) Import();
      // Announce that it has spawned
      //this.AnnounceStatus<SpawnEvent>();
      this.gameObject.Dispatch<SpawnEvent>(new SpawnEvent());
      // Subscribe to events
      this.Subscribe();
      // Initialize the subclass
      this.OnControllerInitialize();
      // Announce the current status
      this.AnnounceStatus<ActiveEvent>();
      // Whenever stamina is consumed, this timer is reset
      this.staminaRecoveryCooldown = new Cooldown(this.Character.staminaRecoveryDelay);
    }

    private void Update()
    {
      this.staminaRecoveryCooldown.Update(Time.deltaTime);
    }

    private void OnValidate()
    {
      if (!this.Character)
        return;

      var skills = new List<Skill>();
      skills.AddRange(this.Character.Skills);
      skills.Add(this.Character.Attack);

      availableSkills = (from skill in skills
                         select skill).ToArray();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Finds all targets of a specified type available to this character
    /// </summary>
    /// <param name="availableTargets"></param>
    /// <param name="type"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public CombatController[] FindTargetsOfType(CombatController[] availableTargets, TargetingParameters type, State state = State.Active)
    {      
      CombatController[] targets = null;
      // var targets = new List<CombatController>();
      if (type == TargetingParameters.Self)
      {
        targets = new CombatController[] { this };
      }
      else if (type == CombatController.TargetingParameters.Ally)
      {
        switch (this.Character.Faction)
        {
          case CombatController.Faction.Player:
            targets = FilterTargets(availableTargets, Faction.Player);
            break;

          case CombatController.Faction.Hostile:
            targets = FilterTargets(availableTargets, Faction.Hostile);
            break;
          case CombatController.Faction.Neutral:
            targets = FilterTargets(availableTargets, Faction.Hostile | Faction.Player);
            break;
        }
      }
      // ENEMIES0
      else if (type == CombatController.TargetingParameters.Enemy)
      {
        switch (this.Character.Faction)
        {
          case CombatController.Faction.Player:
            targets = FilterTargets(availableTargets, Faction.Hostile);
            break;
          case CombatController.Faction.Hostile:
            targets = FilterTargets(availableTargets, Faction.Player);
            break;
          case CombatController.Faction.Neutral:
            targets = FilterTargets(availableTargets, Faction.Hostile | Faction.Player);
            break;
        }
      }

      return (from CombatController controller in targets where controller.currentState == state select controller).ToArray();
    }


    /// <summary>
    /// Finds all targets of a specified type available to this character within a specified range
    /// </summary>
    /// <param name="type"></param>
    /// <param name="radius"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public CombatController[] FindTargetsOfType(TargetingParameters type, float radius = 20f, State state = State.Active)
    {      
      var hits = Physics.OverlapSphere(transform.position, radius);
      var availableTargets = (from Collider hit in hits
                              where hit.GetComponent<CombatController>()  
                              select hit.GetComponent<CombatController>()).ToArray();
      return FindTargetsOfType(availableTargets, type, state);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="availableTargets"></param>
    /// <param name="faction"></param>
    /// <returns></returns>
    CombatController[] FilterTargets(CombatController[] availableTargets, Faction faction)
    {
      return (from CombatController target in availableTargets
              where target.party == faction
              select target).ToArray();
    }

    /* 
     NOTE: The Combat Controller is purely-event driven. It's action cycle is defined as follows:
     * I.   SelectAction:  Asks the player/autonomous agent to pick an action. If an action was
     *                    successfully selected, it will be queued. Otherwise we try again.
     * II.  QueueEvent:    Adds an action to the character's queue, usually setting it as the current action.
     *                    The controller will now start updating that action.
     * III. StartedEvent: Once the action has determined the agent is within the specified range of its target,
     *                    it will start casting.
     * IV.  TriggerEvent:  Once the action has finished casting, it will be triggered. 
     *                    After waiting a specified animation duration time, it will execute.
     * V.   ExecuteEvent:   At this point the action will send its according event to its specified target.
     * VI.  EndedEvent:    The action has finished executing. At this point, we start again from the top.
    */

  }

}