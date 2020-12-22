using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	public class ExportPackageWindow : StratusEditorWindow<ExportPackageWindow>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		public ExportPackagePreset preset;
		private const string recentPresetKey = "Recent Export Package Preset";

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public bool hasPreset => !this.preset.IsNull();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowEnable()
		{
			preset = StratusPreferences.GetObjectReference<ExportPackagePreset>(recentPresetKey);
		}

		protected override void OnWindowGUI()
		{
			//InspectProperty(nameof(preset));
			if (this.InspectObjectFieldWithHeader(ref this.preset, "Preset"))
			{
				StratusPreferences.SaveObjectReference(recentPresetKey, preset);
			}
			if (this.hasPreset)
			{
				this.InspectProperties(this.preset, "Properties");
			}
			//else
			//{
			//	this.InspectProperties("Temporary");
			//}

			//this.InspectProperties();
			this.ExportControls();
		}

		[MenuItem("Stratus/Core/Export Package")]
		public static void Open()
		{
			OnOpen("Export Package");
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[MenuItem("Stratus/Core/")]
		private static void ExportCore()
		{
			Export("Stratus/Core", "Stratus Framework Core");
		}

		[MenuItem("Stratus/Export/Full")]
		private static void ExportAll()
		{
			Export("Stratus", "Stratus Framework");
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private static void Export(string path, string packageName)
		{
			string location = StratusIO.GetFolderPath(path);
			AssetDatabase.ExportPackage(location, $"{packageName}.unitypackage",
			  ExportPackageOptions.Recurse | ExportPackageOptions.Default |
			  ExportPackageOptions.Interactive);
			StratusDebug.Log($"Exported {packageName} to {location}");
		}

		private static void Export(ExportPackageArguments arguments)
		{
			string location = StratusIO.GetFolderPath(arguments.path);
			AssetDatabase.ExportPackage(location, $"{arguments.name}.unitypackage", arguments.options);
			EditorUtility.RevealInFinder(location);
			StratusDebug.Log($"Exported {arguments.name} to {location}");
		}

		private void ExportControls()
		{
			StratusEditorGUI.BeginAligned(TextAlignment.Center);
			{
				StratusEditorGUILayout.Button("Export", () =>
				{

					if (this.hasPreset && this.preset.arguments.valid)
					{
						Export(this.preset.arguments);
					}

				});

				//StratusEditorGUILayout.Button("Export To", () =>
				//{
				//	string path = StratusEditorGUILayout.FolderPath("Export");
				//	if (path.IsValid())
				//	{
				//		if (this.hasPreset && this.preset.arguments.valid)
				//		{
				//			Export(preset.arguments);
				//		}
				//	}
				//});
			}
			StratusEditorGUI.EndAligned();
		}


	}

}