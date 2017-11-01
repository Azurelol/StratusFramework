using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using System.Linq;

namespace Stratus
{
  namespace Utilities
  {
    public static partial class Assets
    {
      /// <summary>
      /// The symbol used to separate folders by Unity's API
      /// </summary>
      public const char UnityDirectorySeparator = '/';
      /// <summary>
      /// The name of the resources folder
      /// </summary>
      public const string ResourcesFolderName = "Resources";

      /// <summary>
      /// Loads the save data from an Unity relative path. Returns null if the data doesn't exist.
      /// </summary>
      /// <typeparam name="T">The type of ScriptableObject</typeparam>
      /// <param name="path">The relative path to the asset. (e.g: "Assets/Resources/MyFile.asset")</param>
      /// <returns>The saved data as a ScriptableObject.</returns>
      public static T LoadSaveData<T>(string path) where T : ScriptableObject
      {
        var resourcesFolder = string.Concat(UnityDirectorySeparator, ResourcesFolderName, UnityDirectorySeparator);
        if (!path.Contains(resourcesFolder))
        {
          var exceptionMessage = string.Format(
            "Failed to load ScriptableObject of type, {0}, from path: {1}. " +
            "Path must begin with Assets and include a directory within the Resources folder.",
            typeof(T).ToString(),
            path);
          throw new UnityException(exceptionMessage);
        }

        // Mkae sure we are using a relative path
        var resourceRelativePath = MakeRelative(path);
        // Remove the file extension
        var fileExtension = Path.GetExtension(path);
        resourceRelativePath = resourceRelativePath.Replace(fileExtension, string.Empty);
        //Trace.Script("Loading data from " + resourceRelativePath);
        return Resources.Load<T>(resourceRelativePath);

      }

      /// <summary>
      /// Loads the saved data, stored as a ScriptableObject at the specified path. If
      /// the file or folders don't exist, it creates them.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="path"></param>
      /// <returns></returns>
      public static T LoadOrCreateSaveData<T>(string path) where T : ScriptableObject
      {
        T data = null;

        // Try loading it at the specified path
        if (data == null)
        {
          data = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        // Try finding it anywhere
        if (data == null)
        {
          var objs = FindAndLoadAssetsByType<T>();
          if (objs.Length > 0)
            data = objs.First();
        }

        // Try creating it
        if (data == null)
        {
          Trace.Script(path + " has not been saved, creating it!");
          data = ScriptableObject.CreateInstance<T>();
          CreateAssetAndDirectories(data, path);
          AssetDatabase.SaveAssets();
        }

        return data;
      }

      /// <summary>
      /// Adds an instance of the specified ScriptableObject class as a child
      /// to the parent ScriptableObject
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="assetObject"></param>
      /// <returns></returns>
      public static T AddInstanceToAsset<T>(UnityEngine.Object assetObject) where T : ScriptableObject
      {
        var instance = ScriptableObject.CreateInstance<T>();
        instance.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(instance, assetObject);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return instance;
      }

      /// <summary>
      /// Adds an instance of the specified ScriptableObject class as a child
      /// to the parent ScriptableObject
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="assetObject"></param>
      /// <returns></returns>
      public static ScriptableObject AddInstanceToAsset(UnityEngine.Object assetObject, Type type)
      {
        var instance = ScriptableObject.CreateInstance(type);
        instance.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(instance, assetObject);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return instance;
      }

      /// <summary>
      /// Returns all assets of a specified type (loading them if necessary)
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns></returns>
      public static T[] FindAndLoadAssetsByType<T>() where T : ScriptableObject
      {
        List<T> assets = new List<T>();
        var typeName = typeof(T).ToString().Replace("UnityEngine.", "");
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeName));
        for (int i = 0; i < guids.Length; ++i)
        {
          string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
          T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
          if (asset != null)
          {
            assets.Add(asset);
          }
        }
        return assets.ToArray();

      }

    }

  }

}