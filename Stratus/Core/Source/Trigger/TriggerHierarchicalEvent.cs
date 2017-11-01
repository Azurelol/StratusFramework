using UnityEngine;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// When triggered, will perform the specified action on a given hierarchy
  /// of objects (self, parent, children).
  /// </summary>
  public abstract class TriggerHierarchicalEvent : Triggerable
  {
    public enum HierarchyType { Self, Parent, Children }
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// What kind of targets this event has
    /// </summary>
    public HierarchyType Hierarchy = HierarchyType.Self;

    //------------------------------------------------------------------------/
    // Members
    //------------------------------------------------------------------------/
    protected List<GameObject> Targets { get; private set; }

    //------------------------------------------------------------------------/
    // Members
    //------------------------------------------------------------------------/
    protected override void PreAwake()
    {
      this.SetTargets();
    }

    /// <summary>
    /// Sets the targets for this event based on the selected hierarchy.
    /// </summary>
    void SetTargets()
    {
      if (Hierarchy == HierarchyType.Self)
      {
        Targets.Add(this.gameObject);
      }
      else if (Hierarchy == HierarchyType.Parent)
      {
        if (this.gameObject.Parent())
          Targets.Add(this.gameObject.Parent());
        else
          Debug.LogError("No parent was found!");
      }
      else if (Hierarchy == HierarchyType.Children)
      {
        foreach (var child in this.gameObject.Children())
        {
          Targets.Add(child);
        }
      }
    }
  }

}