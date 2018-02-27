using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  public abstract class StateDrivenChangeStateEvent<State> : Triggerable where State : struct, IConvertible
  {
    public State state;

    protected override void OnAwake()
    {
      throw new System.NotImplementedException();
    }
    
    protected override void OnTrigger()
    {
      throw new System.NotImplementedException();
    }

  }

}