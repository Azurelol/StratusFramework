
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Allows you to quickly set up an input (button/key/mouse button) as a field and check it with
  /// provided methods.
  /// </summary>
  [Serializable]
  public class InputField 
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// What type of input is being read
    /// </summary>
    public enum Type { Key, MouseButton, Axis }
    /// <summary>
    /// The mouse button
    /// </summary>
    public enum MouseButton { Left, Middle, Right }
    /// <summary>
    /// What action is being checked for
    /// </summary>
    public enum Action { Down, Up, Held }
    /// <summary>
    /// The current state of the axis input
    /// </summary>
    public enum AxisState
    {
      Pressed,
      Down,
      Up,
      None
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [SerializeField]
    private Type type = Type.Key;
    [SerializeField]
    private KeyCode key;
    [SerializeField]
    private MouseButton mouseButton;
    #pragma warning disable 414
    [SerializeField]
    private string axis;
    #pragma warning restore 414

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public InputField()
    {
    }

    public InputField(KeyCode key)
    {
      this.key = key;
      this.type = Type.Key;
    }

    public InputField(MouseButton button)
    {
      this.mouseButton = button;
      this.type = Type.MouseButton;
    }

    public InputField(string axis)
    {
      this.axis = axis;
      this.type = Type.Axis;
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Returns true during the first frame the input is pressed down
    /// </summary>
    public bool isDown
    {
      get
      {
        switch (type)
        {
          case Type.Key:
            return Input.GetKeyDown(key);
          case Type.MouseButton:
            return Input.GetMouseButtonDown((int)mouseButton);
          case Type.Axis:
            return Input.GetButtonDown(axis);
        }

        throw new Exception("Input type not supported");
      }
    }

    /// <summary>
    /// Returns true while the input is pressed down
    /// </summary>
    public bool isPressed
    {
      get
      {
        switch (type)
        {
          case Type.Key:
            return Input.GetKey(key);
          case Type.MouseButton:
            return Input.GetMouseButton((int)mouseButton);
          case Type.Axis:
            return Input.GetButton(axis);
        }

        throw new Exception("Input type not supported");
      }
    }

    /// <summary>
    /// Returns true during the first frame the user releases the input
    /// </summary>
    public bool isUp
    {
      get
      {
        switch (type)
        {
          case Type.Key:
            return Input.GetKeyUp(key);
          case Type.MouseButton:
            return Input.GetMouseButtonUp((int)mouseButton);
          case Type.Axis:
            return Input.GetButtonUp(axis);
        }

        throw new Exception("Input type not supported");
      }
    }

    /// <summary>
    /// Returns true if the current value of the virtual axis is greater than 0
    /// </summary>
    public bool isPositive { get { return Input.GetAxis(axis) > 0f; } }

    /// <summary>
    /// Returns true if the current value of the virtual axis is less than 0
    /// </summary>
    public bool isNegative { get { return Input.GetAxis(axis) < 0f; } }

    /// <summary>
    /// Returns true if the current value of the virtual axis is 0
    /// </summary>
    public bool isNeutral { get { return Input.GetAxis(axis) == 0f; } }

    /// <summary>
    /// Returns the value of the virtual axis
    /// </summary>
    public float value { get { return Input.GetAxis(axis); } }

    /// <summary>
    /// Returns the value of the virtual axis with no smoothing filtering applied
    /// </summary>
    public float rawValue { get { return Input.GetAxisRaw(axis); } }

    public override string ToString()
    {
      string value = null;
      switch (type)
      {
        case Type.Key:
          value = $"Key {key}";
          break;
        case Type.MouseButton:
          value = $"{mouseButton} Mouse Button";
          break;
      }

      return value;
    }    

  }
}