using UnityEngine;
using Stratus;
using System;

namespace Genitus.Effects
{
  public class ProportionalHealingEffect : ProportionalHealthModificationAttribute
  {
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected override void OnApply(CombatController caster, CombatController target)
    {
      float heal = (this.percentage / 100.0f) * QueryPotency(target, PotencyQuery.HealingDealt); // target.health.maximum;
      //Trace.Script("Sending heal event with value of " + heal);
      var healEvent = new Combat.HealEvent();
      healEvent.value = heal;
      target.gameObject.Dispatch<Combat.HealEvent>(healEvent);

    }
  }

}