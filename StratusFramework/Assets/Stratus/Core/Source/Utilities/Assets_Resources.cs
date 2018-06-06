using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Utilities;
using System.IO;

namespace Stratus
{
  /// <summary>
  /// Utility class for managing assets at runtime
  /// </summary>
  public static partial class Assets
  {

    /// <summary>
    /// Currently loaded resources
    /// </summary>
    private static Dictionary<Type, Dictionary<string, UnityEngine.Object>> loadedResources = new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();

    public static T LoadResource<T>(string name) where T : UnityEngine.Object
    {
      Type type = typeof(T);
      if (!loadedResources.ContainsKey(type))
        AddResourcesOfType<T>();

      T resource = (T)loadedResources[type][name];
      return resource;
    }

    private static void AddResourcesOfType<T>() where T : UnityEngine.Object
    {
      Type type = typeof(T);
      loadedResources.Add(type, new Dictionary<string, UnityEngine.Object>());
      T[] resources = Resources.FindObjectsOfTypeAll<T>();
      foreach (var resource in resources)
      {
        Trace.Script($"Loading {resource.name}");
        loadedResources[type].Add(resource.name, resource);
      }

    }

    private static void AddResourcesOfType(Type type)
    {
      loadedResources.Add(type, new Dictionary<string, UnityEngine.Object>());
      UnityEngine.Object[] resources = Resources.FindObjectsOfTypeAll(type);
      foreach (var resource in resources)
      {
        loadedResources[type].Add(resource.name, resource);
      }

    }





  }
}