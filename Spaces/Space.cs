/******************************************************************************/
/*!
@file   Space.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@brief  A space is the abstraction for the physical space that the set of all
        objects on the scene resides in.
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace Stratus {

  class LogicUpdate : Stratus.Event
  {
    public float Dt;
  }

  public class Space : MonoBehaviour {

    static Dictionary<Scene, Space> SceneMap = new Dictionary<Scene, Space>();
    static public bool DoLogicUpdate = false;
    Scene Scene;
    //List<GameObject> ActiveGameObjects = new List<GameObject>();
    static bool Tracing = false;
    public string SceneName;

    //------------------------------------------------------------------------/
    // Instancing    
    //------------------------------------------------------------------------/
    private static Space DefaultSpace;
    public static Space Instance {
      get {
        if (!DefaultSpace) {
          Instantiate();
        }
        return DefaultSpace;
      }
    }

    static void Instantiate()
    {
      DefaultSpace = FindObjectOfType(typeof(Space)) as Space;
      if (!DefaultSpace)
      {
        DefaultSpace = Create("DefaultSpace");
      }
    }
    
    // @TODO better way?
    void OnDestroy()
    {
      if (DefaultSpace == this)
      {
        DefaultSpace = null;
      }
    }

    /**************************************************************************/
    /*!
    @brief Initializes the Space.
    */
    /**************************************************************************/
    void Awake() {

      // Check if the GameSession has been instantiated
      GameSession.Instance.Check();
      //DefaultSpace = this;
      //DontDestroyOnLoad(this);
    }
    //------------------------------------------------------------------------/        
    /**************************************************************************/
    /*!
    @brief Updates this space.
    */
    /**************************************************************************/
    void Update() {

      if (DoLogicUpdate)
      {
        var eventObj = new LogicUpdate();
        eventObj.Dt = Time.deltaTime;
        this.gameObject.Dispatch<LogicUpdate>(eventObj);
      }
    }


    /**************************************************************************/
    /*!
    @brief Subscribes to the event received by this space.
    @param T The event type.
    @param func The callback function.
    */
    /**************************************************************************/
    public void Connect<T>(Action<T> func)
    {
      if (!Instance) Instantiate();
      Stratus.Events.Connect(Instance.gameObject, func);
    }

    /// <summary>
    /// Dispatches an event onto the space.
    /// </summary>
    /// <typeparam name="T">The event class.</typeparam>
    /// <param name="eventObj">The event object.</param>
    /// <param name="nextFrame">Whether it should be dispatched next frame.</param>
    public void Dispatch<T>(T eventObj, bool nextFrame = false) where T : Stratus.Event
    {
      if (!Instance) Instantiate();
      Stratus.Events.Dispatch<T>(Instance.gameObject, eventObj, nextFrame);
      //Instance.gameObject.Dispatch<T>(eventObj);
    }

    /**************************************************************************/
    /*!
    @brief Loads the given scene into this space.
    @param sceneName The name of the scene.
    */
    /**************************************************************************/
    public void LoadScene(string sceneName)
    {
      // Remove the current key from the map
      SceneMap.Remove(this.Scene);

      LoadNormally(sceneName);
      return;
    }

    void LoadNewAndCool(string sceneName)
    {
      // Delete all of the space's children
      foreach (var child in this.gameObject.Children())
      {
        Destroy(child);
      }

      // Load the scene


      // 1. Move the space to the gamesession scene
      //if (Tracing) Trace.Script("Moving space to GameSession temporarily");   
      //SceneManager.MoveGameObjectToScene(this.gameObject, GameSession.Scene);

      // 2. Unload the current scene
      if (Tracing) Trace.Script("Unloading " + this.Scene.name);
      if (!SceneManager.UnloadScene(this.Scene.name))
      {
        if (Tracing) Trace.Script("Failed to unload current scene!");
      }

      // Migrate the space to tdahe next scene?
      //Trace.Script("Loading '" + sceneName + "'");
      SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
      var nextScene = SceneManager.GetSceneByName(sceneName);

      if (Tracing) Trace.Script("Moving space to new scene");
      SceneManager.MoveGameObjectToScene(this.gameObject, nextScene);
      // Set the next scene as the scene for this space
      this.Scene = nextScene;
      SceneManager.SetActiveScene(this.Scene);
      // Update the map
      SceneMap.Add(this.Scene, this);
    }

    void LoadNormally(string sceneName)
    {
      SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    /**************************************************************************/
    /*!
    @brief Creates a space.
    @param name The name of the space.
    */
    /**************************************************************************/
    public static Space Create(string name = "Space")
    {
      var obj = new GameObject();
      obj.name = name;
      if (Tracing) Trace.Script(name);

      // Construct the space component
      var space = obj.AddComponent<Space>();
      space.Scene = space.gameObject.scene;
      space.SceneName = space.Scene.name;
      //if (Tracing) Trace.Script(name + " belongs to the scene " + space.Scene.name);
      // Add it to the static dictionary of scenes/spaces
      SceneMap.Add(space.Scene, space);

      space.transform.parent = GameSession.Instance.transform;

      // If there is no default space
      if (!DefaultSpace)
        DefaultSpace = space;

      return space;
    }

    public static Space getSpace(Scene scene)
    {
      // If the Space is already present...
      if (SceneMap.ContainsKey(scene)) {
        //if (Tracing) Trace.Script("Found the space for the scene: " + scene.name);
        return SceneMap[scene];
      }

      // If not, instantiate it
      //if (Tracing) Trace.Script("Could not find the space for the scene: " + scene.name);
      //PrintMap();
      var space = Create();     
      return space;
    }

    static void PrintMap()
    {
      Trace.Script("Current scene map:");
      foreach(var scene in SceneMap)
      {
        Trace.Script(scene.Key.name);
      }
    }

  }

}
