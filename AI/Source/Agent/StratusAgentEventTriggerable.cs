using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.AI;

namespace Stratus.Gameplay
{
	public class StratusAgentEventTriggerable : StratusTriggerable
	{
		public enum EventType
		{
			Move,
			Death,
			Revive
		}

		public StratusAgent agent;
		public EventType eventType;
		[DrawIf(nameof(StratusAgentEventTriggerable.eventType), EventType.Move, ComparisonType.Equals)]
		public StratusPositionField position = new StratusPositionField();

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
					agent.gameObject.Dispatch<StratusAgentNavigation.MoveEvent>(new StratusAgentNavigation.MoveEvent(position));
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