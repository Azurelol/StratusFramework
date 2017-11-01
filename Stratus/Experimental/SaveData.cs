using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Utilities;
using System.IO;

namespace Stratus
{
  /// <summary>
  /// An abstract class for handling runtime save-data. Useful for player profiles, etc...
  /// </summary>
  [Serializable]
  public abstract class SaveData
  {
    //--------------------------------------------------------------------------------------------/
    // Classes
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Specifies what suffixes to add to the save file name
    /// </summary>
    public enum SuffixFormat
    {
      Incremental,
      SystemTime
    }

    //--------------------------------------------------------------------------------------------/
    // Attributes
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// A required attribute that specifies the wanted folder path and name for a savedata asset
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SaveDataAttribute : Attribute
    {
      /// <summary>
      /// The folder relative to the application's persistent data path where you want this data stored
      /// </summary>
      public string folder { get; set; }
      /// <summary>
      /// What naming convention to use for a save file of this type
      /// </summary>
      public string namingConvention { get; set; } // = typeof(SaveData).DeclaringType.Name;
      /// <summary>
      /// What suffix to use for a save file of this type
      /// </summary>
      public SuffixFormat suffix { get; set; } = SuffixFormat.Incremental;
      /// <summary>
      /// What extension format to use for a save file of this type
      /// </summary>
      public string extension { get; set; } = ".sav";

      public class MissingException : Exception
      {
        public MissingException(string className) : base("The class declaration for " + className + " is missing the [SaveData] attribute, which provides the path information needed in order to construct the asset.")
        {
          // Fill later?
          this.HelpLink = "http://msdn.microsoft.com";
          this.Source = "Exception_Class_Samples";
        }
      }
    }

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The JSON representation of this data
    /// </summary>
    public string json { get { return JsonUtility.ToJson(this, true); } }

    /// <summary>
    /// The character used to separate directories in Unity
    /// </summary>
    public static char DirectorySeparatorChar { get; } = '/';

    /// <summary>
    /// Whether this SaveData has been saved to disk
    /// </summary>
    public bool isSaved { get; protected set; } = false;

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The time at which this save was made
    /// </summary>
    [HideInInspector]
    public string time;
  }

  /// <summary>
  /// An abstract class for handling runtime save-data. Useful for player profiles, etc...
  /// </summary>
  public abstract class SaveData<T> : SaveData
  {
    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The folder inside the relative path to this asset
    /// </summary>
    public static string namingConvention { get; } = attribute.GetProperty<string>("namingConvention");

    /// <summary>
    /// The folder inside the relative path to this asset
    /// </summary>
    public static string folder { get; } = attribute.GetProperty<string>("folder");

    /// <summary>
    /// The extension used by this save data
    /// </summary>
    public static string extension { get; } = attribute.GetProperty<string>("extension");

    /// <summary>
    /// The extension used by this save data
    /// </summary>
    public static SuffixFormat suffix { get; } = attribute.GetProperty<SuffixFormat>("suffix");

    /// <summary>
    /// The persistent data path that Unity is using
    /// </summary>
    public static string relativePath => Application.persistentDataPath;

    /// <summary>
    /// The path to the directory being used by this save data, as specified by the [SaveData] attribute
    /// </summary>
    public static string defaultPath
    {
      get
      {
        var path = relativePath;
        if (folder != null)
          path += DirectorySeparatorChar + folder;
        path += DirectorySeparatorChar;
        return path;
      }
    }

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

    private static SaveDataAttribute attribute_;

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/    
    /// <summary>
    /// Saves the data to the default path in the application's persistent path
    /// using the default naming convention
    /// </summary>
    public static void Save(SaveData<T> saveData)
    {
      Save(saveData, ComposeName(suffix));
    }
    
    /// <summary>
    /// Saves the data to the default path in the application's persistent path
    /// using the specified filename
    /// </summary>
    public static void Save(SaveData<T> saveData, string name)
    {
      // Compose the path
      var fileName = name + extension;
      var fullPath = defaultPath + fileName;

      // Create the directory if missing
      if (!Directory.Exists(defaultPath))
        Directory.CreateDirectory(defaultPath);

      Serialize(saveData, fullPath);
    }

    /// <summary>
    /// Saves the data to the specified folder in the application's persistent path
    /// using the specified filename
    /// </summary>
    public static void Save(SaveData<T> saveData, string name, string folderName)
    {
      // Compose the path
      var fileName = name + extension;
      var path = relativePath + DirectorySeparatorChar + folderName + DirectorySeparatorChar;
      var fullPath = path + fileName;

      // Create the directory if missing
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      Serialize(saveData, fullPath);
    }

    /// <summary>
    /// Loads a save data file from the default folder and returns it
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Load(string name)
    {
      var fileName = name + extension;
      var fullPath = defaultPath + fileName;

      return Deserialize(fullPath);
    }

    /// <summary>
    /// Loads a save data file from the given folder and returns it
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Load(string name, string folderName)
    {
      var fullPath = ComposePath(name, folderName);
      return Deserialize(fullPath);
    }

    /// <summary>
    /// Deletes the save file at the specified folder
    /// </summary>
    /// <param name="name"></param>
    /// <param name="folderName"></param>
    public static void Delete(string name, string folderName)
    {
      var fullPath = ComposePath(name, folderName);
      File.Delete(fullPath);
    }

    /// <summary>
    /// Deletes the save file at the default folder
    /// </summary>
    /// <param name="name"></param>
    /// <param name="folderName"></param>
    public static void Delete(string name)
    {
      var fileName = name + extension;
      var fullPath = defaultPath + fileName;
      File.Delete(fullPath);
    }

    /// <summary>
    /// Checks whether the specified file exists in the specified folder
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string name, string folder)
    {
      var fullPath = ComposePath(name, folder);
      return File.Exists(fullPath);
    }

    /// <summary>
    /// Checks whether the specified file exists in the default folder
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string name)
    {
      var fileName = name + extension;
      var fullPath = defaultPath + fileName;
      return File.Exists(fullPath);
    }

    /// <summary>
    /// Performs the serialization operation
    /// </summary>
    /// <param name="saveData"></param>
    /// <param name="fullPath"></param>
    private static void Serialize(SaveData<T> saveData, string fullPath)
    {
      // Update the time to save at
      saveData.time = DateTime.Now.ToString();
      // Write to disk
      File.WriteAllText(fullPath, saveData.json);
      // Note that it has been saved
      saveData.isSaved = true;
    }

    /// <summary>
    /// Performs the deserialization operation
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>

    public static T Deserialize(string fullPath)
    {
      if (!File.Exists(fullPath))
        throw new FileNotFoundException("The file was not found!");

      string data = File.ReadAllText(fullPath);
      T saveData = JsonUtility.FromJson<T>(data);
      return saveData;
    }

    /// <summary>
    /// Composes a valid path given the name of a file and the folder it's on
    /// </summary>
    /// <param name="name"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    private static string ComposePath(string name, string folderName)
    {
      var fileName = name + extension;
      var path = relativePath + DirectorySeparatorChar + folderName + DirectorySeparatorChar;
      var fullPath = path + fileName;
      return fullPath;
    }

    /// <summary>
    /// Composes a default save data name
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    private static string ComposeName(SuffixFormat format)
    {
      string name = namingConvention;
      switch (format)
      {
        case SuffixFormat.Incremental:
          name += "_" + count;
          break;
        default:
          break;
      }
      return name;
    }



  }

}