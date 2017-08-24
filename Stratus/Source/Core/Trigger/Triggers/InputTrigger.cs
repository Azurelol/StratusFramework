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

namespace Prototype 
{
  public class InputTrigger : Trigger
  {
    public InputField input = new InputField();
    public InputField.Action action = InputField.Action.Down;

    protected override void OnEnabled()
    {
      switch (action)
      {
        case InputField.Action.Down:
          break;
        case InputField.Action.Up:
          break;
        case InputField.Action.Held:
          break;
      }
    }

    protected override void OnInitialize()
    {
      
    }

    void Update()
    {
      bool isTrigger = false;
      switch (action)
      {
        case InputField.Action.Down:
          isTrigger = input.isDown;
          break;
        case InputField.Action.Up:
          isTrigger = input.isUp;
          break;
        case InputField.Action.Held:
          isTrigger = input.isHeld;
          break;
      }
    }
  }
}
