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

        public Blackboard blackboard => agent.blackboard;
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
      /// <summary>
      /// Invoked whenever this behavior ends
      /// </summary>
      public System.Func<Arguments, Status, bool> onEnded { get; private set; }

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
          this.Start(args);        

        // Update
        //Trace.Script($"Updating {fullName}"); 
        this.status = this.OnUpdate(args);

        // End
        if (this.status != Status.Running)
          this.End(args);
      }

      public void Start(Arguments args)
      {
        //Trace.Script($"Starting {fullName}");
        this.started = true;
        this.status = Status.Running;
        args.system.OnBehaviorStarted(this);
        this.OnStart(args);
      }

      public void Start(Arguments args, System.Func<Arguments, Status, bool> onEnded)
      {
        this.onEnded = onEnded;
        this.Start(args);
      }

      public void End(Arguments args)
      {
        //Trace.Script($"Ending {fullName}");
        this.OnEnd(args);
        args.system.OnBehaviorEnded(this, status);
        bool reset = (this.onEnded != null) ? this.onEnded.Invoke(args, this.status) : true;
        //if (this.onEnded != null)
        //{
        //  bool reset this.onEnded?.Invoke(args, this.status);
        //}
        if (reset)
          this.Reset();
      }

      public void End(Arguments args, Status status)
      {
        this.status = status;
        this.End(args);
      }

      public void Reset()
      {
       // Trace.Script($"Resetting {fullName}");
        this.started = false;
        this.onEnded = null;
      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Retrieves the value of a symbol using the reference and the current arguments
      /// </summary>
      /// <param name="args"></param>
      /// <param name="symbol"></param>
      /// <returns></returns>
      protected object GetSymbolValue(Arguments args, Blackboard.SymbolReference symbol)
      {
        return symbol.GetValue(args.blackboard, args.agent.gameObject);
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
