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


    public enum Action
    {
      Move,
      Sprint,
      Jump
    }

    //public abstract class Action {}
    //public abstract class ActionEvent<T> : Stratus.Event where T : Action {}

    /// <summary>
    /// Signals the character to move
    /// </summary>
    public class MoveEvent : Stratus.Event
    {
      public Vector3 direction;
      public bool adjustFacing;
      public bool sprint;
    }

    /// <summary>
    /// Signals the character to jump
    /// </summary>
    public class JumpEvent : Stratus.Event
    {
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Header("Settings")]
    public float movementSpeed = 10f;
    public float movementThreshold = 0.2f;
    public float sprintMuiltiplier = 2f;
    public float rotationSpeed = 1f;
    public bool faceDirection = true;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public NavMeshAgent navigation { get; private set; }
    public new Rigidbody rigidbody { get; private set; }

    public Vector3 heading { get; private set; }
    public bool moving => Math.Abs(rigidbody.velocity.x) > movementThreshold || Math.Abs(rigidbody.velocity.z) > movementThreshold;
    public bool sprinting { get; private set; }
    public bool jumping { get; private set; }
    public bool grounded { get; private set; }
    public Vector3 velocity => rigidbody.velocity;
    public float currentSpeed => sprinting ? movementSpeed * sprintMuiltiplier : movementSpeed;
    public float speedRatio => currentSpeed / movementSpeed;
    public float acceleration { get; set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      rigidbody = GetComponent<Rigidbody>();
      navigation = GetComponent<NavMeshAgent>();
      Subscribe();
    }

    private void Update()
    {
      if (moving)
        OnMove();

      if (jumping)
        OnJump();



      // Decelerate somehow..
    }

    private void FixedUpdate()
    {
      if (faceDirection)
      {
        transform.forward = Vector3.Lerp(transform.forward, heading, Time.fixedDeltaTime * rotationSpeed);
      }
    }

    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
    private void Subscribe()
    {
      gameObject.Connect<MoveEvent>(OnMoveEvent);
      gameObject.Connect<JumpEvent>(OnJumpEvent);
    }

    private void OnMoveEvent(MoveEvent e)
    {
      //Trace.Script($"Direction = {e.direction}", this);
      Move(e.direction);
      sprinting = e.sprint;
    }

    private void OnJumpEvent(JumpEvent e)
    {
      if (!jumping)
        Jump();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Movement
    //--------------------------------------------------------------------------------------------/
    protected void Move(Vector3 dir)
    {
      heading = dir;
      acceleration += (currentSpeed * Time.deltaTime);
      acceleration = Mathf.Min(acceleration, movementSpeed);
      OnMove();
    }

    private void OnMove()
    {      //Trace.Script("Applying velocity");
      rigidbody.velocity = heading * acceleration;
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

    //--------------------------------------------------------------------------------------------/
    // Methods: Utility
    //--------------------------------------------------------------------------------------------/

  }

}