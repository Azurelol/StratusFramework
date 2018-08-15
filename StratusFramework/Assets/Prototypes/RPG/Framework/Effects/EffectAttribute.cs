using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

//public class Cache<T>
//{
//  private T item;
//  private 
//
//  public static implicit operator T(Cache<T> cache)
//  {
//    if (cache.item)
//    return cache.item;
//  }  
//}

namespace Prototype
{
  [Serializable]
  public abstract class EffectAttribute : ScriptableObject
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

    public enum Classification
    {
      Damage,
      Healing,

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
    public abstract void OnInspect();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Inspects this Effect. (Used by the Editor)
    /// </summary>
    public void Inspect()
    {
      this.OnInspect();
      modifier = EditorBridge.Enum<TargetingModifier>("Modifier", modifier); 
    }

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
      switch(this.modifier)
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
    protected float GetPotency(CombatController target)
    {
      return target.GetPotency(this.type);
    }
    
  }

  
  


}