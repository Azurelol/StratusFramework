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
    public abstract class Composite : Behavior, IServiceSupport
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      [OdinSerializer.OdinSerialize]
      public List<Service> services = new List<Service>();

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      List<Service> IServiceSupport.services => this.services;
      public IList<Behavior> children { private set; get; }
      public bool hasChildren => children.NotNullOrEmpty();

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public void Set(IList<Behavior> children)
      {
        this.children = children;
      }

      public override Status Update(Agent agent)
      {
        foreach (var service in this.services)
          service.Execute(agent);
        return base.Update(agent);
      }

    } 
  }

}