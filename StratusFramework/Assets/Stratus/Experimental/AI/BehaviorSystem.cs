using UnityEngine;
using Stratus;
using System.Text;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    public abstract class BehaviorSystem : StratusScriptable
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      [Header("Debug")]
      /// <summary>
      /// Whether this system will print debug output
      /// </summary>
      public bool debug = false;

      /// <summary>
      /// A short description of what this system does
      /// </summary>
      public string description;

      /// <summary>
      /// The blackboard the blackboard is using.
      /// </summary>
      public Blackboard blackboard;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// Whether this behavior system is currently running
      /// </summary>
      public bool active { protected set; get; }

      /// <summary>
      /// The agent this system is using
      /// </summary>
      public Agent agent { private set; get; }

      /// <summary>
      /// The sensor the agent is using
      /// </summary>
      protected Sensor sensor { private set; get; }

      /// <summary>
      /// The current behavior being run by this system
      /// </summary>
      protected abstract Behavior currentBehavior { get; }

      /// <summary>
      /// Whether this system has behaviors present
      /// </summary>
      protected abstract bool hasBehaviors { get; }

      /// <summary>
      /// All currently running behavior systems for given agents
      /// </summary>
      protected static Dictionary<Agent, BehaviorSystem> agentBehaviors { get; set; } = new Dictionary<Agent, BehaviorSystem>();
      
      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnInitialize();
      protected abstract void OnUpdate(Agent agent);
      protected abstract void OnPrint(StringBuilder builder);
      // Behaviors
      protected abstract void OnBehaviorAdded(Behavior behavior);
      protected abstract void OnBehaviorStarted(Behavior behavior);
      protected abstract void OnBehaviorEnded(Behavior behavior);
      protected abstract void OnBehaviorsCleared();
      protected abstract void OnReset();

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes the system for the given agent
      /// </summary>
      public void Initialize(Agent agent)
      {
        if (!agentBehaviors.ContainsKey(agent))
          agentBehaviors.Add(agent, Instantiate(agent));

        agentBehaviors[agent].OnInitialize();
        //this.OnInitialize();
      }

      /// <summary>
      /// Updates this behavior system.
      /// </summary>
      /// <param name="dt"></param>
      public void UpdateSystem(Agent agent)
      {
        agentBehaviors[agent].OnUpdate(agent);
        //this.OnUpdate(dt);
      }

      /// <summary>
      /// Resets the behaviour system so that it must evaluate from the beginning
      /// </summary>
      public void ResetSystem()
      {
        agentBehaviors[agent].OnReset();
        //this.OnReset();
      }

      //------------------------------------------------------------------------/
      // Behavior
      //------------------------------------------------------------------------/
      /// <summary>
      /// Adds a behaviour to the system
      /// </summary>
      /// <param name="behaviorType"></param>
      public void AddBehavior(Type behaviorType)
      {
        this.AddBehavior(Behavior.Instantiate(behaviorType));
      }

      /// <summary>
      /// Adds a behaviour to the system
      /// </summary>
      /// <param name="type"></param>
      public void AddBehavior(Behavior behaviour)
      {
        this.OnBehaviorAdded(behaviour);
      }

      /// <summary>
      /// Clears all behaviors from this system
      /// </summary>
      public void ClearBehaviors()
      {
        this.OnBehaviorsCleared();
      }

      //------------------------------------------------------------------------/
      // Methods: Utility
      //------------------------------------------------------------------------/
      /// <summary>
      /// Prints a representation of the contents of this behavior system
      /// </summary>
      /// <returns></returns>
      public string Print()
      {
        var builder = new StringBuilder();
        OnPrint(builder);
        return builder.ToString();
      }

      //------------------------------------------------------------------------/
      // Methods: Static
      //------------------------------------------------------------------------/
      /// <summary>
      /// Returns an instance of this behavior system to be used by a
      /// single agent.
      /// </summary>
      /// <param name="agent"></param>
      /// <returns></returns>
      public BehaviorSystem Instantiate(Agent agent)
      {
        var behaviorSystem = Instantiate(this);
        behaviorSystem.agent = agent;
        behaviorSystem.sensor = agent.sensor;
        return behaviorSystem;
      }

      public BehaviorSystem Instantiate(Agent agent, Blackboard blackboard)
      {
        var behaviorSystem = Instantiate(this);
        behaviorSystem.blackboard = blackboard;
        behaviorSystem.agent = agent;
        behaviorSystem.sensor = agent.sensor;
        return behaviorSystem;
      }

      //public BehaviorSystem Instantiate(Blackboard blackboard)
      //{
      //  var behaviorSystem = Instantiate(this);
      //  behaviorSystem.blackboard = blackboard;
      //  behaviorSystem.sensor = agent.sensor;
      //  return behaviorSystem;
      //}


    }
  }

}