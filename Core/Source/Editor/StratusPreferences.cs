#define STRATUS_CORE

using System.Collections.Generic;
using Stratus.OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// The main data asset containing all the saved settings present among the Stratus framework's utilities
	/// </summary>
	[StratusScriptableSingleton("Assets", "Stratus Preferences")]
	public class StratusPreferences : StratusScriptableSingleton<StratusPreferences>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Allows scenes to be bookmarked from the project folder, used by the scene browser
		/// </summary>
		[SerializeField]
		public List<SceneAsset> bookmarkedScenes = new List<SceneAsset>();

		/// <summary>
		/// Allows the coloring of GameObjects with tags in the hierarchy window
		/// </summary>
		[SerializeField]
		public Internal.StratusTagColors tagColors = new Internal.StratusTagColors();

		/// <summary>
		/// Allows objects in the scene and project to be bookmarked for quick access
		/// </summary>
		public ObjectBookmarksWindow.ObjectBookmarks objectBookmarks = new ObjectBookmarksWindow.ObjectBookmarks();

		/// <summary>
		/// Object references to store...
		/// </summary>
		[OdinSerialize]
		public Dictionary<string, UnityEngine.Object> objectReferences = new Dictionary<string, Object>();

		/// <summary>
		/// An audio clip to be played whenever the editor reloads scripts
		/// </summary>
		public AudioClip reloadScriptsAudio;

		/// <summary>
		/// Whether to play an audio clip whenever scripts reload
		/// </summary>
		public bool reloadScriptsNotification = true;

		/// <summary>
		/// Automatically isolate Stratus Canvas Windows
		/// </summary>
		public bool isolateCanvases = true;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public const string projectMenuItem = "Project/" + StratusCore.menuItem;
		private static StratusSerializedPropertyMap propertyMap;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Resets all settings to their default
		/// </summary>
		public void Clear()
		{
			// @TODO: Better way would be to just delete asset then add it again?
			this.bookmarkedScenes.Clear();
			this.tagColors.Clear();
		}

		public static void SaveObjectReference(string name, UnityEngine.Object reference)
		{
			if (instance.objectReferences == null)
			{
				instance.objectReferences = new Dictionary<string, Object>();
			}

			instance.objectReferences.AddOrUpdate(name, reference);
		}

		public static T GetObjectReference<T>(string name)
			where T : UnityEngine.Object
		{
			if (instance.objectReferences == null)
			{
				instance.objectReferences = new Dictionary<string, Object>();
			}

			return (T)instance.objectReferences.GetValueOrNull(name);
		}

		[SettingsProvider]
		public static SettingsProvider OnProvider()
		{
			if (propertyMap == null)
			{
				propertyMap = new StratusSerializedPropertyMap(instance);
				//StratusDebug.Log($"Instanced");
			}

			// First parameter is the path in the Settings window.
			// Second parameter is the scope of this setting: it only appears in the Project Settings window.
			SettingsProvider provider = new SettingsProvider(projectMenuItem, SettingsScope.Project)
			{
				// By default the last token of the path is used as display name if no label is provided.
				label = StratusCore.menuItem,

				// Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
				guiHandler = (searchContext) =>
				{
					StratusPreferences settings = instance;
					propertyMap.DrawProperties();
				},

				// Populate the search keywords to enable smart search filtering and label highlighting:
				keywords = new HashSet<string>(new[] { StratusCore.menuItem })
			};

			return provider;
		}

		protected override void OnInitialize()
		{

		}
	}




}