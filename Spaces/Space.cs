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
    Scene CurrentScene;
    List<GameObject> ActiveGameObjects = new List<GameObject>();

    //------------------------------------------------------------------------/
    // Instancing    
    //------------------------------------------------------------------------/
    private static Space ActiveSpace;
    public static Space Instance {
      get {
        if (!ActiveSpace) {
          Instantiate();
        }
        return ActiveSpace;
      }
    }

    static void Instantiate()
    {
      ActiveSpace = FindObjectOfType(typeof(Space)) as Space;
      if (!ActiveSpace)
      {
        ActiveSpace = Create("Space");
      }

      // If instantiated onto a scene, link that scene to this Space
      SceneMap.Add(ActiveSpace.gameObject.scene, ActiveSpace);
    }

    /**************************************************************************/
    /*!
    @brief Initializes the Space.
    */
    /**************************************************************************/
    void Awake() {

      // Check if the GameSession has been instantiated
      GameSession.Instance.Check();
      ActiveSpace = this;
      DontDestroyOnLoad(this);
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

    /**************************************************************************/
    /*!
    @brief Dispatches an event onto the space.
    @param T The event type.
    @param eventObj The event object.
    */
    /**************************************************************************/
    public void Dispatch<T>(T eventObj) where T : Stratus.Event
    {
      if (!Instance) Instantiate();
      Stratus.Events.Dispatch<T>(Instance.gameObject, eventObj);
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
      SceneMap.Remove(this.CurrentScene);
      // Unload the current scene
      Trace.Script("Unloading current scene");
      var gs = SceneManager.GetSceneByName(GameSession.Instance.gameObject.scene.name);
      //var gs = SceneManager.GetSceneByName("GameSession");
      SceneManager.MoveGameObjectToScene(this.gameObject, gs);
      SceneManager.UnloadScene(this.CurrentScene.buildIndex);
      // Migrate the space to the next scene?
      Trace.Script("Loading '" + sceneName + "'");
      SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
      var nextScene = SceneManager.GetSceneByName(sceneName);
      SceneManager.MoveGameObjectToScene(this.gameObject, nextScene);
      // Set the next scene as the current scene      
      this.CurrentScene = nextScene;
      // Update the map
      SceneMap.Add(this.CurrentScene, this);
    }

    /**************************************************************************/
    /*!
    @brief Creates a space.
    @param name The name of the space.
    */
    /**************************************************************************/
    public static Space Create(string name)
    {
      var obj = new GameObject();
      obj.name = name;
      var space = obj.AddComponent<Space>();
      return space;
    }
    public static Space getSpace(Scene scene)
    {
      // If the Space is already present...
      if (SceneMap.ContainsKey(scene))
        return SceneMap[scene];

      // If not, instantiate it
      var space = Create(scene.name + "'s Space");
      SceneMap.Add(scene, space);
      return space;
    }

  }

}
