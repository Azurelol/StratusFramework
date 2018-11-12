using UnityEngine;
using Stratus;
using System;

namespace Stratus.Gameplay
{
  public class StratusInputTrigger : Trigger
  {
    public InputField input = new InputField();
    public InputField.Action action = InputField.Action.Down;

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
        case InputField.Action.Down:
          triggered = input.isDown;
          break;
        case InputField.Action.Up:
          triggered = input.isUp;
          break;
        case InputField.Action.Held:
          triggered = input.isPressed;
          break;
      }

      if (triggered)
        Activate();
    }
  }
}
