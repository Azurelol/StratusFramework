using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
  [CustomExtension(typeof(StratusCharacterController))]
  public class CharacterControllerMovement : CharacterMovement, IExtensionBehaviour<StratusCharacterController>
  {
    public StratusCharacterController extensible { get; set; }

    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
    }

    public void OnExtensibleStart()
    {
      
    }

  }

}