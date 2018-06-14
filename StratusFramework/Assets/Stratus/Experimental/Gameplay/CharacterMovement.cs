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
      CharacterController,
      NavMeshAgent
    }

    public enum GroundDetection
    {
      Raycast,
      CheckSphere,
      Collision
    }

    public enum MovementSpeed
    {
      Walk,
      Sprint
    }

    public struct Settings
    {

    }

    /// <summary>
    /// Base class for all CharacterMovement events
    /// </summary>
    public abstract class BaseMoveEvent : Stratus.Event
    {
      public FloatOverride speedOVerride;
      public bool turn;
    }

    /// <summary>
    /// Signals the character to move towards the given direction
    /// </summary>
    public class MoveEvent : BaseMoveEvent
    {
      public Vector3 direction;
      public bool sprint;
    }

    /// <summary>
    /// Signals the character to move towards the given position
    /// </summary>
    public class MoveToEvent : BaseMoveEvent
    {
      public Vector3 position;
    }

    /// <summary>
    /// Signals the character to jump
    /// </summary>
    public class JumpEvent : Stratus.Event { }

    public class DidSomethingEvent : ActionEvent<DidSomethingEvent> { }


    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Tooltip("How locomotion for this character is handled")]
    public LocomotionMode locomotion = LocomotionMode.Velocity;

    [Header("Movement")]
    [Tooltip("The maximum speed at which the character will move")]
    public float speed = 10f;
    [Tooltip("The curve of determining how the character ramps up in speed")]
    public AnimationCurve accelerationCurve;
    [Tooltip("How long it takes the character to ramp up to full speed"), Range(0f, 1f)]
    public float accelerationRampUp = 1.0f;
    public float movementThreshold = 0.2f;
    public float sprintMuiltiplier = 2f;
    public float rotationSpeed = 1f;
    [Range(0f, 360f)]
    public float rotationThreshold = 180f;
    public bool faceDirection = true;

    [Header("Jumping")]
    public GroundDetection groundDetection = GroundDetection.Raycast;
    public CapsuleCollider groundCollider;
    public LayerMask groundLayer = new LayerMask();
    [Range(0f, 1f)]
    public float groundCastFrequency = 0.1f;
    public bool airControl = true;

    private Vector3 sphereOffset;
    private static Ray groundCastRay = new Ray();
    private static RaycastHit[] groundCast = new RaycastHit[50];
    private Countdown inertiaTimer, accelerationTimer, groundCastTimer, jumpTimer;


    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public new Rigidbody rigidbody { get; private set; }
    public NavMeshAgent navigation { get; private set; }
    public CharacterController characterController { get; private set; }
    private CollisionProxy collisionProxy { get; set; }

    public bool moving { get; private set; }
    public bool turning { get; private set; }
    public bool sprinting { get; private set; }
    public bool jumping { get; private set; }
    public bool grounded { get; private set; } = true;
    private bool movingTo { get; set; }
    public bool receivedInput => !inertiaTimer.isFinished;

    private Vector3 nextVelocity { get; set; }
    public Vector3 heading { get; private set; }
    public Vector3 velocity => rigidbody.velocity;
    public float currentSpeed { get { Vector3 horizontalVelocity = velocity; horizontalVelocity.y = 0f; return horizontalVelocity.magnitude; } }
    public float maximumSpeed => sprinting ? speed * sprintMuiltiplier : speed;
    public float speedRatio => maximumSpeed / speed;
    private Vector3 lastPosition { get; set; } = Vector3.zero;
    public float accelerationRatio => accelerationTimer.inverseNormalizedProgress;
    public float velocityRatio => currentSpeed / maximumSpeed;
    public bool checkingMovement => inertiaTimer.isFinished;

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      inertiaTimer = new Countdown(0.05f);
      jumpTimer = new Countdown(0.05f);
      accelerationTimer = new Countdown(accelerationRampUp);
      rigidbody = GetComponent<Rigidbody>();
      navigation = GetComponent<NavMeshAgent>();
      //navigation.updatePosition = navigation.updateRotation = false;

      SetGroundDetection();

      if (locomotion == LocomotionMode.CharacterController)
        characterController = gameObject.GetOrAddComponent<CharacterController>();

      Subscribe();
    }

    private void Update()
    {
      inertiaTimer.Update(Time.deltaTime);

      CheckMovement();
      CheckGrounded();

      if (jumping)
      {
        jumping = !grounded;
        //if (!jumping)
        //  navigation.updatePosition = true;
        Trace.Script($"Jumping {jumping}");
      }

    }

    private void FixedUpdate()
    {

    }

    private void Reset()
    {
      groundCollider = GetComponent<CapsuleCollider>();
    }

    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
    private void Subscribe()
    {
      gameObject.Connect<MoveEvent>(OnMoveEvent);
      gameObject.Connect<MoveToEvent>(OnMoveToEvent);
      gameObject.Connect<JumpEvent>(OnJumpEvent);
    }

    private void OnMoveEvent(MoveEvent e)
    {
      sprinting = e.sprint;

      // Don't move while jumping if there's no air control set
      if (jumping && !airControl)
        return;

      if (e.turn)
        Turn(e.direction);

      if (!turning)
        Move(e.direction, e.turn);
    }

    private void OnMoveToEvent(MoveToEvent e)
    {
      MoveTo(e.position);
    }

    private void OnJumpEvent(JumpEvent e)
    {
      if (grounded && !jumping)
        Jump();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Movement
    //--------------------------------------------------------------------------------------------/
    protected void Move(Vector3 direction, bool turn)
    {
      // Record the latest heading
      heading = direction;

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

        case LocomotionMode.NavMeshAgent:
          MoveWithNavMeshAgent(direction);
          break;
      }

      OnMove();
    }

    protected void Turn(Vector3 direction)
    {
      transform.forward = Vector3.Lerp(transform.forward, direction, Time.fixedDeltaTime * rotationSpeed);

      // Don't move while the heading is greater than 180 degrees?
      Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
      float headingDifference = Mathf.Abs(rotation.eulerAngles.y - transform.eulerAngles.y);

      // If currently turning greater than threshold, don't move yet
      turning = (headingDifference >= rotationThreshold);

    }

    protected void MoveTo(Vector3 position)
    {
      movingTo = true;
      navigation.SetDestination(position);
      OnMove();
    }

    private void OnMove()
    {
      moving = true;
      inertiaTimer.Reset();
    }

    protected void Jump()
    {
      switch (locomotion)
      {
        case LocomotionMode.Velocity:
          JumpWithVelocity();
          break;

        case LocomotionMode.Force:
          JumpWithVelocity();
          break;

        case LocomotionMode.CharacterController:
          JumpWithCharacterController();
          break;

        case LocomotionMode.NavMeshAgent:
          break;
      }

      OnJump();
    }

    protected void OnJump()
    {
      grounded = false;
      jumping = true;
      //navigation.updatePosition = false;
      jumpTimer.Reset();
      Trace.Script($"Jumping {jumping}");
      //gameObject.Dispatch<JumpEvent>(Event.Cache<JumpEvent>());
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Locomotion
    //--------------------------------------------------------------------------------------------/
    protected virtual void MoveWithVelocity(Vector3 dir)
    {
      rigidbody.velocity = CalculateVelocity(velocity, dir, ComputeInterpolant(velocityRatio));
    }

    protected virtual void MoveWithForce(Vector3 dir)
    {
      Vector3 newVelocity = CalculateVelocity(velocity, dir, ComputeInterpolant(velocityRatio));
      rigidbody.AddForce(newVelocity * Time.deltaTime, ForceMode.Impulse);
    }

    protected virtual void MoveWithCharacterController(Vector3 dir)
    {
      float t = ComputeInterpolant(accelerationRatio);
      Vector3 newVelocity = CalculateVelocity(characterController.velocity, dir, t);
      newVelocity.y += Physics.gravity.y * Time.deltaTime;
      characterController.Move(newVelocity * Time.deltaTime);
    }

    protected virtual void MoveWithNavMeshAgent(Vector3 dir)
    {
      float t = ComputeInterpolant(accelerationRatio);
      Vector3 newVelocity = CalculateVelocity(velocity, dir, t);
      navigation.Move(newVelocity * Time.deltaTime);
    }

    protected virtual void JumpWithVelocity()
    {
      Vector3 jumpVelocity = Vector3.up * (speed + -Physics.gravity.y);
      Trace.Script($"velocity = {velocity}, jump velocity = {jumpVelocity}");
      //rigidbody.velocity += jumpVelocity;
      rigidbody.velocity = velocity + jumpVelocity;
      //rigidbody.AddForce(jumpVelocity, ForceMode.Force);
    }

    protected virtual void JumpWithCharacterController()
    {
      characterController.Move(Vector3.up * speed);
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Utility
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Calculates what the current velocity of the character should be, based on acceleration
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector3 CalculateVelocity(Vector3 currentVelocity, Vector3 direction, float t)
    {
      Vector3 newVelocity = Vector3.Lerp(currentVelocity, direction * maximumSpeed, t);
      //Trace.Script($"t = {t}, newVelocity = {newVelocity}");
      return newVelocity;
    }

    /// <summary>
    /// Computes the intertpolant value 't', to be used in a lerp
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    private float ComputeInterpolant(float ratio, float min = 0.1f)
    {
      return accelerationCurve.Evaluate(Mathf.Max(ratio, min));
    }

    /// <summary>
    /// Rotates this transform to face the target
    /// </summary>
    /// <param name="target"></param>
    protected virtual void RotateToTarget(Vector3 target)
    {
      Quaternion rot = Quaternion.LookRotation(target - transform.position);
      var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
      //targetRotation = Quaternion.Euler(newPos);
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Checks the current state of character movement, updating several flags
    /// </summary>
    private void CheckMovement()
    {
      if (moving)
      {
        accelerationTimer.Update(Time.deltaTime);
      }

 
      if (inertiaTimer.isFinished)
      {
        // Override movement
        if (movingTo)
        {
          moving = movingTo = navigation.hasPath;
        }
        // Moving along a direction
        else if (moving)
        {
          switch (locomotion)
          {
            case LocomotionMode.Velocity:
            case LocomotionMode.Force:
            case LocomotionMode.CharacterController:
            case LocomotionMode.NavMeshAgent:
              moving = IsMovingWithPosition();
              lastPosition = transform.position;
              break;
          }

          // If no longer moving, reset the acceleration state
          if (!moving)
          {
            accelerationTimer.Reset();
          }
        }

        // Check for jumping state

      }


    }

    /// <summary>
    /// Checks whether the character is currently grounded
    /// </summary>
    private void CheckGrounded()
    {
      if (groundDetection == GroundDetection.Collision || grounded)
        return;

      jumpTimer.Update(Time.deltaTime);
      if (!jumpTimer.isFinished)
        return;

      groundCastTimer.Update(Time.deltaTime);
      if (!groundCastTimer.isFinished)
        return;

      switch (groundDetection)
      {
        case GroundDetection.Raycast:
          grounded = IsGroundedRaycast();
          break;
        case GroundDetection.CheckSphere:
          grounded = IsGroundedSphereCast();
          break;
      }
      groundCastTimer.Reset();

      Trace.Script($"Grounded = {grounded}");

    }


    /// <summary>
    /// Determines whether a velocity-driven character is currently moving
    /// </summary>
    /// <returns></returns>
    private bool IsMovingWithVelocity()
    {
      return Math.Abs(rigidbody.velocity.x) > movementThreshold || Math.Abs(rigidbody.velocity.z) > movementThreshold;
    }

    /// <summary>
    /// Determines whether translation-driven character is currently moving
    /// </summary>
    /// <returns></returns>
    private bool IsMovingWithPosition()
    {
      float distance = Vector3.Distance(lastPosition, transform.position);
      bool isMoving = distance >= movementThreshold;
      return isMoving;
    }

    /// <summary>
    /// Determines whether the velocity-driven character is currently jumping
    /// </summary>
    /// <returns></returns>
    private bool IsGroundedRaycast()
    {
      //Physics.RaycastNonAlloc(groundCastRay, groundCast, 0.1f, groundLayer);
      return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
    }

    private bool IsGroundedSphereCast()
    {
      Vector3 position = transform.position + sphereOffset;
      Trace.Script($"CheckSphere({position}, {groundCollider.radius}, {groundLayer.value})");
      return Physics.CheckSphere(position, groundCollider.radius, groundLayer);
    }

    /// <summary>
    /// Determines whether the translation-driven character is currently jumping
    /// </summary>
    /// <returns></returns>
    private bool IsGroundedPosition()
    {
      return !characterController.isGrounded;
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Setup
    //--------------------------------------------------------------------------------------------/
    private void SetGroundDetection()
    {
      if (groundDetection != GroundDetection.Collision)
        groundCastTimer = new Countdown(groundCastFrequency);

      switch (groundDetection)
      {
        case GroundDetection.Raycast:
          break;

        case GroundDetection.CheckSphere:
          sphereOffset = Vector3.up * (groundCollider.radius * 0.9f);
          break;

        case GroundDetection.Collision:
          if (groundCollider.isTrigger)
            collisionProxy = CollisionProxy.Construct(groundCollider, CollisionProxy.CollisionMessage.TriggerEnter, OnCollider);
          else
            collisionProxy = CollisionProxy.Construct(groundCollider, CollisionProxy.CollisionMessage.CollisionEnter, OnCollision);
          break;
      }
    }

    private void OnCollision(Collision collision)
    {
      grounded = collision.gameObject.layer == groundLayer;
      Trace.Script($"Grounded = {grounded}");
    }

    private void OnCollider(Collider collider)
    {
      grounded = collider.gameObject.layer == groundLayer;
      Trace.Script($"Grounded = {grounded}");
    }

  }

}