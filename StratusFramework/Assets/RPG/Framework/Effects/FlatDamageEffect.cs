using UnityEngine;
using Stratus;
using System;

namespace Altostratus
{
  /// <summary>
  /// Applies a flat amount of damage to the target
  /// </summary>
  public class FlatDamageEffect : FlatHealthModificationEffectAttribute
  {
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected override void OnApply(CombatController caster, CombatController target)
    {
      var damageEvent = new Combat.DamageEvent();
      damageEvent.value = (int)this.Value;
      target.gameObject.Dispatch<Combat.DamageEvent>(damageEvent);
    }
  }

}