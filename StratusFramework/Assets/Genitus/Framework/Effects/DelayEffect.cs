/******************************************************************************/
/*!
@file   DelayEffect.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Genitus
{
  /// <summary>
  /// Pushes back the target's waiting/casting progress.
  /// </summary>
  public class DelayEffect : ChanceEffectAttribute
  {
    [Range(0.0f, 1.0f)] public float Delay = 0.25f;

    public override void OnInspect()
    {
      base.OnInspect();
      this.Delay = EditorBridge.Field("Delay", Delay);
    }

    protected override void OnChance(CombatController caster, CombatController target)
    {
      var e = new CombatAction.DelayEvent();
      e.Delay = this.Delay;
      target.gameObject.Dispatch<CombatAction.DelayEvent>(e);
    }
  }

}