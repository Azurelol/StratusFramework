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
  public abstract class JsonSaveData
  {
    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The JSON representation of this data
    /// </summary>
    public string json { get { return JsonUtility.ToJson(this, true); } }

    /// <summary>
    /// Whether this SaveData has been saved to disk
    /// </summary>
    public bool isSaved { get; protected set; } = false;

    /// <summary>
    /// The persistent data path that Unity is using
    /// </summary>
    public static string relativePath => Application.persistentDataPath;

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The time at which this save was made
    /// </summary>
    public string name;

    /// <summary>
    /// The time at which this save was made
    /// </summary>
    public string time;

    //--------------------------------------------------------------------------------------------/
    // Virtual
    //--------------------------------------------------------------------------------------------/    
    protected virtual void OnSave() { }
    protected virtual bool OnLoad() => true;
  }

  /// <summary>
  /// An abstract class for handling runtime save-data. Useful for player profiles, etc...
  /// </summary>
  public abstract class JsonSaveData<T> : JsonSaveData where T : JsonSaveData
  {
    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
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


    //public JsonSaveData(string name)
    //{
    //  this.name = name;
    //}

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/    
    ///// <summary>
    ///// Saves the data to the default path in the application's persistent path
    ///// using the default naming convention
    ///// </summary>
    //public static void Save(JsonSaveData<T> saveData)
    //{
    //  Save(saveData, ComposeName(suffix));
    //}

    /// <summary>
    /// Saves the data to the default path in the application's persistent path
    /// using the specified filename
    /// </summary>
    public static void Save(JsonSaveData<T> saveData, string name)
    {
      saveData.name = name;

      // Compose the path
      string fileName = saveData.name + extension;
      string fullPath = defaultPath + fileName;

      // Create the directory if missing
      if (!Directory.Exists(defaultPath))
        Directory.CreateDirectory(defaultPath);

      Serialize(saveData, fullPath);
    }

    /// <summary>
    /// Saves the data to the specified folder in the application's persistent path
    /// using the specified filename
    /// </summary>
    public static void Save(JsonSaveData<T> saveData, string name, string folderName)
    {
      saveData.name = name;

      // Compose the path
      var fileName = saveData.name + extension;
      var path = relativePath + SaveDataUtility.directorySeparatorChar + folderName + SaveDataUtility.directorySeparatorChar;
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
      var fullPath = SaveDataUtility.ComposePath(relativePath, folderName, name, extension);
      return Deserialize(fullPath);
    }

    /// <summary>
    /// Deletes the save file at the specified folder
    /// </summary>
    /// <param name="name"></param>
    /// <param name="folderName"></param>
    public static bool Delete(string name, string folderName)
    {
      if (!Exists(name, folderName))
        return false;

      var fullPath = SaveDataUtility.ComposePath(relativePath, folderName, name, extension);
      //Trace.Script($"Deleting {fullPath}");
      File.Delete(fullPath);
      return true;
    }

    /// <summary>
    /// Deletes the save file at the default folder
    /// </summary>
    /// <param name="name"></param>
    /// <param name="folderName"></param>
    public static bool Delete(string name)
    {
      if (!Exists(name, defaultPath))
        return false;

      var fileName = name + extension;
      var fullPath = defaultPath + fileName;
      File.Delete(fullPath);
      return true;
    }

    /// <summary>
    /// Deletes this save file at its folder
    /// </summary>
    public void Delete()
    {
      Delete(name);
    }



    ///// <summary>
    ///// Checks whether the specified file exists in the default folder
    ///// </summary>
    ///// <param name="path"></param>
    ///// <returns></returns>
    //public static bool Exists(string name)
    //{
    //  var fileName = name + extension;
    //  var fullPath = defaultPath + fileName;
    //  return File.Exists(fullPath);
    //}

    

    /// <summary>
    /// Performs the serialization operation
    /// </summary>
    /// <param name="saveData"></param>
    /// <param name="fullPath"></param>
    private static void Serialize(JsonSaveData<T> saveData, string fullPath)
    {
      // Update the time to save at
      saveData.time = DateTime.Now.ToString();
      // Call a customized function before writing to disk
      saveData.OnSave();
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
      //JsonSaveData<T> saveData1 = default(JsonSaveData<T>);
      JsonUtility.FromJsonOverwrite(data, saveData);
      // Call a customized function on loading
      var jsd = (JsonSaveData<T>)(object)saveData;
      jsd.OnLoad();
      return saveData;
      //return saveData1 as T;
    }

    public static string ComposeName(SaveDataSuffixFormat format) => SaveDataUtility.ComposeDefaultName(format, namingConvention, count);
    public static string ComposePath(string name, string folder) => SaveDataUtility.ComposePath(relativePath, folder, name, extension);
    public static bool Exists(string name, string folder) => SaveDataUtility.Exists(relativePath, folder, name, extension);





  }

}