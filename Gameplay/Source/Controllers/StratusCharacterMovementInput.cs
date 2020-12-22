using System;
using System.Collections.Generic;

using Cinemachine;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Gameplay
{
	public enum StratusMovementOffset
	{
		PlayerForward,
		CameraForward,
		CameraUp,
		CameraRight,
		None
	}

	public class StratusCharacterMovementInput : StratusBehaviour
	{
		//--------------------------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// A preset specifiying a control/camera scheme for this character
		/// </summary>
		[Serializable]
		public class Preset : StratusSerializable
		{
			/// <summary>
			/// A common template for character control (FPS, 3rd Person, etc)
			/// </summary>
			public enum Template
			{
				FirstPerson,
				ThirdPerson,
				TopDown,
				SideView
			}

			public string label;
			public StratusCinemachineCamera camera;
			public Transform followOverride;
			[Header("Input")]
			public StratusMovementOffset horizontalOffset = StratusMovementOffset.None;
			public StratusMovementOffset verticalOffset = StratusMovementOffset.None;
			[Header("Additional")]
			[Tooltip("Synchronizes the character's forward to face the camera")]
			public StratusCursorLock cursorLock = new StratusCursorLock();
			public bool turn = true;
			public bool synchronizeForward = false;

			public override string ToString()
			{
				return $"{label} ({camera})";
			}

			public static Preset FromTemplate(Template template)
			{
				Preset preset = new Preset();
				switch (template)
				{
					case Template.FirstPerson:
						preset.label = "First Person";
						preset.horizontalOffset = StratusMovementOffset.CameraRight;
						preset.verticalOffset = StratusMovementOffset.CameraForward;
						preset.synchronizeForward = true;
						preset.turn = false;
						preset.cursorLock = new StratusCursorLock(CursorLockMode.Locked, false);
						break;

					case Template.ThirdPerson:
						preset.label = "Third Person";
						preset.horizontalOffset = StratusMovementOffset.CameraRight;
						preset.verticalOffset = StratusMovementOffset.CameraForward;
						preset.turn = true;
						preset.cursorLock = new StratusCursorLock(CursorLockMode.Locked, false);
						break;

					case Template.TopDown:
						preset.label = "Top Down";
						preset.horizontalOffset = StratusMovementOffset.CameraRight;
						preset.verticalOffset = StratusMovementOffset.CameraUp;
						preset.turn = true;
						preset.cursorLock = new StratusCursorLock(CursorLockMode.Locked, false);
						break;

					case Template.SideView:
						preset.label = "Side View";
						preset.horizontalOffset = StratusMovementOffset.CameraRight;
						preset.turn = true;
						preset.cursorLock = new StratusCursorLock(CursorLockMode.Locked, false);
						break;
				}
				return preset;
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Inspector Fields
		//--------------------------------------------------------------------------------------------/
		public bool debug = false;
		[Tooltip("The character being controlled from this input")]
		public StratusCharacterMovement target;
		[Tooltip("The camera being used for input")]
		public new Camera camera;
		public List<Preset> presets = new List<Preset>();

		//--------------------------------------------------------------------------------------------/
		// Private Fields
		//--------------------------------------------------------------------------------------------/
		private StratusPersistentInputAction<Vector2> movePersistentInput;
		private StratusCharacterMovement.MoveEvent moveEvent = new StratusCharacterMovement.MoveEvent();
		private StratusCharacterMovement.MoveToEvent moveToEvent = new StratusCharacterMovement.MoveToEvent();
		private StratusCharacterMovement.JumpEvent jumpEvent = new StratusCharacterMovement.JumpEvent();
		private StratusArrayNavigator<Preset> cameraNavigation;
		private Transform cameraTransform;
		private Dictionary<string, Preset> presetsMap = new Dictionary<string, Preset>();

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public bool active { get; private set; }
		public StratusMovementOffset movementOffset { get; set; } = StratusMovementOffset.PlayerForward;
		public StratusCharacterController extensible { get; set; }
		public Vector2 moveAxis { get; private set; }
		public Preset currentPreset { get; private set; }
		public bool hasCameras => this.presets.NotEmpty();
		private Transform targetTransform { get; set; }
		public bool sprinting { get; private set; }

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		private void Awake()
		{
			movePersistentInput = new StratusPersistentInputAction<Vector2>(Move);
			this.cameraTransform = this.camera.transform;
			this.ConfigureCameraPresets();
			this.cameraNavigation = new StratusArrayNavigator<Preset>(this.presets.ToArray(), true);
			this.cameraNavigation.onIndexChanged += this.ChangeCamera;
			this.presetsMap.AddRange((Preset preset) => preset.label, this.presets);

			this.ChangeCamera(this.presets[0]);
		}

		private void Reset()
		{
			this.camera = Camera.main;
			this.target = this.GetComponent<StratusCharacterControllerMovement>();
		}

		private void OnEnable()
		{
			SetCameraLock();
		}

		private void OnDisable()
		{
			ReleaseCameraLock();
		}

		private void Update()
		{
			movePersistentInput.Update();
			if (active && currentPreset.synchronizeForward)
			{
				targetTransform.forward = currentPreset.camera.transform.forward;
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Input Methods
		//--------------------------------------------------------------------------------------------/
		public void ChangeTarget(StratusCharacterMovement target)
		{
			this.target = target;
			this.ConfigureCameraPresets();
		}

		public void NextCamera()
		{
			this.cameraNavigation.Navigate(StratusArrayNavigator.Direction.Right);
		}

		public void NextCamera(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				this.NextCamera();
			}
		}

		public void Move(StratusInputActionPhase phase, Vector2 dir)
		{
			if (debug)
			{
				this.Log($"{phase} {dir}");
			}
			movePersistentInput.Set(phase, dir);
		}

		public void Move(Vector2 inputDirection)
		{
			moveEvent.args.sprint = this.sprinting;
			moveEvent.turn = this.currentPreset.turn;
			moveEvent.args.direction = this.CalculateDirection(inputDirection, this.currentPreset);
			if (debug)
			{
				this.Log($"Input: {inputDirection}, Direction: {moveEvent.args.direction}");
			}
			target.Move(moveEvent.args);
		}

		public void Move(InputAction.CallbackContext context)
		{
			Vector2 axis = context.ReadValue<Vector2>();
			this.Move(axis);
		}

		public void Sprint(bool value)
		{
			this.sprinting = value;
			if (debug)
			{
				this.Log($"Sprinting: {value}");
			}
		}

		public void Sprint(StratusInputActionPhase phase)
		{
			switch (phase)
			{
				case StratusInputActionPhase.Started:
					Sprint(true);
					break;
				case StratusInputActionPhase.Performed:
					break;
				case StratusInputActionPhase.Canceled:
					Sprint(false);
					break;
			}
		}

		public void Sprint(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Disabled:
					break;
				case InputActionPhase.Waiting:
					break;
				case InputActionPhase.Started:
					this.Sprint(true);
					break;
				case InputActionPhase.Performed:
					break;
				case InputActionPhase.Canceled:
					this.Sprint(false);
					break;
			}
		}

		public void Look(Vector2 delta)
		{
			currentPreset.camera.LookRaw(delta);
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		public void ChangeCamera(Preset preset, int index)
		{
			this.ChangeCamera(preset);
		}

		//--------------------------------------------------------------------------------------------/
		// Procedures
		//--------------------------------------------------------------------------------------------/
		private void ConfigureCameraPresets()
		{
			this.targetTransform = this.target != null ? this.target.transform : null;
			for (int i = 0; i < this.presets.Count; i++)
			{
				Preset preset = this.presets[i];
				preset.camera.follow = preset.followOverride ? preset.followOverride : targetTransform;

				bool setLookAt = true;
				CinemachineVirtualCamera asVirtualCamera = (preset.camera.virtualCameraBase as CinemachineVirtualCamera);
				if (asVirtualCamera != null)
				{
					bool hasTransposer = asVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>() != null;
					bool hasPOV = asVirtualCamera.GetCinemachineComponent<CinemachinePOV>() != null;
					if (hasTransposer || hasPOV)
					{
						setLookAt = false;
					}
				}

				if (setLookAt)
				{
					preset.camera.lookAt = this.targetTransform;
				}
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Methods: Utility
		//--------------------------------------------------------------------------------------------/
		protected Vector3 CalculateDirection(Vector2 axis, Preset preset)
		{
			Vector3 dir = (axis.x * this.GetMovementOffset(preset.horizontalOffset)) +
				  (axis.y * this.GetMovementOffset(preset.verticalOffset));
			dir.y = 0f;

			return dir.normalized;
		}

		protected Vector3 GetMovementOffset(StratusMovementOffset offset)
		{
			switch (offset)
			{
				case StratusMovementOffset.PlayerForward:
					return this.targetTransform.forward;

				case StratusMovementOffset.CameraForward:
					return this.cameraTransform.forward;

				case StratusMovementOffset.CameraUp:
					return this.cameraTransform.up;

				case StratusMovementOffset.CameraRight:
					return this.cameraTransform.right;

				case StratusMovementOffset.None:
					return Vector3.zero;
			}
			throw new NotImplementedException();
		}

		protected Vector3 CalculateMouseDirection(Camera camera)
		{
			Vector3 mousePosition = CalculateMousePosition(camera);
			Vector3 direction = mousePosition - this.targetTransform.position;
			direction.y = 0;
			return direction.normalized;
		}

		//--------------------------------------------------------------------------------------------/
		// Methods: Utility
		//--------------------------------------------------------------------------------------------/
		private void ChangeCamera(Preset preset)
		{
			this.cameraNavigation.previous.camera.priority = 10;
			preset.camera.priority = 15;
			this.currentPreset = preset;
			SetCameraLock();
			this.Log($"Camera set to {preset}");
			active = true;
		}

		public void SetCameraLock()
		{
			currentPreset?.cursorLock?.Enable();
		}

		public void ReleaseCameraLock()
		{
			StratusCursorLock.ReleaseLock();
		}

		public static Vector3 CalculateMousePosition(Camera camera)
		{
			return camera.MouseCastGetPosition();
		}

	}

}