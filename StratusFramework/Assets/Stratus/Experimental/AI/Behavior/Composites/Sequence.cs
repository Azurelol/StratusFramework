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
      public IEnumerator<Behavior> childrenEnumerator { get; private set; }
      public Behavior currentChild => childrenEnumerator.Current;

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
      }

      protected override Status OnUpdate(Arguments args)
      {
        // Keep going until a child behavior says its running
        while (true)
        { 
          currentChild.Update(args);

          if (currentChild.finished)
          {
            if (currentChild.status != Status.Success)
              return currentChild.status;
          }
          else
          {
            return Status.Running;
          }
          

          // If we have reached the end of the collection
          if (!this.childrenEnumerator.MoveNext())
          {
            Trace.Script("Reached end of sequence");
            return Status.Success;
          }

          Trace.Script($"Moved onto next child {currentChild.fullName}");
        }
      }

      protected override void OnEnd(Arguments args)
      {
        foreach(var child in children)
        {
          child.Reset();
        }

        //this.OnStart(args);
      }

      //private void OnChildFinished(Status status)
      //{
      //
      //}

    }
  }

}