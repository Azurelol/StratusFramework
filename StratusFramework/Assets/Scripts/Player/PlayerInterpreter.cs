/******************************************************************************/
/*!
@file   PlayerInterpreter.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Overworld
{
  /**************************************************************************/
  /*!
  @class PlayerInterpreter 
  */
  /**************************************************************************/
  public class PlayerInterpreter : MonoBehaviour
  {
    public enum Direction { Forward, Backward, Left, Right }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class MoveEvent : Stratus.Event
    {
      public Direction Direction;
      public Vector3 Vector;
      public MoveEvent(Direction dir, Vector3 vec) { Direction = dir; Vector = vec; }
    }
    public class JumpEvent : Stratus.Event {}

    //------------------------------------------------------------------------/
    // Members
    //------------------------------------------------------------------------/
    // Keyboard
    public KeyCode ForwardKey = KeyCode.W;
    public KeyCode BackwardKey = KeyCode.S;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode RightKey = KeyCode.D;
    public KeyCode InteractKey = KeyCode.E;
    public KeyCode JumpKey = KeyCode.Space;
    // Directions - XZ Plane
    private Vector3 ForwardDir = new Vector3(0.0f, 0.0f, 1.0f);
    private Vector3 BackwardDir = new Vector3(0.0f, 0.0f, -1.0f);
    private Vector3 RighDir = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 LeftDir = new Vector3(-1.0f, 0.0f, 0.0f);

    /**************************************************************************/
    /*!
    @brief  Polls all input devices for valid input.
    */
    /**************************************************************************/
    void Update()
    {
      this.PollKeyBoard();
      this.PollGamepad();
    }

    /**************************************************************************/
    /*!
    @brief  Polls the keyboard for the specified input.
    */
    /**************************************************************************/
    void PollKeyBoard()
    {
      // If no key is pressed
      if (!Input.anyKey)
        return;

      // Movement
      if (Input.GetKey(this.ForwardKey))
        this.gameObject.Dispatch<MoveEvent>(new MoveEvent(Direction.Forward, this.ForwardDir));
      if (Input.GetKey(this.BackwardKey)) DispatchMoveEvent(Direction.Backward, this.BackwardDir);
      if (Input.GetKey(this.LeftKey)) DispatchMoveEvent(Direction.Left, this.LeftDir);
      if (Input.GetKey(this.RightKey)) DispatchMoveEvent(Direction.Right, this.RighDir);
      if (Input.GetKeyDown(this.InteractKey)) this.gameObject.Dispatch<PlayerInteract.InteractEvent>(new PlayerInteract.InteractEvent());
      if (Input.GetKeyDown(this.JumpKey)) this.gameObject.Dispatch<JumpEvent>(new JumpEvent());
    }

    /**************************************************************************/
    /*!
    @brief  Polls the gamepad for the specified input.
    */
    /**************************************************************************/
    void PollGamepad()
    {

    }

    void DispatchMoveEvent(Direction dir, Vector3 vec)
    {
      var moveEvent = new MoveEvent(dir, vec);
      this.gameObject.Dispatch(moveEvent);
    }

  }

}

