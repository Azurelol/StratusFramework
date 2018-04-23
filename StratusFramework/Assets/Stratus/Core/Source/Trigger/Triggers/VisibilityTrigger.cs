using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public class VisibilityTrigger : Trigger
  {
    [Header("Visibility")]
    [Tooltip("The target to observe, which requires a mesh renderer")]
    public MeshRenderer visibilityTarget;
    [Tooltip("How long the target must be visible")]
    [Range(0f, 3f)]
    public float duration = 1.0f;
    [Tooltip("The object's distance from the camera")]
    public float distance = 10f;

    public override string automaticDescription
    {
      get
      {
        if (visibilityTarget)
          return $"On {visibilityTarget.name} visible for {duration} at {distance} units";
        return string.Empty;
      }
    }



    protected override void OnAwake()
    {
      VisibilityProxy.Construct(visibilityTarget, OnVisible, duration, distance, false);
    }

    protected override void OnReset()
    {

    }

    private void OnVisible(bool visible)
    {
      if (visible)
        Activate();
    }

  }

}