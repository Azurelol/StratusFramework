using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Cinemachine;

namespace Stratus.Gameplay
{
	public enum StratusCharacterMovementAxis
	{
		XZ,
	}

	public struct StratusCharacterMovementArgs
	{
		public Vector3 direction;
		//public Camera camera;
		public bool turn;
		public bool sprint;

		public StratusCharacterMovementArgs(Vector3 direction) : this()
		{
			this.direction = direction;
			//this.camera = camera;
		}
	}

	[DisallowMultipleComponent]
	public partial class StratusCharacterMovement : StratusManagedBehaviour
	{
		//--------------------------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------------------------/
		[Serializable]
		public abstract class Module
		{
			public bool enabled;
			public abstract StratusInputEventField[] inputs { get; }
			public StratusCharacterMovement characterMovement { get; internal set; }
			public abstract void OnModuleAwake();

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

		/// <summary>
		/// Base class for all CharacterMovement events
		/// </summary>
		public abstract class BaseMoveEvent : StratusEvent
		{
			public StratusFloatOverride speedOverride;
			public bool turn;
		}

		/// <summary>
		/// Signals the character to move towards the given direction
		/// </summary>
		public class MoveEvent : BaseMoveEvent
		{
			public StratusCharacterMovementArgs args;
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
		[SerializeField]
		private bool debug = false;
		[Tooltip("How locomotion for this character is handled")]
		public LocomotionMode locomotion = LocomotionMode.Velocity;

		[Header("Movement")]
		[Tooltip("The maximum speed at which the character will move")]
		public float movementSpeed = 3f;
		[Tooltip("The curve of determining how the character ramps up in speed")]
		public StratusEase accelerationCurve = StratusEase.Linear;
		[Tooltip("How long it takes the character to ramp up to full speed"), Range(0f, 1f)]
		public float accelerationRampUp = 1.0f;
		public float movementThreshold = 0.2f;
		public float sprintMultiplier = 2f;
		public float turningSpeed = 1f;
		[Range(45f, 360f)]
		[Tooltip("The maximum angle at which to prevent movement while turning")]
		public float turningThreshold = 135f;
		public bool faceDirection = true;

		[Header("Ground Detection")]
		public GroundDetection groundDetection = GroundDetection.Raycast;
		public CapsuleCollider groundCollider;
		public LayerMask groundLayer = new LayerMask();
		[Range(0f, 1f)]
		public float groundCastFrequency = 0.1f;

		[Header("References")]
		public Transform headTransform;

		[Header("Modules")]
		public List<Module> modules = new List<Module>();

		private Vector3 sphereOffset;
		private StratusCountdown inertiaTimer, accelerationTimer, groundCastTimer, fallTimer;
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

		public bool falling { get; private set; }
		public bool grounded { get; private set; } = true;
		private bool movingTo { get; set; }
		public bool receivedInput => !this.inertiaTimer.isFinished;
		public bool supportsJump => this.locomotion != LocomotionMode.NavMeshAgent;
		public bool supportsWaypoints => this.locomotion == LocomotionMode.NavMeshAgent;
		public bool hasGroundCast => this.groundDetection != GroundDetection.Collision;
		/// <summary>
		/// The direction the character is moving towards
		/// </summary>
		public Vector3 direction { get; private set; }
		/// <summary>
		/// The character's current velocity
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				switch (this.locomotion)
				{
					case LocomotionMode.Velocity:
					case LocomotionMode.Force:
						return this.rigidbody.velocity;
					case LocomotionMode.CharacterController:
						return this.characterController.velocity;
					case LocomotionMode.NavMeshAgent:
						return this.navMeshAgent.velocity;
				}
				throw new NotImplementedException("Missing locomotion mode");
			}
		}
		/// <summary>
		/// The character's target velocity
		/// </summary>
		public Vector3 nextVelocity
		{
			get => this._nextVelocity;
			set => this._nextVelocity = value;
		}
		/// <summary>
		/// The character's current speed, the magnitude of velocity
		/// </summary>
		public float currentSpeed 
		{ 
			get 
			{ 
				Vector3 horizontalVelocity = this.velocity; 
				horizontalVelocity.y = 0f; 
				return horizontalVelocity.magnitude; 
			} 
		}
		/// <summary>
		/// The character's maximum speed
		/// </summary>
		public float maximumSpeed => this.sprinting ? this.movementSpeed * this.sprintMultiplier : this.movementSpeed;
		/// <summary>
		/// The speed at which the character is moving, as a ratio from 0 to 1
		/// </summary>
		public float speed => this.maximumSpeed / this.movementSpeed;

