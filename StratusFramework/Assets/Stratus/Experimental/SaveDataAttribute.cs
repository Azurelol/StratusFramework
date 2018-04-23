using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Stratus
{
  /// <summary>
  /// Utility funtions for the save data classes
  /// </summary>
  public static class SaveDataUtility
  {
    /// <summary>
    /// The character used to separate directories in Unity
    /// </summary>
    public static char directorySeparatorChar { get; } = '/';

    /// <summary>
    /// Composes a valid path given the name of a file and the folder it's on
    /// </summary>
    /// <param name="name"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public static string ComposePath(string path, string folder, string name, string extension)
    {
      var fileName = name + extension;
      var fullPath = path + directorySeparatorChar + folder + directorySeparatorChar + fileName;      
      return fullPath;
    }

    /// <summary>
    /// Composes the default path for the type of save data
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string ComposeDefaultPath(string relativePath, string folder)
    {
      var path = relativePath;
      if (folder != null)
        path += directorySeparatorChar + folder;
      path += directorySeparatorChar;
      return path;
    }

    /// <summary>
    /// Checks whether the specified file exists in the specified folder
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string path, string folder, string name, string extension)
    {
      var fullPath = SaveDataUtility.ComposePath(path, folder, name, extension);
      return File.Exists(fullPath);
    }

    /// <summary>
    /// Composes a default save data name
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ComposeDefaultName(SaveDataSuffixFormat format, string namingConvention, int count)
    {
      string name = namingConvention;
      switch (format)
      {
        case SaveDataSuffixFormat.Incremental:
          name += "_" + count;
          break;
        default:
          break;
      }
      return name;
    }
  }

  /// <summary>
  /// A required attribute that specifies the wanted folder path and name for a savedata asset
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
  public sealed class SaveDataAttribute : Attribute
  {
    /// <summary>
    /// The path inside the relative path where you want this data stored
    /// </summary>
    public string folder { get; set; }
    /// <summary>
    /// What naming convention to use for a save file of this type
    /// </summary>
    public string namingConvention { get; set; } // = typeof(SaveData).DeclaringType.Name;                                                 
    /// <summary>                                                 
    /// What suffix to use for a save file of this type
    /// </summary>
    public SaveDataSuffixFormat suffix { get; set; } = SaveDataSuffixFormat.Incremental;
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

  /// <summary>
  /// Specifies what suffixes to add to the save file name
  /// </summary>
  public enum SaveDataSuffixFormat
  {
    Incremental,
    SystemTime
  }
}