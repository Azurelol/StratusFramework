using Stratus.Utilities;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Also known as a leaf node, an action represents any concrete action
    /// an agent can make (such as moving to a location, attacking a target, etc)
    /// </summary>
    public abstract class Task : Behavior, IServiceSupport
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

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/   
      protected abstract void OnTaskStart(Agent agent);
      protected abstract Status OnTaskUpdate(Agent agent);
      protected abstract void OnTaskEnd(Agent agent);

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      public override Status Update(Agent agent)
      {
        foreach (var service in this.services)
          service.Execute(agent);
        return base.Update(agent);
      }

      protected override void OnStart(Agent agent)
      {
        this.OnTaskStart(agent);
      }

      protected override Status OnUpdate(Agent agent)
      {
        return this.OnTaskUpdate(agent);
      }

      protected override void OnEnd(Agent agent)
      {
        this.OnTaskEnd(agent);
      }      

    }

  }

}