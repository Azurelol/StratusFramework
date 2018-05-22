using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace Stratus.Experimental
{
  [CustomExtension(typeof(StratusPlayerController))]
  public class CinemachineCameraController : ExtensionBehaviour<StratusPlayerController>
  {
    [Serializable]
    public class CameraPreset : StratusSerializable
    {
      public CinemachineVirtualCamera camera;
      public StratusPlayerController.MovementOffset offset;
    }
    
    public List<CameraPreset> cameras = new List<CameraPreset>();
    public InputField changeCamera = new InputField();

    public CameraPreset currentCamera { get; private set; }
    public bool hasCameras => cameras.NotEmpty();
    private ArrayNavigator<CameraPreset> cameraNavigation;

    protected override void OnExtensibleAwake()
    {
      cameraNavigation = new ArrayNavigator<CameraPreset>(cameras.ToArray(), true);
      cameraNavigation.onIndexChanged = ChangeCamera;
      ChangeCamera(cameras[0]);
    }

    protected override void OnExtensibleStart()
    {
      
    }

    private void Update()
    {
      if (changeCamera.isDown)
        cameraNavigation.Navigate(ArrayNavigatorBase.Direction.Right);
    }

    private void ChangeCamera(CameraPreset preset)
    {
      if (extensible.debug)
        Trace.Script($"Switching to {preset.camera.name}");

      cameraNavigation.previous.camera.Priority = 10;
      preset.camera.Priority = 15;
      currentCamera = preset;
      extensible.movementOffset = preset.offset;
    }

  }
}