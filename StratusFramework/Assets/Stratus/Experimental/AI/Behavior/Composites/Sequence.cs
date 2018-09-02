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
      public IEnumerator<Behavior> childrenEnumerator { get; private set; }

      public override string description
      {
        get
        {
          return "Executes each of the child behaviors in sequence until all of the children " +
                 "have executed successfully or until one of the child behaviors fail";
        }
      }

      protected override void OnStart(Arguments args)
      {
        this.childrenEnumerator = children.GetEnumerator();
        this.childrenEnumerator.MoveNext();
        this.currentChild = childrenEnumerator.Current;
      }

      protected override Status OnUpdate(Arguments args)
      {
        // Keep going until a child behavior says its running
        while (true)
        {
          //args.onFinished = OnChildFinished; 
          // Reset the child after going past it
          //if (currentChild.finished)
          //{
          //  currentChild.Reset();
          //}
          //else

          //if (currentChild.status != Status.Suspended)
          //{
          //
          //}
          //{
          currentChild.Update(args);
          if (currentChild.status != Status.Success)
            return status;
          //}

          // If we have reached the end of the collection
          if (!this.childrenEnumerator.MoveNext())
          {
            Trace.Script("Reached end of sequence");
            return Status.Success;
          }

          // Otherwise keep going
          currentChild = this.childrenEnumerator.Current;
          //Trace.Script($"Moved onto next child {currentChild.fullName}");
        }
      }

      protected override void OnEnd(Arguments args)
      {
        this.OnStart(args);
      }

      //private void OnChildFinished(Status status)
      //{
      //
      //}

    }
  }

}