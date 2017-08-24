using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace Stratus
{
  namespace Gameplay
  {
    /// <summary>
    /// An event-driven callback for a Cinemachine virtual camera
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class CinemachineCameraController : CameraControllerBase
    {
      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// The Cinemachine camera this controller is connected to
      /// </summary>
      private CinemachineVirtualCameraBase cinemachineCamera { get; set; }

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      protected override void OnAwake()
      {
        cinemachineCamera = this.gameObject.GetComponent<CinemachineVirtualCameraBase>();
      }
      
      protected override void OnUpdate()
      {
        throw new NotImplementedException();
      }

    } 

  }
}