using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;

namespace Stratus
{
  /// <summary>
  /// Class that listens to Unity's asset pipeline events
  /// </summary>
  public static class StratusAssetProcessor //: UnityEditor.AssetModificationProcessor
  {
    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/  
    static StratusAssetProcessor()
    {
      EditorSceneManager.sceneSaving += OnSceneSaving;
      Undo.postprocessModifications += OnPostProcessModifications;
    }

    //private void OnWillCreateAsset(string assetName)
    //{
    //  Trace.Script($"Creating {assetName}");
    //}
    //
    //private void OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
    //{
    //  Trace.Script($"Deleting {assetName}");
    //}

    /// <summary>
    /// Invoked just after building the scene, and when entering playmode
    /// </summary>
    [PostProcessScene]
    public static void OnPostProcessScene()
    {
      if (UnityEditor.BuildPipeline.isBuildingPlayer)
      {
        Trace.Script($"Removing all instances of:");
        GameObjectBookmark.RemoveAll();
        
      }
      else
      {
        //Trace.Script($"In Editor. Not removing!");
      }
    }

    /// <summary>
    /// Invoked just after building the player
    /// </summary>
    /// <param name="target"></param>
    /// <param name="pathToBuiltProject"></param>
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
      //Debug.Log(pathToBuiltProject);
    }

    static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
    {
      foreach(UndoPropertyModification modification in propertyModifications)
      {
        Trace.Script($"Modified {modification.currentValue.target.name}");
      }
      return propertyModifications;
    }

    private static void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
    {
      Trace.Script($"Saving {scene.name}");
    }

  }

  

}