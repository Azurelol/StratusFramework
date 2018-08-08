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
    public StratusCharacterController extensible { get; set; }
    public CharacterControllerMovement movement { get; private set; }

    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusCharacterController)extensible;
    }

    public void OnExtensibleStart()
    {
    }

  }

}