using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// A trigger that is a composition of many other triggers. It will observe a list
  /// of triggers. When all have been activated, so will it be.
  /// </summary>
  public class CompositeTrigger : Trigger
  {
    [Tooltip("The triggers to observe")]
    public Trigger[] triggers;
    private List<Trigger> triggersLeft = new List<Trigger>();

    protected override void OnAwake()
    {
      triggersLeft.AddRange(triggers);
      foreach (var trigger in triggersLeft)
        trigger.onActivate += OnTriggerActivated;
    }

    void OnTriggerActivated(Trigger other)
    {
      triggersLeft.Remove(other);
      if (triggersLeft.Empty())
        Activate();
    }

  }

}