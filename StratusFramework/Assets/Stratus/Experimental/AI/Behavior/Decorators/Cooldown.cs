using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  /// <summary>
  /// Bases its condition on wheher its duration has expired
  /// </summary>
  public class Cooldown : PreExecutionDecorator 
  {
    public float duration = 5.0f;
    private Stratus.Cooldown cooldown;

    public override string description { get; } = "Bases its condition on wheher its duration has expired";

    protected override void OnDecoratorStart(Arguments args)
    {
      if (this.cooldown == null)
        this.cooldown = new Stratus.Cooldown(this.duration);
      else
        this.cooldown.Activate();
    }

    protected override Status OnUpdate(Arguments args)
    {
      cooldown.Update(Time.deltaTime);
      if (cooldown.isActive)
        return Status.Running;
      return Status.Success;
    }



    protected override bool OnDecoratorCanChildExecute(Arguments args)
    {
      return false;
    }
  }

}