using UnityEngine;
using Stratus;
using System;

namespace Genitus.Effects
{
  public class DamageEffect : HealthModificationEffectAttribute
  {
    protected override void OnApply(CombatController user, CombatController target)
    {
      // Calculate damage to apply
      float damage = (this.potency / 100.0f) * this.QueryPotency(target, PotencyQuery.PhysicalDamageDealt);
      // Send a damage event to the target
      var damageEvent = new Combat.DamageEvent();
      damageEvent.value = damage;
      target.gameObject.Dispatch<Combat.DamageEvent>(damageEvent); 
    }


  }

}