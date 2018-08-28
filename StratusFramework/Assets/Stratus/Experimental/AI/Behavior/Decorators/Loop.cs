using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  /// <summary>
  /// Bases its condition on wheher its loop counter has exceeded
  /// </summary>
  public class Loop : Decorator
  {
    public int counter = 3;

    public override string description => "Bases its condition on wheher its loop counter has exceeded";

    protected override void OnEnd()
    {
      
    }

    protected override void OnStart()
    {
      
    }

    protected override Status OnUpdate(float dt)
    {
      throw new System.NotImplementedException();
    }
  }

}