/******************************************************************************/
/*!
@file   Skill.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;
using System.Text;

namespace Prototype
{
  [CreateAssetMenu(fileName = "Skill", menuName = "Prototype/Skill")]
  [Serializable]
  public partial class Skill : ScriptableObject
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Base class for skill events
    /// </summary>
    public class BaseSkillEvent : Stratus.Event
    {
      public Skill skill { get; set; }
    }

    /// <summary>
    /// A query to determine whether this skill can be used
    /// </summary>
    public class IsAvailableEvent : BaseSkillEvent
    {
      public bool available { get; set; } = false;
    }

    /// <summary>
    /// Signals that the skill has been used
    /// </summary>
    public class UsedEvent : BaseSkillEvent
    {
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Description")]
    /// <summary>
    /// A short description of the skill
    /// </summary>
    public string description;
    /// <summary>
    /// A small graphical icon representing the skill.
    /// </summary>
    public Sprite icon;

    [Header("Targeting Parameters")]
    /// <summary>
    /// The targeting parameters of the skill (Self, Ally, Enemy)
    /// </summary>
    public Combat.TargetingParameters targeting = Combat.TargetingParameters.Enemy;
    /// <summary>
    /// The scope of the skill (single, aoe, all)
    /// </summary>
    public TargetingScope scope = new TargetingScope();

    [Header("Costs")]
    /// <summary>
    /// The cost, in raw value, of the skill.
    /// </summary>
    [Tooltip("The cost of the skill")]
    [Range(0, 100)] public int cost = 5;
    /// <summary>
    /// Time required before the skill can be used again after activation
    /// </summary>
    [Tooltip("Time required before the skill can be used again after activation")]
    [Range(0.0f, 10.0f)] public float cooldown = 0.0f;
    /// <summary>
    /// "Range required for casting the skill
    /// </summary>
    [Tooltip("Range required for casting the skill")]
    [Range(1.0f, 50.0f)] public float range = 3.0f;

    [Header("Phases")]
    [Tooltip("Specific timings for the skill's action")]
    public CombatAction.Timings Timings = new CombatAction.Timings();

    [Header("Telegraph")]
    /// <summary>
    /// Whether the skill is telegraphed
    /// </summary>
    [Tooltip("Whether the skill is telegraphed")]
    public bool telegraphed = true;
    /// <summary>
    /// How the skill is telegraphed.
    /// </summary>
    [Tooltip("How the skill is telegraphed")]
    //[DrawIf("IsTelegraphed", true, ComparisonType.Equals, PropertyDrawingType.DontDraw)]
    public Telegraph.Configuration telegraph;
     

    [Header("Trigger Settings")]
    /// <summary>
    /// Trigger used by the caster when using this skill
    /// </summary>
    [Tooltip("Trigger used by the caster when using this skill")]
    public CombatTrigger onCast;
    /// <summary>
    /// Trigger used by the target when receiving this skill
    /// </summary>
    [Tooltip("Trigger used by the caster when defending from this skill")]
    public CombatTrigger onDefend;

    [Header("Special Effects")]
    /// <summary>
    /// What animation to use for this skill
    /// </summary>
    [Tooltip("What particle effects to play when the skill is executed")]
    [SerializeField]
    public ParticleSystem particles;
    /// <summary>
    /// Whether this skill has triggers set
    /// </summary>
    /// 
    public bool isTriggered { get { return (onCast.Enabled || onDefend.Enabled); } }
    /// <summary>
    /// Effects this skill has
    /// </summary>
    [HideInInspector]
    public List<EffectAttribute> effects = new List<EffectAttribute>();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Casts the skill on the target.
    /// </summary>
    /// <param name="user">The caster of the skill.</param>
    /// <param name="target">The target of this skill.</param>
    public void Cast(CombatController user, CombatController target, Telegraph telegraph)
    {
      // If the skill is cast directly..      
      
      if (user.logging)
        Trace.Script("Casting '" + name + "'", user);

      CombatController[] targets = null;

      if (telegraphed)
      {
        var availableTargets = telegraph.FindTargetsWithinBoundary();
        targets = TargetingScope.FilterTargets(user, availableTargets, targeting);
      }
      else {
        targets = this.scope.FindTargets(user, target, targeting);
      }

      Apply(user, targets, this);

    }


    /// <summary>
    /// Applies the skill on all valid targets given.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targets"></param>
    /// <param name="skill"></param>
    void Apply(CombatController user, CombatController[] targets, Skill skill)
    {
      if (targets == null)
      {
        Trace.Script("No available targets were found!", user);
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


    /// <summary>
    /// Checks whether the skill has a trigger set up for it.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="targets"></param>
    /// <returns></returns>
    public bool Trigger(CombatController caster, CombatController target)
    {
      //var targets = FindTargets(caster, target);

      //// If this skill is cast by a player character and there's a trigger set..
      //if (caster.controlMode == Character.ControlMode.Manual)
      //{
      //  if (this.OnCast.Enabled)
      //  {
      //    this.OnCast.Start(caster, CombatTrigger.Type.Attack, Timings.Trigger);
      //    return true;
      //  }
      //}
      //// Else if cast by an an enemy AI
      //else if (caster.controlMode == Character.ControlMode.Automatic)
      //{
      //  if (this.OnDefend.Enabled)
      //  {
      //    this.OnDefend.Start(caster, CombatTrigger.Type.Defend, Timings.Trigger);
      //    return true;
      //  }
      //}

      // No trigger was set
      return false;
    }


    /// <summary>
    /// Provides a string describing this skill.
    /// </summary>
    /// <returns>The description of the skill, among multiple lines.</returns>
    public string Describe()
    {
      var builder = new StringBuilder();
      builder.AppendLine("Name: " +  name);
      builder.AppendLine("Description: " + description);
      builder.AppendLine("Cost: " + cost);
      builder.AppendLine("Cooldown: " + cooldown);
      return builder.ToString();
    }   
    


  }

}