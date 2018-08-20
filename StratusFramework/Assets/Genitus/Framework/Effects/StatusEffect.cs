using UnityEngine;
using Stratus;
using System;

namespace Genitus
{
  /// <summary>
  /// Applies a status to the target.
  /// </summary>
  public class StatusEffect : EffectAttribute
  {
    public Status Status;

    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Apply the status to the target. This will get it added to the targets's
      // list of active statuses for its duration
      Status.Apply(caster, target);
    }
  }

}