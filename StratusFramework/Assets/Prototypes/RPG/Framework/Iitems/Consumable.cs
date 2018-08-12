/******************************************************************************/
/*!
@file   Consumable.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prototype
{
  /// <summary>
  /// Consumables are items used in combat.
  /// </summary>
  /// 
  [CreateAssetMenu(fileName = "Consumable", menuName = "Prototype/Consumable")]
  public class Consumable : Item
  {
    public override Category Type { get { return Category.Consumable; } }
    public enum PersistenceType { Expendable, Persistent }
    //------------------------------------------------------------------------/
    /// <summary>
    /// The targeting parameters of the skill (Self, Ally, Enemy)
    /// </summary>
    public CombatController.TargetingParameters Target = CombatController.TargetingParameters.Enemy;
    /// <summary>
    /// The scope of the item (single, aoe, all)
    /// </summary>
    public TargetingScope Scope = new TargetingScope();
    /// <summary>
    /// Does the item persist after its charges have run out? Can it be refilled?
    /// </summary>
    public bool Persistent = false;
    /// <summary>
    /// How many times can this item be used in combat, if persistent.
    /// </summary>
    public int Charges = 1;
    /// <summary>
    /// The effects this item carries.
    /// </summary>
    [HideInInspector] public List<EffectAttribute> Effects = new List<EffectAttribute>();
    //------------------------------------------------------------------------/
    /// <summary>
    /// Uses the consumable on the target.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public void Use(CombatController user, CombatController target)
    {
      var targets = Scope.FindTargets(user, target, Target);      

      // For each target, apply every effect
      foreach (var eligibleTargets in targets)
      {
        foreach (var effect in Effects)
        {
          effect.Apply(user, eligibleTargets);
        }
      }     
      


    }

    public override string Describe()
    {
      var builder = new StringBuilder();
      builder.AppendLine("Name: " + Name);
      builder.AppendLine("Description: " + Description);
      if (Persistent)
        builder.AppendLine("Charges: " + Convert.ToString(Charges));
      return builder.ToString();
    }

  }



}