using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  public class SetRandomPosition : Service
  {
    public enum Type
    {
      Point,
      Target
    }

    public string key;
    public Type type;
    public float radius = 5;

    protected override void OnExecute(Agent agent)
    {
      Vector3 position = Vector3.zero;
      switch (this.type)
      {
        case Type.Point:
          position = agent.transform.position;
          Vector2 offset = Random.insideUnitCircle * radius;
          position.x += offset.x;
          position.z += offset.y;
          break;

        case Type.Target:
          Transform[] targets = agent.transform.GetTransformsWithinRadius(radius);
          position = targets.Random().position;
          break;
      }

      //Trace.Script($"{key} set to {position}");
      agent.blackboard.SetLocal(agent.gameObject, key, position);
    }

    
  }

}