using UnityEngine;

namespace Stratus.AI
{
	public class StratusSetRandomPositionService : StratusAIService
	{
		public enum Type
		{
			Point,
			Target
		}

		public string key;
		public Type type;
		public float radius = 5;

		protected override void OnExecute(StratusAgent agent)
		{
			Vector3 position = Vector3.zero;
			switch (this.type)
			{
				case Type.Point:
					position = agent.transform.position;
					Vector2 offset = Random.insideUnitCircle * this.radius;
					position.x += offset.x;
					position.z += offset.y;
					break;

				case Type.Target:
					Transform[] targets = agent.transform.GetTransformsWithinRadius(this.radius);
					position = targets.Random().position;
					break;
			}

			agent.blackboard.SetLocal(agent.gameObject, this.key, position);
		}


	}

}