using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  public class HealingEffect : HealthModificationEffectAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Calculate healing to apply
      int damage = (int)((this.Potency / 100) * GetPotency(caster));
      // Send a damage event to the target
      var healEvent = new Combat.HealEvent();
      healEvent.value = damage;
      target.gameObject.Dispatch<Combat.HealEvent>(healEvent);

    }
  }

}