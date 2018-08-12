/******************************************************************************/
/*!
@file   HealthModificationAttribute.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class HealthModificationAttribute 
  */
  /**************************************************************************/
  public abstract class HealthModificationEffectAttribute : EffectAttribute
  {
    public float Potency = 100.0f;

    public override void OnInspect()
    {
      #if UNITY_EDITOR
      this.Potency = UnityEditor.EditorGUILayout.FloatField("Potency", this.Potency);
      #endif
    }

    //protected override void OnApply(CombatController caster, CombatController target)
    //{
    //  Trace.Script("WHAT");
    //}

    //protected override void OnApply(CombatController caster, CombatController target);



  }

}