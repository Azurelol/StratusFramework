using UnityEngine;
using System;
using Stratus.Dependencies.TypeReferences;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// An action that has enhanced to have a list of preconditions and effects,
    /// necessary ingredients to be used in a goal-oriented planner
    /// </summary>
    //[Serializable]
    [CreateAssetMenu(fileName = "Stateful Action", menuName = "Stratus/AI/Stateful Action")]
    public class StatefulAction : ScriptableObject
    {
      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      //[SerializeField]
      //private Blackboard BlackboardAsset;

      [SerializeField, ClassExtends(typeof(Action), Grouping = ClassGrouping.ByNamespace)]
      public ClassTypeReference Type;      


      [Tooltip("The list of symbols in the agent's world state that need to be present before the action can be used")]
      public WorldState Preconditions = new WorldState();
      [Tooltip("The list of symbols that are applied to the agent's worldstate after the action is completed")]
      public WorldState Effects = new WorldState();
      [Tooltip("The cost of this action. Used by the planner's A*")]
      public float Cost;

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/

      public Action Action;

      /// <summary>
      /// A short description of this action
      /// </summary>
      public string Description { get { return Action.Description; } }

      /// <summary>
      /// Some actions require specific context precondition which needs to be checked
      /// before normal preconditions. Due to us wrapping around normal actions,
      /// we are currently not using it, so it will always return true.
      /// </summary>
      public bool ContextPrecondition { get { return true; } }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Initializes the underlying action
      /// </summary>
      /// <param name="agent"></param>
      public void Initialize(Agent agent)
      {
        Action.Initialize(agent);
      }

      /// <summary>
      /// Updates the underlying action
      /// </summary>
      /// <param name="dt"></param>
      public void Execute(float dt)
      {
        Action.Execute(dt);
      }
        
        /// <summary>
      /// Resets the underlying action
      /// </summary>
      public void Reset() { Action.Reset(); }
      

    }
  }
}
