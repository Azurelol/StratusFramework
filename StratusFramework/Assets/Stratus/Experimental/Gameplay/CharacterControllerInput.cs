using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

namespace Stratus.Gameplay
{
  [RequireComponent(typeof(CharacterControllerMovement))]
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
            //preset.horizontal = Coordinates.Axis.X;
            preset.horizontalOffset = MovementOffset.CameraRight;
            //preset.vertical = Coordinates.Axis.Z;
            preset.verticalOffset = MovementOffset.CameraForward;
            preset.synchronizeForward = true;
            preset.turn = false;
            preset.cursorLock = CursorLockMode.Locked;
            break;

          case Template.ThirdPerson:
            preset.label = "Third Person";
            //preset.horizontal = Coordinates.Axis.X;
            preset.horizontalOffset = MovementOffset.CameraRight;
            //preset.vertical = Coordinates.Axis.Z;
            preset.verticalOffset = MovementOffset.CameraForward;
            preset.turn = true;
            preset.cursorLock = CursorLockMode.Locked;
            break;

          case Template.TopDown:
            preset.label = "Top Down";
            //preset.horizontal = Coordinates.Axis.X;
            preset.horizontalOffset = MovementOffset.CameraRight;
            //preset.vertical = Coordinates.Axis.Z;
            preset.verticalOffset = MovementOffset.CameraUp;
            preset.turn = true;
            preset.cursorLock = CursorLockMode.Locked;
            break;

          case Template.SideView:
            preset.label = "Side View";
            //preset.horizontal = Coordinates.Axis.X;
            preset.horizontalOffset = MovementOffset.CameraRight;
            //preset.vertical = preset.vertical.UnsetAll();
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
    public CharacterControllerMovement movement { get; set; }
    public Vector2 axis => new Vector2(horizontal.value, vertical.value);
    public Preset currentPreset { get; private set; }
    public bool hasCameras => presets.NotEmpty();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
      cameraTransform = camera.transform;
      movement = GetComponent<CharacterControllerMovement>();
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
    }

    protected internal override void OnUpdate()
    {
      PollInput();
    }   

    //--------------------------------------------------------------------------------------------/
    // Methods
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
      if (jump.isDown && !movement.jumping)
        movement.gameObject.Dispatch<CharacterMovement.JumpEvent>(jumpEvent);

      if (!horizontal.isNeutral || !vertical.isNeutral)
      {
        moveEvent.sprint = sprint.isPressed;
        moveEvent.turn = currentPreset.turn;
        moveEvent.direction = CalculateDirection(axis, currentPreset);
        movement.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
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
            movement.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
          }
          break;

        case MouseMovement.Position:
          if (moveButton.isDown)
          {
            moveToEvent.position = CalculateMousePosition(camera);
            movement.gameObject.Dispatch<CharacterMovement.MoveToEvent>(moveToEvent);
          }
          break;
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
          return transform.forward;

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
      Vector3 direction = mousePosition - transform.position;
      direction.y = 0;
      return direction.normalized;
    }

    public static Vector3 CalculateMousePosition(Camera camera)
    {
      return camera.MouseCastGetPosition();
    }

    public void NextCamera()
    {
      cameraNavigation.Navigate(ArrayNavigatorBase.Direction.Right);
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Utility
    //--------------------------------------------------------------------------------------------/
    private void ChangeCamera(Preset preset)
    {
      if (extensible.debug)
        Trace.Script($"Switching to {preset.camera.name}");

      cameraNavigation.previous.camera.Priority = 10;
      preset.camera.Priority = 15;
      Cursor.lockState = preset.cursorLock;

      currentPreset = preset;
    }

    public void ChangeCamera(string label)
    {
      Preset preset = presetsMap.TryGetValue(label);
      ChangeCamera(preset);
    }


  }

}