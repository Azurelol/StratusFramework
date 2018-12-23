using Stratus.Utilities;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
      public static Color color => StratusGUIStyles.Colors.jade;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/   
      protected abstract void OnTaskStart(Agent agent);
      protected abstract Status OnTaskUpdate(Agent agent);
      protected abstract void OnTaskEnd(Agent agent);

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      public override void Update(Arguments args)
      {
        foreach (var service in this.services)
          service.Execute(args.agent);
        base.Update(args);
      }

      protected override void OnStart(Arguments args)
      {
        this.OnTaskStart(args.agent);
      }

      protected override Status OnUpdate(Arguments args)
      {
        return this.OnTaskUpdate(args.agent);
      }

      protected override void OnEnd(Arguments args)
      {
        this.OnTaskEnd(args.agent);
      }

    }

  }

}