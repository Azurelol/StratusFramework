/******************************************************************************/
/*!
@file   ActionSet.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/22/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  
  using ActionContainer = System.Collections.Generic.List<Action>;

  /**************************************************************************/
  /*!
  @class The ActionSet is the base class from which all other sets derive.
         Sets such as Sequence, Group and the unique set used by entities.
  */
  /**************************************************************************/
  public abstract class ActionSet : Action
  {
    protected ActionContainer ActiveActions = new ActionContainer();

    public ActionSet(string type) : base(type) { }
    public override abstract float Update(float dt);

    /**************************************************************************/
    /*!
    @brief Add an action to this set
    @param action The specified action.
    */
    /**************************************************************************/
    public virtual void Add(Action action)
    {
      if (Actions.Trace) Debug.Log("'" + action.Type + "'");
      this.ActiveActions.Add(action);
    }

    /**************************************************************************/
    /*!
    @brief  Clears all inactive actions.
    */
    /**************************************************************************/
    public void Clear()
    {
      // No actions to clear
      if (this.ActiveActions.Count == 0)
        return;

      // Remove all actions that are finished
      this.ActiveActions.RemoveAll(x => x.Finished == true);
    }


  }

}

