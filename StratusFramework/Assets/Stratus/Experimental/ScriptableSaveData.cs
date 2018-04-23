using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Utilities;
using System.IO;
using System;

namespace Stratus.Experimental
{
  /// <summary>
  /// An abstract class for handling runtime save-data using scriptable objects,
  /// which are necessary when needing to keep object references
  /// </summary>
  public abstract class ScriptableSaveData : StratusScriptable
  {
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The time at which this save was made
    /// </summary>
    [HideInInspector]
    public string time;


    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The persistent data path that Unity is using
    /// </summary>
    public static string relativePath => Application.dataPath;

  }

  /// <summary>
  /// An abstract class for handling runtime save-data using scriptable objects,
  /// which are necessary when needing to keep object references
  /// </summary>
  public abstract class ScriptableSaveData<T> : ScriptableSaveData where T : ScriptableObject
  {
    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The attribute containing data about this class
    /// </summary>
    private static SaveDataAttribute attribute //; { get; } = AttributeUtility.FindAttribute<SaveDataAttribute>(typeof(T));
    {
      get
      {
        if (attribute_ == null)
        {
          var type = typeof(T);
          attribute_ = AttributeUtility.FindAttribute<SaveDataAttribute>(type);
          if (attribute_ == null)
            throw new SaveDataAttribute.MissingException(type.Name);
        }
        return attribute_;
      }
    }

    /// <summary>
    /// The folder inside the relative path to this asset
    /// </summary>
    public static string namingConvention { get; } = attribute.GetProperty<string>(nameof(SaveDataAttribute.namingConvention));

    /// <summary>
    /// The folder inside the relative path to this asset
    /// </summary>
    public static string folder { get; } = attribute.GetProperty<string>(nameof(SaveDataAttribute.folder));

    /// <summary>
    /// The extension used by this save data
    /// </summary>
    public static string extension { get; } = attribute.GetProperty<string>(nameof(SaveDataAttribute.extension));

    /// <summary>
    /// The extension used by this save data
    /// </summary>
    public static SaveDataSuffixFormat suffix { get; } = attribute.GetProperty<SaveDataSuffixFormat>(nameof(SaveDataAttribute.suffix));

    /// <summary>
    /// The path to the directory being used by this save data, as specified by the [SaveData] attribute
    /// </summary>
    public static string defaultPath => SaveDataUtility.ComposeDefaultPath(relativePath, folder);

    /// <summary>
    /// The number of save files present in the specified folder for save data
    /// </summary>
    public static int count => files != null ? files.Length : 0;

    /// <summary>
    /// Returns all instances of the save data from the path
    /// </summary>
    public static string[] files
    {
      get
      {
        // If the directory does not exist yet..
        if (!Directory.Exists(defaultPath))
          return null;

        // Look at the files matching the extension in the given folder
        var saves = new List<string>();
        var files = Directory.GetFiles(defaultPath);
        foreach (var file in files)
        {
          string fileExtension = Path.GetExtension(file);
          if (fileExtension == extension)
            saves.Add(file);
        }

        if (saves.Count > 0)
          return saves.ToArray();
        return null;
      }
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Underlying field for the attribute property
    /// </summary>
    protected static SaveDataAttribute attribute_;

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    ///// <summary>
    ///// Deletes the save file at the default folder
    ///// </summary>
    ///// <param name="name"></param>
    ///// <param name="folderName"></param>
    //public static bool Delete(string name)
    //{
    //  if (!Exists(name, defaultPath))
    //    return false;
    //
    //  var fileName = name + extension;
    //  var fullPath = defaultPath + fileName;
    //  //ScriptableSaveData<T> saveData = Resources.Load()
    //  //Destroy(saveData);
    //  //File.Delete(fullPath);
    //  return true;
    //}

    ///// <summary>
    ///// Saves the data to the default path in the application's persistent path
    ///// using the specified filename
    ///// </summary>
    //public static void Save(JsonSaveData<T> saveData, string name)
    //{
    //  // Compose the path
    //  var fileName = name + extension;
    //  var fullPath = defaultPath + fileName;
    //
    //  // Create the directory if missing
    //  if (!Directory.Exists(defaultPath))
    //    Directory.CreateDirectory(defaultPath);
    //
    //  //Serialize(saveData, fullPath);
    //}


    ///// <summary>
    ///// Loads a save data file from the given folder and returns it
    ///// </summary>
    ///// <param name="path"></param>
    ///// <returns></returns>
    //public static T Load(string name)
    //{
    //  string folderPath = IO.GetFolderPath(folder);
    //  if (folderPath == null)
    //    throw new NullReferenceException("The given folder path '" + folder + "' to be used for the asset '" + name + "' could not be found!");
    //
    //  string assetPath = folderPath + SaveDataUtility.directorySeparatorChar + name + extension;
    //  T saveData = Resources.Load<T>(assetPath);
    //  return saveData;
    //}

    public static bool Exists(string name, string folder) => SaveDataUtility.Exists(name, relativePath, folder, extension);
    public static string ComposeName(SaveDataSuffixFormat format) => SaveDataUtility.ComposeDefaultName(format, namingConvention, count);

  }

}