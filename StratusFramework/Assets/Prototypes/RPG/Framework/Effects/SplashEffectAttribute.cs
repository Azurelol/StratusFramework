/******************************************************************************/
/*!
@file   SplashEffectAttribute.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stratus;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class SplashEffectAttribute 
  */
  /**************************************************************************/
  public abstract class SplashEffectAttribute : EffectAttribute
  {
    public float Radius = 3.0f;
    protected abstract void OnSplash(CombatController caster, CombatController target);

    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Find eligible targets within range of the target
      //var eligibleTargets = new List<CombatController>();
      //Trace.Script("Looking for cleave targets!");
      var targetsWithinRange = target.FindTargetsOfType(CombatController.TargetingParameters.Ally, this.Radius);
      foreach (var targetWithinRange in targetsWithinRange)
      //foreach (var targetWithinRange in target.FindTargetsOfType(CombatController.TargetingParameters.Ally))
      {
        var distFromAlly = Vector3.Distance(target.transform.localPosition, targetWithinRange.transform.localPosition);
        if (distFromAlly <= this.Radius)
        {
          //eligibleTargets.Add(targetWithinRange);
          this.OnSplash(caster, targetWithinRange);
        }
      }

    }

    public override void OnInspect()
    {
      this.Radius = EditorBridge.Field("Radius", this.Radius);
    }

  }

}