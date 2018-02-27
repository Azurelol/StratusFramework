/******************************************************************************/
/*!
@file   CreateFolder.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Stratus
{
  /// <summary>
  /// Useful extensions to Unity's hierarchy window
  /// </summary>
  public static class HierarchyWindowExtensions
  {
    //------------------------------------------------------------------------/
    // Methods: Menu Items
    //------------------------------------------------------------------------/
    [MenuItem("GameObject/Transform/Reset Selected", false, 0)]
    private static void Reset(MenuCommand command)
    {
      Undo.RecordObjects(Selection.transforms, "Reset Selected");
      foreach (var t in Selection.transforms)
      {
        var children = t.Children();

        foreach (var child in children)
          child.SetParent(null);

        t.position = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        foreach (var child in children)
          child.SetParent(t, true);
      }
    }

    [MenuItem("GameObject/Transform/Group Selected %g", false, 0)]
    private static void GroupSelected(MenuCommand command)
    {
      if (!Selection.activeTransform)
      {
        Trace.Script("No transform selected");
        return;
      }

      // Prevent executing multiple times when right-clicking
      if (command.context != null)
      {
        var go = command.context as GameObject;
        Transform topTransform = Selection.transforms[0];
        if (go.transform != Selection.transforms[0])
        {
          Trace.Script($"{go.name} != {topTransform}");
          return;
        }
        Trace.Script($"{go.name} = {topTransform}");
      }
      
      
      GroupSelected(Selection.transforms);
    }

    [MenuItem("GameObject/Transform/Replace Selected", false, 0)]
    private static void ReplaceSelectedFromHierarchy(MenuCommand command)
    {
      if (!Selection.activeTransform)
        return;

      GameObjectReplaceWizard.Open(Selection.gameObjects);
    }


    [MenuItem("GameObject/Transform/Detach Children From Selected", false, 0)]
    private static void DetachChildren(MenuCommand command)
    {
      foreach (var parent in Selection.transforms)
      {
        var children = parent.Children();
        var index = parent.GetSiblingIndex();
        foreach (var child in children)
        {
          Undo.SetTransformParent(child, parent.parent, child.name);
          child.SetSiblingIndex(index++);
        }
      }
    }

    [MenuItem("GameObject/Transform/Unparent Selected", false, 0)]
    private static void Unparent(MenuCommand command)
    {
      foreach (var transform in Selection.transforms)
      {
        var parent = transform.parent;
        Undo.SetTransformParent(transform, null, transform.name);
        transform.SetSiblingIndex(parent.GetSiblingIndex());
      }
    }

    [MenuItem("GameObject/Transform/Center Selected On Another", false, 0)]
    private static void CenterOnAnother(MenuCommand command)
    {
      var transform = Selection.activeTransform;
      //Undo.RecordObject(transform, transform.name);
      var selectionMenu = new GenericMenu();
      foreach(var go in Scene.activeScene.gameObjects)
      {
        selectionMenu.AddItem(new GUIContent(go.name), true, () => { Trace.Script("Centering on " + go.name); });
      }
      selectionMenu.ShowAsContext();
    }
        
    [MenuItem("GameObject/Transform/Snap Selected to Ground #END", false, 0)]
    private static void SnapToGround(MenuCommand command)
    {
      Undo.RecordObjects(Selection.transforms, "Snap Selected to Ground");
      Snap(Selection.transforms, Vector3.down);
    }

    //------------------------------------------------------------------------/
    // Methods: Private
    //------------------------------------------------------------------------/
    private static void Rename()
    {
      var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
      var hierarchyWindow = EditorWindow.GetWindow(type);
      var rename = type.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
      rename.Invoke(hierarchyWindow, null);
    }

    private static void RenamebyEvent()
    {
      var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
      var hierarchyWindow = EditorWindow.GetWindow(type);
      var e = new UnityEngine.Event { keyCode = KeyCode.F2, type = EventType.KeyDown }; // or Event.KeyboardEvent("f2");
      hierarchyWindow.SendEvent(e);
      //EditorWindow.focusedWindow.SendEvent(e);
    }

    private static void Snap(Transform[] transforms, Vector3 direction)
    {
      foreach (Transform t in transforms)
      {
        RaycastHit rayhit;
        if (Physics.Raycast(t.position, direction, out rayhit))
        {
          Vector3 offset = Vector3.zero;
          MeshRenderer renderer = t.GetComponentInChildren<MeshRenderer>();
          if (renderer != null)
          {
            bool hasPivot = renderer.bounds.center.y - renderer.bounds.extents.y < t.position.y;
            if (hasPivot)
              offset = new Vector3(0f, renderer.bounds.extents.y, 0f);
          }

          t.position = rayhit.point + offset;
        }
      }
    }

    private static void GroupSelected(Transform[] transforms)
    {
      Transform first = transforms[0];
      var go = new GameObject(first.name + " Group");
      go.transform.SetParent(first.parent);

      Undo.IncrementCurrentGroup();
      Undo.SetCurrentGroupName("Group Selected");
      int undoIndex = Undo.GetCurrentGroup();

      Undo.RegisterCreatedObjectUndo(go, go.name);
      //go.transform.SetParent(Selection.activeTransform.parent, false);
      foreach (var transform in Selection.transforms)
        Undo.SetTransformParent(transform, go.transform, "Group Selected");
      Selection.activeGameObject = go;
      RenamebyEvent();

      Undo.CollapseUndoOperations(undoIndex);
    }

    static void UndoableOperation(string undoGroupName, System.Action func)
    {
      Undo.IncrementCurrentGroup();
      Undo.SetCurrentGroupName(undoGroupName);
      var undoIndex = Undo.GetCurrentGroup();
      {
        func();
      }
      Undo.CollapseUndoOperations(undoIndex);
    }





  }
}
