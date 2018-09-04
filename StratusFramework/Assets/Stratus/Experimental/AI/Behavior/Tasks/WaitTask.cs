using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  public class WaitTask : TimedTask
  {
    public override string description { get; } = "Wait until the wait time is finished";

    protected override void OnTimedActionEnd(Agent agent)
    {      
    }

    protected override void OnTimedActionStart(Agent agent)
    {      
    }

    protected override Status OnTimedActionUpdate(Agent agent)
    {
      return Status.Running;
    }
  }

}