using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Genitus.Effects
{
  public class ProportionalDamageEffect : ProportionalHealthModificationAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Calculate the damage to be applied
      float damage = (this.percentage / 100) * QueryPotency(target, PotencyQuery.PhysicalDamageDealt); 
      // Send a damage event to the target
      var damageEvent = new Combat.DamageEvent();
      damageEvent.value = damage;
      target.gameObject.Dispatch<Combat.DamageEvent>(damageEvent);


    }
  }

}