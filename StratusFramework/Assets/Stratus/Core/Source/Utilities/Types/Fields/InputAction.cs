using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Stratus
{
  /// <summary>
  /// A field for encapsulating custom actions depending on input read
  /// </summary>
  [Serializable]
  public class InputAction : StratusSerializable
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    [Serializable]
    public class OnButtonInput : UnityEvent { }

    [Serializable]
    public class OnAxisInput : UnityEvent<float> { }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public InputField input = new InputField();
    public InputField.AxisType axisType;
    public InputField.State state;
    public OnButtonInput onInput = new OnButtonInput();
    public OnAxisInput onAxisInput = new OnAxisInput();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public InputField.Type type => input.type;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public void Update()
    {
      switch (type)
      {
        case InputField.Type.Key:
        case InputField.Type.MouseButton:
          {
            switch (state)
            {
              case InputField.State.Down:
                if (input.isDown)
                  onInput.Invoke();
                break;

              case InputField.State.Up:
                if (input.isUp)
                  onInput.Invoke();
                break;

              case InputField.State.Pressed:
                if (input.isPressed)
                  onInput.Invoke();
                break;

            }
          }
          break;


        case InputField.Type.Axis:
          if (!input.isNeutral)
            onAxisInput.Invoke(input.value);
          break;
      }
    }
  }

}