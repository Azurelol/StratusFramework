/******************************************************************************/
/*!
@file   PlayerController.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   6/15/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Overworld
{
  /**************************************************************************/
  /*!
  @class PlayerController 
  */
  /**************************************************************************/
  public class PlayerController : MonoBehaviour
  {
    public bool Debug = true;
    public bool Translate = false;
    public bool Rotate = true;
    public float MoveSpeed = 1.0f;
    public float RotSpeed = 3.0f;

    private bool Jumping = false;

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    void Start()
    {
      // Subscribe to events
      this.gameObject.Connect<PlayerInterpreter.MoveEvent>(this.OnMoveEvent);
      this.gameObject.Connect<PlayerInterpreter.JumpEvent>(this.OnJumpEvent);
    }

    void OnJumpEvent(PlayerInterpreter.JumpEvent e)
    {      
      if (!Jumping)
        Jump();
    }
    void OnCollisionEnter(Collision hit)
    {      
      // Trace.Object("Collided?", this);
      Jumping = false;
    }

    void OnMoveEvent(PlayerInterpreter.MoveEvent e) {

      switch (e.Direction)
      {
        case PlayerInterpreter.Direction.Forward:
          if (this.Debug) Trace.Object("Moving forward!", this);
          this.Move(transform.forward);
          break;
        case PlayerInterpreter.Direction.Backward:
          if (this.Debug) Trace.Object("Moving backward!", this);
          this.Move(-transform.forward);
          break;
        case PlayerInterpreter.Direction.Right:
          if (this.Debug) Trace.Object("Moving right!", this);
          if (Rotate) this.transform.Rotate(0, this.RotSpeed, 0, UnityEngine.Space.World);
          else this.Move(transform.right);
          break;
        case PlayerInterpreter.Direction.Left:
          if (this.Debug) Trace.Object("Moving left!", this);
          if (Rotate) this.transform.Rotate(0, -this.RotSpeed, 0, UnityEngine.Space.World);
          else this.Move(-transform.right);
          break;
      }

    }

    void Move(Vector3 vec)
    {
      // Translation
      if (this.Translate)
      {
        var startPos = this.gameObject.transform.position;
        var endPos = transform.position + vec;
        this.transform.position = Vector3.Lerp(startPos, endPos, 0.1f);
      }
      // RigidBody
      else
      {
        GetComponent<Rigidbody>().AddForce(vec * this.MoveSpeed);
      }
    }
    
    void Jump()
    {
      Trace.Object("Jumping!", this);
      GetComponent<Rigidbody>().AddForce(transform.up * this.MoveSpeed / 4, ForceMode.Impulse);
      Jumping = true;
    }





  }

}


