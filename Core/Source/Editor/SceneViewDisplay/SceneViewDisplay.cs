using System;
using System.Collections.Generic;
using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Derive from this class to create a scene view display
	/// </summary>
	[InitializeOnLoad]
	public abstract class SceneViewDisplay : System.Object
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public class GlobalSettings
		{
			public bool enabled = true;
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this window is currently enabled
		/// </summary>
		public bool enabled = true;

		/// <summary>
		/// Whether all windows are currently enabled
		/// </summary>
		public static GlobalSettings global = new GlobalSettings();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether the current window should be displayed in the SceneView
		/// </summary>
		protected abstract bool isValid { get; }

		/// <summary>
		/// Whether this display has been initialized
		/// </summary>
		protected bool initializedDisplay { get; private set; }

		/// <summary>
		/// Whether the display for this state has been initialized
		/// </summary>
		protected bool initializedState { get; private set; }

		/// <summary>
		/// The name of this display
		/// </summary>
		public string name { get; private set; }

		/// <summary>
		/// Whether the settings for this window have been loaded
		/// </summary>
		public bool loaded { get; private set; }

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		protected abstract void OnInitializeDisplay();
		protected abstract void OnSceneGUI(SceneView view);
		protected abstract void OnInitializeState();
		protected abstract void OnReset();
		protected abstract void OnInspect();

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The list of all active displays
		/// </summary>
		public static List<SceneViewDisplay> displays { get; private set; } = new List<SceneViewDisplay>();
		/// <summary>
		/// The list of all active displays
		/// </summary>
		public static Dictionary<string, SceneViewDisplay> displaysMap { get; private set; } = new Dictionary<string, SceneViewDisplay>();
		/// <summary>
		/// The key used for serializing global settings
		/// </summary>
		private const string globalKey = "SceneViewDisplay_GlobalSettings";

		//------------------------------------------------------------------------/
		// Procedures: Static
		//------------------------------------------------------------------------/
		static SceneViewDisplay()
		{
			#if UNITY_EDITOR
			LoadGlobal();
			ConstructAllDisplays();
			EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			#endif
		}

		/// <summary>
		/// Constructs and initializes all declared non-abstract derived displays.
		/// This will add them to the editor's SceneView GUI delegate
		/// </summary>
		private static void ConstructAllDisplays()
		{
			// Get a list of all display classes, then construct them
			Type[] displayClasses = StratusReflection.GetSubclass<SceneViewDisplay>();
			foreach (Type displayType in displayClasses)
			{
				SceneViewDisplay display = Activator.CreateInstance(displayType) as SceneViewDisplay;
				//SceneViewDisplay display = ScriptableObject.CreateInstance(displayType) as SceneViewDisplay;
				//display.name = displayType.Name;
				display.name = displayType.GetNiceName();
				displays.Add(display);
				displaysMap.Add(display.name, display);
			}

			// Now initialize them
			foreach (SceneViewDisplay display in displays)
			{
				display.loaded = display.Load();
				if (!display.loaded)
				{
					display.OnReset();
					display.Save();
				}

				display.InitializeDisplay();
			}
		}


		/// <summary>
		/// Callback for when the hierarchy window changes
		/// </summary>
		public static void OnHierarchyWindowChanged()
		{
			foreach (SceneViewDisplay display in displays)
			{
				display.InitializeState();
			}
		}

		/// <summary>
		/// Callback for when the play mode state changes
		/// </summary>
		/// <param name="playModeState"></param>
		public static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
		{
			foreach (SceneViewDisplay display in displays)
			{
				display.InitializeState();
			}
		}

		//------------------------------------------------------------------------/
		// Methods: Private
		//------------------------------------------------------------------------/  
		/// <summary>
		/// Initializes this display
		/// </summary>
		public void InitializeDisplay()
		{
			this.loaded = this.Load();
			if (!this.loaded)
			{
				this.Reset();
			}

			this.OnInitializeDisplay();
			this.initializedDisplay = true;
			SceneView.onSceneGUIDelegate += this.SceneGUI;
		}

		/// <summary>
		/// Initializes the state for the display, when valid
		/// </summary>
		private void InitializeState()
		{
			if (this.isValid)
			{
				this.OnInitializeState();
				this.initializedState = true;
			}
			else
			{
				this.initializedState = false;
			}
		}

		/// <summary>
		/// Receives the onSceneGUI callback from ScemeVoew
		/// </summary>
		/// <param name="sceneView"></param>
		private void SceneGUI(SceneView sceneView)
		{
			if (!this.initializedState)
			{
				this.InitializeState();
			}

			if (this.IsValid(sceneView))
			{
				this.OnSceneGUI(sceneView);
			}
		}

		/// <summary>
		/// Whether this display should be shown
		/// </summary>
		/// <param name="sceneView"></param>
		/// <returns></returns>
		protected bool IsValid(SceneView sceneView)
		{
			return global.enabled && this.enabled && this.isValid && sceneView.camera != null && this.initializedState;
		}

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/   
		/// <summary>
		/// Saves the current settings for this window
		/// </summary>
		public void Save()
		{
			string data = JsonUtility.ToJson(this);
			EditorPrefs.SetString(this.name, data);
		}

		/// <summary>
		/// Loads the settings for this window
		/// </summary>
		public bool Load()
		{
			if (!EditorPrefs.HasKey(this.name))
			{
				return false;
			}

			string data = EditorPrefs.GetString(this.name);
			EditorJsonUtility.FromJsonOverwrite(data, this);
			return true;
		}

		/// <summary>
		/// Resets the settings to this window to the defaults
		/// </summary>
		public void Reset()
		{
			this.OnReset();
			SceneView.RepaintAll();
			this.Save();
		}

		/// <summary>
		/// Saves the global settings
		/// </summary>
		public static void SaveGlobal()
		{
			string data = JsonUtility.ToJson(SceneViewDisplay.global);
			EditorPrefs.SetString(globalKey, data);
		}

		/// <summary>
		/// Loads the global settings
		/// </summary>
		/// <returns></returns>
		public static bool LoadGlobal()
		{
			if (!EditorPrefs.HasKey(globalKey))
			{
				return false;
			}

			string data = EditorPrefs.GetString(globalKey);
			EditorJsonUtility.FromJsonOverwrite(data, SceneViewDisplay.global);
			return true;
		}

		/// <summary>
		/// Inspects the properties of this display within a GUILayout call
		/// </summary>
		public void Inspect()
		{
			EditorGUI.BeginChangeCheck();
			{
				this.enabled = EditorGUILayout.Toggle("Enabled", this.enabled);
				this.OnInspect();
			}
			if (EditorGUI.EndChangeCheck())
			{
				SceneView.RepaintAll();
				this.Save();
			}

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Save", EditorStyles.miniButton))
			{
				this.Save();
			}

			if (GUILayout.Button("Load", EditorStyles.miniButton))
			{
				this.Load();
			}

			if (GUILayout.Button("Reset", EditorStyles.miniButton))
			{
				this.Reset();
			}

			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Inspects the global properties of SceneViewDisplay
		/// </summary>
		public static void InspectGlobal()
		{
			EditorGUI.BeginChangeCheck();
			{
				global.enabled = EditorGUILayout.Toggle("Enabled", global.enabled);
			}
			if (EditorGUI.EndChangeCheck())
			{
				SceneViewDisplay.SaveGlobal();
				SceneView.RepaintAll();
			}
		}



	}
}