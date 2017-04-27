/******************************************************************************/
/*!
@file   Scene.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.Events;
using Stratus;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

namespace Stratus
{
  /// <summary>
  /// A space for scene-specific events, accessible to all objects
  /// </summary>
  public class Scene : StratusSingleton<Scene>
  {
    /// <summary>
    /// Signals that a new scene has been loaded
    /// </summary>
    public class LoadedEvent : Stratus.Event
    {
      /// <summary>
      /// The name of the scene that has been loaded
      /// </summary>
      public string Name;
    }

    public delegate void SceneCallback();

    //----------------------------------------------------------------------------------/
    // Properties
    //----------------------------------------------------------------------------------/
    /// <summary>
    /// Then name of the currently active scene
    /// </summary>
    public static string Current { get { return SceneManager.GetActiveScene().name; } }

    /// <summary>
    /// The current progress into loading the next scene, on a (0,1) range
    /// </summary>
    public static float LoadingProgress
    {
      get
      {
        if (Instance.LoadingOperation == null)
          return 0.0f;
        return Instance.LoadingOperation.progress;
      }
    }


    public static UnityAction OnSceneChanged;

    //----------------------------------------------------------------------------------/
    // Members
    //----------------------------------------------------------------------------------/
    protected override string Name { get { return "Stratus Scene Events"; } }
    protected override bool IsPersistent { get { return true; } }
    private static SceneCallback OnSceneLoadedCallback;
    private static SceneCallback OnAllScenesLoadedCallback;
    private AsyncOperation LoadingOperation;

    //----------------------------------------------------------------------------------/
    // Messages
    //----------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      DontDestroyOnLoad(this.gameObject);

      OnSceneChanged = new UnityAction(OnActiveSceneChanged);

      SceneManager.activeSceneChanged += OnActiveSceneChanged;
      SceneManager.sceneLoaded += OnSceneLoaded;
      SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    //----------------------------------------------------------------------------------/
    // Methods
    //----------------------------------------------------------------------------------/
    /// <summary>
    /// Loads the scene specified by name.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void Load(string sceneName, bool async = false, LoadSceneMode mode = LoadSceneMode.Single, SceneCallback onSceneLoaded = null)
    {
      if (async)
      {
        Instance.LoadAsync(sceneName, mode);        
        //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
      }
      else
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

      OnSceneLoadedCallback = onSceneLoaded;      
    }

    /// <summary>
    /// Loads the specified scene asynchronously
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="mode"></param>
    void LoadAsync(string sceneName, LoadSceneMode mode)
    {
      StartCoroutine(LoadAsyncRoutine(sceneName, mode));
    }

    /// <summary>
    /// Loads multiple scenes in sequence asynchronouosly
    /// </summary>
    /// <param name="scenes"></param>
    /// <param name="onSceneLoaded"></param>
    public static void Load(SceneField[] scenes, SceneCallback onScenesLoaded = null)
    {
      Instance.StartCoroutine(Instance.LoadAsyncRoutine(scenes));
      OnAllScenesLoadedCallback = onScenesLoaded;
      //OnSceneLoadedCallback = onSceneLoaded;
    }

    /// <summary>
    /// Loads the specified scene asynchronously
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="mode"></param>
    IEnumerator LoadAsyncRoutine(string sceneName, LoadSceneMode mode)
    {
      LoadingOperation = SceneManager.LoadSceneAsync(sceneName, mode);
      while (!LoadingOperation.isDone)
      {   
        yield return null;
      }
    }

    /// <summary>
    /// Loads multiple scenes asynchronously in sequence, additively
    /// </summary>
    /// <param name="scenes"></param>
    /// <returns></returns>
    IEnumerator LoadAsyncRoutine(SceneField[] scenes)
    {
      // Get the scene names in a queue
      var sceneNames = new Queue<string>();
      foreach (var scene in scenes) sceneNames.Enqueue(scene);

      while (sceneNames.Count > 0)
      {
        var nextScene = sceneNames.Dequeue();
        LoadingOperation = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        while (!LoadingOperation.isDone)
        {
          yield return null;
        }
        Trace.Script("Finished loading " + nextScene);
      }

      OnAllScenesLoadedCallback();
    }

    /// <summary>
    /// Unloads the current scene
    /// </summary>
    public static void Unload()
    {
      SceneManager.UnloadSceneAsync(Scene.Current);
    }

    /// <summary>
    /// Reloads the current scene
    /// </summary>
    public static void Reload()
    {
      Load(Scene.Current);
    }
    
    /// <summary>
    /// Subscribes to events dispatched onto the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    public static void Connect<T>(Action<T> func)
    {
      Instance.Poke();
      Stratus.Events.Connect(Instance.gameObject, func);
    }
    
    /// <summary>
    /// Dispatches an event to the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventObj"></param>
    public static void Dispatch<T>(T eventObj) where T : Stratus.Event
    {
      Instance.Poke();
      Stratus.Events.Dispatch<T>(Instance.gameObject, eventObj);
    }

    /// <summary>
    /// Received when the active scene changes
    /// </summary>
    /// <param name="prevScene"></param>
    /// <param name="nextScene"></param>
    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene prevScene, UnityEngine.SceneManagement.Scene nextScene)
    {
      OnSceneChanged.Invoke();
    }
    void OnActiveSceneChanged()
    {

    }

    /// <summary>
    /// Received when a scene has been loaded
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene prevScene, LoadSceneMode mode)
    {
      if (OnSceneLoadedCallback != null)
        OnSceneLoadedCallback.DynamicInvoke();
      OnSceneLoadedCallback = null;
    }

    /// <summary>
    /// Received when a scene has been unloaded
    /// </summary>
    /// <param name="arg0"></param>
    private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
    {
      
    }

    

  }
}
