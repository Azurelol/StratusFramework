using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace Stratus.Gameplay
{
  [CustomExtensionAttribute(typeof(StratusCharacterController))]
  public class CinemachineCameraController : StratusBehaviour, IExtensionBehaviour<StratusCharacterController>
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    [Serializable]
    public class CameraPreset : StratusSerializable
    {
      public CinemachineVirtualCamera camera;
      public StratusCharacterController.MovementOffset offset;
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    public List<CameraPreset> cameras = new List<CameraPreset>();
    public InputField changeCamera = new InputField();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public CameraPreset currentCamera { get; private set; }
    public bool hasCameras => cameras.NotEmpty();
    public StratusCharacterController extensible { get; set; }

    private ArrayNavigator<CameraPreset> cameraNavigation;

    void IExtensionBehaviour.OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
      cameraNavigation = new ArrayNavigator<CameraPreset>(cameras.ToArray(), true);
      cameraNavigation.onIndexChanged = ChangeCamera;
      ChangeCamera(cameras[0]);
    }

    void IExtensionBehaviour.OnExtensibleStart()
    {
      
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
      currentCamera = preset;
      //extensible.movementOffset = preset.offset;
    }


  }
}