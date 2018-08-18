/******************************************************************************/
/*!
@file   PartyEditor.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using UnityEditor;

namespace Altostratus
{
  [CustomEditor(typeof(Party))]
  public class PartyEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      var party = target as Party;
      //ModifyMembers(party);
      EditorGUILayout.LabelField("Members", EditorStyles.centeredGreyMiniLabel);
      EditorBridge.ModifyArray<Party, Character>(party, party.Members);
      if (GUI.changed) EditorUtility.SetDirty(target);
      serializedObject.ApplyModifiedProperties();
    }

    void ModifyMembers(Party party)
    {
      // List all added members, allowing any to be removed
      int indexToRemove = -1;
      for (int i = 0; i < party.Members.Count; ++i)
      {
        var member = party.Members[i];
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.ObjectField("", member, typeof(Character), true);
        if (GUILayout.Button("Remove")) indexToRemove = i;
        EditorGUILayout.EndHorizontal();
      }
      if (indexToRemove > -1) party.Members.RemoveAt(indexToRemove);

      // Add a member

    }


    }
}

#endif