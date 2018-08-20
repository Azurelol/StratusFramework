using UnityEngine;
using Stratus;
using System;

namespace Genitus.Effects
{
  /// <summary>
  /// Pushes back the target's waiting/casting progress.
  /// </summary>
  public class DelayEffect : ChanceEffectAttribute
  {
    [Range(0.0f, 1.0f)] public float delay = 0.25f;

    protected override void OnChance(CombatController caster, CombatController target)
    {
      var e = new CombatAction.DelayEvent();
      e.Delay = this.delay;
      target.gameObject.Dispatch<CombatAction.DelayEvent>(e);
    }
  }

}