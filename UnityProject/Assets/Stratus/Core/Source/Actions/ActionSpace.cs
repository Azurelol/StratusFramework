/******************************************************************************/
/*!
@file   ActionSpace.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Stratus
{
  // Aliases, ho!
  using ActionsOwnerContainer = Dictionary<GameObject, ActionsOwner>;

  /// <summary>
  /// Handles the updating of all actions.
  /// </summary>
  [Singleton("Stratus Action System", true, true)]
  public class ActionSpace : Singleton<ActionSpace>
  {
    //------------------------------------------------------------------------/
    // Members
    //------------------------------------------------------------------------/
    private ActionsOwnerContainer activeActions { get; set; }
    private ActionsOwnerContainer recentlyAddedActions { get; set; }

    //------------------------------------------------------------------------/    
    // Methods
    //------------------------------------------------------------------------/    
    /// <summary>
    /// Initializes the ActionSpace
    /// </summary>
    protected override void OnAwake()
    {
      activeActions = new ActionsOwnerContainer();
      recentlyAddedActions = new ActionsOwnerContainer();
      DontDestroyOnLoad(this.gameObject);
    }        
    
    /// <summary>
    /// Updates all the Actions in the ActionSpace, through the ActionOwners
    /// for every GameObject.
    /// </summary>
    void FixedUpdate()
    {
      Propagate();
    }
    
    /// <summary>
    /// Propagates an update to all active actions through ActionOwners.
    /// </summary>
    void Propagate()
    {
      // Add recently added action owners (to prevent desync)
      if (recentlyAddedActions == null)
      {
        //return;
        Debug.Break();
        throw new System.Exception("Sudden exception in the Action Space");
      }


      foreach (var action in recentlyAddedActions)
      {
        activeActions.Add(action.Key, action.Value);
      }
      recentlyAddedActions.Clear();

      // Update all actions
      //PrintActiveActions();
      foreach (var action in new ActionsOwnerContainer(activeActions))
      {
        //action.Value.Migrate();
        //Trace.Script("Updating " + action.Value.Owner.name);
        action.Value.Update(Time.deltaTime);
      }
    }

    ActionsOwner SubscribeToActions(GameObject gameObj)
    {
      if (activeActions.ContainsKey(gameObj))
        return activeActions[gameObj];

      // If it has not already been added to the ActionSpace, do so
      if (!recentlyAddedActions.ContainsKey(gameObj))
      {
        //Trace.Script("'" + gameObj.name + "'");
        //Trace.Script("Adding '" + gameObj.name +  "' to the ActionSpace");
        if (Actions.debug)
          Trace.Script("Adding the GameObject to the ActionSpace");

        var owner = new ActionsOwner(gameObj);
        recentlyAddedActions.Add(gameObj, owner);
        gameObj.AddComponent<ActionsRegistration>();
      }

      return recentlyAddedActions[gameObj];
    }

    void UnsubscribeRoutine(GameObject gameObj)
    {
      // @TODO: Why is this an issue?
      if (gameObj == null)
        return;

      if (Actions.debug)
        Trace.Script("'" + gameObj.name + "'");
      activeActions.Remove(gameObj);
      recentlyAddedActions.Remove(gameObj);
    }

    /// <summary>
    /// Prints all active actions
    /// </summary>

    public static void PrintActiveActions()
    {
      string actionsLeft = "Active Actions: ";
      foreach (var action in ActionSpace.get.activeActions)
      {
        actionsLeft += action.Key.name + ", ";
      }
      Trace.Script(actionsLeft);
    }

    /// <summary>
    /// Subscribe the specified GameObject to the ActionSpace.
    /// </summary>
    /// <param name="gameObj">reference to the gameobject.</param>
    /// <returns></returns>
    static public ActionsOwner Subscribe(GameObject gameObj)
    {
      return get.SubscribeToActions(gameObj);
    }

    /// <summary>
    /// Unsubscribes the specified GameObject from the ActionSpace.
    /// </summary>
    /// <param name="gameObj">A reference to the gameobject.</param>
    static public void Unsubscribe(GameObject gameObj)
    {
      if (isQuitting)
        return;

      get.UnsubscribeRoutine(gameObj);
    }

    public static void Clear(MonoBehaviour component)
    {
      if (isQuitting)
        return;

      component.gameObject.Actions().Clear();
    }


  }
}
