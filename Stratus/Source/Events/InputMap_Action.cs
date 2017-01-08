/******************************************************************************/
/*!
@file   InputMap_Action.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  public partial class InputMap : StratusSingleton<InputMap>
  {
    /// <summary>
    /// A callback-based input binding. Set the appropiate callbacks in order
    /// to be notified when the input is being used or not.
    /// </summary>
    [Serializable]
    public class Action
    {
      // Callbacks
      public delegate void DownCallback();
      public delegate void PressedCallback();
      public delegate void UpCallback();

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      // Bindings
      public KeyCode Key;
      public MouseButton Mouse;
      public GamepadButton Button;
      // Callbacks
      /// <summary>
      /// Whether the control is currently down
      /// </summary>
      public DownCallback OnDown;
      /// <summary>
      /// Whether the control is currently being pressed 
      /// </summary>
      public PressedCallback OnPressed;
      /// <summary>
      /// Whether the control has been let go
      /// </summary>
      public UpCallback OnUp;

      bool IsCurrentlyDown = false;

      //------------------------------------------------------------------------/
      // Constructor
      //------------------------------------------------------------------------/
      /// <summary>
      /// A named action
      /// </summary>
      /// <param name="name"></param>
      public Action(string name)
      {
        // Adds this action to the input map
      }

      /// <summary>
      /// An unnamed action
      /// </summary>
      /// <param name="name"></param>
      public Action()
      {
      }

      /// <summary>
      /// Configures this action, setting the appropiate callbacks
      /// </summary>
      /// <param name="onDown">The function which will be invoked when this input is first pressed</param>
      /// <param name="onPressed">The function which will be invoked when this input is being pressed every frame</param>
      /// <param name="onUp">The function which will be invoked when this input is first released after being pressed</param>
      public void Setup(DownCallback onDown = null, PressedCallback onPressed = null, UpCallback onUp = null)
      {
        InputMap.AddAction(this);
        OnDown = onDown;
        OnPressed = onPressed;
        OnUp = onUp;
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Converts from our enumeration to a value that Unity can use.
      /// </summary>
      /// <param name="mouse"></param>
      /// <returns></returns>
      public static int Convert(GamepadButton button)
      {
        switch (button)
        {
          case GamepadButton.A:
            return 0;
          case GamepadButton.B:
            return 1;
          case GamepadButton.X:
            return 2;
          case GamepadButton.Y:
            return 3;
          case GamepadButton.LeftBumper:
            return 4;
          case GamepadButton.RightBumper:
            return 5;
          case GamepadButton.Back:
            return 6;
          case GamepadButton.Start:
            return 7;
        }

        throw new System.Exception("No valid button!");
      }

      /// <summary>
      /// Converts from our enumeration to a value that Unity can use.
      /// </summary>
      /// <param name="mouse"></param>
      /// <returns></returns>
      public static int Convert(MouseButton mouse)
      {
        switch (mouse)
        {
          case MouseButton.LeftMouse:
            return 0;
          case MouseButton.MiddleMouse:
            return 2;
          case MouseButton.RightMouse:
            return 1;
        }
        throw new System.Exception("No valid mouse button!");
      }

      /// <summary>
      /// Polls this input
      /// </summary>
      public void Poll()
      {
        // Check if the input is currently down
        if (IsDown && OnDown != null) OnDown();
        // Check if the input is currently being pressed
        if (IsPressed)
        {
         if (OnPressed != null) OnPressed();
        }
        // If the input is not being pressed now, but was on the previous frame,
        // that means it has been released
        else
        {
          if (IsCurrentlyDown)
          {
            IsCurrentlyDown = false;
            if (OnUp != null) OnUp();
          }
        }

      }

      /// <summary>
      /// Returns whether the given button is held down
      /// </summary>
      /// <returns></returns>
      public bool IsPressed
      {
        get
        {
          // Check for keyboard input
          if (Key != KeyCode.None)
          {
            if (Input.GetKey(this.Key))
            {
              IsCurrentlyDown = true;
              return true;
            }

          }

          //  Check the mouse next
          if (Mouse != MouseButton.None)
          {
            if (Input.GetMouseButton(Convert(this.Mouse)))
            {
              IsCurrentlyDown = true;
              return true;
            }

          }

          return false;
        }

      }

      /// <summary>
      /// Returns true during the frame the user pressed the given button.
      /// </summary>
      /// <returns></returns>
      public bool IsDown
      {
        get
        {
          // Check for keyboard input
          if (Key != KeyCode.None)
          {
            if (Input.GetKeyDown(this.Key))
            {              
              return true;
            }
          }

          // Check the mouse input next
          if (Mouse != MouseButton.None)
          {
            if (Input.GetMouseButtonDown(Convert(this.Mouse)))
            {              
              return true;
            }
          }

          return false;
        }
      }

    }
  }
}
