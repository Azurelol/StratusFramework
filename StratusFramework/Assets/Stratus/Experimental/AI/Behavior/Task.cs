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
    public abstract class Task : Behavior, IDecoratorSupport
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


      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/   
      protected abstract void OnTaskStart(Agent agent);
      protected abstract Status OnTaskUpdate(Agent agent);
      protected abstract void OnTaskEnd(Agent agent);

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
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