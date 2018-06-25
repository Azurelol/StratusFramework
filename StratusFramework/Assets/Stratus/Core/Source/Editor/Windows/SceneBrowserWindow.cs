//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using UnityEditor.SceneManagement;
//using System.Collections;
//using System.Collections.Generic;
//using Stratus.Utilities;

//namespace Stratus.Editor
//{
//  /// <summary>
//  /// A window allowing quick and easy scene navigation
//  /// </summary>
//  public class SceneBrowserWindow : EditorWindow
//  {
//    //------------------------------------------------------------------------/
//    // Fields
//    //------------------------------------------------------------------------/
//    private static SceneAsset sceneToAdd;
//    private bool showBookmarkScenes, showBuildScenes;
//    private Vector2 scrollPosition;
//    //------------------------------------------------------------------------/
//    // Properties
//    //------------------------------------------------------------------------/
    
//    private static GUIStyle removeButtonStyle
//    {
//      get
//      {
//        var style = EditorStyles.miniButton;
//        style.fontStyle = FontStyle.Bold;
//        style.font.material.color = Color.red;
//        style.alignment = TextAnchor.MiddleCenter;
//        style.fixedWidth = 20;
//        return style;
//      }
//    }

//    //------------------------------------------------------------------------/
//    // Messages
//    //------------------------------------------------------------------------/
//    [MenuItem("Stratus/Windows/Scene Browser")]
//    public static void Open()
//    {
//      EditorWindow.GetWindow(typeof(SceneBrowserWindow), false, "Scene Browser");
//    }

//    private void OnGUI()
//    {
//      EditorGUILayout.BeginVertical();
//      this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, false, false);
//      {
//        this.ShowBookmarkedScenes();
//        EditorGUILayout.Space();
//        this.AddBookmarkedScene();
//        this.ShowScenesInBuild();
//      }
//      EditorGUILayout.EndScrollView();
//      EditorGUILayout.EndVertical();
//    }

//    //------------------------------------------------------------------------/
//    // Methods
//    //------------------------------------------------------------------------/
//    void ShowScenesInBuild()
//    {
//      GUILayout.Label("Build", EditorStyles.centeredGreyMiniLabel);
//      for (var i = 0; i < EditorBuildSettings.scenes.Length; ++i)
//      {
//        var scene = EditorBuildSettings.scenes[i];
//        if (scene.enabled)
//        {
//          var sceneName = Path.GetFileNameWithoutExtension(scene.path);
//          var pressed = GUILayout.Button(i + ": " + sceneName, EditorStyles.toolbarButton);
//          if (pressed)
//          {
//            var button = UnityEngine.Event.current.button;
//            if (button == 0 && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//            {
//              EditorSceneManager.OpenScene(scene.path);
//            }
//          }
//        }
//      }
//    }


//    void ShowBookmarkedScenes()
//    {
//      GUILayout.Label("Bookmarks", EditorStyles.centeredGreyMiniLabel);
//      foreach (var scene in bookmarkedScenes)
//      {
//        // If it was deleted from the outside, we need to remove this reference
//        if (scene == null)
//        {
//          RemoveBookmarkedScene(scene);
//          return;
//        }

//        EditorGUILayout.BeginHorizontal();
//        // Open scene
//        if (GUILayout.Button(scene.name, EditorStyles.toolbarButton))
//        {
//          StratusEditorUtility.OnMouseClick(
//            () =>
//            {
//              var scenePath = AssetDatabase.GetAssetPath(scene);
//              if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//              {
//                EditorSceneManager.OpenScene(scenePath);
//              }
//            },

//            () =>
//            {
//              var menu = new GenericMenu();
//              menu.AddItem(new GUIContent("Remove"), false,
//                () =>
//                {
//                  RemoveBookmarkedScene(scene);
//                }
//                );
//              menu.ShowAsContext();
//            },

//            null,             
//            true);
//        }
//        EditorGUILayout.EndHorizontal();
//      }

//    }

//    void AddBookmarkedScene()
//    {
//      EditorGUILayout.BeginHorizontal();
//      sceneToAdd = EditorGUILayout.ObjectField(sceneToAdd, typeof(SceneAsset), false) as SceneAsset;
//      if (GUILayout.Button("Add", EditorStyles.miniButtonRight) && sceneToAdd != null && !bookmarkedScenes.Contains(sceneToAdd))
//      {
//        bookmarkedScenes.Add(sceneToAdd);
//        Preferences.Save();
//        sceneToAdd = null;
//      }

//      EditorGUILayout.EndHorizontal();

//    }

//    void RemoveBookmarkedScene(SceneAsset scene)
//    {
//      bookmarkedScenes.Remove(scene);
//      Preferences.Save();
//      Repaint();
//    }


//  }

//}