using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// History:
// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
//  March 2010: Modified by Kristian Helle Jespersen
//  June 2011: Modified by Connor Cadellin McKee for Excamedia
//  April 2015: Modified by Fernando Medina (fermmmm)
//  April 2015: Modified by Julien Tonsuso (www.julientonsuso.com)
//  July 2015: Changed into editor window and added instant preview in scene view
//  Modified by Alex Dovgodko: June 2017, Made changes to make things work with Unity 5.6.1
// July 2017: 

namespace Stratus
{
  /// <summary>
  /// An editor tools that allows a GameObject (or several)  to be replaceed in the scene hierarchy to be replaced
  /// by another GameObject in the scene (or a prefab)
  /// </summary>  
  public class GameObjectReplaceWizard : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/    
    // Options
    public GameObject prefab;
    public static bool keepNames { get; set; }
    public static bool mantainHierarchy { get; set; }
    public static bool maintainScale { get; set; }
    public static bool maintainRotation { get; set; }

    private SerializedProperty prefabProperty;
    private GameObject[] selectedObjects { get; set; }
    private Vector2 selectedObjectsScrollPosition;

    //------------------------------------------------------------------------/
    // Menu, Hierarchy
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Windows/GameObject Replace Wizard")]
    private static void Open() 
    {
      EditorWindow.GetWindow(typeof(GameObjectReplaceWizard), true, "GameObject Replace Wizard");
    }

    public static void Open(GameObject[] selectedObjs)
    {
      var window = EditorWindow.GetWindow(typeof(GameObjectReplaceWizard), true, "GameObject Replace Wizard") as GameObjectReplaceWizard;
      window.selectedObjects = selectedObjs;
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public void OnEnable()
    {
      var a = new SerializedObject(this);
      prefabProperty = a.FindProperty("prefab");
    }

    float CalculateWidth(GUIStyle style, string text)
    {
      var size = style.CalcSize(new GUIContent(text));
      return size.x;
    }

    public void OnGUI()
    {
      ShowOptions();
      selectedObjects = Selection.gameObjects;
      ShowSelectedObjects();
      ShowReplaceButton();
    }

    //------------------------------------------------------------------------/
    // Methods: GUI
    //------------------------------------------------------------------------/
    private void ShowToolbar()
    {
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
      {
        // Select prefab
        {
          EditorGUIUtility.labelWidth = CalculateWidth(EditorStyles.label, "Prefab") + 5f;
          EditorGUILayout.PropertyField(prefabProperty);
        }
      }
      EditorGUILayout.EndHorizontal();
    }

    private void ShowOptions()
    {
      EditorGUILayout.LabelField("Options", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
      EditorGUILayout.Separator();
      {
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;

        EditorGUILayout.BeginHorizontal();
        {
          EditorGUILayout.BeginVertical();
          keepNames = EditorGUILayout.Toggle(new GUIContent("Keep Original Names"), keepNames);
          mantainHierarchy = EditorGUILayout.Toggle(new GUIContent("Mantain Hierarchy"), mantainHierarchy);

          EditorGUILayout.EndVertical();

          if (prefab != null)
            ShowPreview();
        }
        EditorGUILayout.EndHorizontal();

      }
      EditorGUILayout.Separator();
    }

    private void ShowPreview()
    {
      var previewTexture = AssetPreview.GetAssetPreview(prefab);
      if (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()))
        return;

      GUILayout.Box(previewTexture);
    }

    private void ShowSelectedObjects()
    {
      EditorGUILayout.LabelField("Selected Objects", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
      EditorGUILayout.Separator();
      ShowList(selectedObjects, ref selectedObjectsScrollPosition);
    }

    private void ShowReplaceButton()
    {
      if (GUILayout.Button("Replace"))
      {
        if (prefab != null)
        {
          ReplaceSelected(selectedObjects, prefab, keepNames, mantainHierarchy, true);
          selectedObjects = null;
          this.Close();
        }
      }
    }

    private void ShowList(GameObject[] objs, ref Vector2 scrollPosition)
    {
      if (objs == null)
        return;

      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
      foreach (var go in objs)
      {
        EditorGUILayout.LabelField(go.name);
      }
      EditorGUILayout.EndScrollView();
    }

    //------------------------------------------------------------------------/
    // Methods: Replacement
    //------------------------------------------------------------------------/
    private static void ReplaceSelected(GameObject[] originals, GameObject prefab, bool keepNames, bool respectHierarchy, bool verbose = false)
    {
      if (verbose)
        Trace.Script("Now replacing " + originals.Length + " objects!");

      Undo.IncrementCurrentGroup();
      Undo.SetCurrentGroupName(typeof(GameObjectReplaceWizard).Name);
      var undoIndex = Undo.GetCurrentGroup();
      {
        for (var index = 0; index < originals.Length; index++)
        {
          Replace(originals[index], prefab, keepNames, mantainHierarchy);
        }
      }
      Undo.CollapseUndoOperations(undoIndex);
    }

    private static GameObject Replace(GameObject original, GameObject prefab, bool keepName, bool respectHierarchy)
    {
      var newObject = PrefabUtility.InstantiatePrefab(prefab, original.scene) as GameObject;

      if (keepName)
        newObject.name = original.name;

      if (respectHierarchy)
        newObject.transform.SetParent(original.transform.parent);

      // Copy the transform
      newObject.transform.CopyFrom(original.transform);

      Undo.RegisterCreatedObjectUndo(newObject, original.name + " replaced");
      Undo.DestroyObjectImmediate(original);

      return newObject;
    }


  }

}