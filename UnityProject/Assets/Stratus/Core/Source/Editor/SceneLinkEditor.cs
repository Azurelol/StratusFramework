using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Stratus.Experimental;
using System;

namespace Stratus
{
  [CustomEditor(typeof(SceneLinkerEvent))]
  public class SceneLinkerEventEditor : TriggerableEditor<SceneLinkerEvent>
  {
    //private Vector2 poolScrollPos;

    protected override void OnTriggerableEditorEnable()
    {
      propertyConstraints.Add(propertyMap["selectedScenes"], False);
      drawGroupRequests.Add(new DrawGroupRequest(SelectScenes, ()=> { return triggerable.scenePool != null; }));
    }

    //public override void OnInspectorGUI()
    //{
    //  base.OnInspectorGUI();
    //  if (link.scenePool == null)
    //    return;
    //  SelectScenes();
    //
    //  if (GUI.changed)
    //  {
    //    EditorUtility.SetDirty(link);
    //    AssetDatabase.SaveAssets();
    //  }
    //
    //}

    private string GetName(SceneField sceneField) => sceneField.name;

    private void SelectScenes2(Rect rect)
    {
      EditorGUILayout.BeginHorizontal();
      {
        // Pool
        //poolScrollPos = EditorGUILayout.BeginScrollView(poolScrollPos);
        EditorGUILayout.BeginVertical();
        {
          EditorGUILayout.LabelField("Available", EditorStyles.centeredGreyMiniLabel);
          foreach (var scene in triggerable.scenePool.scenes)
          {
            var matchingScene = triggerable.selectedScenes.Find(x => x.name == scene.name);
            if (matchingScene != null)
              continue;

            if (GUILayout.Button(scene.name, EditorStyles.miniButton))
            {
              triggerable.selectedScenes.Add(scene);
            }
          }
        }
        //EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        // Selected scenes
        EditorGUILayout.BeginVertical();
        {
          EditorGUILayout.LabelField("Selected", EditorStyles.centeredGreyMiniLabel);
          int indexToRemove = -1;
          for (int i = 0; i < triggerable.selectedScenes.Count; ++i)
          {
            var scene = triggerable.selectedScenes[i];
            if (GUILayout.Button(scene.name, EditorStyles.miniButton))
            {
              indexToRemove = i;
            }
          }
          if (indexToRemove > -1)
            triggerable.selectedScenes.RemoveAt(indexToRemove);
        }
        EditorGUILayout.EndVertical();
      }
      EditorGUILayout.EndHorizontal();
    }

    private void SelectScenes(Rect rect)
    {
      StratusEditorUtility.SelectSubset(triggerable.scenePool.scenes, triggerable.selectedScenes, GetName);      
    }


  }
}

