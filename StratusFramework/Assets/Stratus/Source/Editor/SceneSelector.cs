/******************************************************************************/
/*!
@file   SceneSelector.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

namespace Stratus
{
  public class SceneSelector : EditorWindow
  {
    /// <summary>
    /// Tracks the scroll position
    /// </summary>
    private Vector2 ScrollPosition;
    
    /// <summary>
    /// The title to use for this window
    /// </summary>
    private static string Title = "Scene View";
    
    [MenuItem("Stratus/Tools/Scene Selector %g")]
    public static void Open()
    {
      EditorWindow.GetWindow(typeof(SceneSelector), false, Title);
    }

    private void OnGUI()
    {
      EditorGUILayout.BeginVertical();
      this.ScrollPosition = EditorGUILayout.BeginScrollView(this.ScrollPosition, false, false);

      this.ShowScenes();

      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();  
    }

    void ShowScenes()
    {
      GUILayout.Label("Scenes in build", EditorStyles.boldLabel);
      for (var i = 0; i < EditorBuildSettings.scenes.Length; ++i)
      {
        var scene = EditorBuildSettings.scenes[i];
        if (scene.enabled)
        {
          var sceneName = Path.GetFileNameWithoutExtension(scene.path);
          var pressed = GUILayout.Button(i + ": " + sceneName, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });
          if (pressed)
          {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
              EditorSceneManager.OpenScene(scene.path);
            }

          }
        }
      }
    }

  }

}