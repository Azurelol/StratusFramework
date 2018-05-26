using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Experimental
{
  [CustomExtension(typeof(StratusPlayerController))]
  public class StratusPlayerControllerAnimator : CharacterAnimator, IExtensionBehaviour<StratusPlayerController>
  {
    public StratusPlayerController extensible { get; set; }

    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      this.extensible = (StratusPlayerController)extensible;
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