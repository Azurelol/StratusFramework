/******************************************************************************/
/*!
@file   StatusEditor.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using Stratus.Utilities;
using UnityEditor;

namespace Prototype
{
  /// <summary>
  /// 
  /// </summary>
  [CustomEditor(typeof(Status))]
  public class StatusEditor : Editor
  {
    private string[] EffectAttributeNames = new string[0];
    private int EffectAttributeIndex = -1;

    void OnEnable()
    {
      EffectAttributeNames = Reflection.GetSubclassNames<PersistentEffectAttribute>();
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var status = target as Status;
      EditorGUILayout.LabelField("Effects", EditorStyles.centeredGreyMiniLabel);
      ModifyEffects(status);
    }

    void ModifyEffects(Status status)
    {
      //////////////////////////////////////////////////////////
      // Draw attributes with a remove button below each one:
      //////////////////////////////////////////////////////////
      int indexToRemove = -1;
      for (int i = 0; i < status.Effects.Count; i++)
      {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(status.Effects[i].GetType().Name, EditorStyles.boldLabel);
        if (status.Effects[i] != null)
        {;
          status.Effects[i].Inspect();
        }
        if (GUILayout.Button("Remove")) indexToRemove = i;
        EditorGUILayout.EndVertical();
      }

      //////////////////////////////////////////////////////////
      // If something has been tagged for removal...
      //////////////////////////////////////////////////////////
      if (indexToRemove > -1) status.Effects.RemoveAt(indexToRemove);

      //////////////////////////////////////////////////////////
      // Draw a popup and a button to add new attributes
      //////////////////////////////////////////////////////////
      EditorGUILayout.BeginHorizontal();
      EffectAttributeIndex = EditorGUILayout.Popup(EffectAttributeIndex, EffectAttributeNames);
      if (GUILayout.Button("Add"))
      {
        //var newAttribute = Activator.CreateInstance(EffectAttributes[EffectAttributeIndex]) as EffectAttribute;
        // A little tricky because we need to record it in the asset database as well
        var newAttribute = CreateInstance(EffectAttributeNames[EffectAttributeIndex]) as PersistentEffectAttribute;
        newAttribute.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(newAttribute, status);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // Oh jesus what was that
        status.Effects.Add(newAttribute);
      }
      EditorGUILayout.EndHorizontal();

      // Allow clearing of all effects
      if (GUILayout.Button("Clear"))
      {
        status.Effects.Clear();
      }

      if (UnityEngine.GUI.changed) EditorUtility.SetDirty(status);
    }

  }

} 
#endif