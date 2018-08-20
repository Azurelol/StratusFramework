using UnityEngine;
using Stratus;

namespace Genitus.Effects
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