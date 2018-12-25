using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Stratus.Gameplay
{
	[CustomExtensionAttribute(typeof(StratusCharacterController))]
	public class CharacterControllerInput : ManagedBehaviour, IExtensionBehaviour<StratusCharacterController>
	{
		//--------------------------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------------------------/
		public enum MovementOffset
		{
			PlayerForward,
			CameraForward,
			CameraUp,
			CameraRight,
			None
		}

		public enum InputMode
		{
			Controller,
			Mouse
		}

		public enum MouseMovement
		{
			Direction,
			Position
		}

		/// <summary>
		/// A preset specifiying a control scheme for this character
		/// </summary>
		[Serializable]
		public class Preset : StratusSerializable
		{
			public string label;
			public CinemachineVirtualCameraBase camera;
			[Header("Input")]
			//public Coordinates.Axis horizontal;
			public MovementOffset horizontalOffset = MovementOffset.None;
			//public Coordinates.Axis vertical;      
			public MovementOffset verticalOffset = MovementOffset.None;
			[Header("Additional")]
			[Tooltip("Synchronizes the character's forward to face the camera")]
			public CursorLockMode cursorLock = CursorLockMode.None;
			public bool turn = true;
			public bool synchronizeForward = false;

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

			public static Preset FromTemplate(Template template)
			{
				Preset preset = new Preset();
				switch (template)
				{
					case Template.FirstPerson:
						preset.label = "First Person";
						preset.horizontalOffset = MovementOffset.CameraRight;
						preset.verticalOffset = MovementOffset.CameraForward;
						preset.synchronizeForward = true;
						preset.turn = false;
						preset.cursorLock = CursorLockMode.Locked;
						break;

					case Template.ThirdPerson:
						preset.label = "Third Person";
						preset.horizontalOffset = MovementOffset.CameraRight;
						preset.verticalOffset = MovementOffset.CameraForward;
						preset.turn = true;
						preset.cursorLock = CursorLockMode.Locked;
						break;

					case Template.TopDown:
						preset.label = "Top Down";
						preset.horizontalOffset = MovementOffset.CameraRight;
						preset.verticalOffset = MovementOffset.CameraUp;
						preset.turn = true;
						preset.cursorLock = CursorLockMode.Locked;
						break;

					case Template.SideView:
						preset.label = "Side View";
						preset.horizontalOffset = MovementOffset.CameraRight;
						preset.turn = true;
						preset.cursorLock = CursorLockMode.Locked;
						break;
				}
				return preset;
			}

		}

		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		[Tooltip("The character being controlled from this input")]
		public CharacterControllerMovement target;
		[Tooltip("The camera being used for input")]
		public new Camera camera;
		public InputMode mode = InputMode.Controller;

		// Controller    
		public InputBinding horizontal = new InputBinding();
		public InputBinding vertical = new InputBinding();
		public InputBinding sprint = new InputBinding();
		public InputBinding jump = new InputBinding();
		// Mouse    
		public MouseMovement mouseMovement = MouseMovement.Direction;
		public InputBinding moveButton = new InputBinding(InputBinding.MouseButton.Right);

		[Header("Custom")]
		public List<InputAction> additional = new List<InputAction>();

		[Header("Camera")]
		public InputBinding changeCamera = new InputBinding(KeyCode.C);
		public List<Preset> presets = new List<Preset>();

		private static CharacterMovement.MoveEvent moveEvent = new CharacterMovement.MoveEvent();
		private static CharacterMovement.MoveToEvent moveToEvent = new CharacterMovement.MoveToEvent();
		private static CharacterMovement.JumpEvent jumpEvent = new CharacterMovement.JumpEvent();
		private ArrayNavigator<Preset> cameraNavigation;
		private Transform cameraTransform;
		private Dictionary<string, Preset> presetsMap = new Dictionary<string, Preset>();

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public MovementOffset movementOffset { get; set; } = MovementOffset.PlayerForward;
		public StratusCharacterController extensible { get; set; }
		//public CharacterControllerMovement movement { get; set; }
		public Vector2 axis => new Vector2(this.horizontal.value, this.vertical.value);
		public Preset currentPreset { get; private set; }
		public bool hasCameras => this.presets.NotEmpty();
		private Transform targetTransform { get; set; }

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		public void OnExtensibleAwake(ExtensibleBehaviour extensible)
		{
			this.extensible = (StratusCharacterController)extensible;

		}

		protected override void OnManagedAwake()
		{
			this.OnTargetChanged();
			this.cameraTransform = this.camera.transform;
			this.cameraNavigation = new ArrayNavigator<Preset>(this.presets.ToArray(), true)
			{
				onIndexChanged = this.ChangeCamera
			};
			this.presetsMap.AddRange(this.presets, (Preset preset) => preset.label);
			this.ChangeCamera(this.presets[0]);
		}

		public void OnExtensibleStart()
		{
		}

		private void Reset()
		{
			this.camera = Camera.main;
			this.target = this.GetComponent<CharacterControllerMovement>();
		}

		protected override void OnManagedUpdate()
		{
			this.PollInput();
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		public void ChangeTarget(CharacterControllerMovement target)
		{
			this.target = target;
			this.OnTargetChanged();
		}

		public void ChangeCamera(string label)
		{
			Preset preset = this.presetsMap.GetValueOrError(label);
			this.ChangeCamera(preset);
		}

		public void NextCamera()
		{
			this.cameraNavigation.Navigate(ArrayNavigatorBase.Direction.Right);
		}

		//--------------------------------------------------------------------------------------------/
		// Procedures
		//--------------------------------------------------------------------------------------------/
		private void PollInput()
		{
			switch (this.mode)
			{
				case InputMode.Controller:
					this.PollController();
					break;
				case InputMode.Mouse:
					this.PollMouse();
					break;
			}

			foreach (InputAction input in this.additional)
			{
				input.Update();
			}

			if (this.changeCamera.isDown)
			{
				this.NextCamera();
			}
		}

		private void PollController()
		{
			if (this.jump.isDown && !this.target.jumping)
			{
				this.target.gameObject.Dispatch<CharacterMovement.JumpEvent>(jumpEvent);
			}

			if (!this.horizontal.isNeutral || !this.vertical.isNeutral)
			{
				moveEvent.sprint = this.sprint.isPressed;
				moveEvent.turn = this.currentPreset.turn;
				moveEvent.direction = this.CalculateDirection(this.axis, this.currentPreset);
				this.target.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
			}

			if (this.currentPreset.synchronizeForward)
			{
				Vector3 newForward = this.cameraTransform.forward.XZ();
				if (newForward != Vector3.zero)
				{
					this.transform.forward = newForward;
				}
			}
		}

		private void PollMouse()
		{
			switch (this.mouseMovement)
			{
				case MouseMovement.Direction:
					if (this.moveButton.isPressed)
					{
						moveEvent.sprint = this.sprint.isPressed;
						moveEvent.direction = this.CalculateMouseDirection(this.camera);
						this.target.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
					}
					break;

				case MouseMovement.Position:
					if (this.moveButton.isDown)
					{
						moveToEvent.position = CalculateMousePosition(this.camera);
						this.target.gameObject.Dispatch<CharacterMovement.MoveToEvent>(moveToEvent);
					}
					break;
			}
		}

		private void OnTargetChanged()
		{
			this.targetTransform = this.target != null ? this.target.transform : null;
			foreach (Preset preset in this.presets)
			{
				preset.camera.Follow = this.targetTransform;
				bool setLookAt = true;

				CinemachineVirtualCamera asVirtualCamera = (preset.camera as CinemachineVirtualCamera);
				if (asVirtualCamera != null)
				{
					bool hasTransposer = asVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>() != null;
					if (hasTransposer)
					{
						setLookAt = false;
					}
				}

				if (setLookAt)
				{
					preset.camera.LookAt = this.targetTransform;
				}
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Methods: Utility
		//--------------------------------------------------------------------------------------------/
		protected Vector3 CalculateDirection(Vector2 axis, Preset preset)
		{
			Vector3 dir = Vector3.zero;

			dir = (axis.x * this.GetMovementOffset(preset.horizontalOffset)) +
				  (axis.y * this.GetMovementOffset(preset.verticalOffset));
			dir.y = 0f;

			return dir.normalized;
		}

		protected Vector3 GetMovementOffset(MovementOffset offset)
		{
			switch (offset)
			{
				case MovementOffset.PlayerForward:
					return this.targetTransform.forward;

				case MovementOffset.CameraForward:
					return this.cameraTransform.forward;

				case MovementOffset.CameraUp:
					return this.cameraTransform.up;

				case MovementOffset.CameraRight:
					return this.cameraTransform.right;

				case MovementOffset.None:
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
			//if (extensible.debug)
			//  Trace.Script($"Switching to {preset.camera.name}");

			this.cameraNavigation.previous.camera.Priority = 10;
			preset.camera.Priority = 15;
			Cursor.lockState = preset.cursorLock;

			this.currentPreset = preset;
		}

		public static Vector3 CalculateMousePosition(Camera camera)
		{
			return camera.MouseCastGetPosition();
		}






	}

}