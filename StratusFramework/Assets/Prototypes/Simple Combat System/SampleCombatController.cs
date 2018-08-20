using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Genitus;

namespace Prototype
{
  public class SampleCombatController : CombatController<StandardCharacter>
  {
    public override bool isActing
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    protected override void OnControllerInitialize()
    {
      
    }

    protected override void OnTimeStep(float step)
    {

    }

    public override float QueryPotency(EffectAttribute.PotencyQuery query)
    {
      float potency = 0.0f;
      switch (query)
      {
        case EffectAttribute.PotencyQuery.PhysicalDamageDealt:
          potency = character.physicalOffense;
          break;

        case EffectAttribute.PotencyQuery.MagicalDamageDealt:
        case EffectAttribute.PotencyQuery.HealingDealt:
          potency = character.magicalOffense;
          break;

        case EffectAttribute.PotencyQuery.MaximumHealth:
          potency = character.hitpoints.maximum;
          break;

        case EffectAttribute.PotencyQuery.Evasion:
          potency = character.evasion;
          break;

      }
      return potency;
    }

    protected override void OnActionCanceled()
    {
      throw new NotImplementedException();
    }

    protected override void OnActionDelay(float delay)
    {
      throw new NotImplementedException();
    }

    protected override void OnActionStarted()
    {
      throw new NotImplementedException();
    }

    protected override void OnActionEnded()
    {
      throw new NotImplementedException();
    }
  }

}