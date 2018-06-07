using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

namespace Stratus.Gameplay
{
  [RequireComponent(typeof(CharacterControllerMovement))]
  [CustomExtension(typeof(StratusCharacterController))]
  public class CharacterControllerInput : StratusBehaviour, IExtensionBehaviour<StratusCharacterController>
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public enum MovementOffset
    {
      PlayerForward,
      CameraForward,
      CameraUp
    }

    public enum CameraMode
    {
      Free,
      FreeStrafe,

      TopDown,
      SideView
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

    [Serializable]
    public class CameraPreset : StratusSerializable
    {
      public CinemachineVirtualCameraBase camera;
      public CameraMode mode;
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    public new Camera camera;
    public InputMode mode = InputMode.Controller;

    [Header("Default")]    
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
    public InputField cameraHorizontal = new InputField();
    public InputField cameraVertical = new InputField();
    public InputField changeCamera = new InputField();
    public List<CameraPreset> cameras = new List<CameraPreset>();

    private static CharacterMovement.MoveEvent moveEvent = new CharacterMovement.MoveEvent();
    private static CharacterMovement.MoveToEvent moveToEvent = new CharacterMovement.MoveToEvent();
    private static CharacterMovement.JumpEvent jumpEvent = new CharacterMovement.JumpEvent();
    private ArrayNavigator<CameraPreset> cameraNavigation;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public MovementOffset movementOffset { get; set; } = MovementOffset.PlayerForward;
    public StratusCharacterController extensible { get; set; }
    public CharacterControllerMovement movement { get; set; }
    public Vector2 axis => new Vector2(horizontal.value, vertical.value);
    public CameraPreset cameraPreset { get; private set; }
    public bool hasCameras => cameras.NotEmpty();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
      movement = GetComponent<CharacterControllerMovement>();
      cameraNavigation = new ArrayNavigator<CameraPreset>(cameras.ToArray(), true);
      cameraNavigation.onIndexChanged = ChangeCamera;
      ChangeCamera(cameras[0]);
    }

    public void OnExtensibleStart()
    {
    }

    private void Update()
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
        moveEvent.direction = CalculateDirection(axis, camera, cameraPreset);
        movement.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
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

    //public Vector3 CalculateDirection(Vector2 axis, MovementOffset offset)
    //{
    //  Vector3 dir = Vector3.zero;
    //  switch (offset)
    //  {
    //    case MovementOffset.PlayerForward:
    //      dir.x = axis.x;
    //      dir.z = axis.y;
    //      break;
    //
    //    case MovementOffset.CameraUp:
    //      dir = (axis.y * camera.transform.up) + (axis.x * camera.transform.right);
    //      dir.y = 0f;
    //      break;
    //
    //    case MovementOffset.CameraForward:
    //      dir = (axis.y * camera.transform.forward) + (axis.x * camera.transform.right);
    //      dir.y = 0f;
    //      break;
    //  }
    //  return dir;
    //}

    public static Vector3 CalculateDirection(Vector2 axis, Camera camera, CameraPreset preset)
    {
      Vector3 dir = Vector3.zero;
      switch (preset.mode)
      {
        case CameraMode.Free:
          dir = (axis.y * camera.transform.forward) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;

        case CameraMode.FreeStrafe:
          break;

        case CameraMode.TopDown:
          dir = (axis.y * camera.transform.up) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;

        case CameraMode.SideView:
          dir.x = axis.x;
          break;
      }
      return dir.normalized;
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

    private void ChangeCamera(CameraPreset preset)
    {
      if (extensible.debug)
        Trace.Script($"Switching to {preset.camera.name}");

      cameraNavigation.previous.camera.Priority = 10;
      preset.camera.Priority = 15;
      cameraPreset = preset;
    }


  }

}