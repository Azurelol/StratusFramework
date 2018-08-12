using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(CharacterMovement), true)]
  public class CharacterMovementEditor : BehaviourEditor<CharacterMovement>
  {
    protected override void OnStratusEditorEnable()
    {
      // Whether to show jump properties
      string[] jumpProperties = new string[]
      {
        nameof(CharacterMovement.jumpSpeed),
        nameof(CharacterMovement.jumpCurve),
        nameof(CharacterMovement.fallCurve),
        nameof(CharacterMovement.jumpApex),
        nameof(CharacterMovement.groundDetection),
        nameof(CharacterMovement.groundLayer),
        nameof(CharacterMovement.airControl),
      };
      AddConstraint(()=> target.supportsJump, jumpProperties);
      AddConstraint(() => target.supportsJump && target.hasGroundCast, 
        nameof(CharacterMovement.groundCollider), 
        nameof(CharacterMovement.groundCastFrequency));

      AddPropertyChangeCallback(nameof(CharacterMovement.locomotion), OnLocomotionChange);
    }

    private void OnLocomotionChange()
    {
      target.SetComponents();
    }

  }

}