using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genitus
{
  [CreateAssetMenu(fileName = "Consumable", menuName = "Prototype/Consumable")]
  public class Consumable : Item
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public enum PersistenceType { Expendable, Persistent }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The skill used by this consumable
    /// </summary>
    public Skill skill;

    ///// <summary>
    ///// The targeting parameters of the skill (Self, Ally, Enemy)
    ///// </summary>
    //public Combat.TargetingParameters target = Combat.TargetingParameters.Enemy;
    ///// <summary>
    ///// The scope of the item (single, aoe, all)
    ///// </summary>
    //public TargetingModel scope;
    /// <summary>
    /// Does the item persist after its charges have run out? Can it be refilled?
    /// </summary>
    public bool persistent = false;
    /// <summary>
    /// How many times can this item be used in combat, if persistent.
    /// </summary>
    public int charges = 1;
    ///// <summary>
    ///// The effects this item carries.
    ///// </summary>
    //[HideInInspector] public List<EffectAttribute> effects = new List<EffectAttribute>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public override Category type => Category.Consumable; 

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Uses the consumable on the target.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public void Use(CombatController user, CombatController[] targets)
    {
      skill.Cast(user, targets);
      //var targets = this.scope.EvaluateTargets(user, target, this.target);      
      //
      //// For each target, apply every effect
      //foreach (var eligibleTargets in targets)
      //{
      //  foreach (var effect in effects)
      //  {
      //    effect.Apply(user, eligibleTargets);
      //  }
      //}     
    }

    public override string Describe()
    {
      var builder = new StringBuilder();
      builder.AppendLine("Name: " + Name);
      builder.AppendLine("Description: " + Description);
      if (persistent)
        builder.AppendLine("Charges: " + Convert.ToString(charges));
      return builder.ToString();
    }

  }



}