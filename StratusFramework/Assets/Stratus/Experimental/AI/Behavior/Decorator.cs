using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Stratus.AI
{
  /// <summary>
  /// Decorator, also known as conditionals in other Behavior Tree systems, 
  /// are attached to either a Composite or a Task node and define whether or not a branch in the tree, 
  /// or even a single node, can be executed.
  /// </summary>
  public abstract class Decorator : Behavior
  {
    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    //protected override void OnStart(Agent agent)
    //{
    //}
    //
    //protected override void OnEnd(Agent agent)
    //{
    //}
    //
    //protected override Status OnUpdate(Agent agent)
    //{
    //}
  }
}
