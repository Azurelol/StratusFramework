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

    public class JumpEvent : Stratus.Event { }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Tooltip("Whether to print debug output")]
    public bool debug = false;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public NavMeshAgent navigation { get; private set; }
    public new Rigidbody rigidbody { get; private set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      rigidbody = GetComponent<Rigidbody>();
      navigation = GetComponent<NavMeshAgent>();
      //navigation.Warp(transform.position);
    }

    protected override void OnStart()
    {
      
    }

  }
}
