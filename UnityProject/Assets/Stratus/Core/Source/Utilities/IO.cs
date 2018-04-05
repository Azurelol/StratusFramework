using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Stratus
{
  namespace Utilities
  {
    /// <summary>
    /// Utlity functions for IO operations
    /// </summary>
    public static partial class IO
    {
      /// <summary>
      /// Returns a relative path of an asset
      /// </summary>
      /// <param name="path"></param>
      /// <returns></returns>
      public static string MakeRelative(string path)
      {
        var relativePath = "Assets" + path.Substring(Application.dataPath.Length);
        relativePath = relativePath.Replace("\\", "/");
        return relativePath;
      }

      /// <summary>
      /// Returns the relative path of a given folder name (if found within the application's assets folder)
      /// </summary>
      /// <param name="folderName"></param>
      /// <returns></returns>
      public static string GetFolderPath(string folderName)
      {
        //var dirInfo = new DirectoryInfo(Application.dataPath);
        var dirs = GetDirectories(Application.dataPath);
        var folderPath = dirs.Find(x => x.Contains(folderName));
        return folderPath;
      }

      private static List<string> GetDirectories(string path)
      {
        List<string> dirs = new List<string>();
        ProcessDirectory(path, dirs);
        return dirs;
      }

      private static void ProcessDirectory(string dirPath, List<string> dirs)
      {
        // Add this directory
        dirs.Add(MakeRelative(dirPath));
        // Now look for any subdirectories
        var subDirectoryEntries = Directory.GetDirectories(dirPath);
        foreach (var dir in subDirectoryEntries)
        {
          ProcessDirectory(dir, dirs);
        }
      }
    }
  }
}
