using UnityEngine;
using System.Collections.Generic;
using Stratus;
using UnityEditor;

namespace Genitus
{
  [CustomEditor(typeof(PlayerParty))]
  public class PlayerPartyEditor : BehaviourEditor<PlayerParty>
  {
    //private SerializedProperty PartyMembers;

    protected override void OnStratusEditorEnable()
    {
      AddArea(DrawImport);
    }

    private void DrawImport(Rect rect)
    {
      EditorGUILayout.LabelField("Import", EditorStyles.centeredGreyMiniLabel);
      EditorGUILayout.BeginHorizontal();
      target.party = EditorGUILayout.ObjectField("Party", target.party, typeof(Party), true) as Party;
      if (GUILayout.Button("Import")) target.Import();
      EditorGUILayout.EndHorizontal();
    }

    //void ModifyParty(PlayerParty party)
    //{
    //  int indexToRemove = -1;
    //  for (int i = 0; i < party.members.Count; ++i)
    //  {
    //    var member = party.members[i];
    //    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
    //    EditorGUILayout.LabelField(member.name);
    //    if (GUILayout.Button("Remove")) indexToRemove = i;
    //    EditorGUILayout.EndHorizontal();
    //  }
    //  if (indexToRemove > -1) party.members.RemoveAt(indexToRemove);
    //}

    //PlayerParty.PartyMember DrawListItem(Rect position, PlayerParty.PartyMember item)
    //{
    //  return item;
    //}






  }
}