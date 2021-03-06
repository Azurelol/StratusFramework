using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// A plan is a sequence of actions that will fulfill a specified goal world state,
    /// given a starting world state.
    /// </summary>
    public partial class Plan
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      public class FormulatedEvent : Stratus.StratusEvent
      {
        public Plan Plan;
        public FormulatedEvent(Plan plan) { Plan = plan; }
      }
      public class ExecutedEvent : Stratus.StratusEvent { }

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// A sequence of actions, where each action represents a state transition.
      /// </summary>
      public LinkedList<StatefulAction> Actions = new LinkedList<StatefulAction>();
      /// <summary>
      /// Whether this plan has finished running
      /// </summary>
      public bool IsFinished { get { return Actions.Count == 0; } }
      /// <summary>
      /// Whether A* should be used to formulate the plan
      /// </summary>
      static bool UseAstar = true;

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Gets the next action in the sequence.
      /// </summary>
      /// <returns></returns>
      public StatefulAction Next()
      {
        var action = Actions.First();
        Actions.RemoveFirst();
        return action;
      }

      /// <summary>
      /// Adds an action to the plan
      /// </summary>
      /// <param name="action"></param>
      public void Add(StatefulAction action)
      {
        Actions.AddLast(action);
      }

      /// <summary>
      /// Given a goal, formulates a plan.
      /// </summary>
      /// <param name="goal"></param>
      /// <returns></returns>
      public static Plan Formulate(Planner planner, StatefulAction[] actions, StratusWorldState currentState, StratusGoal goal)
      {
        // Reset all actions
        foreach (var action in actions)
          action.Reset();

        // Get all valid actions whose context preconditions are true
        var usableActions = (from action
                             in actions
                             where action.contextPrecondition && !currentState.Satisfies(action.effects)
                             select action).ToArray();

        if (planner.debug)
        {
          StratusDebug.Log("Making plan to satisfy the goal '" + goal.Name + "' with preconditions:" + goal.DesiredState.ToString(), planner.agent);
          StratusDebug.Log("Actions available:", planner.agent);
          foreach (var action in usableActions)
            StratusDebug.Log("- " + action.description, planner.agent);
        }

        // The path of actions
        Search.Path path;

        if (Plan.UseAstar)
        {
          Search search = new Search(currentState, goal.DesiredState, usableActions);
          search.Tracing = planner.debug;
          search.Initialize();
          path = search.FindSolution();
        }
        else
        {
          // Build up a tree of nodes
          path = new Search.Path();
          Search.Node starting = new Search.Node(null, 0f, goal.DesiredState, null);
          // Look for a solution, backtracking from the goal's desired world state until
          // we have fulfilled every precondition leading up to it!
          var hasFoundPath = FindSolution(path, starting, usableActions, planner);
          // If the path has not been found
          if (!hasFoundPath)
          {
            if (planner.debug) StratusDebug.Log("No plan could be formulated!", planner.agent);
            return new Plan();
          }
        }

        // If no solution was found
        if (path == null)
          return null;

        // Make the plan
        var plan = new Plan();
        foreach (var action in path)
          plan.Add(action);
        return plan;
      }

      /// <summary>
      /// Looks for a solution, backtracking from the goal to the current world state
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="actions"></param>
      /// <param name="goal"></param>
      /// <param name="currentState"></param>
      /// <returns></returns>
      static bool FindSolution(Search.Path path, Search.Node parent, StatefulAction[] actions, Planner planner)
      {
        bool solutionFound = false;
        Search.Node cheapestNode = null;

        if (planner.debug) StratusDebug.Log("Looking to fulfill the preconditions:" + parent.State.ToString());

        // Look for actions that fulfill the preconditions
        foreach (var action in actions)
        {
          if (action.effects.Satisfies(parent.State))
          {
            if (planner.debug) StratusDebug.Log(action.description + " satisfies the preconditions");

            // Create a new node
            var node = new Search.Node(parent, parent.Cost + action.cost, action.preconditions, action);

            // Replace the previous best node
            if (cheapestNode == null) cheapestNode = node;
            else if (cheapestNode.Cost > node.Cost) cheapestNode = node;
          }
          else
          {
            if (planner.debug) StratusDebug.Log(action.description + " does not satisfy the preconditions");
          }
        }

        // If no satisfactory action was found
        if (cheapestNode == null)
        {
          // If the current state is already fulfilling this condition, we are done!
          if (planner.state.Satisfies(parent.State)) return true;
          // Otherwise, no valid solution could be found
          if (planner.debug) StratusDebug.Log("No actions could fulfill these preconditions");
          return false;
        }

        // Add the cheapest node to the path
        path.Add(cheapestNode.Action);
        if (planner.debug) StratusDebug.Log("Adding " + cheapestNode.Action.description + " to the path");

        // If this action has no more preconditions left to fulfill
        if (cheapestNode.Action.preconditions.isEmpty)
        {
          //Trace.Script("No preconditions left!");
          solutionFound = true;
        }
        // Else if it has a precondition left to fulfill, keep looking
        else
        {
          var actionSubset = (from remainingAction in actions where !remainingAction.Equals(cheapestNode.Action) select remainingAction).ToArray();
          bool found = FindSolution(path, cheapestNode, actionSubset, planner);
          if (found) solutionFound = true;
        }

        return solutionFound;
      }

      /// <summary>
      /// Plans all the actions in this plan.
      /// </summary>
      /// <returns></returns>
      public string Print()
      {
        var builder = new StringBuilder();
        foreach (var action in Actions)
        {
          builder.AppendLine("- " + action.description + " (" + action.cost + ")");
        }
        return builder.ToString();
      }



    }
  }



}