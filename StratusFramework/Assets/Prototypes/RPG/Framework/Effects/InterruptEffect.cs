/******************************************************************************/
/*!
@file   InterruptEffect.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /// <summary>
  /// Interrupts an action that is currently being cast.
  /// </summary>
  public class InterruptEffect : ChanceEffectAttribute
  {
    protected override void OnChance(CombatController caster, CombatController target)
    {
      // Interrupt the target's current action
      if (target.isActing)
      {
        Trace.Script("Sending interrupt event");
        target.gameObject.Dispatch<CombatAction.CancelEvent>(new CombatAction.CancelEvent());
      }

    }
  }

}