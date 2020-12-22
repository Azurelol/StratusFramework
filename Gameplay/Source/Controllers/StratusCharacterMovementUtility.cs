using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Stratus.Gameplay
{
	public partial class StratusCharacterMovement : StratusManagedBehaviour
	{
		///// <summary>
		///// Given an input direction vector, converts it into a direction
		///// </summary>
		///// <param name="inputDirection"></param>
		///// <returns></returns>
		//protected virtual Vector3 GetDirectionFromInput(Vector2 inputDirection)
		//{
		//	return new Vector3(inputDirection.x, 0f, inputDirection.y);
		//}

		/// <summary>
		/// Determines whether this character needs to do a turn over the turn threshold
		/// </summary>
		/// <returns></returns>
		private bool IsTurningOverThreshold(Vector3 direction)
		{
			if (direction == Vector3.zero)
			{
				return false;
			}

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
				case StratusCharacterMovement.LocomotionMode.Velocity:
				case StratusCharacterMovement.LocomotionMode.Force:
					{
						gameObject.RemoveComponents(typeof(NavMeshAgent), typeof(CharacterController));
						this.rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
						this.rigidbody.drag = 1f;
						this.rigidbody.freezeRotation = true;
					}
					break;
				case StratusCharacterMovement.LocomotionMode.CharacterController:
					{
						gameObject.RemoveComponents(typeof(Rigidbody), typeof(NavMeshAgent));
						this.characterController = gameObject.GetOrAddComponent<CharacterController>();
						this.characterController.center = new Vector3(0f, this.characterController.height / 2f, 0f);
					}
					break;
				case StratusCharacterMovement.LocomotionMode.NavMeshAgent:
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
				groundCastTimer = new StratusCountdown(groundCastFrequency);

			switch (groundDetection)
			{
				case GroundDetection.Raycast:
					break;

				case GroundDetection.CheckSphere:
					sphereOffset = Vector3.up * (groundCollider.radius * 0.9f);
					break;

				case GroundDetection.Collision:
					if (groundCollider.isTrigger)
						collisionProxy = StratusCollisionProxy.Construct(groundCollider, StratusCollisionProxy.CollisionMessage.TriggerEnter, OnCollider);
					else
						collisionProxy = StratusCollisionProxy.Construct(new StratusCollisionProxy.Arguments(groundCollider, StratusCollisionProxy.CollisionMessage.CollisionEnter, OnCollision));
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
			StratusDebug.Log($"Grounded = {grounded}");
		}

		private void OnCollider(Collider collider)
		{
			grounded = collider.gameObject.layer == groundLayer;
			StratusDebug.Log($"Grounded = {grounded}");
		}
	}

}