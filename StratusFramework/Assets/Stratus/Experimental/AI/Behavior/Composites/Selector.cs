using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Nodes execute their children from left to right, and will stop executing
    /// for its children when one of their children succeeds. If all the selector's
    /// children succeed, the selector succeeds. if all fail, the selector fails.
    /// </summary>
    public class Selector : Composite
    {
      public IEnumerator<Behavior> childrenEnumerator { get; private set; }
      public override Behavior currentChild => childrenEnumerator.Current;

      public override string description { get; } = "Nodes execute their children from left to right, and will stop executing " +
            "its children when one of their children succeeds. If a Selector's child succeeds, " +
            "the Selector succeeds. If all the Selector's children fail, the Selector fails.";

      protected override void OnCompositeStart(Arguments args)
      {
        this.childrenEnumerator = children.GetEnumerator();
      }

      protected override bool OnCompositeSetNextChild(Arguments args)
      {
        bool valid = this.childrenEnumerator.MoveNext();
        if (valid)
        {
          this.currentChild.Start(args, this.OnCompositeChildEnded);
        }
        return valid;
      }


      protected override bool OnCompositeChildEnded(Arguments args, Status status)
      {
        if (status == Status.Success)
        {
          this.End(args, Status.Success);
          return true;
        }
        else if (!this.OnCompositeSetNextChild(args))
        {
          this.End(args, Status.Failure);
        }
        return true;
      }

    }
  }

}