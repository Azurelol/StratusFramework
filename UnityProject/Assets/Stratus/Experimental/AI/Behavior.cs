/******************************************************************************/
/*!
@file   Behavior.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// An abstract interface that can be activated, run
    /// and deactivated. For example, actions (leaf nodes),
    /// decorators and composites are all behaviors
    /// </summary>
    [Serializable]
    public abstract class Behavior : ScriptableObject
    {
      /// <summary>
      /// A behavior that needs to be updated every frame
      /// </summary>
      public interface IUpdatable
      {
        Status Update(float dt);
      }

      public enum Status
      {
        /// <summary>
        /// Everything went as expected
        /// </summary>
        Success,
        /// <summary>
        /// Something apparently went wrong
        /// </summary>
        Failure,
        Suspended,
        Running
      }
      
      public class BehaviorEvent : Stratus.Event { public Behavior Behavior; }
      public class StartedEvent : BehaviorEvent { }
      public class UpdateEvent : BehaviorEvent { }
      public class SuspendEvent : BehaviorEvent { }
      public class EndedEvent : BehaviorEvent { }
      public class CanceledEvent : BehaviorEvent { }

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      public bool Enabled { protected set; get; }
      public bool Active { protected set; get; }
      /// <summary>
      /// The name of this behavior
      /// </summary>
      public string Name { get { return GetType().DeclaringType.Name; } }
      /// <summary>
      /// A short description of what this behavior does
      /// </summary>
      public abstract string Description { get; }
      /// <summary>
      /// The agent this behavior is acting upon
      /// </summary>
      public Agent Agent { private set; get; }
      /// <summary>
      /// The current state of this behavior
      /// </summary>
      public Status CurrentStatus { protected set; get; }
      /// <summary>
      /// Whether this behavior is currently being debugged
      /// </summary>
      protected bool Tracing = true;

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      /// <summary>
      /// Called once after hte behavior has finished executing
      /// </summary>
      protected abstract void OnEnd();
      /// <summary>
      /// If the behavior needs to be updated, it is called
      /// </summary>
      /// <param name="dt"></param>
      /// <returns></returns>
      protected abstract Status OnUpdate(float dt);
      /// <summary>
      /// Called once when the behavior is activated
      /// </summary>
      protected abstract void OnStart();
      /// <summary>
      /// Whether this behavior needs to be polled 
      /// </summary>
      public bool IsUpdated { private set; get; }

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      /// <summary>
      /// This method is called once, immediately before the first call to this
      /// behavior's update method
      /// </summary>
      /// <param name="agent"></param>
      public virtual void Initialize(Agent agent)
      {
        this.Agent = agent;

        // Whether this behavior needs to be updated every frame
        this.IsUpdated = (typeof(IUpdatable).IsAssignableFrom(GetType().DeclaringType));
        // Now initialize the subclass
        this.OnStart();
        this.Agent.gameObject.Dispatch<StartedEvent>(new StartedEvent() { Behavior = this });
      }


      public virtual Status Execute(float dt)
      {
        return this.OnUpdate(dt);
      }      

      /// <summary>
      /// Ends the behavior
      /// </summary>
      public virtual void End()
      {
        this.OnEnd();
        this.Agent.gameObject.Dispatch<EndedEvent>(new EndedEvent() { Behavior = this });
      }





    }
  } 
}
