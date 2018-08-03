using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using System;

namespace Stratus.Gameplay
{
  [DisallowMultipleComponent]
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
    public float turningSpeed = 1f;
    [Range(0f, 360f)]
    [Tooltip("The maximum angle at which to prevent movement while turning")]
    public float turningThreshold = 180f;
    public bool faceDirection = true;

    [Header("Jumping")]
    public float jumpSpeed = 5f;
    [Tooltip("The curve of determining how the character ramps up in speed")]
    public AnimationCurve jumpCurve;
    public AnimationCurve fallCurve;
    [Range(0f, 1f), Tooltip("How long it takes to reach the top of the jump")]
    public float jumpApex = 0.5f;
    public GroundDetection groundDetection = GroundDetection.Raycast;
    public CapsuleCollider groundCollider;
    public LayerMask groundLayer = new LayerMask();
    [Range(0f, 1f)]
    public float groundCastFrequency = 0.1f;
    public bool airControl = true;

    private Vector3 sphereOffset;
    //private static Ray groundCastRay = new Ray();
    //private static RaycastHit[] groundCast = new RaycastHit[50];
    private Countdown inertiaTimer, accelerationTimer, groundCastTimer, jumpTimer, fallTimer;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public new Rigidbody rigidbody { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }
    public CharacterController characterController { get; private set; }
    private CollisionProxy collisionProxy { get; set; }

    public bool moving { get; private set; }
    public bool turning { get; private set; }
    public bool sprinting { get; private set; }
    public bool jumping { get; private set; }
    public bool falling { get; private set; }
    public bool grounded { get; private set; } = true;
    private bool movingTo { get; set; }
    public bool receivedInput => !inertiaTimer.isFinished;

    private Vector3 nextVelocity;
    private float nextSpeedSquared;
    public Vector3 heading { get; private set; }
    public Vector3 velocity
    {
      get
      {
        switch (locomotion)
        {
          case LocomotionMode.Velocity:
          case LocomotionMode.Force:
            return rigidbody.velocity;
          case LocomotionMode.CharacterController:
            return characterController.velocity;
          case LocomotionMode.NavMeshAgent:
            return navMeshAgent.velocity;
        }
        throw new NotImplementedException("Missing locomotion mode");
      }
    }
    public float currentSpeed { get { Vector3 horizontalVelocity = velocity; horizontalVelocity.y = 0f; return horizontalVelocity.magnitude; } }
    public float maximumSpeed => sprinting ? speed * sprintMuiltiplier : speed;
    public float speedRatio => maximumSpeed / speed;
    private Vector3 lastPosition { get; set; } = Vector3.zero;
    public float velocityRatio => currentSpeed / maximumSpeed;
    public bool checkingMovement => inertiaTimer.isFinished;
    private float deltaTime => Time.fixedDeltaTime;
    private bool turnOverThreshold { get; set; }
    private bool applyMovement { get; set; }
    public float accelerationProgress => accelerationTimer.inverseNormalizedProgress;
    private float jumpProgress => jumpTimer.inverseNormalizedProgress;
    private float fallProgress => fallTimer.inverseNormalizedProgress;


    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      inertiaTimer = new Countdown(0.05f);
      jumpTimer = new Countdown(jumpApex);
      fallTimer = new Countdown(jumpApex);
      accelerationTimer = new Countdown(accelerationRampUp);

      rigidbody = GetComponent<Rigidbody>();
      navMeshAgent = GetComponent<NavMeshAgent>();
      characterController = gameObject.GetComponent<CharacterController>();

