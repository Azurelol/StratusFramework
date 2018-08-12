/******************************************************************************/
/*!
@file   HealingEffect.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class HealingEffect 
  */
  /**************************************************************************/
  public class HealingEffect : HealthModificationEffectAttribute
  {
    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Calculate healing to apply
      int damage = (int)((this.Potency / 100) * caster.attack.current);
      // Send a damage event to the target
      var healEvent = new CombatController.HealEvent();
      healEvent.value = damage;
      target.gameObject.Dispatch<CombatController.HealEvent>(healEvent);

    }
  }

}