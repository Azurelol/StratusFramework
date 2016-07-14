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

namespace Stratus
{
  // Aliases, ho!
  using ActionsOwnerContainer = Dictionary<GameObject, ActionsOwner>;

  /**************************************************************************/
  /*!
  @class ActionSpace Manages the updating of all actions.
  */
  /**************************************************************************/
  public class ActionSpace : MonoBehaviour
  {
    public bool Debugging
    {
      get { return Actions.Debugging; }
      set { Actions.Debugging = value; }
    }

    //------------------------------------------------------------------------/
    private bool Active = false;
    private ActionsOwnerContainer AllActions;
    private ActionsOwnerContainer RecentlyAddedActions;
    //------------------------------------------------------------------------/
    private static ActionSpace ActionSpaceInst;
    public static ActionSpace Instance
    {
      get
      {
        if (!ActionSpaceInst)
        {
          // Look for it in the scene
          ActionSpaceInst = FindObjectOfType(typeof(ActionSpace)) as ActionSpace;
          // If not instantiated, do it ourselves
          if (!ActionSpaceInst)
          {
            var gameObj = new GameObject();
            gameObj.name = "Stratus Action System";
            gameObj.AddComponent<ActionSpace>();
          }
        }
        return ActionSpaceInst;
      }
    }
    //------------------------------------------------------------------------/

    /**************************************************************************/
    /*!
    @brief Initializes the ActionSpace
    */
    /**************************************************************************/
    void Initialize()
    {
      if (Active)
        return;

      if (Actions.Debugging) Trace.Script("Initializing the ActionSpace");

      AllActions = new ActionsOwnerContainer();
      RecentlyAddedActions = new ActionsOwnerContainer();
      ActionSpaceInst = this;
      Active = true;
      DontDestroyOnLoad(this);
    }

    void Awake()
    {
      if (!ActionSpaceInst)
        this.Initialize();
    }

    /**************************************************************************/
    /*!
    @brief ActionSpace destructor.
    */
    /**************************************************************************/
    ~ActionSpace()
    {
      AllActions.Clear();
      Active = false;
    }

    /**************************************************************************/
    /*!
    @brief Updates all the Actions in the ActionSpace, through the ActionOwners
           for every GameObject.
    */
    /**************************************************************************/
    void LateUpdate()
    {
      Propagate();
    }

    /**************************************************************************/
    /*!
    @brief Propagates an update to all active actions through ActionOwners.
    */
    /**************************************************************************/
    void Propagate()
    {
      // Add recently added action owners (to prevent desync)
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

    /**************************************************************************/
    /*!
    @brief Subscribe the specified GameObject to the ActionSpace.
    @param gameObj A reference to the gameobject.
    */
    /**************************************************************************/
    static public ActionsOwner Subscribe(GameObject gameObj)
    {
      //if (Actions.Trace)

      // If it is already present in the ActionSpace
      if (ActionSpace.Instance.AllActions.ContainsKey(gameObj))
        return ActionSpace.Instance.AllActions[gameObj];

      // If it has not already been added to the ActionSpace, do so
      if (!ActionSpace.Instance.RecentlyAddedActions.ContainsKey(gameObj))
      {
        //Trace.Script("'" + gameObj.name + "'");
        //Trace.Script("Adding '" + gameObj.name +  "' to the ActionSpace");
        if (Actions.Debugging) Trace.Script("Adding the GameObject to the ActionSpace");
        var owner = new ActionsOwner(gameObj);
        ActionSpace.Instance.RecentlyAddedActions.Add(gameObj, owner);
        gameObj.AddComponent<ActionsRegistration>();
      }

      return ActionSpace.Instance.RecentlyAddedActions[gameObj];
    }

    /**************************************************************************/
    /*!
    @brief Unsubscribes the specified GameObject from the ActionSpace.
    @param gameObj A reference to the gameobject.
    */
    /**************************************************************************/
    static public void Unsubscribe(GameObject gameObj)
    {
      if (!ActionSpaceInst)
        return;

      if (Actions.Debugging)
        Trace.Script("'" + gameObj.name + "'");
      ActionSpace.Instance.AllActions.Remove(gameObj);
      ActionSpace.Instance.RecentlyAddedActions.Remove(gameObj);
    }

    void PrintActiveActions()
    {
      string actionsLeft = "Active Actions: ";
      foreach (var action in ActionSpace.Instance.AllActions)
      {
        actionsLeft += action.Key.name + " ";
      }
      Trace.Script(actionsLeft);
    }

  }
}
