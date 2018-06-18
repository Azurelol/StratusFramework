using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(CharacterMovement), true)]
  public class CharacterMovementEditor : BehaviourEditor<CharacterMovement>
  {
    protected override void OnStratusEditorEnable()
    {
      AddConstraint(nameof(CharacterMovement.groundCollider), () => target.groundDetection != CharacterMovement.GroundDetection.Collision);
      AddConstraint(nameof(CharacterMovement.groundCastFrequency), () => target.groundDetection != CharacterMovement.GroundDetection.Collision);
      AddPropertyChangeCallback(nameof(CharacterMovement.locomotion), OnLocomotionChange);
    }

    private void OnLocomotionChange()
    {
      target.SetComponents();
    }

  }

}