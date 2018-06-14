using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(CharacterMovement), true)]
  public class CharacterMovementEditor : BehaviourEditor<CharacterMovement>
  {
    protected override void OnStratusEditorEnable()
    {
      //AddConstraint(nameof(CharacterMovement.groundLayer), () => target.groundDetection == CharacterMovement.GroundDetection.Layer);
      AddConstraint(nameof(CharacterMovement.groundCollider), () => target.groundDetection != CharacterMovement.GroundDetection.Collision);
      AddConstraint(nameof(CharacterMovement.groundCastFrequency), () => target.groundDetection != CharacterMovement.GroundDetection.Collision);
    }

  }

}