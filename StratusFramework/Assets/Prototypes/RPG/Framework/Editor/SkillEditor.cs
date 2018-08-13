/******************************************************************************/
/*!
@file   SkillEditor.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using Stratus.Utilities;
using UnityEditor;

namespace Prototype
{
  /// <summary>
  /// Editor for the Skill class.
  /// </summary>
  [CustomEditor(typeof(Skill))]
  public class SkillEditor : Editor
  {
    private string[] EffectAttributeNames = new string[0];
    private int EffectAttributeIndex = -1;
    /**************************************************************************/
    /*!
    @brief  Initializes the SkillEditor.
    */
    /**************************************************************************/
    void OnEnable()
    {
      EffectAttributeNames = Reflection.GetSubclassNames<EffectAttribute>();
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var skill = target as Skill;
      EditorGUILayout.LabelField("Effects", EditorStyles.centeredGreyMiniLabel);
      ModifyEffects(skill);
    }

    void ModifyEffects(Skill skill)
    {
      //////////////////////////////////////////////////////////
      // Draw attributes with a remove button below each one:
      //////////////////////////////////////////////////////////
      int indexToRemove = -1;
      for (int i = 0; i < skill.effects.Count; i++)
      {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(skill.effects[i].GetType().Name, EditorStyles.boldLabel);
        if (skill.effects[i] != null)
        {
          skill.effects[i].Inspect();
        }
        if (GUILayout.Button("Remove")) indexToRemove = i;
        EditorGUILayout.EndVertical();
      }

      //////////////////////////////////////////////////////////
      // If something has been tagged for removal...
      //////////////////////////////////////////////////////////
      if (indexToRemove > -1) skill.effects.RemoveAt(indexToRemove);

      //////////////////////////////////////////////////////////
      // Draw a popup and a button to add new attributes
      //////////////////////////////////////////////////////////
      EditorGUILayout.BeginHorizontal();
      EffectAttributeIndex = EditorGUILayout.Popup(EffectAttributeIndex, EffectAttributeNames);
      if (GUILayout.Button("Add"))
      {
        //var newAttribute = Activator.CreateInstance(EffectAttributes[EffectAttributeIndex]) as EffectAttribute;
        // A little tricky because we need to record it in the asset database as well
        var newAttribute = CreateInstance(EffectAttributeNames[EffectAttributeIndex]) as EffectAttribute;
        newAttribute.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(newAttribute, skill);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // Oh jesus what was that
        skill.effects.Add(newAttribute);
      }
      EditorGUILayout.EndHorizontal();

      // Allow clearing of all effects
      if (GUILayout.Button("Clear"))
      {
        skill.effects.Clear();
      }

      if (UnityEngine.GUI.changed) EditorUtility.SetDirty(skill);
    }

  }

} 
#endif