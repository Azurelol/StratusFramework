/******************************************************************************/
/*!
@file   ProportionalDamageEffect.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class ProportionalDamageEffect 
  */
  /**************************************************************************/
  public class ProportionalDamageEffect : ProportionalHealthModificationAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Calculate the damage to be applied
      float damage = (this.Percentage / 100) * target.health.maximum;
      // Send a damage event to the target
      var damageEvent = new CombatController.DamageEvent();
      damageEvent.value = damage;
      target.gameObject.Dispatch<CombatController.DamageEvent>(damageEvent);


    }
  }

}