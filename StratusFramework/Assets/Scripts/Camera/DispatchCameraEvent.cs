/******************************************************************************/
/*!
@file   DispatchCameraEvent.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

/**************************************************************************/
/*!
@class DispatchCameraEvent Provides static methods to quickly construct
       events for the CamereController.
*/
/**************************************************************************/
static public class DispatchCameraEvent {
  
  static public void Follow(CameraController.Config config, CameraController.Transition transition)
  {
    var followEvent = new CameraController.FollowEvent();
    followEvent.Configuration.Copy(config, true);
    followEvent.Transition = transition;
    Stratus.Space.Dispatch<CameraController.FollowEvent>(followEvent);
  }

  static public void LookAt(GameObject target, CameraController.Transition transition)
  {
    var lookAtEvent = new CameraController.LookAtEvent();
    lookAtEvent.Target = target;
    lookAtEvent.Transition = transition;
    Stratus.Space.Dispatch<CameraController.LookAtEvent>(lookAtEvent);
  }

  static public void Track(GameObject target)
  {

  }

  static public void Zoom(GameObject target, Real amount, Real speed, Real duration)
  {

  }

  static public void Orbit(Real3 target, Real3 velocity, Real delay)
  {

  }

  static public void Rotate(Real3 rotation, Real duration)
  {

  }

}

