using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace Stratus
{  
  using ActionContainer = System.Collections.Generic.List<StratusAction>;

  /// <summary>
  /// The ActionSet is the base class from which all other sets derive.
  /// Sets such as Sequence, Group and the unique set used by entities.
  /// </summary>
  public abstract class StratusActionSet : StratusAction
  {
    public TimeScale mode = TimeScale.Delta;
    protected ActionContainer activeActions = new ActionContainer();
    protected ActionContainer recentlyAddedActions = new ActionContainer();
    //public bool TraceStack = false;

    public StratusActionSet(TimeScale mode)
    {
      this.mode = mode;
    }
    public override abstract float Update(float dt);

    /// <summary>
    /// Add an action to this set
    /// </summary>
    /// <param name="action">The specified action.</param>
    public virtual void Add(StratusAction action)
    { 
      //if (TraceStack)
      //{
      //  StackTrace stackTrace = new StackTrace();
      //  var methodName = stackTrace.GetFrame(2).GetMethod().Name;
      //  //var className = stackTrace.GetFrame(2).GetMethod().ReflectedType.Name;
      //  //Trace.Script("Adding " + action.type + " from " + methodName);
      //}

      //if (StratusActions.debug) Trace.Script("'" + action.type + "'");
      this.recentlyAddedActions.Add(action);
    }
    
    /// <summary>
    /// Migrates new actions over.
    /// </summary>
    public void Migrate()
    {
      // Add the new actions (to prevent desync)
      foreach (var action in recentlyAddedActions)
      {        
        activeActions.Add(action);
      }
      recentlyAddedActions.Clear();
    }

    /// <summary>
    /// Sweeps all inactive actions.
    /// </summary>
    public void Sweep()
    {
      // No actions to clear
      if (this.activeActions.Count == 0)
        return;

      // Remove all actions that are finished
      this.activeActions.RemoveAll(x => x.isFinished == true);
    }

    /// <summary>
    /// Clears all actions.
    /// </summary>
    public void Clear()
    {
      this.activeActions.Clear();
      this.recentlyAddedActions.Clear();
    }


  }

}

