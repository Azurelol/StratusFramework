using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Stratus.Gameplay.StratusCharacterMovement;

namespace Stratus.Gameplay
{
	[Serializable]
	public class StratusCharacterMovementJump : StratusCharacterMovement.Module
	{
		//--------------------------------------------------------------------------------------------/
		// Events
		//--------------------------------------------------------------------------------------------/
		public class JumpEvent : StratusEvent
		{
		}

		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		public float jumpSpeed = 5f;
		[Tooltip("The curve of determining how the character ramps up in speed")]
		public StratusEase jumpCurve = StratusEase.Linear;
		public StratusEase fallCurve = StratusEase.QuadraticOut;
		[Range(0f, 1f), Tooltip("How long it takes to reach the top of the jump")]
		public float jumpApex = 0.5f;
		public bool airControl = true;

		private StratusCountdown jumpTimer, fallTimer;

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public override StratusInputEventField[] inputs { get; }
		public bool jumping { get; private set; }

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		public override void OnModuleAwake()
		{
			this.jumpTimer = new StratusCountdown(this.jumpApex);
			this.fallTimer = new StratusCountdown(this.jumpApex);
			//characterMovement.gameObject.Connect<JumpEvent>(this.OnJumpEvent);
		}

		//--------------------------------------------------------------------------------------------/
		// Events
		//--------------------------------------------------------------------------------------------/
		private void OnJumpEvent(JumpEvent e)
		{
			if (characterMovement.grounded && !this.jumping)
			{
				this.Jump();
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Methods: Jumping
		//--------------------------------------------------------------------------------------------/
		protected void Jump()
		{
			switch (characterMovement.locomotion)
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

			//StratusDebug.Log($"Started jump at velocity = {characterMovement.nextVelocity}", this);
			//this.OnJump();
		}

		//protected void OnJump()
		//{
		//	characterMovement.grounded = false;
		//	this.jumping = true;
		//	characterMovement.falling = false;
		//	this.jumpTimer.Reset();
		//}

		//protected void OnFall()
		//{
		//	this.falling = true;
		//	this.fallTimer.Reset();
		//}

		//protected float CalculateVerticalSpeed()
		//{
		//	float verticalSpeed = 0f;
		//	if (this.jumping)
		//	{
		//		float t = this.ComputeInterpolant(this.jumpCurve, this.jumpProgress);
		//		switch (this.locomotion)
		//		{
		//			case LocomotionMode.Velocity:
		//			case LocomotionMode.Force:
		//				verticalSpeed = this.jumpSpeed * this.jumpCurve.Evaluate(t);
		//				break;
		//			case LocomotionMode.CharacterController:
		//				verticalSpeed = this.jumpSpeed * this.jumpCurve.Evaluate(t);
		//				break;
		//			case LocomotionMode.NavMeshAgent:
		//				break;
		//		}
		//	}
		//	else if (!this.grounded)
		//	{
		//		float t = this.ComputeInterpolant(this.fallCurve, this.fallProgress);
		//		switch (this.locomotion)
		//		{
		//			case LocomotionMode.Velocity:
		//			case LocomotionMode.Force:
		//				break;
		//			case LocomotionMode.CharacterController:
		//				verticalSpeed = this.fallCurve.Evaluate(t) * gravity.y;
		//				break;
		//			case LocomotionMode.NavMeshAgent:
		//				break;
		//		}
		//	}
		//	else
		//	{
		//		verticalSpeed = Physics.gravity.y * this.deltaTime;
		//	}
		//	return verticalSpeed;
		//}



	}

}