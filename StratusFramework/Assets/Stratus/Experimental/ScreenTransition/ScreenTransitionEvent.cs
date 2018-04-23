using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Stratus.Experimental
{
  public class ScreenTransitionEvent : Triggerable
  {
    public ScreenTransition.TransitionEvent transition = new ScreenTransition.TransitionEvent();

    protected override void OnAwake()
    {
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      Scene.Dispatch<ScreenTransition.TransitionEvent>(this.transition);
    }

  }

}