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

    [PostProcessScene]
    public static void OnPostProcessScene()
    {

    }

  }

  

}