/******************************************************************************/
/*!
@file   ActionSpace.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/22/2016
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
    private static ActionSpace Inst;
    private ActionsOwnerContainer AllActions;

    public static ActionSpace Instance
    {
      get
      {
        return Inst;
      }
    }

    /**************************************************************************/
    /*!
    @brief Initializes the ActionSpace
    */
    /**************************************************************************/
    void Initialize()
    {
      if (Actions.Trace) Trace.Script("Initializing the ActionSpace");

      AllActions = new ActionsOwnerContainer();
      Inst = this;
    }

    void Awake()
    {
      if (!Inst)
        this.Initialize();
    }

    /**************************************************************************/
    /*!
    @brief ActionSpace destructor.
    */
    /**************************************************************************/
    ~ActionSpace() {
      AllActions.Clear();
    }


    /**************************************************************************/
    /*!
    @brief Updates all the Actions in the ActionSpace, through the ActionOwners
           for every GameObject.
    */
    /**************************************************************************/
    void Update()
    {
      foreach(var action in AllActions)
      {
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
      if (Actions.Trace) Trace.Script("'" + gameObj.name + "'");

      // If it has not already been added to the ActionSpace, do so
      if (!ActionSpace.Instance.AllActions.ContainsKey(gameObj))
      {
        if (Actions.Trace) Trace.Script("Adding the GameObject to the ActionSpace");
        var owner = new ActionsOwner(gameObj);
        ActionSpace.Instance.AllActions.Add(gameObj, owner);
      }

      return ActionSpace.Instance.AllActions[gameObj];
    }

    /**************************************************************************/
    /*!
    @brief Unsubscribes he specified GameObject from the ActionSpace.
    @param gameObj A reference to the gameobject.
    */
    /**************************************************************************/
    static public void Unsubscribe(GameObject gameObj)
    {
      if (Actions.Trace) Trace.Script("'" + gameObj.name + "'");

      ActionSpace.Instance.AllActions.Remove(gameObj);
    }



  }


}
