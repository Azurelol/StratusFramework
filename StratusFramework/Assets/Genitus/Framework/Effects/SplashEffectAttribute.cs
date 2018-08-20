using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stratus;

namespace Genitus
{
  public abstract class SplashEffectAttribute : EffectAttribute
  {
    public float radius = 3.0f;
    protected abstract void OnSplash(CombatController caster, CombatController target);

    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Find eligible targets within range of the target
      var targetsWithinRange = target.FindTargetsOfType(Combat.TargetingParameters.Ally, this.radius);
      foreach (var targetWithinRange in targetsWithinRange)
      {
        var distFromAlly = Vector3.Distance(target.transform.localPosition, targetWithinRange.transform.localPosition);
        if (distFromAlly <= this.radius)
        {
          this.OnSplash(caster, targetWithinRange);
        }
      }

    }

  }

}