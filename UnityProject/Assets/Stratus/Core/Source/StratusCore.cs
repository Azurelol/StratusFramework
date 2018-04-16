#define STRATUS_CORE

namespace Stratus
{
  /// <summary>
  /// Contains information regarding the current modules of the framework.
  /// </summary>
  public static partial class StratusCore
  {
    public static readonly string rootFolder = "Stratus/Core";
    public static readonly string guiFolder = "GUI";
    public static readonly string fontFolder = "Fonts";

    public static string rootPath => Utilities.IO.GetFolderPath(rootFolder);
    public static string resourcesFolder => rootPath + "/Resources";
    public static string guiPath => resourcesFolder + $"/{guiFolder}";
  }

}