using UnityEngine;
using System.Text;
using System;

namespace Stratus
{
  namespace AI
  {
    [CreateAssetMenu(fileName = "Planner", menuName = "Stratus/AI/Planner")]
    public partial class Planner : BehaviorSystem
    {
      //------------------------------------------------------------------------/
      // Declarations: Event
      //------------------------------------------------------------------------/      


      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      [Header("Planner")]
      /// <summary>
      /// The goal set for this planner.
      /// </summary>
      public Goal goal;

      /// <summary>
      /// The current state of this agent.
      /// </summary>
      public WorldState state = new WorldState();

      /// <summary>
      /// List of all available actions to this planner.
      /// </summary>
      public StatefulAction[] availableActions;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The currently formulated plan
      /// </summary>
      public Plan currentPlan { private set; get; }

      /// <summary>
      /// The currently running action
      /// </summary>
      public StatefulAction currentAction { private set; get; }

      protected override Behavior currentBehavior => currentAction.task;

      protected override bool hasBehaviors => availableActions.Length > 0;

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      protected override void OnInitialize()
      {
        this.agent.gameObject.Connect<WorldState.ModifySymbolEvent>(this.OnModifySymbolEvent);
      }

      protected override void OnReset()
      {
        this.currentAction.Reset();
        this.FormulatePlan();
      }

      protected override void OnUpdate()
      {
        currentBehavior.Update(agent);        
      }

      //------------------------------------------------------------------------/
      // Messages: Behaviors
      //------------------------------------------------------------------------/
      protected override void OnBehaviorAdded(Behavior behavior)
      {
        throw new NotImplementedException();
      }

      protected override void OnBehaviorStarted(Behavior behavior)
      {        
      }

      protected override void OnBehaviorEnded(Behavior behavior)
      {
        // Modify the current world state due to the previous action
        // We already have a reference to the current action so
        // don't really use the behavior here (it wouldn't know its
        // part of a stateful action anyway)
        this.state.Merge(currentAction.effects);
        ContinuePlan();
      }

      protected override void OnBehaviorsCleared()
      {
        throw new NotImplementedException();
      }

      public override string ToString()
      {
        StringBuilder builder = new StringBuilder();
        foreach (var action in availableActions)
        {
          builder.AppendFormat(" - {0}", action.description);
        }
        return builder.ToString();
      }


      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/
      /// <summary>
      /// Modifies a single symbol of this planner's world state
      /// </summary>
      /// <param name="e"></param>
      void OnModifySymbolEvent(WorldState.ModifySymbolEvent e)
      {
        this.state.Apply(e.Symbol);
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Executes the next action in the current plan
      /// </summary>
      public void ContinuePlan()
      {
        // If there's nothing actions left in the plan, reassess?
        if (currentPlan.IsFinished)
        {
          this.goal.Complete(this);
          this.agent.gameObject.Dispatch<Plan.ExecutedEvent>(new Plan.ExecutedEvent());
          //if (Tracing) Trace.Script("The plan for " + this.CurrentGoal.Name + " has been fulfilled!", this);
          //this.gameObject.Dispatch<Agent.>
          return;
        }

        this.currentAction = currentPlan.Next();
        this.currentAction.OnUpdate(this.agent);
      }

      /// <summary>
      /// Makes a plan given the current goal and actions available to this planner.
      /// </summary>
      public void FormulatePlan()
      {
        this.agent.sensor.Scan();
        this.currentPlan = Plan.Formulate(this, this.availableActions, this.state, this.goal);

        if (this.currentPlan != null)
        {
          //if (Tracing)
            Trace.Script("Executing new plan!", this.agent);
          this.agent.gameObject.Dispatch<Plan.FormulatedEvent>(new Plan.FormulatedEvent(this.currentPlan));
          this.ContinuePlan();
        }
        else
        {
          //if (Tracing)
            Trace.Script("The plan could not be formulated!", this.agent);
        }
      }


    }
  }

}