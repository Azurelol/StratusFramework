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
    public InputBinding input = new InputBinding();
    public InputBinding.AxisType axisType;
    public InputBinding.State state;
    public OnButtonInput onInput = new OnButtonInput();
    public OnAxisInput onAxisInput = new OnAxisInput();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public InputBinding.Type type => input.type;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public void Update()
    {
      switch (type)
      {
        case InputBinding.Type.Key:
        case InputBinding.Type.MouseButton:
          {
            switch (state)
            {
              case InputBinding.State.Down:
                if (input.isDown)
                  onInput.Invoke();
                break;

              case InputBinding.State.Up:
                if (input.isUp)
                  onInput.Invoke();
                break;

              case InputBinding.State.Pressed:
                if (input.isPressed)
                  onInput.Invoke();
                break;

            }
          }
          break;


        case InputBinding.Type.Axis:
          if (!input.isNeutral)
            onAxisInput.Invoke(input.value);
          break;
      }
    }
  }

}