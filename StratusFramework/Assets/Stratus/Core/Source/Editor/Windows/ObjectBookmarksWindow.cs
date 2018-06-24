using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

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
    public class SceneBookmarks
    {
      public SceneField scene;
      public List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
    }

    [Serializable]
    public class ObjectBookmarks
    {
      public List<UnityEngine.Object> projectBookmarks = new List<UnityEngine.Object>();
      public List<SceneBookmarks> sceneBookmarks = new List<SceneBookmarks>();

      //public void AddAsset(UnityEngine.Object asset)
      //{
      //  projectBookmarks.Add(asset);
      //}
      //
      //public void AddGameObject(UnityEngine.Object gameObject)
      //{
      //  SceneBookmarks currentSceneBookmarks = GetActiveSceneBookmark();
      //  projectBookmarks.Add(asset);
      //}

      public SceneBookmarks GetActiveSceneBookmark()
      {
        SceneBookmarks nextSceneBookmark = null;
        foreach (var bookmark in sceneBookmarks)
        {
          if (bookmark.scene == Scene.activeScene)
          {
            nextSceneBookmark = bookmark;
            break;
          }
        }

        if (nextSceneBookmark == null)
        {
          nextSceneBookmark = new SceneBookmarks() { scene = Scene.activeScene };
          sceneBookmarks.Add(nextSceneBookmark);
        }

        return nextSceneBookmark;
      }

    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private Vector2 scrollPosition;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public static ObjectBookmarks bookmarks { get; private set; }
    public static SceneBookmarks currentSceneBookmark { get; private set; }
    private static ObjectBookmarksWindow instance { get; set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Windows/Object Bookmarks")]
    public static void Open()
    {
      instance = (ObjectBookmarksWindow)EditorWindow.GetWindow(typeof(ObjectBookmarksWindow), false, "Bookmarks");
    }

    private void OnEnable()
    {
      bookmarks = Preferences.instance.objectBookmarks;
      SetSceneBookmarks();
    }

    private void OnGUI()
    {
      EditorGUILayout.BeginVertical();
      this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, false, false);
      {
        ShowSceneBookmarks();
        EditorGUILayout.Space();
        ShowProjectBookmarks();
      }
      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
    }


    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    [MenuItem("Assets/Bookmark")]
    private static void BookmarkAsset()
    {
      Preferences.instance.objectBookmarks.projectBookmarks.Add(Selection.activeObject);
      Preferences.Save();
    }

    [MenuItem("GameObject/Bookmark", false, 0)]
    private static void BookmarkGameObject()
    {
      currentSceneBookmark = bookmarks.GetActiveSceneBookmark();
      currentSceneBookmark.objects.Add(Selection.activeGameObject);
      Preferences.Save();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void ShowSceneBookmarks()
    {
      GUILayout.Label("Scene Objects", EditorStyles.centeredGreyMiniLabel);
      int indexToRemove = -1;
      for (int b = 0; b < currentSceneBookmark.objects.Count; ++b)
      {
        UnityEngine.Object currentObject = currentSceneBookmark.objects[b];
        currentSceneBookmark.objects[b] = EditorGUILayout.ObjectField(currentObject, currentObject.GetType(), true);
        //if (GUILayout.Button(currentObject.name, EditorStyles.toolbarButton))
        //{
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
          menu.AddItem(new GUIContent("Remove"), false,
            () =>
            {
              Trace.Script($"Removing {currentObject}");
              indexToRemove = b;
                //bookmarks.projectBookmarks.RemoveAt(b);
                //OnChange();
              }
            );
          menu.ShowAsContext();
        },

        null);
      }      

      if (indexToRemove > -1)
      {
        currentSceneBookmark.objects.RemoveAt(indexToRemove);
        OnChange();
      }
    }

    private void ShowProjectBookmarks()
    {
      GUILayout.Label("Project Assets", EditorStyles.centeredGreyMiniLabel);
      int indexToRemove = -1;
      for (int b = 0; b < bookmarks.projectBookmarks.Count; ++b)
      {
        UnityEngine.Object currentObject = bookmarks.projectBookmarks[b];
        bookmarks.projectBookmarks[b] = EditorGUILayout.ObjectField(currentObject, currentObject.GetType(), false);        
        //if (GUILayout.Button(currentObject.name, EditorStyles.objectField))
        //{
        
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
            menu.AddItem(new GUIContent("Remove"), false,
              () =>
              {
                Trace.Script($"Removing {currentObject}");
                indexToRemove = b;
                //bookmarks.projectBookmarks.RemoveAt(b);
                //OnChange();
              }
              );
            menu.ShowAsContext();
          },

          null);
        //}
      }

      if (indexToRemove > -1)
      {

        bookmarks.projectBookmarks.RemoveAt(indexToRemove);
        OnChange();
      }
    }

    private void SetSceneBookmarks()
    {
      currentSceneBookmark = bookmarks.GetActiveSceneBookmark();
    }

    private void SelectBookmark(UnityEngine.Object obj)
    {
      Trace.Script($"Selecting {obj}");
      Selection.activeObject = obj;
    }

    private void OnChange()
    {
      Preferences.Save();
      Repaint();
    }

    





  }
}