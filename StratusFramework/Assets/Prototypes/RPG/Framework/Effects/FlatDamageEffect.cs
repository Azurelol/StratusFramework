/******************************************************************************/
/*!
@file   FlatDamageEffect.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /// <summary>
  /// Applies a flat amount of damage to the target
  /// </summary>
  public class FlatDamageEffect : FlatHealthModificationEffectAttribute
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected override void OnApply(CombatController caster, CombatController target)
    {
      var damageEvent = new CombatController.DamageEvent();
      damageEvent.value = (int)this.Value;
      target.gameObject.Dispatch<CombatController.DamageEvent>(damageEvent);
    }
  }

}