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
    private Stratus.Cooldown cooldown;
    public override string description { get; } = "Bases its condition on wheher its duration has expired";

    protected override void OnStart(Agent agent)
    {
      this.cooldown = new Stratus.Cooldown(this.duration);
    }

    protected override void OnEnd(Agent agent)
    {
      this.cooldown.Activate();
    }

    protected override Status OnUpdate(Agent agent)
    {
      cooldown.Update(Time.deltaTime);
      if (cooldown.isActive)
        return Status.Running;
      return Status.Success;
    }
  }

}