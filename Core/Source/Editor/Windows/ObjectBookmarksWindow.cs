using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// A window for easily bookmarking frequently used objects
	/// </summary>
	public class ObjectBookmarksWindow : EditorWindow
	{
		//------------------------------------------------------------------------/
		// Declaration
		//------------------------------------------------------------------------/
		[Serializable]
		public class ObjectBookmarks
		{
			public List<UnityEngine.Object> projectBookmarks = new List<UnityEngine.Object>();
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private Vector2 scrollPosition;
		private SceneAsset sceneToAdd;
		private AnimBool showScenesInBuild, showScenes, showSceneObjects, showProjectAssets;


		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static ObjectBookmarks bookmarks { get; private set; }
		public static List<SceneAsset> bookmarkedScenes => StratusPreferences.instance.bookmarkedScenes;
		public static List<StratusGameObjectBookmark> sceneBookmarks { get; private set; }
		private static ObjectBookmarksWindow instance { get; set; }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		[MenuItem("Stratus/Core/Object Bookmarks")]
		public static void Open()
		{
			instance = (ObjectBookmarksWindow)EditorWindow.GetWindow(typeof(ObjectBookmarksWindow), false, "Bookmarks");
		}

		private void OnEnable()
		{
			bookmarks = StratusPreferences.instance.objectBookmarks;
			sceneBookmarks = StratusGameObjectBookmark.availableList;
			this.showScenes = new AnimBool(true); this.showScenes.valueChanged.AddListener(this.Repaint);
			this.showSceneObjects = new AnimBool(true); this.showSceneObjects.valueChanged.AddListener(this.Repaint);
			this.showScenesInBuild = new AnimBool(true); this.showScenesInBuild.valueChanged.AddListener(this.Repaint);
			this.showProjectAssets = new AnimBool(true); this.showProjectAssets.valueChanged.AddListener(this.Repaint);
		}


		private void OnGUI()
		{
			// Top bar
			EditorGUILayout.BeginHorizontal();
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Remove GameObject Bookmarks"), false, StratusGameObjectBookmark.RemoveAll);
				StratusEditorUtility.DrawContextMenu(menu, StratusEditorUtility.ContextMenuType.Options);
			}
			EditorGUILayout.EndHorizontal();

			// Bookmarks list
			EditorGUILayout.BeginVertical();
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, false, false);
			{
				StratusEditorUtility.DrawVerticalFadeGroup(this.showScenesInBuild, "Scenes in Build", this.ShowScenesInBuild, EditorStyles.helpBox, EditorBuildSettings.scenes.Length > 0);
				StratusEditorUtility.DrawVerticalFadeGroup(this.showScenes, "Scenes", this.ShowBookmarkedScenes, EditorStyles.helpBox, bookmarkedScenes.Count > 0);
				StratusEditorUtility.DrawVerticalFadeGroup(this.showSceneObjects, "Scene Objects", this.ShowSceneObjects, EditorStyles.helpBox, StratusGameObjectBookmark.hasAvailable);
				StratusEditorUtility.DrawVerticalFadeGroup(this.showProjectAssets, "Project Assets", this.ShowProjectAssets, EditorStyles.helpBox, bookmarks.projectBookmarks.Count > 0);
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[MenuItem("Assets/Bookmark", false, 0)]
		private static void BookmarkAsset()
		{
			UnityEngine.Object activeObject = Selection.activeObject;
			// Special case for scenes
			if (activeObject.GetType() == typeof(SceneAsset))
			{
				SceneAsset scene = activeObject as SceneAsset;
				if (!bookmarkedScenes.Contains(scene))
				{
					bookmarkedScenes.Add(scene);
				}
			}
			else
			{
				StratusPreferences.instance.objectBookmarks.projectBookmarks.Add(activeObject);
			}

			StratusPreferences.Save();
		}

		[MenuItem("GameObject/Bookmark", false, 49)]
		private static void BookmarkGameObject()
		{
			//if (Selection.activeGameObject != null)
			GameObject go = Selection.activeGameObject;
			if (go != null)
			{
				go.GetOrAddComponent<StratusGameObjectBookmark>();
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		private void ShowScenesInBuild()
		{
			for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
			{
				EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
				if (scene.enabled)
				{
					string sceneName = Path.GetFileNameWithoutExtension(scene.path);
					bool pressed = GUILayout.Button(i + ": " + sceneName, EditorStyles.toolbarButton);
					if (pressed)
					{
						int button = UnityEngine.Event.current.button;
						if (button == 0 && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
						{
							EditorSceneManager.OpenScene(scene.path);
						}
					}
				}
			}
		}

		private void ShowBookmarkedScenes()
		{
			//GUILayout.Label("Scenes", EditorStyles.centeredGreyMiniLabel);
			foreach (SceneAsset scene in bookmarkedScenes)
			{
				// If it was deleted from the outside, we need to remove this reference
				if (scene == null)
				{
					this.RemoveBookmarkedScene(scene);
					return;
				}

				//EditorGUILayout.BeginHorizontal();
				// Open scene
				if (GUILayout.Button(scene.name, EditorStyles.toolbarButton))
				{
					StratusEditorUtility.OnMouseClick(
					  () =>
					  {
						  string scenePath = AssetDatabase.GetAssetPath(scene);
						  if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
						  {
							  EditorSceneManager.OpenScene(scenePath);
						  }
					  },

					  () =>
					  {
						  GenericMenu menu = new GenericMenu();
						  menu.AddItem(new GUIContent("Remove"), false,
				  () =>
				  {
							  this.RemoveBookmarkedScene(scene);
						  }
				  );
						  menu.ShowAsContext();
					  },

					  null,
					  true);
				}
				//EditorGUILayout.EndHorizontal();
			}

		}

		private void ShowSceneObjects()
		{
			for (int b = 0; b < sceneBookmarks.Count; ++b)
			{
				StratusGameObjectBookmark bookmark = sceneBookmarks[b];

				EditorGUILayout.ObjectField(bookmark, bookmark.GetType(), true);
				StratusEditorUtility.OnLastControlMouseClick(
				// Left
				() =>
				{
					this.SelectBookmark(bookmark);
				},

				// Right
				() =>
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Inspect"), false, () =>
			{
					  MemberInspectorWindow.Inspect(bookmark.gameObject);
				  });
					menu.AddItem(new GUIContent("Remove"), false,
			  () =>
			  {
						DestroyImmediate(bookmark);
					}
			  );
					menu.ShowAsContext();
				},

				null);
			}
		}


		private void ShowProjectAssets()
		{
			for (int b = 0; b < bookmarks.projectBookmarks.Count; ++b)
			{
				UnityEngine.Object currentObject = bookmarks.projectBookmarks[b];

				if (currentObject == null)
				{
					bookmarks.projectBookmarks.RemoveAt(b);
					this.OnChange();
					return;
				}

				Type objectType = currentObject.GetType();
				bookmarks.projectBookmarks[b] = EditorGUILayout.ObjectField(currentObject, objectType, false);

				StratusEditorUtility.OnLastControlMouseClick(
				// Left
				() =>
				{
					this.SelectBookmark(currentObject);
				},

				// Right
				() =>
				{
					GenericMenu menu = new GenericMenu();

			// If it's a prefab, instantiate
			if (PrefabUtility.GetPrefabType(currentObject) != PrefabType.None)
					{
						menu.AddItem(new GUIContent("Instantiate"), false, () =>
				{
						GameObject instantiated = (GameObject)GameObject.Instantiate(currentObject);
						instantiated.name = currentObject.name;
					});
					}

			// Remove
			menu.AddItem(new GUIContent("Remove"), false,
			  () =>
			  {
						bookmarks.projectBookmarks.Remove(currentObject);
						this.OnChange();
					}
			  );
					menu.ShowAsContext();
				},
				null);

			}
		}
		private void AddBookmarkedScene()
		{
			EditorGUILayout.BeginHorizontal();
			this.sceneToAdd = EditorGUILayout.ObjectField(this.sceneToAdd, typeof(SceneAsset), false) as SceneAsset;
			if (GUILayout.Button("Add", EditorStyles.miniButtonRight) && this.sceneToAdd != null && !bookmarkedScenes.Contains(this.sceneToAdd))
			{
				bookmarkedScenes.Add(this.sceneToAdd);
				StratusPreferences.Save();
				this.sceneToAdd = null;
			}

			EditorGUILayout.EndHorizontal();

		}

		private void RemoveBookmarkedScene(SceneAsset scene)
		{
			bookmarkedScenes.Remove(scene);
			StratusPreferences.Save();
			this.Repaint();
		}

		private void SelectBookmark(UnityEngine.Object obj)
		{
			Selection.activeObject = obj;
		}

		private void OnChange()
		{
			StratusPreferences.Save();
			this.Repaint();
		}
	}
}