		private Vector3 lastPosition { get; set; } = Vector3.zero;
		public float velocityRatio => this.currentSpeed / this.maximumSpeed;
		public bool checkingMovement => this.inertiaTimer.isFinished;
		private float deltaTime => Time.fixedDeltaTime;
		private bool turnOverThreshold { get; set; }
		private bool applyMovement { get; set; }
		public float accelerationProgress => this.accelerationTimer.inverseNormalizedProgress;
		public float fallProgress => this.fallTimer.inverseNormalizedProgress;

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnManagedAwake()
		{
			this.inertiaTimer = new StratusCountdown(0.05f);

			this.accelerationTimer = new StratusCountdown(this.accelerationRampUp);

			this.rigidbody = this.GetComponent<Rigidbody>();
			this.navMeshAgent = this.GetComponent<NavMeshAgent>();
			this.characterController = this.gameObject.GetComponent<CharacterController>();

			this.Subscribe();

			foreach (var module in modules)
			{
				module.characterMovement = this;
				module.OnModuleAwake();
			}

			// Check grounded state on the first frame
			this.SetGroundDetection();
			this.CheckGrounded();
		}

		protected override void OnManagedUpdate()
		{
			this.UpdateTimers();
		}

		protected override void OnManagedFixedUpdate()
		{
			if (this.turning)
			{
				this.ApplyTurn();
			}
			if (this.applyMovement || this.falling)
			{
				this.ApplyMovement();
			}
			else if (!this.grounded)
			{
				this.ApplyFall();
			}
		}

		protected override void OnManagedLateUpdate()
		{
			this.CheckMovement();

			if (!this.grounded)
			{
				this.CheckGrounded();
			}
		}

		private void Reset()
		{
			this.groundCollider = this.GetComponent<CapsuleCollider>();
		}

		//--------------------------------------------------------------------------------------------/
		// Events
		//--------------------------------------------------------------------------------------------/
		private void Subscribe()
		{
			this.gameObject.Connect<MoveEvent>(this.OnMoveEvent);
			this.gameObject.Connect<MoveToEvent>(this.OnMoveToEvent);
		}

		private void OnMoveEvent(MoveEvent e)
		{
			this.Move(e.args);
		}

		private void OnMoveToEvent(MoveToEvent e)
		{
			this.MoveTo(e);
		}

		//--------------------------------------------------------------------------------------------/
		// Methods: Movement
		//--------------------------------------------------------------------------------------------/
		//public void Move(MoveEvent e)
		//{
		//	this.sprinting = e.args.sprint;
		//	Move(new StratusCharacterMovementArgs(e.direction, e.turn));
		//}

		public void Move(StratusCharacterMovementArgs args)
		{
			this.direction = args.direction; // GetDirectionFromInput(args.inputDirection);
			if (debug)
			{
				this.Log($"Applying movement for direction '{direction}'");
			}

			if (args.turn)
			{
				this.Turn(direction);
			}
			if (!this.turnOverThreshold)
			{
				this.Move(direction);
			}
		}

		protected void Move(Vector3 direction)
		{
			// Compute the next velocity
			switch (this.locomotion)
			{
				case LocomotionMode.Velocity:
					this.nextVelocity = this.CalculateVelocity(this.velocity, direction, this.ComputeInterpolant(this.accelerationCurve, this.velocityRatio));
					break;

				case LocomotionMode.Force:
					this.nextVelocity = this.CalculateVelocity(this.velocity, direction, this.ComputeInterpolant(this.accelerationCurve, this.velocityRatio));
					break;

				case LocomotionMode.CharacterController:
					this.nextVelocity = this.CalculateVelocity(this.characterController.velocity, direction, this.ComputeInterpolant(this.accelerationCurve, this.accelerationProgress));
					//nextVelocity.y = characterController.velocity.y;
					break;

				case LocomotionMode.NavMeshAgent:
					this.nextVelocity = this.CalculateVelocity(this.velocity, direction, this.ComputeInterpolant(this.accelerationCurve, this.accelerationProgress));
					break;
			}

			// Begin movement          
			this.OnMove();
		}

		private void OnMove()
		{
			this.nextSpeedSquared = this.nextVelocity.sqrMagnitude;
			this.applyMovement = true;
			this.moving = true;
			this.inertiaTimer.Reset();
		}

		private void ApplyMovement()
		{
			switch (this.locomotion)
			{
				case LocomotionMode.Velocity:
					this.rigidbody.velocity = this.nextVelocity;
					this.applyMovement = false;
					break;

				case LocomotionMode.Force:
					if (this.rigidbody.velocity.sqrMagnitude >= this.nextSpeedSquared)
					{
						//nextVelocity = Vector3.zero;
						this.applyMovement = false;
					}
					else
					{
						Vector3 force = this.nextVelocity;
						this.rigidbody.AddForce(force, ForceMode.Impulse);
					}
					break;

				case LocomotionMode.CharacterController:
					CollisionFlags flags = this.characterController.Move(this.nextVelocity * this.deltaTime);
					if (!flags.HasFlag(CollisionFlags.CollidedBelow))
					{
						this.grounded = false;
					}

					this.applyMovement = false;
					break;

				case LocomotionMode.NavMeshAgent:
					this.navMeshAgent.Move(this.nextVelocity * this.deltaTime);
					this.applyMovement = false;
					break;
			}

			if (!this.falling)
			{
				this.nextVelocity = Vector3.zero;
			}
		}

