using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// A simple observer camera controller for navigating a scene at runtime
	/// </summary>
	public class StratusObserverCamera : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Header("Controls")]
		public Camera target;
		public StratusInputBinding movementHorizontal = new StratusInputBinding("Horizontal");
		public StratusInputBinding movementVertical = new StratusInputBinding("Vertical");
		public StratusInputBinding lookHorizontal = new StratusInputBinding("Mouse X");
		public StratusInputBinding lookVertical = new StratusInputBinding("Mouse Y");
		public StratusInputBinding turbo = new StratusInputBinding(KeyCode.LeftShift);
		public StratusInputBinding ascend = new StratusInputBinding(KeyCode.Space);
		public StratusInputBinding descend = new StratusInputBinding(KeyCode.LeftControl);
		public bool lockCursor = true;
		public StratusInputBinding lockCursorInput = new StratusInputBinding(StratusMouseButton.Left);
		public StratusInputBinding releaseCursorInput = new StratusInputBinding(KeyCode.Escape);
		[Tooltip("Warps to the next checkpoint in the scene, provided the Stratus Checkpoint component is present")]
		public StratusInputBinding skipToCheckpoint = new StratusInputBinding(KeyCode.Tab);

		[Header("Configuration")]
		[Range(0f, 300f)] public float sensitivity = 90f;
		[Range(0f, 20f)] public float ascendSpeed = 10f;
		[Range(0f, 20f)] public float moveSpeed = 10f;
		[Range(1f, 10f)] public float turboMultiplier = 3f;
		public Vector2 horizontalRotationLimit = Vector2.zero;
		public Vector2 verticalRotationLimit = new Vector2(-90, 90);

		private Vector2 cameraRotation;
		private StratusArrayNavigator<StratusPositionCheckpoint> checkpointNavigator;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		float speedMultiplier
		{
			get
			{
				if (this.turbo.isPressed)
					return this.turboMultiplier;
				return 1f;
			}
		}

		public bool limitHorizontalRotation => horizontalRotationLimit != Vector2.zero;
		public bool limitVerticalRotation => verticalRotationLimit != Vector2.zero;
		private bool cursorLocked => Cursor.lockState == CursorLockMode.Locked;
		public bool hasCheckpoints => this.checkpointNavigator.notEmpty;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Start()
		{
			this.checkpointNavigator = StratusPositionCheckpoint.GetNavigator();
			this.checkpointNavigator.onIndexChanged += this.CheckpointNavigator_onIndexChanged;
		}

		private void CheckpointNavigator_onIndexChanged(StratusPositionCheckpoint arg1, int arg2)
		{
			StratusPositionCheckpoint.WarpOnto(arg1, this.transform); 
		}

		private void Reset()
		{
			this.target = GetComponent<Camera>();
		}

		private void Update()
		{
			if (this.releaseCursorInput.isDown)
				Cursor.lockState = CursorLockMode.None;
			else if (this.lockCursorInput.isDown)
				Cursor.lockState = CursorLockMode.Locked;

			if (this.hasCheckpoints && this.skipToCheckpoint.isDown)
				this.checkpointNavigator.Next();
		}

		private void LateUpdate()
		{
			// Don't move unless the cursor is locked
			if (!this.cursorLocked)
				return;

			// Read rotation
			this.cameraRotation.x += lookHorizontal.value * this.sensitivity * Time.deltaTime;
			this.cameraRotation.y += lookVertical.value * this.sensitivity * Time.deltaTime;

			// Optionally Clamp
			if (this.limitHorizontalRotation)
				this.cameraRotation.x = Mathf.Clamp(this.cameraRotation.x, this.horizontalRotationLimit.x, this.horizontalRotationLimit.y);
			if (this.limitVerticalRotation)
				this.cameraRotation.y = Mathf.Clamp(this.cameraRotation.y, this.verticalRotationLimit.x, this.verticalRotationLimit.y);

			// Apply rotation
			this.transform.localRotation = Quaternion.AngleAxis(this.cameraRotation.x, Vector3.up) * Quaternion.AngleAxis(this.cameraRotation.y, Vector3.left);

			Vector3 newPosition = Vector3.zero;

			if (!movementHorizontal.isNeutral)
				newPosition += transform.right * moveSpeed * movementHorizontal.value * Time.deltaTime;
			if (!movementVertical.isNeutral)
				newPosition += transform.forward * moveSpeed * movementVertical.value * Time.deltaTime;
			if (this.ascend.isPressed)
				newPosition += transform.up * this.ascendSpeed * Time.deltaTime;
			if (this.descend.isPressed)
				newPosition -= transform.up * this.ascendSpeed * Time.deltaTime;

			transform.position += newPosition;

		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/

	}

}