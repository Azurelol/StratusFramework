using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(CharacterControllerInput))]
  public class CharacterControllerInputEditor : StratusBehaviourEditor<CharacterControllerInput>
  {
    CharacterControllerInput.Preset.Template presetTemplate;

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

      AddPropertyChangeCallback(nameof(CharacterControllerInput.target), ()=> target.ChangeTarget(this.target.target));

      AddArea(AddTemplate);
    }

    private void AddTemplate(Rect rect)
    {
      EditorGUILayout.BeginHorizontal();
      {
        presetTemplate = (CharacterControllerInput.Preset.Template)EditorGUILayout.EnumPopup("Template", presetTemplate);
        if (GUILayout.Button("Add", EditorStyles.miniButtonRight)) target.presets.Add(CharacterControllerInput.Preset.FromTemplate(presetTemplate)); 
      }
      EditorGUILayout.EndHorizontal();
    }
  }

}