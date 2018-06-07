using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(CharacterControllerInput))]
  public class CharacterControllerInputEditor : BehaviourEditor<CharacterControllerInput>
  {
    protected override void OnStratusEditorEnable()
    {
      AddConstraint(() => target.mode == CharacterControllerInput.InputMode.Controller,
        nameof(CharacterControllerInput.horizontal),
        nameof(CharacterControllerInput.vertical),
        nameof(CharacterControllerInput.sprint),
        nameof(CharacterControllerInput.jump));

      AddConstraint(() => target.mode == CharacterControllerInput.InputMode.Mouse,
        nameof(CharacterControllerInput.mouseMovement),
        nameof(CharacterControllerInput.moveButton));
    }
  }

}