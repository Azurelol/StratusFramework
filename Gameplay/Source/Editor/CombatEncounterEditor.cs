using UnityEngine;
using System.Collections;
using Stratus;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace Stratus.Gameplay
{
	[CustomEditor(typeof(StratusCombatEncounter))]
	public class CombatEncounterEditor : UnityEditor.Editor
	{
		StratusCharacterScriptable[] enemyCharacters = new StratusCharacterScriptable[0];
		string[] enemyCharactersNames = new string[0];
		int enemyCharacterIndex = 0;

		void OnEnable()
		{
			enemyCharacters = Resources.FindObjectsOfTypeAll<StratusCharacterScriptable>();
			enemyCharactersNames = enemyCharacters.ToStringArray();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var encounter = target as StratusCombatEncounter;

			EditorGUILayout.LabelField("Group", EditorStyles.centeredGreyMiniLabel);

			// Display current foes with a remove button below each
			int indexToRemove = -1;
			for (int i = 0; i < encounter.Group.Count; i++)
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				// Display its name
				EditorGUILayout.LabelField(encounter.Group[i].name, EditorStyles.boldLabel);
				if (GUILayout.Button("Remove")) indexToRemove = i;
				EditorGUILayout.EndVertical();
			}

			// If something was tagged for removal
			if (indexToRemove > -1) encounter.Group.RemoveAt(indexToRemove);

			// Add new foes
			//EditorGUILayout.LabelField("Add")
			EditorGUILayout.BeginHorizontal();
			enemyCharacterIndex = EditorGUILayout.Popup(enemyCharacterIndex, enemyCharactersNames);
			if (GUILayout.Button("Add"))
			{
				encounter.Group.Add(enemyCharacters[enemyCharacterIndex]);
			}
			EditorGUILayout.EndHorizontal();

			// So it gets serialized, ya know
			if (GUI.changed) EditorUtility.SetDirty(encounter);


		}


	}

}