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
      public override Behavior currentChild => childrenEnumerator.Current;

      public override string description
      {
        get
        {
          return "Executes each of the child behaviors in sequence until all of the children " +
                 "have executed successfully or until one of the child behaviors fail";
        }
      }

      protected override void OnCompositeStart(Arguments args)
      {
        this.childrenEnumerator = children.GetEnumerator();
      }

      protected override bool OnCompositeSetNextChild(Arguments args)
      {
        bool valid = this.childrenEnumerator.MoveNext();
        if (valid)
        {
          //Trace.Script($"Moved onto next child {currentChild.fullName}");
          this.currentChild.Start(args, this.OnCompositeChildEnded);
        }
        else
        {
          //Trace.Script("Reached end of sequence");
        }
        return valid;
      }

      protected override bool OnCompositeChildEnded(Arguments args, Status status)
      {
        if (status == Status.Failure)
        {
          this.End(args, Status.Failure);
          return true;
        }
        else if (!this.OnCompositeSetNextChild(args))
        {
          this.End(args, Status.Success);
        }
        return true;
      }

    }
  }

}