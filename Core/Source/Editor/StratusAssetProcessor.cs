using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Class that listens to Unity's asset pipeline events
	/// </summary>
	public static class StratusAssetProcessor //: UnityEditor.AssetModificationProcessor
	{
		public class AssetModification : UnityEditor.AssetModificationProcessor
		{
			/// <summary>
			/// Invoked when any asset is saved
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			public static string[] OnWillSaveAssets(string[] paths)
			{
				return paths;
			}
		}

		/// <summary>
		/// Invoked just after building the scene, and when entering playmode
		/// </summary>
		[PostProcessScene]
		public static void OnPostProcessScene()
		{
			if (UnityEditor.BuildPipeline.isBuildingPlayer)
			{
				StratusDebug.Log($"Removing all instances of:");
				StratusGameObjectBookmark.RemoveAll();

			}
			else
			{
				//Trace.Script($"In Editor. Not removing!");
			}
		}

		//------------------------------------------------------------------------/
		// Declaration
		//------------------------------------------------------------------------/  
		public delegate void AssetChangeCallback();
		public delegate void ObjectModificationCallback(Object target);
		public delegate void ModificationCallback();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/  
		private static Dictionary<string, List<AssetChangeCallback>> onFileModified { get; set; } = new Dictionary<string, List<AssetChangeCallback>>();
		private static Dictionary<Object, List<ObjectModificationCallback>> onObjectModified { get; set; } = new Dictionary<Object, List<ObjectModificationCallback>>();
		private static Dictionary<string, List<ModificationCallback>> onObjectByNameModified { get; set; } = new Dictionary<string, List<ModificationCallback>>();

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/      
		static StratusAssetProcessor()
		{
			EditorSceneManager.sceneSaving += OnSceneSaving;
			Undo.postprocessModifications += OnPostProcessModifications;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static void RegisterFileModified(string fileName, AssetChangeCallback callback)
		{
			onFileModified.AddIfMissing(fileName, callback);
		}

		/// <summary>
		/// Registers a callback for an object of given name whenever it is changed
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="callback"></param>
		public static void ObjectModified(Object target, ObjectModificationCallback callback)
		{
			onObjectModified.AddIfMissing(target, callback);
		}

		/// <summary>
		/// Registers a callback for an object of given name whenever it is changed
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="callback"></param>
		public static void ObjectModified(string name, ModificationCallback callback)
		{
			onObjectByNameModified.AddIfMissing(name, callback);
		}

		/// <summary>
		/// Invoked just after building the player
		/// </summary>
		/// <param name="target"></param>
		/// <param name="pathToBuiltProject"></param>
		[PostProcessBuildAttribute(1)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			//Debug.Log(pathToBuiltProject);
		}

		private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
		{
			foreach (UndoPropertyModification modification in propertyModifications)
			{
				Object target = modification.currentValue.target;
				onObjectModified.TryInvoke(target, (ObjectModificationCallback callback) => callback.Invoke(target));
				onObjectByNameModified.TryInvoke(target.name, (ModificationCallback callback) => callback.Invoke());
			}
			return propertyModifications;
		}

		private static void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
		{
			//Trace.Script($"Saving {scene.name}");
		}

		[DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			if (StratusPreferences.instantiated)
			{
				if (StratusPreferences.instance.reloadScriptsNotification && StratusPreferences.instance.reloadScriptsAudio.IsValid())
				{
					StratusEditorUtility.PlayAudioClip(StratusPreferences.instance.reloadScriptsAudio);
				}
			}
		}

	}



}