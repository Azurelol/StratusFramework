using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Inspired by:  http://wiki.unity3d.com/index.php/SceneField

namespace Stratus
{
  /// <summary>
  /// Allows you to refer to any scene by reference, instead of name or index. Provides a variety of utility functions and properties too!
  /// </summary>
  [Serializable]
  public class SceneField : ISerializationCallbackReceiver
  {
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
#endif

#pragma warning disable 414
    [SerializeField, HideInInspector]
    private string SceneName = "";
#pragma warning restore 414
    
    /// <summary>
    /// The name of the scene
    /// </summary>
    public string name { get { return SceneName; } }

    /// <summary>
    /// Whether this is the currently active scene 
    /// </summary>
    public bool isActiveScene => (this.name == Scene.activeScene.name);

    /// <summary>
    /// Whether the scene is currently loaded
    /// </summary>
    public bool isLoaded { get { return runtime.isLoaded; } }

    /// <summary>
    /// Whether the scene is currently being loaded (it could be open in the editor)
    /// </summary>
    public bool isLoading { get { return Array.Find(Scene.activeScenes, x => x.name == this.name) != null; } }

    /// <summary>
    /// Whether the scene is currently opened in the editor
    /// </summary>
    public bool isOpened
    {
      get
      {
#if UNITY_EDITOR
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByName(this);        
        return scene.isLoaded;
#else
        return false;
#endif
      }
    }

    /// <summary>
    /// Returns all the root gameobjects in the scene
    /// </summary>
    public GameObject[] gameObjects => runtime.GetRootGameObjects();
    
    /// <summary>
    /// Returns the visible boundaries of the scene. If no renderers are
    /// present, returns an empty bounds composed of zero vectors.
    /// (Note: This is quite an expensive call)
    /// </summary>
    public Bounds visibleBoundaries
    {
      get
      {
        var renderers = GetComponents<Renderer>(true);
        bool renderersPresent = renderers.Length != 0;

        if (!renderersPresent)
          return new Bounds();

        // Let's spend a LOT of resources transforming all the bounds to world space :-)
        Renderer firstRenderer = renderers[0];
        Bounds totalBounds = firstRenderer.bounds;
        //totalBounds.center = firstRenderer.transform.TransformPoint(totalBounds.center);

        for (var i = 1; i < renderers.Length; ++i)
        {
          var renderer = renderers[i];
          var bounds = renderer.bounds;
          //bounds.center = renderer.transform.TransformPoint(bounds.center);
          totalBounds.Encapsulate(bounds);
        }

        return totalBounds;
      }
    }

    /// <summary>
    /// Returns the relative path for the scene asset in the project
    /// </summary>
    public string path
    {
      get
      {        
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(SceneAsset);
#else
        return string.Empty;
#endif
      }
    }

    /// <summary>
    /// Returns a reference to the runtimee data structure for the scene
    /// </summary>
    public UnityEngine.SceneManagement.Scene runtime { get { return SceneManager.GetSceneByName(name); } }    

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public SceneField() {}
    public SceneField(string sceneName) { SceneName = sceneName; }

    /// <summary>
    /// Makes it work with the existing Unity methods (LoadLevel/LoadScene) 
    /// </summary>
    /// <param name="sceneField"></param>
    public static implicit operator string(SceneField sceneField)
    {
      string name;
      #if UNITY_EDITOR
      name = System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(sceneField.SceneAsset));
      if (name == null || name == string.Empty)
        name = sceneField.SceneName;
      #else
        name = sceneField.SceneName;
      #endif
      return name;
    }

    public override string ToString()
    {
      return name;
    }

    public void OnBeforeSerialize()
    {
      #if UNITY_EDITOR
      SceneName = this;
      #endif
    }
    public void OnAfterDeserialize() { }

    /// <summary>
    /// Loads this scene
    /// </summary>
    /// <param name="mode"></param>
    public void Load(LoadSceneMode mode)
    {      
      Scene.Load(this, true, mode);
    }

    /// <summary>
    /// Unloads this scene (asynchronously)
    /// </summary>
    public void Unload()
    {
      Scene.Unload(this);
    }

    /// <summary>
    /// Finds all the components of a given type in the active scene, possibly including inactive ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public T[] GetComponents<T>(bool includeInactive = false) where T : Component
    {
      List<T> components = new List<T>();

      GameObject[] sceneObjects = runtime.GetRootGameObjects();
      foreach (var go in sceneObjects)
      {
        var se = go.GetComponentsInChildren<T>(includeInactive);
        if (se != null)
        {
          components.AddRange(se);
        }
      }
      return components.ToArray();
    }    
    
    /// <summary>
    /// Opens/loads the scene in the editor, adding it to the current list of scenes
    /// </summary>
    /// <param name="mode"></param>
    public void Add()
    {
      Scene.AddOrLoad(this);
    }

    /// <summary>
    /// Closes/unloads the scene in the editor
    /// </summary>
    /// <returns>True if the operation succeeded</returns>
    public void Close()
    {
      Scene.CloseOrUnload(this);
    }




    }

}