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

      /// <summary>
      /// Arguments used for updating behaviors
      /// </summary>
      protected Behavior.Arguments behaviorArguments { private set;  get; }
      
      /// <summary>
      /// A description of the state of the system
      /// </summary>
      public string stateDescription { get; }
      
      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnInitialize();
      protected abstract void OnUpdate();
      protected abstract void OnReset();
      protected abstract void OnBehaviorAdded(Behavior behavior);
      public abstract void OnBehaviorStarted(Behavior behavior);
      public abstract void OnBehaviorEnded(Behavior behavior, Behavior.Status status);
      protected abstract void OnBehaviorsCleared();

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes the system for the given agent
      /// </summary>
      public void InitializeSystem()
      {
        this.behaviorArguments = new Behavior.Arguments() { agent = this.agent, system = this };
        this.OnInitialize();
      }

      /// <summary>
      /// Updates this behavior system.
      /// </summary>
      /// <param name="dt"></param>
      public void UpdateSystem()
      {
        this.OnUpdate();
      }

      /// <summary>
      /// Resets the behaviour system so that it must evaluate from the beginning
      /// </summary>
      public void ResetSystem()
      {
        this.OnReset();
      }

      /// <summary>
      /// Initializes an instance of this system, unique for the given agent
      /// </summary>
      /// <param name="agent"></param>
      /// <param name="system"></param>
      public static BehaviorSystem InitializeSystemInstance(Agent agent, BehaviorSystem system)
      {
        if (!agentBehaviors.ContainsKey(agent))
          agentBehaviors.Add(agent, system.Instantiate(agent));

        BehaviorSystem instance = agentBehaviors[agent];
        instance.InitializeSystem();
        return instance;
      }

      /// <summary>
      /// Updates an instance of the system that is unique for the given agent
      /// </summary>
      /// <param name="agent"></param>
      /// <param name="system"></param>
      public static void UpdateSystemInstance(Agent agent)
      {
        agentBehaviors[agent].UpdateSystem();
      }

      /// <summary>
      /// Resets the behaviour system so that it must evaluate from the beginning
      /// </summary>
      public void ResetSystemInstance(Agent agent)
      {
        agentBehaviors[agent].ResetSystem();
      }

      //------------------------------------------------------------------------/
      // Behavior
      //------------------------------------------------------------------------/
      /// <summary>
      /// Adds a behaviour to the system
      /// </summary>
      /// <param name="behaviorType"></param>
      public Behavior AddBehavior(Type behaviorType)
      {
        Behavior behavior = Behavior.Instantiate(behaviorType);
        this.AddBehavior(behavior);
        return behavior;
      }

      /// <summary>
      /// Adds a behaviour to the system
      /// </summary>
      /// <param name="behaviorType"></param>
      public T AddBehavior<T>() where T : Behavior
      {
        T behavior = Behavior.Instantiate<T>();
        this.AddBehavior(behavior);
        return behavior;
      }

      /// <summary>
      /// Adds a behaviour to the system
      /// </summary>
      /// <param name="type"></param>
      public void AddBehavior(Behavior behavior)
      {
        this.OnBehaviorAdded(behavior);
      }

      /// <summary>
      /// Adds a behaviour to the system
      /// </summary>
      /// <param name="type"></param>
      public void AddBehavior<T>(T behavior) where T : Behavior
      {
        this.OnBehaviorAdded(behavior);
      }

      /// <summary>
      /// Clears all behaviors from this system
      /// </summary>
      public void ClearBehaviors()
      {
        this.OnBehaviorsCleared();
      }

      //public void StartBehavior(Behavior behavior) => OnBehaviorStarted(behavior);
      //public void EndBehavior(Behavior behavior) => OnBehaviorStarted(behavior);

      //------------------------------------------------------------------------/
      // Methods: Static
      //------------------------------------------------------------------------/
      /// <summary>
      /// Returns an instance of this behavior system to be used by a single agent.
      /// </summary>
      /// <param name="agent"></param>
      /// <returns></returns>
      public BehaviorSystem Instantiate(Agent agent)
      {
        var behaviorSystem = Instantiate(this);
        behaviorSystem.agent = agent;
        return behaviorSystem;
      }

      /// <summary>
      /// Returns an instance of this behavior system to be used by a single agent.
      /// </summary>
      /// <param name="agent"></param>
      /// <returns></returns>
      public BehaviorSystem Instantiate(Blackboard blackboard)
      {
        var behaviorSystem = Instantiate(this);
        behaviorSystem.blackboard = blackboard;
        return behaviorSystem;
      }

    }
  }

}