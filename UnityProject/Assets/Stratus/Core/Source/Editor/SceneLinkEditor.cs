using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Stratus.Experimental;
using System;

namespace Stratus
{
  [CustomEditor(typeof(SceneLink))]
  public class SceneLinkEditor : Editor
  {
    SceneLink link;
    private Vector2 poolScrollPos;
    //List<SceneField> selected => link.selectedScenes;
    //SerializedProperty poolProp;
    //SerializedProperty selectedScenesProp;

    private void OnEnable()
    {
      link = target as SceneLink;
      //SerializedObject so = new SerializedObject(link);
      //poolProp = so.FindProperty("scenePool");
      //selectedScenesProp = so.FindProperty("selectedScenes");
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      if (link.scenePool == null)
        return;
      SelectScenes();

      if (GUI.changed)
      {
        EditorUtility.SetDirty(link);
        AssetDatabase.SaveAssets();
      }

    }

    private void SelectScenes()
    {
      EditorGUILayout.BeginHorizontal();
      {
        // Pool
        //poolScrollPos = EditorGUILayout.BeginScrollView(poolScrollPos);
        EditorGUILayout.BeginVertical();
        {
          EditorGUILayout.LabelField("Available", EditorStyles.centeredGreyMiniLabel);
          foreach(var scene in link.scenePool.scenes)
          {
            var matchingScene = link.selectedScenes.Find(x => x.name == scene.name);
            if (matchingScene != null)
              continue;
          
            if (GUILayout.Button(scene.name, EditorStyles.miniButton))
            {
              link.selectedScenes.Add(scene);
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
          for(int i = 0; i < link.selectedScenes.Count; ++i)
          {
            var scene = link.selectedScenes[i];
            if (GUILayout.Button(scene.name, EditorStyles.miniButton))
            {
              indexToRemove = i;
            }
          }
          if (indexToRemove > -1)
            link.selectedScenes.RemoveAt(indexToRemove);
        }
        EditorGUILayout.EndVertical();
      }
      EditorGUILayout.EndHorizontal();
    }
  }
}

