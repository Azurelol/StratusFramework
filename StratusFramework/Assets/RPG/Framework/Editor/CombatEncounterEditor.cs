/******************************************************************************/
/*!
@file   CombatEncounterEditor.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace Altostratus
{
  /**************************************************************************/
  /*!
  @class CombatEncounterEditor 
  */
  /**************************************************************************/
  [CustomEditor(typeof(CombatEncounter))]
  public class CombatEncounterEditor : Editor
  {
    Character[] EnemyCharacters = new Character[0];
    string[] EnemyCharactersNames = new string[0];
    int EnemyCharacterIndex = 0;

    void OnEnable()
    {
      // Grab a list of all enemy characters
      var characters = Resources.FindObjectsOfTypeAll<Character>() ;
      EnemyCharacters = (from Character character in characters where character.faction == CombatController.Faction.Hostile select character).ToArray();
      // this is hella cool
      EnemyCharactersNames = (from Character character in EnemyCharacters select character.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var encounter = target as CombatEncounter;

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
      EnemyCharacterIndex = EditorGUILayout.Popup(EnemyCharacterIndex, EnemyCharactersNames);
      if (GUILayout.Button("Add"))
      {
        encounter.Group.Add(EnemyCharacters[EnemyCharacterIndex]);
      }
      EditorGUILayout.EndHorizontal();

      // So it gets serialized, ya know
      if (GUI.changed) EditorUtility.SetDirty(encounter);


    }


  }

}