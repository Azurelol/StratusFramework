/******************************************************************************/
/*!
@file   SplashDamageEffect.cs
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
  @class SplashDamageEffect 
  */
  /**************************************************************************/
  public class SplashDamageEffect : SplashEffectAttribute
  {
    public DamageEffect Damage;//  = CreateInstance(typeof(DamageEffect)) as DamageEffect;

    void OnEnable()
    {
      if (Damage == null)
        Damage = CreateInstance(typeof(DamageEffect)) as DamageEffect;
    }

    public override void OnInspect()
    {
      base.OnInspect();
      Damage.OnInspect();
    }

    protected override void OnSplash(CombatController caster, CombatController target)
    {
      //Trace.Script("Applying damage on " + target.Name, caster);
      Damage.Apply(caster, target);
    }
  }

}