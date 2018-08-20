using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Genitus.Effects
{
  public class SplashDamageEffect : SplashEffectAttribute
  {
    public DamageEffect damage;//  = CreateInstance(typeof(DamageEffect)) as DamageEffect;

    void OnEnable()
    {
      if (damage == null)
        damage = CreateInstance(typeof(DamageEffect)) as DamageEffect;
    }

    protected override void OnSplash(CombatController caster, CombatController target)
    {
      //Trace.Script("Applying damage on " + target.Name, caster);
      damage.Apply(caster, target);
    }
  }

}