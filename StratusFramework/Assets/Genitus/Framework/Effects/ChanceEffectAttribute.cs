/******************************************************************************/
/*!
@file   ChanceEffectAttribute.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Genitus
{
  /// <summary>
  /// Applies the effect on a percentage-based chance.
  /// </summary>
  public abstract class ChanceEffectAttribute : EffectAttribute
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public float Chance = 1.0f;
    protected abstract void OnChance(CombatController caster, CombatController target);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public override void OnInspect()
    {
      this.Chance = EditorBridge.Field("Chance", this.Chance);
    }

    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Roll to see whether the effect should be applied or not
      if (Roll())
      {
        this.OnChance(caster, target);
      }
    }

    bool Roll()
    {
      var roll = UnityEngine.Random.Range(0.0f, 1.0f);
      if (roll >= (1.0f - this.Chance))
      {
        return true;
      }
      return false;
    }

  }

}