/******************************************************************************/
/*!
@file   InputTrigger.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus 
{
  public class InputTrigger : Trigger
  {
    public InputField input = new InputField();
    public InputField.Action action = InputField.Action.Down;
    
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
          triggered = input.isHeld;
          break;
      }

      if (triggered)
        Activate();
    }
  }
}
