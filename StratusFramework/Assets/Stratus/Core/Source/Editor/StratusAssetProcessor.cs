using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Stratus
{
  public class StratusAssetProcessor : UnityEditor.AssetModificationProcessor
  {
    private void OnWillCreateAsset(string assetName)
    {
      Trace.Script($"Creating {assetName}");
    }

    private void OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
    {
      Trace.Script($"Deleting {assetName}");
    }

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

  }

  

}