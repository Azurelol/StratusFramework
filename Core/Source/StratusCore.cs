#define STRATUS_CORE

using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Contains information regarding the current modules of the framework.
	/// </summary>
	public static partial class StratusCore
	{
        public const string menuItem = "Stratus";
		public static readonly string rootFolder = menuItem + "/Core";
		public static readonly string guiFolder = "GUI";
		public static readonly string fontFolder = "Fonts";

		public static string rootPath => StratusIO.GetFolderPath(rootFolder);
		public static string resourcesFolder => rootPath + "/Resources";
		public static string guiPath => resourcesFolder + $"/{guiFolder}";

		public static void QuitApplication()
		{
			Application.Quit();
		}


	}

}