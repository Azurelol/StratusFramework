using UnityEngine;
using Stratus;
using System;

namespace Genitus.Effects
{
  public class HealingEffect : HealthModificationEffectAttribute
  {
    protected override void OnApply(CombatController source, CombatController target)
    {
      // Calculate healing to apply
      int damage = (int)((this.potency / 100) * QueryPotency(source, PotencyQuery.HealingDealt));
      // Send a damage event to the target
      var healEvent = new Combat.HealEvent();
      healEvent.value = damage;
      target.gameObject.Dispatch<Combat.HealEvent>(healEvent);

    }
  }

}