using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  public class EventTask : Task 
  {
    public override string description => "Invokes an event";


    protected override void OnTaskEnd(Agent agent)
    {
      Trace.Script("Woof!");
    }

    protected override void OnTaskStart(Agent agent)
    {
    }

    protected override Status OnTaskUpdate(Agent agent)
    {
      return Status.Success;
    }
  }

  public class LogTask : Task
  {
    public string message;
    public override string description => "Prints a message to the console";

    protected override void OnTaskEnd(Agent agent)
    {
      Trace.Script(message);
    }

    protected override void OnTaskStart(Agent agent)
    {
    }

    protected override Status OnTaskUpdate(Agent agent)
    {
      return Status.Success;
    }
  }



}
