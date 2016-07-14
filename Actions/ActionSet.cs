/******************************************************************************/
/*!
@file   ActionSet.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Diagnostics;

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
    protected ActionContainer RecentlyAddedActions = new ActionContainer();
    public bool TraceStack = false;

    public ActionSet(string type) : base(type) {}
    public override abstract float Update(float dt);

    /**************************************************************************/
    /*!
    @brief Add an action to this set
    @param action The specified action.
    */
    /**************************************************************************/
    public virtual void Add(Action action)
    { 
      if (TraceStack)
      {
        StackTrace stackTrace = new StackTrace();
        var methodName = stackTrace.GetFrame(2).GetMethod().Name;
        var className = stackTrace.GetFrame(2).GetMethod().ReflectedType.Name;
        Trace.Script("Adding " + action.Type + " from " + methodName);
      }

      if (Actions.Debugging) Trace.Script("'" + action.Type + "'");
      this.RecentlyAddedActions.Add(action);
    }

    /**************************************************************************/
    /*!
    @brief Migrates new actions over.
    */
    /**************************************************************************/
    public void Migrate()
    {
      // Add the new actions (to prevent desync)
      foreach (var action in RecentlyAddedActions)
      {        
        ActiveActions.Add(action);
      }
      RecentlyAddedActions.Clear();
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

