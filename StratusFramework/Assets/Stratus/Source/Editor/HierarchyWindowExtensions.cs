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
  public static class HierarchyWindowExtensions
  {
    [MenuItem("GameObject/Create Folder", false, 0)]
    static void MakeFolder()
    {
      var go = new GameObject("Folder");
      go.transform.localPosition = Vector3.zero;
      Undo.RegisterCreatedObjectUndo(go, "Create Folder");
      Selection.activeGameObject = go;
    }
    

    [MenuItem("GameObject/Group Selected %g", false, 0)]
    private static void GroupSelected(MenuCommand command)
    {
      if (!Selection.activeTransform)
        return;
           

      // Prevent executing multiple times when right-clicking
      if (Selection.objects.Length > 1)
      {  
        if (command.context != Selection.objects[0])
        {
          //Trace.Script("That's not the selected object!");
          return;
        }
      }
         
      var go = new GameObject(Selection.activeTransform.name + " Group");
      Undo.RegisterCreatedObjectUndo(go, "Group Selected");
      go.transform.SetParent(Selection.activeTransform.parent, false);
      foreach (var transform in Selection.transforms)
        Undo.SetTransformParent(transform, go.transform, "Group Selected");
      Selection.activeGameObject = go;

      Rename();     
      
    }

    private static void PromptRename()
    {
    }

    private static void Rename()
    {
      var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
      var hierarchyWindow = EditorWindow.GetWindow(type);
      var rename = type.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
      rename.Invoke(hierarchyWindow, null);
    }

    



  }
}
