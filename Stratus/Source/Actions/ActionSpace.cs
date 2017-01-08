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
  public class ActionSpace : Singleton<ActionSpace>
  {
    protected override string Name { get { return "Action System"; } }
    //------------------------------------------------------------------------/
    private ActionsOwnerContainer AllActions;
    private ActionsOwnerContainer RecentlyAddedActions;
    //------------------------------------------------------------------------/    
    /// <summary>
    /// Initializes the ActionSpace
    /// </summary>
    protected override void OnAwake()
    {
      AllActions = new ActionsOwnerContainer();
      RecentlyAddedActions = new ActionsOwnerContainer();
      DontDestroyOnLoad(this.gameObject);
    }        
    
    /// <summary>
    /// Updates all the Actions in the ActionSpace, through the ActionOwners
    /// for every GameObject.
    /// </summary>
    void LateUpdate()
    {
      Propagate();
    }
    
    /// <summary>
    /// Propagates an update to all active actions through ActionOwners.
    /// </summary>
    void Propagate()
    {
      // Add recently added action owners (to prevent desync)
      if (RecentlyAddedActions == null)
      {
        Debug.Break();
        throw new System.Exception("WTF?");
      }


      foreach (var action in RecentlyAddedActions)
      {
        AllActions.Add(action.Key, action.Value);
      }
      RecentlyAddedActions.Clear();

      // Update all actions
      //PrintActiveActions();
      foreach (var action in new ActionsOwnerContainer(AllActions))
      {
        //action.Value.Migrate();
        //Trace.Script("Updating " + action.Value.Owner.name);
        action.Value.Update(Time.deltaTime);
      }
    }

    ActionsOwner SubscribeRoutine(GameObject gameObj)
    {
      if (AllActions.ContainsKey(gameObj))
        return AllActions[gameObj];

      // If it has not already been added to the ActionSpace, do so
      if (!RecentlyAddedActions.ContainsKey(gameObj))
      {
        //Trace.Script("'" + gameObj.name + "'");
        //Trace.Script("Adding '" + gameObj.name +  "' to the ActionSpace");
        if (Actions.Debugging) Trace.Script("Adding the GameObject to the ActionSpace");
        var owner = new ActionsOwner(gameObj);
        RecentlyAddedActions.Add(gameObj, owner);
        gameObj.AddComponent<ActionsRegistration>();
      }

      return RecentlyAddedActions[gameObj];
    }

    void UnsubscribeRoutine(GameObject gameObj)
    {
      if (Actions.Debugging)
        Trace.Script("'" + gameObj.name + "'");
      AllActions.Remove(gameObj);
      RecentlyAddedActions.Remove(gameObj);
    }

    public static void PrintActiveActions()
    {
      string actionsLeft = "Active Actions: ";
      foreach (var action in ActionSpace.Instance.AllActions)
      {
        actionsLeft += action.Key.name + ", ";
      }
      Trace.Script(actionsLeft);
    }

    /// <summary>
    /// Subscribe the specified GameObject to the ActionSpace.
    /// </summary>
    /// <param name="gameObj">< reference to the gameobject./param>
    /// <returns></returns>
    static public ActionsOwner Subscribe(GameObject gameObj)
    {
      return Instance.SubscribeRoutine(gameObj);
    }

    /// <summary>
    /// Unsubscribes the specified GameObject from the ActionSpace.
    /// </summary>
    /// <param name="gameObj">A reference to the gameobject.</param>
    static public void Unsubscribe(GameObject gameObj)
    {
      if (Quitting)
        return;

      Instance.UnsubscribeRoutine(gameObj);
    }


  }
}
