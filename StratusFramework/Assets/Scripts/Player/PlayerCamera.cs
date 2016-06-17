/******************************************************************************/
/*!
@file   PlayerCamera.cs
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
@class PlayerCamera 
*/
/**************************************************************************/
public class PlayerCamera : MonoBehaviour {

  public bool Debugging = true;
  public float Radius = 10.0f;
  public float Height = 15.0f;
  public float Angle = 90.0f;
  public float Damping = 1.0f;

  private float DebugRadius = 5.0f;

  /**************************************************************************/
  /*!
  @brief  Initializes the Script.
  */
  /**************************************************************************/
  void Start () {
    // Request the camera to follow
    var config = new CameraController.Config();
    config.Target = this.gameObject;
    config.Radius = this.Radius;
    config.Height = this.Height;
    config.Angle = this.Angle;
    config.Damping = this.Damping;
    var transition = new CameraController.Transition();
    transition.Duration = 0.5f;
    DispatchCameraEvent.Follow(config, transition);
  }

  void Update()
  {
    if (Debugging)
    {
      // Forward
      Vector3 forwardVec = this.transform.forward * DebugRadius;
      Debug.DrawRay(this.transform.position, forwardVec, Color.red);
      // Up
      Vector3 upVec = this.transform.up * DebugRadius;
      Debug.DrawRay(this.transform.position, upVec, Color.green);
    }

  }
  
}