		public void MoveTo(MoveToEvent e)
		{
			this.MoveTo(e.position);
		}

		public void MoveTo(Vector3 position)
		{
			this.movingTo = true;
			this.navMeshAgent.SetDestination(position);
			this.OnMove();
		}

		protected void Turn(Vector3 direction)
		{
			// Decide whether this turn is over the threshold
			this.turnOverThreshold = this.IsTurningOverThreshold(this.direction);
			// Now do turn during fixed update
			this.turning = true;
		}

		private void ApplyTurn()
		{
			Vector3 turnVector = Vector3.Lerp(this.transform.forward, this.direction, this.deltaTime * this.turningSpeed);
			this.transform.forward = turnVector;
			this.turning = false;
		}

		private void UpdateTimers()
		{
			this.inertiaTimer.Update(Time.deltaTime);

			//jumpTimer.Update(Time.deltaTime);
			//if (!jumpTimer.isFinished)
			//  return;

			if (this.moving)
			{
				this.accelerationTimer.Update(Time.deltaTime);
			}

			//if (this.jumping)
			//{
			//	this.jumpTimer.Update(Time.deltaTime);
			//	if (this.jumpTimer.isFinished)
			//	{
			//		this.jumping = false;
			//		this.OnFall();
			//	}
			//}

			if (this.falling)
			{
				this.fallTimer.Update(Time.deltaTime);
				if (this.fallTimer.isFinished)
				{
					this.falling = false;
					return;
				}
			}
		}

		protected void ApplyFall()
		{
			switch (this.locomotion)
			{
				case LocomotionMode.Velocity:
				case LocomotionMode.Force:
					break;
				case LocomotionMode.CharacterController:
					this.characterController.Move(Physics.gravity * Time.fixedDeltaTime);
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
			Vector3 newVelocity = Vector3.Lerp(currentVelocity, direction * this.maximumSpeed, t);
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
		private float ComputeInterpolant(StratusEase curve, float progress, float min = 0.1f)
		{
			return curve.Evaluate(Mathf.Max(progress, min));
		}

		/// <summary>
		/// Rotates this transform to face the target
		/// </summary>
		/// <param name="target"></param>
		protected virtual void RotateToTarget(Vector3 target)
		{
			Quaternion rot = Quaternion.LookRotation(target - this.transform.position);
			Vector3 newPos = new Vector3(this.transform.eulerAngles.x, rot.eulerAngles.y, this.transform.eulerAngles.z);
			//targetRotation = Quaternion.Euler(newPos);
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(newPos), this.turningSpeed * Time.deltaTime);
		}

		/// <summary>
		/// Checks the current state of character movement, updating several flags
		/// </summary>
		private void CheckMovement()
		{
			//if (locomotion == LocomotionMode.NavMeshAgent)
			//  moving = navMeshAgent.velocity != Vector3.zero;

			if (this.inertiaTimer.isFinished)
			{
				// Override movement
				if (this.movingTo)
				{
					this.moving = this.movingTo = this.navMeshAgent.velocity != Vector3.zero;
				}
				// Moving along a direction
				else if (this.moving)
				{
					switch (this.locomotion)
					{
						case LocomotionMode.Velocity:
						case LocomotionMode.Force:
						case LocomotionMode.CharacterController:
						case LocomotionMode.NavMeshAgent:
							this.moving = this.IsMovingWithPosition();
							this.lastPosition = this.transform.position;
							break;
					}

					// If no longer moving, reset the acceleration state
					if (!this.moving)
					{
						this.accelerationTimer.Reset();
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
						this.groundCastTimer.Update(Time.deltaTime);
						if (!this.groundCastTimer.isFinished)
						{
							return;
						}

						switch (this.groundDetection)
						{
							case GroundDetection.Raycast:
								this.grounded = this.IsGroundedRaycast();
								break;
							case GroundDetection.CheckSphere:
								this.grounded = this.IsGroundedSphereCast();
								break;
						}
						this.groundCastTimer.Reset();
					}
					break;

				case LocomotionMode.CharacterController:
					this.grounded = this.characterController.isGrounded;
					break;

				case LocomotionMode.NavMeshAgent:
					break;
			}
		}



	}

}