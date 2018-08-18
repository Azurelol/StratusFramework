using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Genitus
{
  public class ProportionalDamageEffect : ProportionalHealthModificationAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Calculate the damage to be applied
      float damage = (this.Percentage / 100) * GetPotency(target); // health.maximum;
      // Send a damage event to the target
      var damageEvent = new Combat.DamageEvent();
      damageEvent.value = damage;
      target.gameObject.Dispatch<Combat.DamageEvent>(damageEvent);


    }
  }

}