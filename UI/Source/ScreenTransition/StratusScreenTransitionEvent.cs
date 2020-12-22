using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Stratus.Experimental
{
  public class StratusScreenTransitionEvent : StratusTriggerable
  {
    public StratusScreenShaderTransition.TransitionEvent transition = new StratusScreenShaderTransition.TransitionEvent();

    protected override void OnAwake()
    {
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      StratusScene.Dispatch<StratusScreenShaderTransition.TransitionEvent>(this.transition);
    }

  }

}