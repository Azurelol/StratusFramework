using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Genitus.Effects
{
  public class PushEffect : KineticEffectAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      target.gameObject.Dispatch<KineticAction.PushEvent>(new KineticAction.PushEvent(caster.transform, Amount));
    }
  }
}
