using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
  public class SetRandomPosition : Service
  {
    public string key;

    protected override void OnExecute(Agent agent)
    {
      Vector2 offset = Random.insideUnitCircle * 5;
      Vector3 position = agent.transform.position;
      Trace.Script($"{key} set to {position}");
      position.x += offset.x;
      position.z += offset.y;
      agent.blackboard.SetLocal(agent.gameObject, key, position);
    }
  }

}