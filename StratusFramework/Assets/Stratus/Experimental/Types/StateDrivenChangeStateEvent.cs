using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
  public abstract class StateDrivenChangeStateEvent<State> : StratusTriggerable where State : struct, IConvertible
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