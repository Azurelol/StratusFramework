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

      public class Arguments
      {
        public Agent agent;
        public BehaviorSystem system;
        public System.Action<Status> onFinished;

        public Status lastStatus;
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
      /// The latest status of this behavior
      /// </summary>
      public Status status { protected set; get; }
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
      /// <summary>
      /// The name of the type of this behavior
      /// </summary>
      public string typeName => GetType().Name;
      /// <summary>
      /// A more descriptive name for this behavior
      /// </summary>
      public string fullName
      {
        get
        {
          if (!string.IsNullOrEmpty(label))
            return $"{typeName} ({label})";
          return $"{typeName}";
        }
      }
      /// <summary>
      /// Whether this behavior has started
      /// </summary>
      public bool started { get; private set; }
      public bool finished { get; private set; }

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      /// <summary>
      /// Called once when the behavior is activated
      /// </summary>
      protected abstract void OnStart(Arguments agent);
      /// <summary>
      /// If the behavior needs to be updated, it is called
      /// </summary>
      /// <param name="dt"></param>
      /// <returns></returns>
      protected abstract Status OnUpdate(Arguments agent);
      /// <summary>
      /// Called once after hte behavior has finished executing
      /// </summary>
      protected abstract void OnEnd(Arguments agent);
      /// <summary>
      /// Resets the state of the behavior
      /// </summary>
      protected virtual void OnReset()
      {
      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      public virtual void Update(Arguments args)
      {
        // Start
        if (!this.started)
        {
          //Trace.Script($"Starting {fullName}");
          this.status = Status.Running;
          this.started = true;
          this.finished = false;
          this.OnStart(args);
          args.system.OnBehaviorStarted(this);
        }

        // Update
        //Trace.Script($"Updating {fullName}"); 
        this.status = this.OnUpdate(args);

        // End
        if (this.status != Status.Running)
        {
          //Trace.Script($"Ending {fullName}");
          this.finished = true;
          this.OnEnd(args);
          args.system.OnBehaviorEnded(this, status);
          this.Reset();
        }
      }      

      public void Reset()
      {
        //this.status = Status.Suspended;
        this.started = false;
        //this.finished = true;
      }

      //----------------------------------------------------------------------/
      // Static Methods
      //----------------------------------------------------------------------/
      public static Behavior Instantiate(Type behaviorType)
      {
        if (!behaviorType.IsSubclassOf(typeof(Behavior)))
          return null;
        return (Behavior)Utilities.Reflection.Instantiate(behaviorType);
      }

      public static T Instantiate<T>() where T : Behavior
      {
        return (T)Instantiate(typeof(T));
      }





    }
  } 
}
