
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
    public enum Type { Key, MouseButton }
    public enum MouseButton { Left, Middle, Right }
    public enum Action { Down, Up, Held }

    [SerializeField]
    private Type type = Type.Key;
    [SerializeField]
    private KeyCode key;
    [SerializeField]
    private MouseButton mouseButton;

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
        }

        throw new Exception("Input type not supported");
      }
    }

    /// <summary>
    /// Returns true while the input is pressed down
    /// </summary>
    public bool isHeld
    {
      get
      {
        switch (type)
        {
          case Type.Key:
            return Input.GetKey(key);
          case Type.MouseButton:
            return Input.GetMouseButton((int)mouseButton);
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
        }

        throw new Exception("Input type not supported");
      }
    }

  }
}