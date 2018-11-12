using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEditor.AnimatedValues;

namespace Stratus
{
  /// <summary>
  /// A window for easily bookmarking frequently used objects
  /// </summary>
  public class ObjectBookmarksWindow : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Declaration
    //------------------------------------------------------------------------/
    [Serializable]
    public class ObjectBookmarks
    {
      public List<UnityEngine.Object> projectBookmarks = new List<UnityEngine.Object>();
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private Vector2 scrollPosition;
    private SceneAsset sceneToAdd;
    private AnimBool showScenesInBuild, showScenes, showSceneObjects, showProjectAssets;
    

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public static ObjectBookmarks bookmarks { get; private set; }
    public static List<SceneAsset> bookmarkedScenes { get { return StratusPreferences.instance.bookmarkedScenes; } }
    public static List<GameObjectBookmark> sceneBookmarks { get; private set; }
    private static ObjectBookmarksWindow instance { get; set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Core/Object Bookmarks")]
    public static void Open()
    {
      instance = (ObjectBookmarksWindow)EditorWindow.GetWindow(typeof(ObjectBookmarksWindow), false, "Bookmarks");
    }

    private void OnEnable()
    {
      bookmarks = StratusPreferences.instance.objectBookmarks;
      sceneBookmarks = GameObjectBookmark.availableList;
      showScenes = new AnimBool(true); showScenes.valueChanged.AddListener(Repaint);
      showSceneObjects = new AnimBool(true); showSceneObjects.valueChanged.AddListener(Repaint);
      showScenesInBuild = new AnimBool(true); showScenesInBuild.valueChanged.AddListener(Repaint);
      showProjectAssets = new AnimBool(true); showProjectAssets.valueChanged.AddListener(Repaint);
    }


    private void OnGUI()
    {
      // Top bar
      EditorGUILayout.BeginHorizontal();
      {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Remove GameObject Bookmarks"), false, GameObjectBookmark.RemoveAll);
        StratusEditorUtility.DrawContextMenu(menu, StratusEditorUtility.ContextMenuType.Options);
      }
      EditorGUILayout.EndHorizontal();

      // Bookmarks list
      EditorGUILayout.BeginVertical();
      this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, false, false);
      {
        StratusEditorUtility.DrawVerticalFadeGroup(showScenesInBuild, "Scenes in Build", ShowScenesInBuild, EditorStyles.helpBox, EditorBuildSettings.scenes.Length > 0);
        StratusEditorUtility.DrawVerticalFadeGroup(showScenes, "Scenes", ShowBookmarkedScenes, EditorStyles.helpBox, bookmarkedScenes.Count > 0);
        StratusEditorUtility.DrawVerticalFadeGroup(showSceneObjects, "Scene Objects", ShowSceneObjects, EditorStyles.helpBox, GameObjectBookmark.hasAvailable);
        StratusEditorUtility.DrawVerticalFadeGroup(showProjectAssets, "Project Assets", ShowProjectAssets, EditorStyles.helpBox, bookmarks.projectBookmarks.Count > 0);
      }
      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    [MenuItem("Assets/Bookmark", false, 0)]
    private static void BookmarkAsset()
    {
      UnityEngine.Object activeObject = Selection.activeObject;
      // Special case for scenes
      if (activeObject.GetType() == typeof(SceneAsset))
      {
        StratusDebug.Log("That's a scene!");
        SceneAsset scene = activeObject as SceneAsset;
        if (!bookmarkedScenes.Contains(scene))
        {
          bookmarkedScenes.Add(scene);
        }
      }
      else
      {
        StratusPreferences.instance.objectBookmarks.projectBookmarks.Add(activeObject);
      }

      StratusPreferences.Save();
    }

    [MenuItem("GameObject/Bookmark", false, 49)]
    private static void BookmarkGameObject()
    {
      //if (Selection.activeGameObject != null)
      GameObject go = Selection.activeGameObject;
      if (go != null)
      {
        go.GetOrAddComponent<GameObjectBookmark>();
      }
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void ShowScenesInBuild()
    {
      for (var i = 0; i < EditorBuildSettings.scenes.Length; ++i)
      {
        var scene = EditorBuildSettings.scenes[i];
        if (scene.enabled)
        {
          var sceneName = Path.GetFileNameWithoutExtension(scene.path);
          var pressed = GUILayout.Button(i + ": " + sceneName, EditorStyles.toolbarButton);
          if (pressed)
          {
            var button = UnityEngine.Event.current.button;
            if (button == 0 && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
              EditorSceneManager.OpenScene(scene.path);
            }
          }
        }
      }
    }

    void ShowBookmarkedScenes()
    {
      //GUILayout.Label("Scenes", EditorStyles.centeredGreyMiniLabel);
      foreach (var scene in bookmarkedScenes)
      {
        // If it was deleted from the outside, we need to remove this reference
        if (scene == null)
        {
          RemoveBookmarkedScene(scene);
          return;
        }

        //EditorGUILayout.BeginHorizontal();
        // Open scene
        if (GUILayout.Button(scene.name, EditorStyles.toolbarButton))
        {
          StratusEditorUtility.OnMouseClick(
            () =>
            {
              var scenePath = AssetDatabase.GetAssetPath(scene);
              if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
              {
                EditorSceneManager.OpenScene(scenePath);
              }
            },

            () =>
            {
              var menu = new GenericMenu();
              menu.AddItem(new GUIContent("Remove"), false,
                () =>
                {
                  RemoveBookmarkedScene(scene);
                }
                );
              menu.ShowAsContext();
            },

            null,
            true);
        }
        //EditorGUILayout.EndHorizontal();
      }

    }

    private void ShowSceneObjects()
    { 
      for (int b = 0; b < sceneBookmarks.Count; ++b)
      {
        GameObjectBookmark bookmark = sceneBookmarks[b];

        EditorGUILayout.ObjectField(bookmark, bookmark.GetType(), true);
        StratusEditorUtility.OnLastControlMouseClick(
        // Left
        () =>
        {
          SelectBookmark(bookmark);
        },

        // Right
        () =>
        {
          var menu = new GenericMenu();
          menu.AddItem(new GUIContent("Inspect"), false, () =>
          {
            MemberInspectorWindow.Inspect(bookmark.gameObject);
          });
          menu.AddItem(new GUIContent("Remove"), false,
            () =>
            {
              DestroyImmediate(bookmark);
            }
            );
          menu.ShowAsContext();
        },

        null);
      }
    }


    private void ShowProjectAssets()
    {
      for (int b = 0; b < bookmarks.projectBookmarks.Count; ++b)
      {
        UnityEngine.Object currentObject = bookmarks.projectBookmarks[b];

        if (currentObject == null)
        {
          bookmarks.projectBookmarks.RemoveAt(b);
          OnChange();
          return;
        }

        Type objectType = currentObject.GetType();
        bookmarks.projectBookmarks[b] = EditorGUILayout.ObjectField(currentObject, objectType, false);

        StratusEditorUtility.OnLastControlMouseClick(
        // Left
        () =>
        {
          SelectBookmark(currentObject);
        },

        // Right
        () =>
        {
          var menu = new GenericMenu();

          // If it's a prefab, instantiate
          if (PrefabUtility.GetPrefabType(currentObject) != PrefabType.None)
          {
            menu.AddItem(new GUIContent("Instantiate"), false, () => 
            {
              GameObject instantiated = (GameObject)GameObject.Instantiate(currentObject);
              instantiated.name = currentObject.name;
            });
          }

          // Remove
          menu.AddItem(new GUIContent("Remove"), false,
            () =>
            {
              bookmarks.projectBookmarks.Remove(currentObject);
              OnChange();
            }
            );
          menu.ShowAsContext();
        },
        null);

      }
    }


    void AddBookmarkedScene()
    {
      EditorGUILayout.BeginHorizontal();
      sceneToAdd = EditorGUILayout.ObjectField(sceneToAdd, typeof(SceneAsset), false) as SceneAsset;
      if (GUILayout.Button("Add", EditorStyles.miniButtonRight) && sceneToAdd != null && !bookmarkedScenes.Contains(sceneToAdd))
      {
        bookmarkedScenes.Add(sceneToAdd);
        StratusPreferences.Save();
        sceneToAdd = null;
      }

      EditorGUILayout.EndHorizontal();

    }

    void RemoveBookmarkedScene(SceneAsset scene)
    {
      bookmarkedScenes.Remove(scene);
      StratusPreferences.Save();
      Repaint();
    }


    private void SelectBookmark(UnityEngine.Object obj)
    {
      Selection.activeObject = obj;
    }

    private void OnChange()
    {
      StratusPreferences.Save();
      Repaint();
    }

    //[PostProcessScene]
    //private static void OnPostProcessScene()
    //{
    //  //#if UNITY_EDITOR
    //  //#else
    //  //  RemoveGameObjectBookmarks();
    //  //#endif
    //}
    //
    //private static void RemoveGameObjectBookmarks()
    //{
    //  GameObjectBookmark[] bookmarks = FindObjectsOfType<GameObjectBookmark>();
    //  foreach (var bookmark in bookmarks)
    //    DestroyImmediate(bookmark);
    //}
        







  }
}