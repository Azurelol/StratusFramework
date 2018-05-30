using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using System;

namespace Stratus.Gameplay
{
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(NavMeshAgent))]
  public class CharacterMovement : StratusBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public class MovementPreset
    {
    }

    public enum MovementOffset
    {
      PlayerForward,
      CameraForward,
      CameraUp
    }

    public enum Action
    {
      Move,
      Sprint,
      Jump
    }

    //public abstract class Action {}
    //public abstract class ActionEvent<T> : Stratus.Event where T : Action {}
    public class MoveEvent : Stratus.Event
    {
      public bool sprint;
    }
    public class JumpEvent : Stratus.Event {}
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Header("Settings")]
    public new Camera camera;
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
    public Vector3 heading { get; private set; }
    public bool moving => Math.Abs(rigidbody.velocity.x) > movementThreshold || Math.Abs(rigidbody.velocity.z) > movementThreshold;
    public bool sprinting { get; private set; }
    public bool jumping { get; private set; }
    public Vector3 velocity => rigidbody.velocity;
    public float currentSpeed => sprinting ? navigation.speed * sprintMuiltiplier : navigation.speed;
    public float speedRatio => currentSpeed / navigation.speed;


    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Update()
    {
      if (jumping)
        OnJump();

      // Decelerate somehow..
    }

    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
    

    //--------------------------------------------------------------------------------------------/
    // Methods: Movement
    //--------------------------------------------------------------------------------------------/
    protected void Move(Vector2 axis)
    {
      Vector3 dir = CalculateDirection(axis, movementOffset);
      heading = dir;
      rigidbody.velocity = dir * currentSpeed;
    }
    
    protected void Jump()
    {
      rigidbody.AddRelativeForce(Vector3.up * navigation.speed);
      jumping = true;
      gameObject.Dispatch<JumpEvent>(Event.Cache<JumpEvent>());
    }
    
    protected void OnJump()
    {
      if (velocity.y == 0.0f)
        jumping = false;
    }
    
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