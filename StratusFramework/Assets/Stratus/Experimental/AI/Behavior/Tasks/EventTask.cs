using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  public class EventAction : Task 
  {
    public override string description => "Invokes an event";


    protected override void OnTaskEnd(Agent agent)
    {
    }

    protected override void OnTaskStart(Agent agent)
    {
    }

    protected override Status OnTaskUpdate(Agent agent)
    {
      Trace.Script("Woof!");
      return Status.Success;
    }
  }
}
