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

    [Serializable]
    public class CameraPreset : StratusSerializable
    {
      public CinemachineVirtualCameraBase camera;
      //public VectorAxis horizontal;
      //public VectorAxis vertical;
      public CameraMode mode;
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    public new Camera camera;
    [Header("Movement")]
    public InputField horizontal = new InputField();
    public InputField vertical = new InputField();
    public InputField sprint = new InputField();
    public InputField jump = new InputField();
    [Header("Camera")]
    public InputField cameraHorizontal = new InputField();
    public InputField cameraVertical = new InputField();
    public InputField changeCamera = new InputField();    
    public List<CameraPreset> cameras = new List<CameraPreset>();
    
    private static CharacterMovement.MoveEvent moveEvent = new CharacterMovement.MoveEvent();
    private static CharacterMovement.JumpEvent jumpEvent = new CharacterMovement.JumpEvent();
    private ArrayNavigator<CameraPreset> cameraNavigation;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public MovementOffset movementOffset { get; set; } = MovementOffset.PlayerForward;
    public StratusCharacterController extensible { get; set; }
    public CharacterControllerMovement movement { get; set; }
    public Vector2 axis => new Vector2(horizontal.value, vertical.value);
    public CameraPreset currentPreset { get; private set; }
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
      if (jump.isDown && !movement.jumping) 
        movement.gameObject.Dispatch<CharacterMovement.JumpEvent>(jumpEvent);

      if (!horizontal.isNeutral || !vertical.isNeutral)
      {
        moveEvent.sprint = sprint.isPressed;
        moveEvent.direction = CalculateDirection(axis, currentPreset);
        //moveEvent.adjustFacing = 
        movement.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
      }

      if (changeCamera.isDown)
        NextCamera();
    }

    public Vector3 CalculateDirection(Vector2 axis, MovementOffset offset)
    {
      Vector3 dir = Vector3.zero;
      switch (offset)
      {
        case MovementOffset.PlayerForward:
          dir.x = axis.x;
          dir.z = axis.y;
          break;

        case MovementOffset.CameraUp:
          dir = (axis.y * camera.transform.up) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;

        case MovementOffset.CameraForward:
          dir = (axis.y * camera.transform.forward) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;
      }
      return dir;
    }

    public Vector3 CalculateDirection(Vector2 axis, CameraPreset preset)
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
      return dir;
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
      currentPreset = preset;
    }


  }

}