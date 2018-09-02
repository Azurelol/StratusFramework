using UnityEngine;
using System;
using Stratus.Dependencies.TypeReferences;
using OdinSerializer;

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
      [SerializeField, ClassExtends(typeof(Task), Grouping = ClassGrouping.ByNamespace)]
      public ClassTypeReference type;      
      [Tooltip("The list of symbols in the agent's world state that need to be present before the action can be used")]
      public WorldState preconditions = new WorldState();
      [Tooltip("The list of symbols that are applied to the agent's worldstate after the action is completed")]
      public WorldState effects = new WorldState();
      [Tooltip("The cost of this action. Used by the planner's A*")]
      public float cost;
      /// <summary>
      /// The encapsulated action
      /// </summary>
      [OdinSerialize]
      public Task task;

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/

      /// <summary>
      /// A short description of this action
      /// </summary>
      public string description => task.description;
      /// <summary>
      /// Some actions require specific context precondition which needs to be checked
      /// before normal preconditions. Due to us wrapping around normal actions,
      /// we are currently not using it, so it will always return true.
      /// </summary>
      public bool contextPrecondition => true;

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Updates the underlying action
      /// </summary>
      /// <param name="dt"></param>
      public void OnUpdate(Agent agent)
      {
        task.Update(agent);
      }
        
        /// <summary>
      /// Resets the underlying action
      /// </summary>
      public void Reset() { task.Reset(); }
      

    }
  }
}
