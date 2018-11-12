using UnityEngine;

/// <summary>
/// An ActionOwner is a container of all actions a particular GameObject
/// has.They propagate updates to all actions attached to it.
/// </summary>
namespace Stratus
{
  public class StratusActionsOwner : StratusActionGroup
  {
    public GameObject owner;

    public StratusActionsOwner(GameObject owner, TimeScale mode = TimeScale.Delta) : base(mode)
    {
      this.owner = owner;
      if (StratusActions.debug) Trace.Script("Owner = '" + this.owner + "'");
    }

    ~StratusActionsOwner() { }
    
    /// <summary>
    /// Updates a GameObject's actions, updating all the actions one tier below
    /// in parallel.
    /// </summary>
    /// <param name="dt">The time to be updated.</param>
    /// <returns>How much time was consumed while updating.</returns>
    public override float Update(float dt)
    {
      Migrate();
      return base.Update(dt);
    }
  }
}