/******************************************************************************/
/*!
@file   InputEvents.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// A mouse button
  /// </summary>
  public enum MouseButton
  {
    None,
    LeftMouse,
    MiddleMouse,
    RightMouse
  }

  /// <summary>
  /// A gamepad button
  /// </summary>
  public enum GamepadButton
  {
    A,
    B,
    X,
    Y,
    LeftBumper,
    RightBumper,
    LeftStick,
    RightStick,
    Back,
    Start
  }

  /// <summary>
  /// An event-driven input map
  /// </summary>
  public partial class InputMap : StratusSingleton<InputMap>
  {
    protected override string Name { get { return "Input System"; } }

    /// <summary>
    /// Named actions
    /// </summary>
    Dictionary<string, Action> Actions = new Dictionary<string, Action>();

    /// <summary>
    /// Unnamed actions
    /// </summary>
    List<Action> UnnamedActions = new List<Action>();

    
    
    protected override void OnAwake()
    {
      DontDestroyOnLoad(this);
    }
        
    public static void AddAction(Action action)
    {
      Instance.UnnamedActions.Add(action);
    }

    private void FixedUpdate()
    {
      // Poll all actions
      foreach (var action in Actions)
        action.Value.Poll();

      // Poll all unnamed actions
      foreach (var action in UnnamedActions)
        action.Poll();
    }

  }
}
