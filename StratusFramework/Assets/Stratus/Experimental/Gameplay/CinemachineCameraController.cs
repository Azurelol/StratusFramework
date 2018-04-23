using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace Stratus.Experimental
{
  public class CinemachineCameraController : StratusPlayerControllerExtension
  {
    [Serializable]
    public class CameraPreset : SerializableClass
    {
      public CinemachineVirtualCamera camera;
      public StratusPlayerController.MovementOffset offset;
    }
    
    public List<CameraPreset> cameras = new List<CameraPreset>();
    public InputField changeCamera = new InputField();    

    private ArrayNavigator<CameraPreset> cameraNavigation;

    protected override void OnAwake()
    {
      cameraNavigation = new ArrayNavigator<CameraPreset>(cameras.ToArray(), true);
      cameraNavigation.onIndexChanged = ChangeCamera;
    }

    private void Update()
    {
      if (changeCamera.isDown)
        cameraNavigation.Navigate(ArrayNavigatorBase.Direction.Right);
    }

    private void ChangeCamera(CameraPreset preset)
    {
      Trace.Script($"Swwitching to {preset.camera.name}");
      cameraNavigation.previous.camera.Priority = 10;
      preset.camera.Priority = 15;
      playerController.movementOffset = preset.offset;
    }

  }
}