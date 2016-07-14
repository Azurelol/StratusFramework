/******************************************************************************/
/*!
@file   GameSession.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace Stratus
{  
  /**************************************************************************/
  /*!
  @class GameSession 
  */
  /**************************************************************************/
  public class GameSession : Singleton<GameSession>
  {
    protected override string getInstanceName() { return "GameSession"; }
    List<Space> ActiveSpaces = new List<Space>();
    Space DefaultSpace;
    public string DefaultScene;
    public bool Load = false;

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    protected override void Initialize()
    {
      // If the GameSession is not already on its own scene, do so
      if (this.gameObject.scene.name != getInstanceName())
      {
        var sceneName = getInstanceName();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.MoveGameObjectToScene(this.gameObject, scene);
        Trace.Script("Created the GameSession scene!");
      }

      if (this.Load)
        this.LoadDefaultScene();
      DontDestroyOnLoad(this);
    }

    /**************************************************************************/
    /*!
    @brief Loads the default scene onto the default space.
    */
    /**************************************************************************/
    void LoadDefaultScene()
    {
      // Create a space
      this.DefaultSpace = this.CreateSpace();
      // Load its scene
      this.DefaultSpace.LoadScene(this.DefaultScene);
    }

    /**************************************************************************/
    /*!
    @brief Creates a space.
    @param name The name of the space.
    */
    /**************************************************************************/
    public Space CreateSpace(string name = "Space")
    {
      var space = Space.Create(name);
      ActiveSpaces.Add(space);
      return space;
    }

    /**************************************************************************/
    /*!
    @brief Subscribes to the event received by this GameSession.
    @param T The event type.
    @param func The callback function.
    */
    /**************************************************************************/
    public static void Connect<T>(Action<T> func)
    {
      if (!Instance) Instantiate();
      Stratus.Events.Connect(Instance.gameObject, func);
    }

    /**************************************************************************/
    /*!
    @brief Dispatches an event onto the GameSession.
    @param T The event type.
    @param eventObj The event object.
    */
    /**************************************************************************/
    public static void Dispatch<T>(T eventObj) where T : Stratus.Event
    {
      if (!Instance) Instantiate();
      Stratus.Events.Dispatch<T>(Instance.gameObject, eventObj);
    }

    public void Check()
    {

    }

  }

}