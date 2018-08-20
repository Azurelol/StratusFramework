using System;
using UnityEngine;

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