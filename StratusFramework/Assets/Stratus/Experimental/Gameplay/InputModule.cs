using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Stratus
{
  public class InputModule : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    [Serializable]
    public class OnButtonInput : UnityEvent { }

    [Serializable]
    public class OnAxisInput : UnityEvent<float> { }

    [Serializable]
    public class InputAction : StratusSerializable
    {
      public InputField input = new InputField();
      public InputField.AxisTyoe axisType;
      public InputField.ButtonState buttonState;
      public OnButtonInput onInput;
      public OnAxisInput onAxisInput;

      public InputField.Type type => input.type;
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public List<InputAction> inputs = new List<InputAction>();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {

    }

    private void Update()
    {
      foreach (var input in inputs)
      {
        switch (input.type)
        {
          case InputField.Type.Key:
          case InputField.Type.MouseButton:
            {
              switch (input.buttonState)
              {
                case InputField.ButtonState.Down:
                  if (input.input.isDown)
                    input.onInput.Invoke();
                  break;

                case InputField.ButtonState.Up:
                  if (input.input.isUp)
                    input.onInput.Invoke();
                  break;

                case InputField.ButtonState.Pressed:
                  if (input.input.isPressed)
                    input.onInput.Invoke();
                  break;

              }
            }
            break;


          case InputField.Type.Axis:
            if (!input.input.isNeutral)
              input.onAxisInput.Invoke(input.input.value);
            break;
        }

      }
    }
  }



}