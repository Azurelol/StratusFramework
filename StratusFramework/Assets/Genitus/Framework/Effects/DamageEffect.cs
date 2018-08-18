using UnityEngine;
using Stratus;
using System;

namespace Genitus
{
  public class DamageEffect : HealthModificationEffectAttribute
  {
    protected override void OnApply(CombatController user, CombatController target)
    {
      // Calculate damage to apply
      float damage =  (this.Potency / 100.0f) * user.GetPotency(this.type);      
      // Send a damage event to the target
      var damageEvent = new Combat.DamageEvent();
      damageEvent.value = damage;
      target.gameObject.Dispatch<Combat.DamageEvent>(damageEvent); 
    }


  }

}