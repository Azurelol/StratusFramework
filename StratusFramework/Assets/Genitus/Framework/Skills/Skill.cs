using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;
using System.Text;

namespace Genitus
{
  /// <summary>
  /// Base class for all skills used within the framework.
  /// </summary>
  public abstract class Skill : StratusScriptable
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Base class for skill events
    /// </summary>
    public abstract class BaseSkillEvent : Stratus.Event { public Skill skill { get; set; } }
    /// <summary>
    /// A query to determine whether this skill can be used
    /// </summary>
    public class ValidationEvent : BaseSkillEvent { public bool valid { get; set; } = false; }
    /// <summary>
    /// Signals that the skill has been used
    /// </summary>
    public class ActivationEvent : BaseSkillEvent {}

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The targeting parameters of the skill (Self, Ally, Enemy)
    /// </summary>
    public Combat.TargetingParameters targetType = Combat.TargetingParameters.Enemy;
    /// <summary>
    /// Effects this skill has
    /// </summary>
    [HideInInspector]
    public List<EffectAttribute> effects = new List<EffectAttribute>();

    //public List<SkillComponent> components = new List<SkillComponent>();

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    /// <summary>
    /// Evaluates a list of possible targets in order to find out which ones are
    /// targetable by this skill.
    /// </summary>
    public abstract CombatController[] EvaluateTargets(CombatController user, CombatController[] availableTargets);
    /// <summary>
    /// Casts the skill onto the given targets
    /// </summary>
    public abstract void Cast(CombatController user, CombatController[] targets);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Applies the skill on all valid targets given.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targets"></param>
    /// <param name="skill"></param>
    protected void Apply(CombatController user, CombatController[] targets, Skill skill)
    {
      if (targets == null)
      {
        throw new Exception("No valid targets for this skill!");
        //Trace.Script("No available targets were found!", user);
      }

      // For each target, apply every effect
      foreach (var eligibleTarget in targets)
      {
        //Trace.Script("Casting '" + skill.Name + "' on <i>" + eligibleTarget + "</i>", user);
        foreach (var effect in skill.effects)
        {
          effect.Apply(user, eligibleTarget);
        }
      }
    }
  }

  public abstract class Skill<Resource, Targeting, Description> : Skill 
    where Resource : Character.ResourceModel
    where Targeting : TargetingModel
    where Description : DescriptionModel
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public class Instance
    {
      public Skill reference;
      public string name => reference.name;

      public Instance(Skill<Resource, Targeting, Description> reference)
      {
        this.reference = reference;
      }

      /// <summary>
      /// Uses the instantiated skill on the target.
      /// </summary>
      /// <param name="user">The user of this skill.</param>
      /// <param name="target">The target of this skill.</param>
      public void Use(CombatController user, CombatController[] targets)
      {
        Skill.ActivationEvent skillUsed = Stratus.Event.Cache<Skill.ActivationEvent>();
        reference.Cast(user, targets);
        skillUsed.skill = reference;
        user.gameObject.Dispatch<Skill.ActivationEvent>(skillUsed);
      }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// How the skill is described
    /// </summary>
    [Tooltip("How the character is described")]
    public Description description;

    /// <summary>
    /// The scope of the skill (single, aoe, all)
    /// </summary>
    public Targeting targeting;

    /// <summary>
    /// The cost, in resources, of the skill
    /// </summary>
    [Tooltip("The resource cost of the skill")]
    public Resource cost;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public override CombatController[] EvaluateTargets(CombatController user, CombatController[] availableTargets)
    {
      return this.targeting.EvaluateTargets(user, availableTargets, this.targetType);
    }

    public override void Cast(CombatController user, CombatController[] targets)
    {
      this.Apply(user, targets, this);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
  }

}