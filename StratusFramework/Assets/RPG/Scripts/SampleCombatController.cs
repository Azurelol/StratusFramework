using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altostratus;

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

    public override float GetPotency(Enum enumeratedType)
    {
      throw new NotImplementedException();
    }

    public override bool IsAvailable(Ability ability)
    {
      throw new NotImplementedException();
    }

    protected override void OnActionCanceled()
    {
      throw new NotImplementedException();
    }

    protected override void OnActionDelay(float delay)
    {
      throw new NotImplementedException();
    }

    protected override void OnControllerInitialize()
    {
      throw new NotImplementedException();
    }

    protected override void OnTimeStep(float step)
    {
      throw new NotImplementedException();
    }
  }

}