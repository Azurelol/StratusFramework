using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Genitus.Effects
{
  public class PullEffect : KineticEffectAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      target.gameObject.Dispatch<KineticAction.PullEvent>(new KineticAction.PullEvent(caster.transform, Amount));
    }
  }
}
