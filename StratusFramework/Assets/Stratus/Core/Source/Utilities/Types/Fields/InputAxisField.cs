using System;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Allows you to refer to any saved input axis as a field (rather than manually writing the string name)
  /// </summary>
  [Serializable]
  public class InputAxisField
  {
    /// <summary>
    /// The current state of the input
    /// </summary>
    public enum State
    {
      Pressed,
      Down,
      Up,
      None
    }

    #pragma warning disable 414
    [SerializeField]
    private string axis;
    #pragma warning restore 414
    public static implicit operator string(InputAxisField inputAxisField) { return inputAxisField.axis; }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// set axis
    /// </summary>
    public void setAxis(string axisName) { axis = axisName; }
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
    /// <summary>
    /// Returns true while the virtual button is held down
    /// </summary>
    public bool isPressed { get { return Input.GetButton(axis); } }
    /// <summary>
    /// Returns true the first frame the user pressed the button
    /// </summary>
    public bool isDown { get { return Input.GetButtonDown(axis); } }
    /// <summary>
    /// Returns true the first frame the user releases the button
    /// </summary>
    public bool isUp { get { return Input.GetButtonUp(axis); } }



  }

}