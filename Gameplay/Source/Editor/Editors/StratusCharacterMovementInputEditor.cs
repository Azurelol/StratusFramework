using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
	[CustomEditor(typeof(StratusCharacterMovementInput))]
	public class StratusCharacterMovementInputEditor : StratusBehaviourEditor<StratusCharacterMovementInput>
	{
		StratusCharacterMovementInput.Preset.Template presetTemplate;

		protected override void OnStratusEditorEnable()
		{
			AddPropertyChangeCallback(nameof(StratusCharacterMovementInput.target), () => target.ChangeTarget(this.target.target));
			AddArea(AddTemplate);
		}

		private void AddTemplate(Rect rect)
		{
			EditorGUILayout.BeginHorizontal();
			{
				presetTemplate = (StratusCharacterMovementInput.Preset.Template)EditorGUILayout.EnumPopup("Template", presetTemplate);
				if (GUILayout.Button("Add", EditorStyles.miniButtonRight)) target.presets.Add(StratusCharacterMovementInput.Preset.FromTemplate(presetTemplate));
			}
			EditorGUILayout.EndHorizontal();
		}
	}

}