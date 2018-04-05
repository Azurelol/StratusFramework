using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Stratus.Utilities;
using Stratus.Editor;

namespace Stratus
{
  public static partial class Extensions
  {
    public static T AddInstanceToAsset<T>(this ScriptableObject assetObject) where T : ScriptableObject
    {
      return Assets.AddInstanceToAsset<T>(assetObject);
    }

    public static ScriptableObject AddInstanceToAsset(this ScriptableObject assetObject, Type type)
    {
      return Assets.AddInstanceToAsset(assetObject, type);
    }

  }

}