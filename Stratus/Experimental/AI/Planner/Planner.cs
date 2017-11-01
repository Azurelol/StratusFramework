/******************************************************************************/
/*!
@file   Planner.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
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
      public Goal Goal;

      /// <summary>
      /// The current state of this agent.
      /// </summary>
      public WorldState State = new WorldState();

      /// <summary>
      /// List of all available actions to this planner.
      /// </summary>
      public StatefulAction[] AvailableActions;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The currently formulated plan
      /// </summary>
      public Plan CurrentPlan { private set; get; }

      /// <summary>
      /// The currently running action
      /// </summary>
      public StatefulAction CurrentAction { private set; get; }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected override void OnStart()
      {        
      }

      protected override void OnSubscribe()
      {
        this.Agent.gameObject.Connect<WorldState.ModifySymbolEvent>(this.OnModifySymbolEvent);
      }

      public override void OnAssess()
      {
        this.CurrentAction.Reset();
        this.FormulatePlan();
      }


      protected override void OnUpdate(float dt)
      {
        CurrentBehavior.Execute(dt);        
      }

      public override void OnBehaviorStarted(Behavior behavior)
      {        
      }

      public override void OnBehaviorEnded(Behavior behavior)
      {
        // Modify the current world state due to the previous action
        // We already have a reference to the current action so
        // don't really use the behavior here (it wouldn't know its
        // part of a stateful action anyway)
        this.State.Merge(CurrentAction.Effects);
        ContinuePlan();
      }
      protected override void OnPrint(StringBuilder builder)
      {
        foreach (var action in AvailableActions)
        {
          builder.AppendFormat(" - {0}", action.Description);
        }
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
        this.State.Apply(e.Symbol);
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
        if (CurrentPlan.IsFinished)
        {
          this.Goal.Complete(this);
          this.Agent.gameObject.Dispatch<Plan.ExecutedEvent>(new Plan.ExecutedEvent());
          //if (Tracing) Trace.Script("The plan for " + this.CurrentGoal.Name + " has been fulfilled!", this);
          //this.gameObject.Dispatch<Agent.>
          return;
        }

        this.CurrentAction = CurrentPlan.Next();
        this.CurrentAction.Initialize(this.Agent);
      }

      /// <summary>
      /// Makes a plan given the current goal and actions available to this planner.
      /// </summary>
      public void FormulatePlan()
      {
        this.Sensor.Scan();
        this.CurrentPlan = Plan.Formulate(this, this.AvailableActions, this.State, this.Goal);

        if (this.CurrentPlan != null)
        {
          //if (Tracing)
            Trace.Script("Executing new plan!", this.Agent);
          this.Agent.gameObject.Dispatch<Plan.FormulatedEvent>(new Plan.FormulatedEvent(this.CurrentPlan));
          this.ContinuePlan();
        }
        else
        {
          //if (Tracing)
            Trace.Script("The plan could not be formulated!", this.Agent);
        }
      }


    }
  }

}