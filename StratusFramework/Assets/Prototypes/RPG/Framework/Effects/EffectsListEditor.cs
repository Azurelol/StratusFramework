/******************************************************************************/
/*!
@file   EffectAttributeListDrawer.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;
using System.Collections.Generic;
using System;

namespace Prototype
{
  [CustomEditor(typeof(EffectsList))]
  public class EffectsListEditor : Editor
  {
    private string[] EffectAttributeNames = new string[0];
    private int EffectAttributeIndex = -1;

    void OnEnable()
    {
      EffectAttributeNames = Reflection.GetSubclassNames<EffectAttribute>();
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var list = target as EffectsList;
      EditorGUILayout.LabelField("Effects", EditorStyles.centeredGreyMiniLabel);
      ModifyEffects(list);
    }

    void ModifyEffects(EffectsList list)
    {
      //////////////////////////////////////////////////////////
      // Draw attributes with a remove button below each one:
      //////////////////////////////////////////////////////////
      int indexToRemove = -1;
      for (int i = 0; i < list.Effects.Count; i++)
      {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(list.Effects[i].GetType().Name, EditorStyles.boldLabel);
        if (list.Effects[i] != null)
        {
          list.Effects[i].Inspect();
        }
        if (GUILayout.Button("Remove")) indexToRemove = i;
        EditorGUILayout.EndVertical();
      }

      //////////////////////////////////////////////////////////
      // If something has been tagged for removal...
      //////////////////////////////////////////////////////////
      if (indexToRemove > -1) list.Effects.RemoveAt(indexToRemove);

      //////////////////////////////////////////////////////////
      // Draw a popup and a button to add new attributes
      //////////////////////////////////////////////////////////
      EditorGUILayout.BeginHorizontal();
      EffectAttributeIndex = EditorGUILayout.Popup(EffectAttributeIndex, EffectAttributeNames);
      if (GUILayout.Button("Add"))
      {
        //var newAttribute = Activator.CreateInstance(EffectAttributes[EffectAttributeIndex]) as EffectAttribute;
        // A little tricky because we need to record it in the asset database as well
        var newAttribute = ScriptableObject.CreateInstance(EffectAttributeNames[EffectAttributeIndex]) as EffectAttribute;
        newAttribute.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(newAttribute, list);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // Oh jesus what was that
        list.Effects.Add(newAttribute);
      }
      EditorGUILayout.EndHorizontal();

      // Allow clearing of all effects
      if (GUILayout.Button("Clear"))
      {
        list.Effects.Clear();
      }

      if (UnityEngine.GUI.changed) EditorUtility.SetDirty(list);
    }

  }
}

#endif