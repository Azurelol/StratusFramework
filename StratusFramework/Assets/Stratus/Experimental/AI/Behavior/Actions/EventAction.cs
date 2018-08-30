using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  public class EventAction : Action
  {
    public override string description => "Invokes an event";

    protected override void OnActionEnd()
    {
      
    }

    protected override void OnActionReset()
    {
      
    }

    protected override void OnActionStart()
    {
      
    }

    protected override Status OnActionUpdate(float dt)
    {
      Trace.Script("Woof!");
      return Status.Success;
    }
  }
}
