using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  public class AgentEvent : Triggerable
  {
    public Agent agent;
    public Agent.Event eventType;
    [DrawIf(nameof(AgentEvent.eventType), Agent.Event.Move, ComparisonType.Equals)]
    public PositionField position = new PositionField();

    public override string automaticDescription
    {
      get
      {
        if (agent)
          return $"{agent.name} {eventType}";
        return string.Empty;
      }
    }

    protected override void OnAwake()
    {
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      switch (eventType)
      {
        case Agent.Event.Move:
          agent.gameObject.Dispatch<Agent.MoveEvent>(new Agent.MoveEvent(position));
          break;
        case Agent.Event.Death:
          agent.gameObject.Dispatch<Agent.DeathEvent>(new Agent.DeathEvent());
          break;
        default:
          break;
      }
    }


  }

}