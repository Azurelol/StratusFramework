using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  /// <summary>
  /// Bases its condition on wheher its duration has expired
  /// </summary>
  public class Cooldown : Decorator
  {
    public float duration = 5.0f;

    public override string description { get; } = "Bases its condition on wheher its duration has expired";

    protected override void OnEnd()
    {      
    }

    protected override void OnStart()
    {      
    }

    protected override Status OnUpdate(float dt)
    {
      return Status.Running;      
    }
  }

}