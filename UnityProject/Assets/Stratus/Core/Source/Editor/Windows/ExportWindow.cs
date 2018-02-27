using UnityEngine;
using Stratus;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
  public class ExportUtlity : EditorWindow
  {
    [MenuItem("Stratus/Export/Core")]
    private static void ExportCore()
    {
      Export("Stratus/Core", "Stratus Framework Core");
    }

    [MenuItem("Stratus/Export/Full")]
    private static void ExportAll()
    {
      Export("Stratus", "Stratus Framework");
    }

    private static void Export(string path, string packageName)
    {
      var location = Assets.GetFolderPath(path);
      AssetDatabase.ExportPackage(location, $"{packageName}.unitypackage",
        ExportPackageOptions.Recurse | ExportPackageOptions.Default |
        ExportPackageOptions.Interactive);
      Trace.Script($"Exported {packageName}");
    }
  }

}