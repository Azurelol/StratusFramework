/******************************************************************************/
/*!
@file   Scene.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Stratus
{
  /// <summary>
  /// A space for scene-specific events, accessible to all objects
  /// </summary>  
  [Singleton("Stratus Scene Events", true, true)]
  public class Scene : Singleton<Scene>
  {
    public abstract class StatusEvent : Stratus.Event { public string name; }
    public class ChangedEvent : StatusEvent { }
    public class LoadedEvent : StatusEvent { }
    public class UnloadedEvent : StatusEvent { }
    public class ReloadEvent : Stratus.Event { }
    public delegate void SceneCallback();

    //----------------------------------------------------------------------------------/
    // Properties: Public
    //----------------------------------------------------------------------------------/
    /// <summary>
    /// Whether the scene (and thus the editor) is currently being edited
    /// </summary>
    public static bool isEditMode => EditorBridge.isEditMode;

    /// <summary>    
    /// The currently active scene is the scene which will be used as the target for new GameObjects instantiated by scripts.
    /// </summary>
    public static SceneField activeScene
    {
      get
      {
        #if UNITY_EDITOR
        if (isEditMode)
          return new SceneField(EditorSceneManager.GetActiveScene().name);
        #endif

        return new SceneField(SceneManager.GetActiveScene().name);
      }
    }

    /// <summary>
    /// Returns a list of all active scenes
    /// </summary>
    public static SceneField[] activeScenes
    {
      get
      {
        var scenes = new SceneField[sceneCount];
        for (var i = 0; i < sceneCount; ++i)
        {
          scenes[i] = new SceneField(GetSceneAt(i).name);
        }
        return scenes;
      }
    }

    /// <summary>
    /// The current progress into loading the next scene, on a (0,1) range
    /// </summary>
    public static float loadingProgress
    {
      get; private set;
      //get
      //{
      //  if (get.loadingOperation == null)
      //    return 0.0f;
      //  return get.loadingOperation.progress;
      //}
    }

    /// <summary>
    /// A provided callback for when the scene has changed. Add your methods here to be
    /// notified.
    /// </summary>
    public static UnityAction onSceneChanged { get; set; }

    /// <summary>
    /// A provided callback for when all scenes in a loading operation have finished loading
    /// </summary>
    public static UnityAction onAllScenesLoaded { get; set; }

    /// <summary>
    /// A provided callback for when any scene has been loaded
    /// </summary>
    public static UnityAction onSceneLoaded { get; set; } = () => { };

    /// <summary>
    /// A provided callback for when any scene has been unloaded
    /// </summary>
    public static UnityAction onSceneUnloaded { get; set; } = () => { };

    //----------------------------------------------------------------------------------/
    // Properties: Private
    //----------------------------------------------------------------------------------/
    private static SceneCallback onSceneLoadedCallback { get; set; }
    private static SceneCallback onSceneUnloadedCallback { get; set; }
    private static SceneCallback onAllScenesLoadedCallback { get; set; }
    /// <summary>
    /// The current asynchronous loading operation
    /// </summary>
    private AsyncOperation loadingOperation { get; set; }

    private static Func<UnityEngine.SceneManagement.Scene> getActiveScene
    {
      get
      {
#if UNITY_EDITOR
        if (isEditMode)
          return EditorSceneManager.GetActiveScene;
#endif
        return SceneManager.GetActiveScene;
      }
    }

    /// <summary>
    /// Keeps track of how many active scenes there are
    /// </summary>
    private static int sceneCount
    {
      get
      {
#if UNITY_EDITOR
        if (isEditMode)
          return EditorSceneManager.sceneCount;
#endif
        return SceneManager.sceneCount;

      }
    }

    private static UnityEngine.SceneManagement.Scene GetSceneAt(int index)
    {
#if UNITY_EDITOR
      if (isEditMode)
        return EditorSceneManager.GetSceneAt(index);
#endif
      return SceneManager.GetSceneAt(index);
    }

    //private static Dictionary<SceneField, SceneCallback> 


    //----------------------------------------------------------------------------------/
    // Messages
    //----------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      DontDestroyOnLoad(this.gameObject);

      onSceneChanged = new UnityAction(OnActiveSceneChanged);

      SceneManager.activeSceneChanged += OnActiveSceneChanged;
      SceneManager.sceneLoaded += OnSceneLoaded;
      SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    //----------------------------------------------------------------------------------/
    // Methods: Static
    //----------------------------------------------------------------------------------/
    /// <summary>
    /// Loads the scene specified by name.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void Load(SceneField scene, LoadSceneMode mode = LoadSceneMode.Additive, SceneCallback onSceneLoaded = null)
    {
      // Edit mode
      #if UNITY_EDITOR
      if (isEditMode)
      {
        EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
        return;
      }
      #endif

      // Play mode
      get.StartCoroutine(get.LoadAsync(scene, mode, onSceneLoaded));
    }

    /// <summary>
    /// Unloads the specified scene asynchronously
    /// </summary>
    /// <param name="sceneName"></param>
    public static void Unload(SceneField scene, SceneCallback onSceneUnloaded = null)
    {
      // Editor mode
      #if UNITY_EDITOR
      if (isEditMode)
      {         
        EditorSceneManager.CloseScene(scene.runtime, true);
        return;
      }
      #endif

      // Play Mode
      get.StartCoroutine(get.UnloadAsync(scene, onSceneUnloaded));
    }

    /// <summary>
    /// Loads multiple scenes in sequence asynchronouosly (additively)
    /// </summary>
    /// <param name="scenes"></param>
    /// <param name="onSceneLoaded"></param>
    public static void Load(SceneField[] scenes, SceneCallback onScenesLoaded = null)
    {
      // Editor mode
      #if UNITY_EDITOR
      if (isEditMode)
      {
        foreach (var scene in scenes)
        {
          EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
          //EditorSceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
        return;
      }
      #endif

      // Play mode
      get.StartCoroutine(get.LoadAsync(scenes, onScenesLoaded));
    }


    /// <summary>
    /// Unloads multiple scenes in sequence asynchronously
    /// </summary>
    /// <param name="scenes"></param>
    public static void Unload(SceneField[] scenes, SceneCallback onScenesUnloaded = null)
    {
      // Editor mode
      #if UNITY_EDITOR
      if (isEditMode)
      {
        foreach (var scene in scenes)
        {
          Trace.Script($"Closing {scene.name}");
          EditorSceneManager.CloseScene(scene.runtime, true);
        }
        return;
      }
      #endif
      
      // Play mode
      get.StartCoroutine(get.UnloadAsync(scenes, onScenesUnloaded));

    }

    /// <summary>
    /// Reloads the current scene
    /// </summary>
    public static void Reload()
    {
      Scene.Load(Scene.activeScene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Reloads the specified scene
    /// </summary>
    public static void Reload(SceneField scene, SceneCallback onFinished = null)
    {      
      Scene.Load(Scene.activeScene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Subscribes to events dispatched onto the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    public static void Connect<T>(Action<T> func)
    {
      get.Poke();
      Stratus.Events.Connect(get.gameObject, func);
    }

    /// <summary>
    /// Subscribes to events dispatched onto the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    public static void Connect(Action<Stratus.Event> func, Type type)
    {
      get.Poke();
      Stratus.Events.Connect(get.gameObject, func, type);
    }

    /// <summary>
    /// Dispatches an event to the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventObj"></param>
    public static void Dispatch<T>(T eventObj) where T : Stratus.Event
    {
      get.Poke();
      Stratus.Events.Dispatch<T>(get.gameObject, eventObj);
    }

    /// <summary>
    /// Finds all the components of a given type in the active scene, possibly including inactive ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static T[] GetComponentsInActiveScene<T>(bool includeInactive = false) where T : Component
    {
      return activeScene.GetComponents<T>(includeInactive);
    }

    /// <summary>
    /// Finds all the components of a given type in all active scenes, possibly including inactive ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static T[] GetComponentsInAllActiveScenes<T>(bool includeInactive = false) where T : Component
    {
      List<T> components = new List<T>();

      foreach (var scene in activeScenes)
      {
        var matchingComponents = scene.GetComponents<T>(includeInactive);
        components.AddRange(matchingComponents);
      }

      return components.ToArray();
    }
    

    //------------------------------------------------------------------------/
    // Methods: Private
    //------------------------------------------------------------------------/
    /// <summary>
    /// Received when the active scene changes
    /// </summary>
    /// <param name="prevScene"></param>
    /// <param name="nextScene"></param>
    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene prevScene, UnityEngine.SceneManagement.Scene nextScene)
    {
      onSceneChanged.Invoke();
    }

    /// <summary>
    /// Received when a scene has been loaded
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene prevScene, LoadSceneMode mode)
    {
      onSceneLoaded.Invoke();
      onSceneLoadedCallback?.DynamicInvoke();
      onSceneLoadedCallback = null;
    }

    /// <summary>
    /// Received when a scene has been unloaded
    /// </summary>
    /// <param name="arg0"></param>
    private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
    {
      onSceneLoaded.Invoke();
      onSceneUnloadedCallback?.DynamicInvoke();
      onSceneUnloadedCallback = null;
    }
    void OnActiveSceneChanged()
    {
    }


    /// <summary>
    /// Loads the specified scene asynchronously
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="mode"></param>
    IEnumerator LoadAsync(string sceneName, LoadSceneMode mode, SceneCallback onFinished = null)
    {
      loadingProgress = 0f;
      loadingOperation = SceneManager.LoadSceneAsync(sceneName, mode);
      if (loadingOperation != null)
      {
        while (!loadingOperation.isDone)
        {
          loadingProgress = loadingOperation.progress;
          yield return null;
        }
      }

      loadingProgress = 1f;
      onFinished?.Invoke();
    }

    /// <summary>
    /// Loads the specified scene asynchronously
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="mode"></param>
    IEnumerator UnloadAsync(string sceneName, SceneCallback onFinished = null)
    {
      loadingProgress = 0f;
      loadingOperation = SceneManager.UnloadSceneAsync(sceneName);
      if (loadingOperation != null)
      {
        while (!loadingOperation.isDone)
        {
          loadingProgress = loadingOperation.progress;
          yield return null;
        }
      }

      loadingProgress = 1f;
      onFinished?.Invoke();
    }

    /// <summary>
    /// Loads multiple scenes asynchronously in sequence, additively
    /// </summary>
    /// <param name="scenes"></param>
    /// <returns></returns>
    IEnumerator LoadAsync(SceneField[] scenes, SceneCallback onFinished = null)
    {
      loadingProgress = 0f;
      // Get the scene names in a queue
      var sceneNames = new Queue<string>();
      foreach (var scene in scenes)
        sceneNames.Enqueue(scene);
      
      while (sceneNames.Count > 0)
      {
        var nextScene = sceneNames.Dequeue();
        loadingOperation = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        while (!loadingOperation.isDone)
        {
          loadingProgress = loadingOperation.progress / (float)scenes.Length;
          yield return null;
        }
        //Trace.Script("Finished loading " + nextScene);
      }

      loadingProgress = 1f;
      onFinished?.Invoke();
      onAllScenesLoadedCallback?.Invoke();
    }

    /// <summary>
    /// Loads multiple scenes asynchronously in sequence, additively
    /// </summary>
    /// <param name="scenes"></param>
    /// <returns></returns>
    IEnumerator UnloadAsync(SceneField[] scenes, SceneCallback onFinished = null)
    {
      loadingProgress = 0f;
      // Get the scene names in a queue
      var sceneQueue = new Queue<SceneField>();
      foreach (var scene in scenes)
        sceneQueue.Enqueue(scene);

      while (sceneQueue.Count > 0)
      {
        var nextScene = sceneQueue.Dequeue();
        if (!nextScene.isLoaded)
          continue;

        loadingOperation = SceneManager.UnloadSceneAsync(nextScene);
        while (!loadingOperation.isDone)
        {
          loadingProgress = loadingOperation.progress / (float)scenes.Length;
          yield return null;
        }
        //Trace.Script("Finished unloading " + nextScene);
      }

      loadingProgress = 1f;
      onFinished?.Invoke();
      onAllScenesLoadedCallback?.Invoke();
    }



  }
}
