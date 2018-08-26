using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.AI;

namespace Stratus.Gameplay
{
  public class AgentEvent : Triggerable
  {
    public enum EventType
    {
      Move,
      Death,
      Revive
    }

    public Agent agent;
    public EventType eventType;
    [DrawIf(nameof(AgentEvent.eventType), EventType.Move, ComparisonType.Equals)]
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
        case EventType.Move:
          agent.gameObject.Dispatch<Agent.MoveEvent>(new Agent.MoveEvent(position));
          break;
        case EventType.Death:
          //agent.gameObject.Dispatch<CombatAgent.DeathEvent>(new CombatAgent.DeathEvent());
          break;
        default:
          break;
      }
    }


  }

}