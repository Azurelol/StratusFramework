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
    public abstract class Composite : Behavior, IDecoratorSupport
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      [OdinSerializer.OdinSerialize]
      public List<Decorator> decorators = new List<Decorator>();

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      List<Decorator> IDecoratorSupport.decorators => this.decorators;
      public List<Behavior> children { private set; get; }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public void Add(Behavior child)
      {
        children.Add(child);
      }

      public void Remove(Behavior child)
      {
        children.Remove(child);
      }

      public void Clear()
      {
        children.Clear();
      }

    } 
  }

}