using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
  [CustomExtension(typeof(StratusCharacterController))]
  [RequireComponent(typeof(CharacterControllerMovement))]
  public class CharacterControllerInput : StratusBehaviour, IExtensionBehaviour<StratusCharacterController>
  {
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Header("Input")]
    public InputField horizontal = new InputField();
    public InputField vertical = new InputField();
    public InputField sprint = new InputField();
    public InputField jump = new InputField();

    private static CharacterMovement.MoveEvent moveEvent = new CharacterMovement.MoveEvent();
    private static CharacterMovement.JumpEvent jumpEvent = new CharacterMovement.JumpEvent();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public StratusCharacterController extensible { get; set; }
    public CharacterControllerMovement movement { get; set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
      movement = GetComponent<CharacterControllerMovement>();
    }

    public void OnExtensibleStart()
    {

    }

    private void Update()
    {
      if (jump.isDown && !movement.jumping)
        movement.gameObject.Dispatch<CharacterMovement.JumpEvent>(jumpEvent);

      if (!horizontal.isNeutral || !vertical.isNeutral)
      {
        moveEvent.sprint = sprint.isPressed;
        movement.gameObject.Dispatch<CharacterMovement.MoveEvent>(moveEvent);
      }

      //if (movement.moving && sprint.isPressed)
      //  movement.sprinting = true;
      //else if (sprint.isUp)
      //  movement.sprinting = false;

    }


  }

}