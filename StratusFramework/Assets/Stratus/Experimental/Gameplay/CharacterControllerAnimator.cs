using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Gameplay;

namespace Stratus.Gameplay
{
  [RequireComponent(typeof(CharacterControllerMovement))]
  [CustomExtension(typeof(StratusCharacterController))]
  public class CharacterControllerAnimator : CharacterAnimator, IExtensionBehaviour<StratusCharacterController>
  {
    public bool boo;
    public StratusCharacterController extensible { get; set; }
    public CharacterControllerMovement movement { get; private set; }

    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
      //onUpdate += FaceDirection;
    }

    public void OnExtensibleStart()
    {      
    }

    //private void FaceDirection()
    //{
    //  FaceDirection(extensible.heading, extensible.rotationSpeed);
    //}




  }

}