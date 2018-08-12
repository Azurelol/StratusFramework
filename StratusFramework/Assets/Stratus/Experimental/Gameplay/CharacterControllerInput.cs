using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

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
    public InputField horizontal = new InputField();
    public InputField vertical = new InputField();
    public InputField sprint = new InputField();
    public InputField jump = new InputField();
    // Mouse    
    public MouseMovement mouseMovement = MouseMovement.Direction;
    public InputField moveButton = new InputField(InputField.MouseButton.Right);

    [Header("Custom")]
    public List<InputAction> additional = new List<InputAction>();

    [Header("Camera")]
    public InputField changeCamera = new InputField(KeyCode.C);
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
    public Vector2 axis => new Vector2(horizontal.value, vertical.value);
    public Preset currentPreset { get; private set; }
    public bool hasCameras => presets.NotEmpty();
    private Transform targetTransform { get; set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;

    }

    protected internal override void OnBehaviourAwake()
    {
      this.OnTargetChanged();
      cameraTransform = camera.transform;
      cameraNavigation = new ArrayNavigator<Preset>(presets.ToArray(), true);
      cameraNavigation.onIndexChanged = ChangeCamera;
      presetsMap.AddRange(presets, (Preset preset) => preset.label);
      ChangeCamera(presets[0]);
    }

    public void OnExtensibleStart()
    {
    }

    private void Reset()
    {
      this.camera = Camera.main;
      this.target = GetComponent<CharacterControllerMovement>();
    }

    protected internal override void OnUpdate()
    {
      PollInput();
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
      Preset preset = presetsMap.TryGetValue(label);
      ChangeCamera(preset);
    }

    public void NextCamera()
    {
      cameraNavigation.Navigate(ArrayNavigatorBase.Direction.Right);
    }

    //--------------------------------------------------------------------------------------------/
    // Procedures
    //--------------------------------------------------------------------------------------------/
    private void PollInput()
    {
      switch (mode)
      {
        case InputMode.Controller:
          PollController();
          break;
        case InputMode.Mouse:
          PollMouse();
          break;
      }

      foreach (var input in additional)
        input.Update();

      if (changeCamera.isDown)
        NextCamera();
    }

    private void PollController()
    {
      if (jump.isDown && !target.jumping)
        target.gameObject.Dispatch<CharacterMovement.JumpEvent>(jumpEvent);

      if (!horizontal.isNeutral || !vertical.isNeutral)
      {
        moveEvent.sprint = sprint.isPressed;
        moveEvent.turn = currentPreset.turn;
        moveEvent.direction = CalculateDirection(axis, currentPreset);
        target.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
      }

      if (currentPreset.synchronizeForward)
      {
        Vector3 newForward = cameraTransform.forward.Strip(VectorAxis.y);
        if (newForward != Vector3.zero)
          transform.forward = newForward;
      }
    }

    private void PollMouse()
    {
      switch (mouseMovement)
      {
        case MouseMovement.Direction:
          if (moveButton.isPressed)
          {
            moveEvent.sprint = sprint.isPressed;
            moveEvent.direction = CalculateMouseDirection(camera);
            target.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
          }
          break;

        case MouseMovement.Position:
          if (moveButton.isDown)
          {
            moveToEvent.position = CalculateMousePosition(camera);
            target.gameObject.Dispatch<CharacterMovement.MoveToEvent>(moveToEvent);
          }
          break;
      }
    }

    private void OnTargetChanged()
    {
      this.targetTransform = this.target != null ? this.target.transform : null;
      foreach(var preset in this.presets)
      {
        preset.camera.Follow = this.targetTransform;
        bool setLookAt = true;

        CinemachineVirtualCamera asVirtualCamera = (preset.camera as CinemachineVirtualCamera);
        if (asVirtualCamera != null)
        {
          bool hasTransposer = asVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>() != null;
          if (hasTransposer)
            setLookAt = false;
        }

        if (setLookAt)
          preset.camera.LookAt = this.targetTransform;
      }
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Utility
    //--------------------------------------------------------------------------------------------/
    protected Vector3 CalculateDirection(Vector2 axis, Preset preset)
    {
      Vector3 dir = Vector3.zero;

      dir = (axis.x * GetMovementOffset(preset.horizontalOffset)) +
            (axis.y * GetMovementOffset(preset.verticalOffset));
      dir.y = 0f;

      return dir.normalized;
    }

    protected Vector3 GetMovementOffset(MovementOffset offset)
    {      
      switch (offset)
      {
        case MovementOffset.PlayerForward:
          return targetTransform.forward;

        case MovementOffset.CameraForward:
          return cameraTransform.forward;

        case MovementOffset.CameraUp:
          return cameraTransform.up;

        case MovementOffset.CameraRight:
          return cameraTransform.right;

        case MovementOffset.None:
          return Vector3.zero;
      }
      throw new NotImplementedException();
    }

    protected Vector3 CalculateMouseDirection(Camera camera)
    {
      Vector3 mousePosition = CalculateMousePosition(camera);
      Vector3 direction = mousePosition - targetTransform.position;
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

      cameraNavigation.previous.camera.Priority = 10;
      preset.camera.Priority = 15;
      Cursor.lockState = preset.cursorLock;

      currentPreset = preset;
    }

    public static Vector3 CalculateMousePosition(Camera camera)
    {
      return camera.MouseCastGetPosition();
    }






  }

}