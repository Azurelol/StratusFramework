using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  /// <summary>
  /// A trigger that pairs itself to another trigger. When that one is activated,
  /// so will this one be.
  /// </summary>
  public class PairedTrigger : Trigger
  {
    [Header("Pair Settings")]
    [Tooltip("The other trigger which we activate on")]
    public Trigger other;
    
    protected override void OnAwake()
    {
      other.onActivate += OnPairActivated;
    }

    void OnPairActivated(Trigger other)
    {
      Activate();
    }

  }
}