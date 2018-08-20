using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Genitus
{
  /// <summary>
  /// The collection of all active effects on a combat controller
  /// </summary>
  public class EffectsModule : CombatControllerModule
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    List<Status.Instance> statuses = new List<Status.Instance>();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnInitialize()
    {
      
    }

    public override void OnTimeStep(float step)
    {
      foreach (var status in statuses)
      {
        // If the status has run its duration...
        if (status.Persist(step))
        {
          // Apply its final effect
          status.End();
          // Remove it
          Remove(status);
          break;
        }
      }
    }

    void OnStatusEvent(Status.StartedEvent e)
    {
      statuses.Add(e.status);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Adds the status. If it is already present, it will follow whatever
    /// rules for stackability/diminishing returns it has.
    /// </summary>
    /// <param name="status">The instance of the status to be added.</param>
    public void Add(Status.Instance newStatus)
    {
      //Trace.Script("Adding" + newStatus.Name);

      // Apply its initial effect
      newStatus.Start();

      // Check whether an instance of the status is already present
      var presentStatus = Find(newStatus.status);
      if (presentStatus != null)
      {
        // Determine whether to stack it
        if (presentStatus.status.IsStackable)
        {
          presentStatus.Stack();
        }
        // Determine whether to reset its duration
        presentStatus.Reset();
      }
      // If not present, add it
      else
      {
        statuses.Add(newStatus);
      }
    }

    //public void Add()

    /// <summary>
    /// Removes this status.
    /// </summary>
    /// <param name="status"></param>
    public void Remove(Status.Instance status)
    {
      // Announce the status has been removed
      var endedEvent = new Status.EndedEvent();
      endedEvent.status = status;
      status.target.gameObject.Dispatch<Status.EndedEvent>(endedEvent);
      // Now remove it
      statuses.Remove(status);
    }

    /// <summary>
    /// Validates whether the status is currently present.
    /// </summary>
    /// <param name="status">A reference to the status.</param>
    /// <returns>True if the status is present, false otherwise. </returns>
    public bool Has(Status status)
    {
      return (Find(status) != null);
    }

    /// <summary>
    /// Finds a current instance of the specified status.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public Status.Instance Find(Status status)
    {
      return (statuses.Find(x => x.status == status));
    }


  }

}