using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Branches with multiple children in a behavior tree
    /// are called composite beaviors. We make more interesting,
    /// intelligent behaviors by combining simpler behaviors together.
    /// </summary>
    public abstract class Composite : Behavior
    {
      public List<Behavior> Children { private set; get; }

      public void Add(Behavior child)
      {
        Children.Add(child);
      }

      public void Remove(Behavior child)
      {
        Children.Remove(child);
      }

      public void Clear()
      {
        Children.Clear();
      }

    } 
  }

}