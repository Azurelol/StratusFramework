using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Stratus.Gameplay
{
  [DisallowMultipleComponent]
  public partial class CharacterMovement : ManagedBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
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
    public abstract class BaseMoveEvent : Stratus.StratusEvent
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
    public class JumpEvent : Stratus.StratusEvent { }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Tooltip("How locomotion for this character is handled")]
    public LocomotionMode locomotion = LocomotionMode.Velocity;

    [Header("Movement")]
    [Tooltip("The maximum speed at which the character will move")]
    public float movementSpeed = 3f;
    [Tooltip("The curve of determining how the character ramps up in speed")]
    public Ease accelerationCurve = Ease.Linear;
    [Tooltip("How long it takes the character to ramp up to full speed"), Range(0f, 1f)]
    public float accelerationRampUp = 1.0f;
    public float movementThreshold = 0.2f;
    public float sprintMuiltiplier = 2f;
    public float turningSpeed = 1f;
    [Range(45f, 360f)]
    [Tooltip("The maximum angle at which to prevent movement while turning")]
    public float turningThreshold = 135f;
    public bool faceDirection = true;

    [Header("Jumping")]
    public float jumpSpeed = 5f;
    [Tooltip("The curve of determining how the character ramps up in speed")]
    public Ease jumpCurve = Ease.Linear;
    public Ease fallCurve = Ease.QuadraticOut;
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
    private float nextSpeedSquared;
    private Vector3 _nextVelocity;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public new Rigidbody rigidbody { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }
    public CharacterController characterController { get; private set; }
    private StratusCollisionProxy collisionProxy { get; set; }

    public static Vector3 gravity => Physics.gravity;
    public bool moving { get; private set; }
    public bool turning { get; private set; }
    public bool sprinting { get; private set; }
    public bool jumping { get; private set; }
    public bool falling { get; private set; }
    public bool grounded { get; private set; } = true;
    private bool movingTo { get; set; }
    public bool receivedInput => !inertiaTimer.isFinished;
    /// <summary>
    /// Whether jump is enabled for this character
    /// </summary>
    public bool supportsJump => locomotion != LocomotionMode.NavMeshAgent;
    /// <summary>
    /// Whether waypoints are enabled for this character
    /// </summary>
    public bool supportsWaypoints => locomotion == LocomotionMode.NavMeshAgent;
    public bool hasGroundCast => groundDetection != GroundDetection.Collision;

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
    public Vector3 nextVelocity
    {
      get { return this._nextVelocity; }
      set
      {
        //Trace.Script($"Setting next velocity to {value}");
        this._nextVelocity = value;
      }
    }
    public float currentSpeed { get { Vector3 horizontalVelocity = velocity; horizontalVelocity.y = 0f; return horizontalVelocity.magnitude; } }
    public float maximumSpeed => sprinting ? movementSpeed * sprintMuiltiplier : movementSpeed;
    /// <summary>
    /// The speed at which the character is moving, as a ratio from 0 to 1
    /// </summary>
    public float speed => maximumSpeed / movementSpeed;
    private Vector3 lastPosition { get; set; } = Vector3.zero;
    public float velocityRatio => currentSpeed / maximumSpeed;
    public bool checkingMovement => inertiaTimer.isFinished;
    private float deltaTime => Time.fixedDeltaTime;
    private bool turnOverThreshold { get; set; }
    private bool applyMovement { get; set; }
    public float accelerationProgress => accelerationTimer.inverseNormalizedProgress;
    public float jumpProgress => jumpTimer.inverseNormalizedProgress;
    public float fallProgress => fallTimer.inverseNormalizedProgress;

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnManagedAwake()
    {
      inertiaTimer = new Countdown(0.05f);
      jumpTimer = new Countdown(jumpApex);
      fallTimer = new Countdown(jumpApex);
      accelerationTimer = new Countdown(accelerationRampUp);

      rigidbody = GetComponent<Rigidbody>();
      navMeshAgent = GetComponent<NavMeshAgent>();
      characterController = gameObject.GetComponent<CharacterController>();

      Subscribe();

      // Check grounded state on the first frame
      SetGroundDetection();
      CheckGrounded();
    }

    protected override void OnManagedUpdate()
    {
      UpdateTimers();
    }

    protected override void OnManagedFixedUpdate()
    {
      if (turning)
        ApplyTurn();

      if (applyMovement || jumping || falling)
        ApplyMovement();

      else if (!grounded)
        ApplyFall();
    }

    protected override void OnManagedLateUpdate()
    {      
      CheckMovement();

      if (!jumping || !grounded)
        CheckGrounded();
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
      if (this.supportsJump)
        gameObject.Connect<JumpEvent>(OnJumpEvent);
    }

    private void OnMoveEvent(MoveEvent e)
    {
      sprinting = e.sprint;
      heading = e.direction;
      //Trace.Script($"Heading = {heading}");

      // Don't move while jumping if there's no air control set
      if (jumping && !airControl)
      {
        //Trace.Script("No air control!", this);
        return;
      }

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
      _nextVelocity.y = CalculateVerticalSpeed();

      switch (locomotion)
      {
        case LocomotionMode.Velocity:
          rigidbody.velocity = nextVelocity;
          applyMovement = false;
          break;

        case LocomotionMode.Force:
          if (rigidbody.velocity.sqrMagnitude >= nextSpeedSquared)
          {
            //nextVelocity = Vector3.zero;
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
          applyMovement = false;
          break;

        case LocomotionMode.NavMeshAgent:
          navMeshAgent.Move(nextVelocity * deltaTime);
          applyMovement = false;
          break;
      }

      if (!jumping && !falling)
        nextVelocity = Vector3.zero;
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

      //jumpTimer.Update(Time.deltaTime);
      //if (!jumpTimer.isFinished)
      //  return;

      if (moving)
        accelerationTimer.Update(Time.deltaTime);

      if (jumping)
      {
        jumpTimer.Update(Time.deltaTime);
        if (jumpTimer.isFinished)
        {
          jumping = false;
          this.OnFall();
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

      StratusDebug.Log($"Started jump at velocity = {nextVelocity}", this);
      OnJump();
    }

    protected void OnJump()
    {
      grounded = false;
      jumping = true;
      falling = false;
      jumpTimer.Reset();
    }

    protected void OnFall()
    {
      falling = true;
      fallTimer.Reset();
    }

    protected float CalculateVerticalSpeed()
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
      else if (!grounded)
      {
        float t = ComputeInterpolant(fallCurve, fallProgress);
        switch (locomotion)
        {
          case LocomotionMode.Velocity:
          case LocomotionMode.Force:
            break;
          case LocomotionMode.CharacterController:
            verticalSpeed = fallCurve.Evaluate(t) * gravity.y;
            break;
          case LocomotionMode.NavMeshAgent:
            break;
        }
      }
      else
      {
        verticalSpeed = Physics.gravity.y * deltaTime;
      }
      return verticalSpeed;
    }

    //protected void ApplyJump()
    //{
    //
    //  float t = ComputeInterpolant(jumpCurve, jumpProgress);
    //  Vector3 newVelocity = velocity;
    //
    //  switch (locomotion)
    //  {
    //    case LocomotionMode.Velocity:
    //    case LocomotionMode.Force:
    //      newVelocity.y = jumpSpeed * jumpCurve.Evaluate(t);
    //      rigidbody.velocity = newVelocity;
    //      break;
    //    case LocomotionMode.CharacterController:
    //      newVelocity = Vector3.up * jumpSpeed * jumpCurve.Evaluate(t) * deltaTime;
    //      characterController.Move(newVelocity);
    //      break;
    //    case LocomotionMode.NavMeshAgent:
    //      break;
    //  }
    //  //Trace.Script($"Applying jump velocity ({t}) = {newVelocity}");
    //}

    protected void ApplyFall()
    {
      switch (locomotion)
      {
        case LocomotionMode.Velocity:
        case LocomotionMode.Force:
          break;
        case LocomotionMode.CharacterController:
          characterController.Move(Physics.gravity * Time.fixedDeltaTime);
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
    /// Computes the intertpolant value 't', to be used in a lerp
    /// </summary>
    /// <param name="progress"></param>
    /// <returns></returns>
    private float ComputeInterpolant(Ease curve, float progress, float min = 0.1f)
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
      //if (locomotion == LocomotionMode.NavMeshAgent)
      //  moving = navMeshAgent.velocity != Vector3.zero;

      if (inertiaTimer.isFinished)
      {
        // Override movement
        if (movingTo)
        {
          moving = movingTo = navMeshAgent.velocity != Vector3.zero;
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
      //if (groundDetection == GroundDetection.Collision || grounded)
      //  return;

      switch (this.locomotion)
      {
        case LocomotionMode.Velocity:
        case LocomotionMode.Force:
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
          break;

        case LocomotionMode.CharacterController:
          grounded = characterController.isGrounded;
          break;

        case LocomotionMode.NavMeshAgent:
          break;
      }
    }



  }

}