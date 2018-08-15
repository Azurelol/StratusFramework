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
  public abstract partial class CombatController : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// The faction the controller belongs
    /// </summary>
    [Flags]
    public enum Faction
    {
      /// <summary>
      /// The player
      /// </summary>
      Player,
      /// <summary>
      /// Agents hostile to everyone else
      /// </summary>
      Neutral,
      /// <summary>
      /// Agents hostile to the player
      /// </summary>
      Hostile
    }



    /// <summary>
    /// The current state of the controller
    /// </summary>
    public enum State
    {
      /// <summary>
      /// The controller is taking action
      /// </summary>
      Active,
      /// <summary>
      /// The controller is currently unable to take action
      /// </summary>
      Stunned,
      /// <summary>
      /// This character cannot be targeted at the current time
      /// </summary>
      Untargetable,
      /// <summary>
      /// The controller is unable to take action
      /// </summary>
      Inactive
    }

    public delegate void Callback();

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
    ///// <summary>
    ///// The character used by this controller
    ///// </summary>
    //[Tooltip("The character used by this controller")]
    //public Character character;

    /// <summary>
    /// What party this character belogns to
    /// </summary>
    public Faction faction;

    //------------------------------------------------------------------------/
    // Private Fields
    //------------------------------------------------------------------------/


    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The effects this character is currently under
    /// </summary>
    public Effects effects { get; protected set; } = new Effects();
    /// <summary>
    /// The current state of this character
    /// </summary>
    public State currentState { get; protected set; } = State.Active;    
    /// <summary>
    /// This controller's current target
    /// </summary>
    public CombatController currentTarget { get; protected set; }   
    /// <summary>
    /// Whether this character is currently receiving damage events
    /// </summary>
    public bool invulnerable { get; protected set; }
    ///// <summary>
    ///// The available skills to this character
    ///// </summary>
    //public Skill[] availableSkills { get; private set; }
    /// <summary>
    /// This controller's current action
    /// </summary>
    protected CombatAction currentAction { get; set; }
    /// <summary>
    /// The current modules for the combat controller
    /// </summary>
    public CombatControllerModule[] modules { get; set; }
    /// <summary>
    /// Whether the target is currently performing an action.
    /// </summary>
    public abstract bool isActing { get; }
    /// <summary>
    /// Callback for when the character is fully restored
    /// </summary>
    public Callback onRestore { get; set; }

    ///// <summary>
    ///// Callback for when the character has used a skill
    ///// </summary>
    //public System.Action<Skill> onSkillUsed { get; set; }

    ///// <summary>
    ///// Callback to check whether a skill can be used
    ///// </summary>
    //public System.Func<Skill, bool> canSkillBeUsed { get; set; }    

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/        
    protected abstract void OnControllerInitialize();
    protected abstract void OnTimeStep(float step);
    protected virtual void OnIncapacitate() {}
    protected abstract bool OnDamage(float value);
    protected abstract void OnHeal(float value);
    protected abstract void OnRestore();
    protected abstract void OnInvulnerable(bool toggle);
    public abstract float GetPotency(Enum enumeratedType);
    public abstract float GetPotency(Type type);
    public abstract bool IsAvailable(Ability ability);


    //public abstract bool IsAvailable(Skill skill);
    //protected abstract void OnUsed(Skill skill);

    // Actions
    protected abstract void OnActionCanceled();
    protected abstract void OnActionDelay(float delay);

    

    

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
      //if (this.character) Import();
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
      
    }


    //private void OnValidate()
    //{
    //  if (!this.character)
    //    return;
    //
    //  var skills = new List<Skill>();
    //  skills.AddRange(this.character.Skills);
    //  skills.Add(this.character.Attack);
    //
    //  availableSkills = (from skill in skills
    //                     select skill).ToArray();
    //}

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Advances time for the combat controller
    /// </summary>
    /// <param name="step"></param>
    public void TimeStep(float step)
    {
      // Update all effects
      effects.Update(step);
      // Update all modules
      foreach (var module in modules)
        module.OnTimeStep(step);
      // If inactive/incapacitated, do nothing else
      if (this.currentState == State.Stunned ||
          this.currentState == State.Inactive)
        return;

      // Called on the subclass
      this.OnTimeStep(step);
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
    public CombatController[] FindTargetsOfType(CombatController[] availableTargets, Combat.TargetingParameters type, State state = State.Active)
    {      
      CombatController[] targets = null;
      // var targets = new List<CombatController>();
      if (type == Combat.TargetingParameters.Self)
      {
        targets = new CombatController[] { this };
      }
      else if (type == Combat.TargetingParameters.Ally)
      {
        switch (this.faction)
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
      else if (type == Combat.TargetingParameters.Enemy)
      {
        switch (this.faction)
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
    public CombatController[] FindTargetsOfType(Combat.TargetingParameters type, float radius = 20f, State state = State.Active)
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
              where target.faction == faction
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