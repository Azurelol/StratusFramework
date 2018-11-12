using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Stratus.Gameplay
{
  /// <summary>
  /// A simple, modular player controller
  /// </summary>
  public class StratusCharacterController : ExtensibleBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public enum MovementOffset
    {
      PlayerForward,
      CameraForward,
      CameraUp
    }

    public enum InputAxis
    {
      Horizontal,
      Vertical
    }

    public enum Action
    {
      Move,
      Sprint,
      Jump
    }

    public class MovementPreset
    {
      public VectorAxis horizontalAxisInput;
      public VectorAxis verticalAxisInput;
      public MovementOffset offset;
    }

    public class JumpEvent : Stratus.StratusEvent { }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [HideInInspector]
    public bool debug = false;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
    }

    protected override void OnStart()
    {      
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    [ContextMenu("Show Debug")]
    private void ShowDebug()
    {
      this.debug = true;
    }

    [ContextMenu("Hide Debug")]
    private void HideDebug()
    {
      this.debug = false;
    }

  }
}
