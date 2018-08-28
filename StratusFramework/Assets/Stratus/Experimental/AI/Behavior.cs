using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// An abstract interface that can be activated, run and deactivated. For example, actions (leaf nodes),
    /// decorators and composites are all behaviors
    /// </summary>
    [Serializable]
    public abstract class Behavior : INamed
    {
      /// <summary>
      /// A behavior that needs to be updated every frame
      /// </summary>
      public interface IUpdatable
      {
        Status Update(float dt);
      }

      /// <summary>
      /// Enumerated status code
      /// </summary>
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
        /// <summary>
        /// The behavior is currently not running, pending outside reactivation
        /// </summary>
        Suspended,
        /// <summary>
        /// The behaviour is still running
        /// </summary>
        Running
      }
      
      public class BehaviorEvent : Stratus.Event { public Behavior behavior; }
      public class StartedEvent : BehaviorEvent { }
      public class UpdateEvent : BehaviorEvent { }
      public class SuspendEvent : BehaviorEvent { }
      public class EndedEvent : BehaviorEvent { }
      public class CanceledEvent : BehaviorEvent { }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The name for this behavior
      /// </summary>      
      public string label;

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// The current state of this behavior
      /// </summary>
      public Status status { protected set; get; }
      /// <summary>
      /// The agent this behavior is acting upon
      /// </summary>
      public Agent agent { private set; get; }
      /// <summary>
      /// Whether this behavior needs to be polled 
      /// </summary>
      public bool isUpdated { private set; get; }
      /// <summary>
      /// A short description of what this behavior does
      /// </summary>
      public abstract string description { get; }
      /// <summary>
      /// The name for this behavior
      /// </summary>
      public string name => this.label;

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      /// <summary>
      /// Called once when the behavior is activated
      /// </summary>
      protected abstract void OnStart();
      /// <summary>
      /// If the behavior needs to be updated, it is called
      /// </summary>
      /// <param name="dt"></param>
      /// <returns></returns>
      protected abstract Status OnUpdate(float dt);
      /// <summary>
      /// Called once after hte behavior has finished executing
      /// </summary>
      protected abstract void OnEnd();

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public Behavior()
      {
        //this.label = GetType().Name;
      }

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      /// <summary>
      /// This method is called once, immediately before the first call to this
      /// behavior's update method
      /// </summary>
      /// <param name="agent"></param>
      public virtual void Start(Agent agent)
      {
        this.agent = agent;
        // Whether this behavior needs to be updated every frame
        this.isUpdated = (typeof(IUpdatable).IsAssignableFrom(GetType().DeclaringType));
        // Now initialize the subclass
        this.OnStart();
        this.agent.gameObject.Dispatch<StartedEvent>(new StartedEvent() { behavior = this });
      }

      public virtual Status Execute(float dt)
      {
        return this.OnUpdate(dt);
      }      

      public virtual void End()
      {
        this.OnEnd();
        this.agent.gameObject.Dispatch<EndedEvent>(new EndedEvent() { behavior = this });
      }

      public static Behavior Instantiate(Type behaviorType)
      {
        if (!behaviorType.IsSubclassOf(typeof(Behavior)))
          return null;
        return (Behavior)Utilities.Reflection.Instantiate(behaviorType);
      }





    }
  } 
}
