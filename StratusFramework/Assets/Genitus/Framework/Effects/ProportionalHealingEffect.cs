/******************************************************************************/
/*!
@file   ProportionalHealingEffect.cs
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
  /// 
  /// </summary>
  public class ProportionalHealingEffect : ProportionalHealthModificationAttribute
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected override void OnApply(CombatController caster, CombatController target)
    {
      float heal = (this.Percentage / 100.0f) * target.GetPotency(this.type); // target.health.maximum;
      //Trace.Script("Sending heal event with value of " + heal);
      var healEvent = new Combat.HealEvent();
      healEvent.value = heal;
      target.gameObject.Dispatch<Combat.HealEvent>(healEvent);

    }
  }

}