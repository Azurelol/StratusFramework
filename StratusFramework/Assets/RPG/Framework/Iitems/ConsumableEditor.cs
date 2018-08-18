/******************************************************************************/
/*!
@file   ConsumableEditor.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using Stratus.Utilities;
using UnityEditor;

namespace Altostratus
{
  /// <summary>
  /// Editor for the Consumable class.
  /// </summary>
  [CustomEditor(typeof(Consumable))]
  public class ConsumableEditor : Editor
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    private string[] EffectAttributeNames = new string[0];
    private int EffectAttributeIndex = -1;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the ConsumableEditor
    /// </summary>
    void OnEnable()
    {
      EffectAttributeNames = Reflection.GetSubclassNames<EffectAttribute>();
    }

    /// <summary>
    /// Called every frame in order to draw its inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var consumable = target as Consumable;
      EditorGUILayout.LabelField("Effects", EditorStyles.centeredGreyMiniLabel);
      ModifyEffects(consumable);
    }

    void ModifyEffects(Consumable consumable)
    {
      //////////////////////////////////////////////////////////
      // Draw attributes with a remove button below each one:
      //////////////////////////////////////////////////////////
      int indexToRemove = -1;
      for (int i = 0; i < consumable.effects.Count; i++)
      {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(consumable.effects[i].GetType().Name, EditorStyles.boldLabel);
        if (consumable.effects[i] != null)
        {
          consumable.effects[i].modifier = (EffectAttribute.TargetingModifier)UnityEditor.EditorGUILayout.EnumPopup("Modifier",
                                       consumable.effects[i].modifier);
          consumable.effects[i].OnInspect();
        }
        if (GUILayout.Button("Remove")) indexToRemove = i;
        EditorGUILayout.EndVertical();
      }

      //////////////////////////////////////////////////////////
      // If something has been tagged for removal...
      //////////////////////////////////////////////////////////
      if (indexToRemove > -1) consumable.effects.RemoveAt(indexToRemove);

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
        AssetDatabase.AddObjectToAsset(newAttribute, consumable);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // Oh jesus what was that
        consumable.effects.Add(newAttribute);

        //EditorUtility.SetDirty(skill);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
      }
      EditorGUILayout.EndHorizontal();

      // Allow clearing of all effects
      if (GUILayout.Button("Clear"))
      {
        consumable.effects.Clear();
      }

      if (UnityEngine.GUI.changed) EditorUtility.SetDirty(consumable);
    }

  }

}
#endif