/******************************************************************************/
/*!
@file   EffectAttribute.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class EffectAttribute 
  */
  /**************************************************************************/
  [Serializable]
  public abstract class EffectAttribute : ScriptableObject
  {    
    public enum TargetingModifier { None, Self, Allies, Enemies, All }
    //------------------------------------------------------------------------/
    public TargetingModifier Modifier = TargetingModifier.None;
    //------------------------------------------------------------------------/
    protected abstract void OnApply(CombatController caster, CombatController target);
    public abstract void OnInspect();
    //------------------------------------------------------------------------/
    /// <summary>
    /// Inspects this Effect. (Used by the Editor)
    /// </summary>
    public void Inspect()
    {
      this.OnInspect();
      Modifier = EditorBridge.Enum<TargetingModifier>("Modifier", Modifier); 
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
      switch(this.Modifier)
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
    
  }

  
  


}