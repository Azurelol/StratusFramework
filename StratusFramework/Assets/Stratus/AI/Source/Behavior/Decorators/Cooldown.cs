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
    private Countdown cooldown;

    public override string description { get; } = "Bases its condition on wheher its duration has expired";

    protected override bool OnDecoratorCanChildExecute(Arguments args)
    {
      if (this.cooldown == null)
      {
        this.cooldown = new Stratus.Countdown(this.duration);
        StratusUpdateSystem.Add(this.cooldown);
      }

      if (this.cooldown.isFinished)
      {
        this.cooldown.Reset();
        return true;
      }

      //Trace.Script($"Timer @ {cooldown.progress}");
      return false;
    }
  }

}