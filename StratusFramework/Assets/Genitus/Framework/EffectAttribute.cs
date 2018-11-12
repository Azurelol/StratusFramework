using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Genitus
{
  [Serializable]
  public abstract class EffectAttribute
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// If this effect has any modifiers, such that it affects additional targets, etc
    /// </summary>
    public enum TargetingModifier
    {
      None,
      Self,
      Allies,
      Enemies,
      All
    }

    public enum PotencyQuery
    {
      PhysicalDamageDealt,
      PhysicalDamageTaken,

      MagicalDamageDealt,
      MagicalDamageTaken,

      HealingDealt,
      HealingReceived,

      MaximumHealth,
      Accuracy,
      Evasion
    }

    /// <summary>
    /// Queries for a given potency for an effect based on a customized query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PotencyQueryEvent<T> : Stratus.StratusEvent
    {
      public T query { get; private set; }
      public float potency { get; set; }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public TargetingModifier modifier = TargetingModifier.None;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public System.Type type
    {
      get
      {
        if (_type == null)
          _type = this.GetType();
        return _type;
      }
    }
    private System.Type _type;

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void OnApply(CombatController user, CombatController target);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Applies this effect from the caster to the target.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    public void Apply(CombatController caster, CombatController target)
    {
      // If the effect is persistent, add it to the list of effects
      // on the target

      // Otherwise, apply it right away
      switch (this.modifier)
      {
        case TargetingModifier.None:
          OnApply(caster, target);
          break;
        case TargetingModifier.Self:
          OnApply(caster, caster);
          break;
        case TargetingModifier.Allies:
          throw new NotImplementedException();
        //foreach (var targetAlly in caster.FindTargetsOfType(CombatController.TargetingParameters.Ally))
        //  OnApply(caster, targetAlly);
        //break;
        case TargetingModifier.Enemies:
          throw new NotImplementedException();
          //foreach(var targetEnemy in caster.FindTargetsOfType(CombatController.TargetingParameters.Enemy))
          //  OnApply(caster, targetEnemy);          
          //break;

      }
    }

    /// <summary>
    /// Calculates the potency based from this type
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected float QueryPotency(CombatController target, PotencyQuery query)
    {
      return target.QueryPotency(query);
    }

    /// <summary>
    /// Calculates the potency based from this type
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected float QueryPotency<Query, PotencyQueryEvent>(CombatController target, Query query) 
      where PotencyQueryEvent : PotencyQueryEvent<Query>, new()
    {
      PotencyQueryEvent potencyQueryEvent = new PotencyQueryEvent();
      target.gameObject.Dispatch<PotencyQueryEvent>(potencyQueryEvent);
      return potencyQueryEvent.potency;
    }

  }





}