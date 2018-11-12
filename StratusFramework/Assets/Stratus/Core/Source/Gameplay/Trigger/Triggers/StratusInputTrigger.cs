using UnityEngine;
using Stratus;
using System;

namespace Stratus.Gameplay
{
  public class StratusInputTrigger : Trigger
  {
    public InputBinding input = new InputBinding();
    public InputBinding.Action action = InputBinding.Action.Down;

    public override string automaticDescription => $"On {input} {action}";

    protected override void OnAwake()
    {      
    }

    protected override void OnReset()
    {

    }

    void Update()
    {
      bool triggered = false;
      switch (action)
      {
        case InputBinding.Action.Down:
          triggered = input.isDown;
          break;
        case InputBinding.Action.Up:
          triggered = input.isUp;
          break;
        case InputBinding.Action.Held:
          triggered = input.isPressed;
          break;
      }

      if (triggered)
        Activate();
    }
  }
}
