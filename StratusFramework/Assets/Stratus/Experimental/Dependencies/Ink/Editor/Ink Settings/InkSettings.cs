using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

/// <summary>
/// Holds a reference to an InkFile object for every .ink file detected in the Assets folder.
/// Provides helper functions to easily obtain these files.
/// </summary>
namespace Ink.UnityIntegration {
	public class InkSettings : ScriptableObject {
		public static bool created {
			get {
				return _Instance != null || FindSettings() != null;
			}
		}
		private static InkSettings _Instance;
		public static InkSettings Instance {
			get {
				if(_Instance == null)
					_Instance = FindOrCreateSettings();
				Debug.Assert(_Instance != null, "InkSettings could not be created! This is a bug.");
				return _Instance;
			}
		}
		public const string defaultPath = "Assets/InkSettings.asset";
		public const string pathPlayerPrefsKey = "InkSettingsAssetPath";

		public TextAsset templateFile;
		public string templateFilePath {
			get {
				if(templateFile == null) return "";
				else return AssetDatabase.GetAssetPath(templateFile);
			}
		}

        public DefaultAsset defaultJsonAssetPath;

        public bool compileAutomatically = true;
		public bool delayInPlayMode = true;
		public bool handleJSONFilesAutomatically = true;

		public int compileTimeout = 6;

		public CustomInklecateOptions customInklecateOptions = new CustomInklecateOptions();
		[System.Serializable]
		public class CustomInklecateOptions {
			#if UNITY_EDITOR_LINUX
			public bool runInklecateWithMono = true;
			#else
			public bool runInklecateWithMono;
			#endif
			public string[] monoPaths = {
				"/usr/bin/mono", 
				"/usr/local/bin/mono", 
				"/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono"
			};
			public string additionalCompilerOptions;
			public DefaultAsset inklecate;
		}

		[MenuItem("Edit/Project Settings/Ink", false, 500)]
		public static void SelectFromProjectSettings() {
			Selection.activeObject = Instance;
		}

		static InkSettings FindSettings () {
			return InkEditorUtils.FastFindAndEnforceSingletonScriptableObjectOfType<InkSettings>(pathPlayerPrefsKey);
		}

		static InkSettings FindOrCreateSettings () {
			return InkEditorUtils.FindOrCreateSingletonScriptableObjectOfType<InkSettings>(defaultPath, pathPlayerPrefsKey);
		}

		private static void Save () {
			EditorUtility.SetDirty(Instance);
			AssetDatabase.SaveAssets();
			EditorApplication.RepaintProjectWindow();
		}
	}	
}
