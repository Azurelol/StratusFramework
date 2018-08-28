using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Executes each of the child behaviors in sequence
    /// until all of the children have executed successfully
    /// or until one of the child behaviors fail
    /// </summary>
    public class Sequence : Composite
    {
      public Behavior currentChild { private set; get; }
      public List<Behavior>.Enumerator childrenEnumerator { get; private set; }

      public override string description
      {
        get
        {
          return "Executes each of the child behaviors in sequence until all of the children " +
                 "have executed successfully or until one of the child behaviors fail";
        }
      }

      protected override void OnStart()
      {
        this.childrenEnumerator = children.GetEnumerator();
        this.currentChild = childrenEnumerator.Current;
      }

      protected override Status OnUpdate(float dt)
      {
        // Keep going until a child behavior says its running
        while (true)
        {
          var status = currentChild.Execute(dt);
          if (status != Status.Success)
            return status;
          // If we have reached the end of the collection
          if (!this.childrenEnumerator.MoveNext())
            return Status.Success;
          // Otherwise keep going
          currentChild = this.childrenEnumerator.Current;
        }
        
      }
      protected override void OnEnd()
      {        
      }

    }
  }

}