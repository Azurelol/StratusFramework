using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Gameplay;

namespace Stratus.Gameplay
{
  [CustomExtension(typeof(StratusCharacterController))]
  public class CharacterControllerAnimator : CharacterAnimator, IExtensionBehaviour<StratusCharacterController>
  {
    public StratusCharacterController extensible { get; set; }

    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
      onUpdate += FaceDirection;
    }

    public void OnExtensibleStart()
    {

    }

    private void FaceDirection()
    {
      FaceDirection(extensible.heading, extensible.rotationSpeed);
    }




  }

}