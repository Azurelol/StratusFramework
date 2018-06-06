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

    public enum LocomotionMode
    {
      Velocity,
      Force,
      CharacterController
    }

    public struct Settings
    {

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
    [Tooltip("How locomotion for this character is handled")]
    public LocomotionMode locomotion = LocomotionMode.Velocity;
    [Header("Settings")]
    [Tooltip("The maximum speed at which the character will move")]
    public float speed = 10f;
    [Tooltip("How fast the character achieves the desired speed")]
    public float acceleration = 10f;
    public float movementThreshold = 0.2f;
    public float sprintMuiltiplier = 2f;
    public float rotationSpeed = 1f;
    [Range(0f, 360f)]
    public float rotationThreshold = 180f;
    public bool faceDirection = true;

    private Countdown inertiaTimer;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public NavMeshAgent navigation { get; private set; }
    public new Rigidbody rigidbody { get; private set; }

    public Vector3 heading { get; private set; }
    public bool moving { get; private set; } 
    public bool sprinting { get; private set; }
    public bool jumping { get; private set; }
    public bool grounded { get; private set; }
    public Vector3 velocity => rigidbody.velocity;
    public float maximumSpeed => sprinting ? speed * sprintMuiltiplier : speed;
    public float speedRatio => maximumSpeed / speed;
    public float currentSpeed => rigidbody.velocity.magnitude;
    private CharacterController characterController { get; set; }
    private Vector3 lastPosition { get; set; }
    //public float acceleration { get; set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      inertiaTimer = new Countdown(0.1f);
      rigidbody = GetComponent<Rigidbody>();
      navigation = GetComponent<NavMeshAgent>();

      if (locomotion == LocomotionMode.CharacterController)
        characterController = gameObject.AddComponent<CharacterController>();

      Subscribe();
    }

    private void Update()
    {
      inertiaTimer.Update(Time.deltaTime);

      if (moving && currentSpeed <= movementThreshold)
        rigidbody.velocity = Vector3.zero;

      if (jumping)
        OnJump();

      // Decelerate somehow..
    }

    private void FixedUpdate()
    {
       moving = locomotion == LocomotionMode.CharacterController ? IsMovingWithPosition() : IsMovingWithVelocity();
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
    protected void Move(Vector3 direction)
    {
      heading = direction;

      if (faceDirection)
      {
        transform.forward = Vector3.Lerp(transform.forward, heading, Time.fixedDeltaTime * rotationSpeed);
      }

      // Don't move while the heading is greater than 180 degrees?
      Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
      float headingDifference = Mathf.Abs(rotation.eulerAngles.y - transform.eulerAngles.y);
      //Trace.Script($"Heading difference = {headingDifference}", this);
      if (headingDifference >= rotationThreshold)
        return;

      switch (locomotion)
      {
        case LocomotionMode.Velocity:
          MoveWithVelocity(direction);
          break;

        case LocomotionMode.Force:
          MoveWithForce(direction);
          break;

        case LocomotionMode.CharacterController:
          MoveWithCharacterController(direction);
          break;
      }

      OnMove();

      // If the difference in rotation is less than 180
      //Vector3 

      //acceleration += (currentSpeed * Time.deltaTime);
      //acceleration = Mathf.Min(acceleration, movementSpeed);
      //OnMove();
    }

    private void OnMove()
    {
      //moving = true;
      inertiaTimer.Reset();
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
    protected virtual void MoveWithVelocity(Vector3 dir)
    {
      Vector3 newVelocity = dir * maximumSpeed; 
      float t = rigidbody.velocity.magnitude / maximumSpeed;
      rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, newVelocity, t);
    }

    protected virtual void MoveWithForce(Vector3 dir)
    {
      //rigidbody.AddForce(dir * maximumSpeed, ForceMode.Force);
      rigidbody.AddForce(dir * acceleration * Time.deltaTime, ForceMode.VelocityChange);
    }

    protected virtual void MoveWithCharacterController(Vector3 dir)
    {
      dir.y -= Physics.gravity.y;
      characterController.Move(dir * acceleration * Time.deltaTime);
    }

    protected virtual void RotateToTarget(Vector3 target)
    {
      Quaternion rot = Quaternion.LookRotation(target - transform.position);
      var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
      //targetRotation = Quaternion.Euler(newPos);
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), rotationSpeed * Time.deltaTime);
    }

    private bool IsMovingWithVelocity() => Math.Abs(rigidbody.velocity.x) > movementThreshold || Math.Abs(rigidbody.velocity.z) > movementThreshold;
    private bool IsMovingWithPosition() => Vector3.Distance(lastPosition, transform.position) <= movementThreshold;

  }

}