      SetGroundDetection();
      Subscribe();
    }

    private void Update()
    {
      UpdateTimers();
    }

    private void FixedUpdate()
    {
      if (turning)
        ApplyTurn();

      if (applyMovement || jumping || falling)
        ApplyMovement();
      else if (!grounded)
        ApplyFall();

      //if (jumping)
      //  ApplyJump();
      //
      //if (falling)
      //  ApplyFall();
    }

    private void LateUpdate()
    {
      CheckMovement();

      if (!jumping)
        CheckGrounded();
    }

    private void Reset()
    {
      groundCollider = GetComponent<CapsuleCollider>();
    }

    private void OnValidate()
    {

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
      heading = e.direction;
      //Trace.Script($"Heading = {heading}");

      // Don't move while jumping if there's no air control set
      if (jumping && !airControl)
        return;

      if (e.turn)
        Turn(e.direction);

      if (!turnOverThreshold)
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
      // Compute the next velocity
      switch (locomotion)
      {
        case LocomotionMode.Velocity:
          nextVelocity = CalculateVelocity(velocity, direction, ComputeInterpolant(accelerationCurve, velocityRatio));
          break;

        case LocomotionMode.Force:
          nextVelocity = CalculateVelocity(velocity, direction, ComputeInterpolant(accelerationCurve, velocityRatio));
          break;

        case LocomotionMode.CharacterController:
          nextVelocity = CalculateVelocity(characterController.velocity, direction, ComputeInterpolant(accelerationCurve, accelerationProgress));
          //nextVelocity.y = characterController.velocity.y;
          break;

        case LocomotionMode.NavMeshAgent:
          nextVelocity = CalculateVelocity(velocity, direction, ComputeInterpolant(accelerationCurve, accelerationProgress));
          break;
      }

      // Begin movement          
      OnMove();
    }

    private void OnMove()
    {
      nextSpeedSquared = nextVelocity.sqrMagnitude;
      applyMovement = true;
      moving = true;
      inertiaTimer.Reset();
    }

    private void ApplyMovement()
    {
      float verticalSpeed = 0f;
      if (jumping)
      {
        float t = ComputeInterpolant(jumpCurve, jumpProgress);
        switch (locomotion)
        {
          case LocomotionMode.Velocity:
          case LocomotionMode.Force:
            verticalSpeed = jumpSpeed * jumpCurve.Evaluate(t);
            break;
          case LocomotionMode.CharacterController:
            verticalSpeed = jumpSpeed * jumpCurve.Evaluate(t);
            break;
          case LocomotionMode.NavMeshAgent:
            break;
        }
      }
      else if (falling)
      {
        float t = ComputeInterpolant(fallCurve, fallProgress);
        switch (locomotion)
        {
          case LocomotionMode.Velocity:
          case LocomotionMode.Force:
            break;
          case LocomotionMode.CharacterController:
            verticalSpeed = fallCurve.Evaluate(t) * -jumpSpeed;
            break;
          case LocomotionMode.NavMeshAgent:
            break;
        }
      }
      else
      {
        verticalSpeed = Physics.gravity.y * deltaTime;
      }

      //Trace.Script($"Vertical speed = {verticalSpeed}");
      nextVelocity.y = verticalSpeed;
      //Trace.Script($"Next velocity = {nextVelocity}");

      //if (!applyMovement)
      //  nextVelocity = Vector3.zero;
      
      switch (locomotion)
      {
        case LocomotionMode.Velocity:
          //nextVelocity.y = rigidbody.velocity.y;
          rigidbody.velocity = nextVelocity;
          nextVelocity = Vector3.zero;
          applyMovement = false;
          break;

        case LocomotionMode.Force:
          if (rigidbody.velocity.sqrMagnitude >= nextSpeedSquared)
          {
            nextVelocity = Vector3.zero;
            applyMovement = false;
          }
          else
          {
            Vector3 force = nextVelocity;
            rigidbody.AddForce(force, ForceMode.Impulse);
          }
          break;

        case LocomotionMode.CharacterController:
          CollisionFlags flags = characterController.Move(nextVelocity * deltaTime);
          if (!flags.HasFlag(CollisionFlags.CollidedBelow))
            grounded = false;

          nextVelocity = Vector3.zero;
          applyMovement = false;
          break;

        case LocomotionMode.NavMeshAgent:
          navMeshAgent.Move(nextVelocity * deltaTime);
          nextVelocity = Vector3.zero;
          applyMovement = false;
          break;
      }
    }

    protected void MoveTo(Vector3 position)
    {
      movingTo = true;
      navMeshAgent.SetDestination(position);
      OnMove();
    }

    protected void Turn(Vector3 direction)
    {
      // Decide whether this turn is over the threshold
      turnOverThreshold = IsTurningOverThreshold(heading);
      // Now do turn during fixed update
      turning = true;
    }

    private void ApplyTurn()
    {
      Vector3 turnVector = Vector3.Lerp(transform.forward, heading, deltaTime * turningSpeed);
      transform.forward = turnVector;
      turning = false;
    }

    private void UpdateTimers()
    {
      inertiaTimer.Update(Time.deltaTime);

      if (moving)
        accelerationTimer.Update(Time.deltaTime);

      if (jumping)
      {
        jumpTimer.Update(Time.deltaTime);
        if (jumpTimer.isFinished)
        {
          jumping = false;
          falling = true;
        }
      }

      if (falling)
      {
        fallTimer.Update(Time.deltaTime);
        if (fallTimer.isFinished)
        {
          falling = false;
          return;
        }
      }
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Jumping
    //--------------------------------------------------------------------------------------------/
    protected void Jump()
    {
      switch (locomotion)
      {
        case LocomotionMode.Velocity:
          break;

        case LocomotionMode.Force:
          break;

        case LocomotionMode.CharacterController:
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
      falling = false;
      jumpTimer.Reset();
      fallTimer.Reset();
    }

    protected void ApplyJump()
    {

      float t = ComputeInterpolant(jumpCurve, jumpProgress);
      Vector3 newVelocity = velocity;

      switch (locomotion)
      {
        case LocomotionMode.Velocity:
        case LocomotionMode.Force:
          newVelocity.y = jumpSpeed * jumpCurve.Evaluate(t);
          rigidbody.velocity = newVelocity;
          break;
        case LocomotionMode.CharacterController:
          newVelocity = Vector3.up * jumpSpeed * jumpCurve.Evaluate(t) * deltaTime;
          characterController.Move(newVelocity);
          break;
        case LocomotionMode.NavMeshAgent:
          break;
      }
      //Trace.Script($"Applying jump velocity ({t}) = {newVelocity}");
    }

    protected void ApplyFall()
    {
      switch (locomotion)
      {
        case LocomotionMode.Velocity:
        case LocomotionMode.Force:
          break;
        case LocomotionMode.CharacterController:
          characterController.Move(Physics.gravity * deltaTime);
          //float t = ComputeInterpolant(fallCurve, fallProgress);
          //characterController.Move(fallCurve.Evaluate(t) * );
          break;
        case LocomotionMode.NavMeshAgent:
          break;
      }

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
      //Trace.Script($"t = {t}, currentVelocity = {currentVelocity}, newVelocity = {newVelocity}");
      return newVelocity;
    }

    /// <summary>
    /// Computes the intertpolant value 't', to be used in a lerp
    /// </summary>
    /// <param name="progress"></param>
    /// <returns></returns>
    private float ComputeInterpolant(AnimationCurve curve, float progress, float min = 0.1f)
    {
      return curve.Evaluate(Mathf.Max(progress, min));
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
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), turningSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Checks the current state of character movement, updating several flags
    /// </summary>
    private void CheckMovement()
    {
      if (inertiaTimer.isFinished)
      {
        // Override movement
        if (movingTo)
        {
          moving = movingTo = navMeshAgent.hasPath;
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

      if (locomotion == LocomotionMode.CharacterController)
      {
        grounded = characterController.isGrounded;
      }
      else
      {
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
      }


      //Trace.Script($"Grounded = {grounded}");
    }

    /// <summary>
    /// Deteremins whether this character needs to do a turn over the turn threshold
    /// </summary>
    /// <returns></returns>
    private bool IsTurningOverThreshold(Vector3 direction)
    {
      // Don't move while the heading is greater than 180 degrees?
      Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
      float headingDifference = Mathf.Abs(rotation.eulerAngles.y - transform.eulerAngles.y);
      // If currently turning greater than threshold, don't move yet
      return (headingDifference >= turningThreshold);
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
      RaycastHit hitInfo;
      bool condition = Physics.SphereCast(
        groundCollider.transform.position + groundCollider.center + (Vector3.up * 0.1f),
        groundCollider.height / 2,
        Vector3.down,
        out hitInfo,
        0.1f
      );

      return condition;

      //Vector3 position = transform.position + sphereOffset;
      //Trace.Script($"CheckSphere({position}, {groundCollider.radius}, {groundLayer.value})");
      //return Physics.CheckSphere(position, groundCollider.radius, groundLayer);
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
    public void SetComponents()
    {
      switch (locomotion)
      {
        case CharacterMovement.LocomotionMode.Velocity:
        case CharacterMovement.LocomotionMode.Force:
          {
            gameObject.RemoveComponents(typeof(NavMeshAgent), typeof(CharacterController));
            Rigidbody rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            rigidbody.freezeRotation = true;
          }
          break;
        case CharacterMovement.LocomotionMode.CharacterController:
          {
            gameObject.RemoveComponents(typeof(Rigidbody), typeof(NavMeshAgent));
            this.characterController = gameObject.GetOrAddComponent<CharacterController>();
          }
          break;
        case CharacterMovement.LocomotionMode.NavMeshAgent:
          {
            gameObject.RemoveComponents(typeof(Rigidbody), typeof(CharacterController));
            this.navMeshAgent = gameObject.GetOrAddComponent<NavMeshAgent>();
          }
          break;
      }
    }

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

      switch (locomotion)
      {
        case LocomotionMode.Velocity:
        case LocomotionMode.Force:
          break;
        case LocomotionMode.CharacterController:
          characterController.Move(-Vector3.up);
          break;
        case LocomotionMode.NavMeshAgent:
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