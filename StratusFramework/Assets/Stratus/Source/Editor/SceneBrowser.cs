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
using System.Collections;

namespace Stratus
{
  public class SceneBrowser : EditorWindow
  {
    /// <summary>
    /// Tracks the scroll position
    /// </summary>
    private Vector2 ScrollPosition;
    
    /// <summary>
    /// The title to use for this window
    /// </summary>
    private static string Title = "Scene Browser";
    
    [MenuItem("Stratus/Tools/Scene Browser %g")]
    public static void Open()
    {
      EditorWindow.GetWindow(typeof(SceneBrowser), false, Title);
    }

    private void OnGUI()
    {
      EditorGUILayout.BeginVertical();
      this.ScrollPosition = EditorGUILayout.BeginScrollView(this.ScrollPosition, false, false);

      this.ShowScenesInBuild();
      this.ShowBookmarkedScenes();

      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();  
    }

    void ShowScenesInBuild()
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

    string BookmarkFolder = "Resources/Moonflower/Scenes/";

    public static T[] GetAtPath<T>(string path)
    {

      ArrayList al = new ArrayList();
      string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

      foreach (string fileName in fileEntries)
      {
        int assetPathIndex = fileName.IndexOf("Assets");
        string localPath = fileName.Substring(assetPathIndex);

        Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

        if (t != null)
          al.Add(t);
      }
      T[] result = new T[al.Count];
      for (int i = 0; i < al.Count; i++)
        result[i] = (T)al[i];

      return result;
    }

    //public static T[] GetAtPath<T>(string path, string extension)
    //{

    //  ArrayList al = new ArrayList();
    //  string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

    //  foreach (string fileName in fileEntries)
    //  {
    //    int assetPathIndex = fileName.IndexOf("Assets");
    //    string localPath = fileName.Substring(assetPathIndex);

    //    Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

    //    if (t != null)
    //      al.Add(t);
    //  }
    //  T[] result = new T[al.Count];
    //  for (int i = 0; i < al.Count; i++)
    //    result[i] = (T)al[i];

    //  return result;
    //}

    void ShowBookmarkedScenes()
    {
      GUILayout.Label("Bookmarks", EditorStyles.boldLabel);
      var scenes = GetAtPath<UnityEngine.SceneManagement.Scene>(BookmarkFolder);
      foreach(var scene in scenes)
      {

      }

    }

    

  }

}