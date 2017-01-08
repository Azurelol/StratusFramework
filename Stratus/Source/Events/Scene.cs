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


namespace Stratus
{
  /// <summary>
  /// A space for scene-specific events, accessible to all objects
  /// </summary>
  public class Scene : StratusSingleton<Scene>
  {
    public static UnityAction OnSceneChanged;

    protected override string Name
    {
      get { return "SceneEvents"; }
    }

    protected override void OnAwake()
    {
      OnSceneChanged = new UnityAction(OnSceneChangedBark);
      UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChangedFunc;
    }

    /// <summary>
    /// Loads the specified scene
    /// </summary>
    /// <param name="sceneName"></param>
    public static void Load(string sceneName)
    {
      UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void Reload()
    {
      Load(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Subscribes to events dispatched onto the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    public static void Connect<T>(Action<T> func)
    {
      if (!Instance) Instantiate();
      Stratus.Events.Connect(Instance.gameObject, func);
    }
    
    /// <summary>
    /// Dispatches an event to the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventObj"></param>
    public static void Dispatch<T>(T eventObj) where T : Stratus.Event
    {
      if (!Instance) Instantiate();
      Stratus.Events.Dispatch<T>(Instance.gameObject, eventObj);
    }

    /// <summary>
    /// Received when the scene changes
    /// </summary>
    /// <param name="prevScene"></param>
    /// <param name="nextScene"></param>
    void OnSceneChangedFunc(UnityEngine.SceneManagement.Scene prevScene, UnityEngine.SceneManagement.Scene nextScene)
    {
      OnSceneChanged.Invoke();
    }

    void OnSceneChangedBark()
    {

    }
  }
}
