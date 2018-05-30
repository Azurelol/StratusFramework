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
  [RequireComponent(typeof(Collider))]
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(NavMeshAgent))]
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
    [Header("Input")]
    public InputField movementX = new InputField();
    public InputField movementY = new InputField();
    public InputField sprint = new InputField();
    public InputField jump = new InputField();
    [Tooltip("The camera used to orient this movement by")]
    public new Camera camera;

    [Header("Movement")]    
    public float movementThreshold = 0.2f;
    public float sprintMuiltiplier = 2f;
    public float rotationSpeed = 1f;
    public bool faceDirection = true;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public NavMeshAgent navigation { get; private set; }
    public new Rigidbody rigidbody { get; private set; }

    public MovementOffset movementOffset { get; set; } = MovementOffset.PlayerForward;
    public Func<Vector3> calculateDirectionFunction { get; private set; }
    public bool moving => Math.Abs(rigidbody.velocity.x) > movementThreshold || Math.Abs(rigidbody.velocity.z) > movementThreshold;
    public bool sprinting { get; private set; }
    public bool jumping { get; private set; }
    public Vector3 heading { get; private set; }
    public Vector3 velocity => rigidbody.velocity;
    public float currentSpeed => sprinting ? navigation.speed * sprintMuiltiplier : navigation.speed;
    public float speedRatio => currentSpeed / navigation.speed;

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      heading = transform.forward;
      rigidbody = GetComponent<Rigidbody>();
      navigation = GetComponent<NavMeshAgent>();
      navigation.Warp(transform.position);
    }

    protected override void OnStart()
    {
      
    }

    void Update()
    {
      if (jump.isDown && !jumping)
      {
        Jump();
      }
      else if (jumping)
      {
        OnJump();
      }


      if (!movementX.isNeutral || !movementY.isNeutral)
      {
        Move();

        if (moving && sprint.isPressed)
          sprinting = true;
        else if (sprint.isUp)
          sprinting = false;      
      }


      if (faceDirection)
        transform.forward = Vector3.Lerp(transform.forward, heading, Time.deltaTime * 2f);
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Movement
    //--------------------------------------------------------------------------------------------/
    private void Move()
    {
      Vector2 axis = new Vector2(movementX.value, movementY.value);
      Vector3 dir = CalculateDirection(axis, movementOffset);
      heading = dir;
      rigidbody.velocity = dir * currentSpeed; 
    }

    private void Jump()
    {
      rigidbody.AddRelativeForce(Vector3.up * navigation.speed);
      jumping = true;
      gameObject.Dispatch<JumpEvent>(Event.Cache<JumpEvent>());
    }

    private void OnJump()
    {
      if (velocity.y == 0.0f)
        jumping = false;
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public Vector3 CalculateDirection(Vector2 axis, MovementOffset offset)
    {
      Vector3 dir = Vector3.zero;
      switch (offset)
      {
        case MovementOffset.PlayerForward:
          dir.x = axis.x;
          dir.z = axis.y;
          break;

        case MovementOffset.CameraUp:
          dir = (axis.y * camera.transform.up) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;

        case MovementOffset.CameraForward:
          dir = (axis.y * camera.transform.forward) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;
      }
      return dir;
    }


  }
}
