/******************************************************************************/
/*!
@file   DamageEffect.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class DamageEffect 
  */
  /**************************************************************************/
  public class DamageEffect : HealthModificationEffectAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Calculate damage to apply
      float damage =  (this.Potency / 100.0f) * caster.attack.current;
      // Send a damage event to the target
      var damageEvent = new CombatController.DamageEvent();
      damageEvent.value = damage;
      target.gameObject.Dispatch<CombatController.DamageEvent>(damageEvent); 
    }


  }

}