/******************************************************************************/
/*!
@file   Plan_Astar.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    public partial class Plan
    {
      /// <summary>
      /// This planner uses A* to do a search for a valid path of actions that will lead
      /// to the desired state.
      /// </summary>
      public class Search
      {
        public class Path : List<StatefulAction> {}

        /// <summary>
        /// Represents a node in the graph of actions.
        /// </summary>
        public class Node
        {
          public enum ListStatus { Open, Closed, Unexplored }

          /// <summary>
          /// The parent of this node, whose preconditions this node fulfills
          /// </summary>
          public Node Parent;
          /// <summary>
          /// Whether this node is on the open or closed list
          /// </summary>
          public ListStatus Status = ListStatus.Unexplored;
          /// <summary>
          /// f(x) = g(x) + h(x): The current cost of the node
          /// </summary>
          public float Cost;
          /// <summary>
          /// g(x): How much it costs to get back to the starting node
          /// </summary>
          public float GivenCost = 0f;
          /// <summary>
          /// Everytime we do a search, we increment this. Behaves like a dirty bit.
          /// </summary>
          public int Iteration = 0;
          /// <summary>
          /// A description of this node
          /// </summary>
          public string Description
          {
            get
            {
              if (Action != null) return Action.Description + " (" + Cost + ")";
              return null;
            }
          }

          public WorldState State;
          public StatefulAction Action;
          public Node(Node parent, float cost, WorldState state, StatefulAction action)
          {
            Parent = parent;
            Cost = cost;
            State = state;
            Action = action;
          }
        }

        /// <summary>
        /// The results of this search.
        /// </summary>
        public struct Results
        {
          List<Action> Path;
        }

        /// <summary>
        /// Configuration parameters for this search
        /// </summary>
        public struct Configuration
        {
          public bool SaveSearch;
          public bool Tracing;
        }


        //------------------------------------------------------------------------/
        // Properties
        //------------------------------------------------------------------------/
        public bool Tracing = true;
        List<Node> OpenList = new List<Node>();
        Dictionary<WorldState, StatefulAction> ActionEffectsTable = new Dictionary<WorldState, StatefulAction>();
        Node StartingNode, DestinationNode;
        // Planner properties
        WorldState StartState;
        WorldState EndState;
        StatefulAction[] Actions;
        public bool SaveSearch = false;
        int CurrentIteration = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="startingState"></param>
        /// <param name="goal"></param>
        /// <param name="actions"></param>
        public Search(WorldState startingState, WorldState goal, StatefulAction[] actions)
        {
          StartState = startingState.Copy();
          EndState = goal;
          Actions = actions;
        }

        /// <summary>
        /// Configures the search, creating the starting and destination nodes out of the states,
        /// as well as making the map.
        /// </summary>
        public void Initialize()
        {
          this.StartingNode = new Node(null, 0f, this.EndState, null);
          this.DestinationNode = new Node(null, 0f, this.StartState, null);

          this.MakeMap();
          // 1. Put the starting node on the open list
          PutOnList(this.StartingNode, Node.ListStatus.Open);
        }

        void MakeMap()
        {
          // Make an action-effects table (Orkin, 2004)
          foreach (var action in Actions)
          {
            ActionEffectsTable.Add(action.Effects, action);
          }
        }

        /// <summary>
        /// Finds a solution to the goal using A*
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public Path FindSolution()
        {
          while (!OpenList.Empty())
          {
            // Pop the cheapest node off the open list
            var parent = FindCheapest();
            if (Tracing) Trace.Script("Iteration #" + CurrentIteration + " | Parent = " + parent.Description);

            // if the a route to the starting node was found...
            if (IsFinished(parent))
            {
              if (Tracing) Trace.Script("Valid path found!");
              return BuildPath(parent);
            }
            // For all neighboring child nodes...
            var neighbors = FindNeighbors(parent);
            foreach (var child in neighbors)
            {
              // If the node is unexplored
              if (child.Status == Node.ListStatus.Unexplored)
              {
                child.Cost = child.GivenCost + CalculateHeuristicCost(child, this.DestinationNode);
                child.Parent = parent;
                PutOnList(child, Node.ListStatus.Open);
              }
              // Else if the node is on the open or closed list
              else
              {
                // If the new cost is lesser
                var cost = child.GivenCost + CalculateHeuristicCost(child, this.DestinationNode);
                if (cost < child.Cost)
                {
                  child.Parent = parent;
                  child.Cost = cost;
                }
              }
            }

            // Place the parent node on the closed list
            PutOnList(parent, Node.ListStatus.Closed);
            CurrentIteration++;
            if (CurrentIteration > 10)
              break;
          }

          // If the open list is empty, no path was found
          if (Tracing) Trace.Script("No valid path found!");
          return null;
        }

        static float CalculateHeuristicCost(Node node, Node target)
        {
          return node.Action.Cost;
        }

        /// <summary>
        /// Puts a node on the list
        /// </summary>
        /// <param name="openList"></param>
        /// <param name="node"></param>
        /// <param name="list"></param>
        void PutOnList(Node node, Node.ListStatus list)
        {
          if (list == Node.ListStatus.Open)
          {
            if (Tracing) Trace.Script(node.Description + " has been added to the open list!");
            OpenList.Add(node);
            node.Status = Node.ListStatus.Open;
          }
          else
          {
            if (Tracing) Trace.Script(node.Description + " has been removed from the open list!");
            OpenList.Remove(node);
            node.Status = Node.ListStatus.Closed;
          }
        }

        /// <summary>
        /// Finds the cheapest node in the open list
        /// </summary>
        /// <returns></returns>
        Node FindCheapest()
        {
          int cheapestIndex = 0;
          for (int i = 0; i < OpenList.Count; ++i)
          {
            var node = OpenList[i];
            if (node.Cost < OpenList[cheapestIndex].Cost)
            {
              cheapestIndex = i;
              if (Tracing) Trace.Script("Current cheapest = " + OpenList[cheapestIndex].Description);
            }
            else if (Tracing) Trace.Script(OpenList[i].Description + " is not cheaper than " + OpenList[cheapestIndex].Description);
          }
          var cheapestNode = OpenList[cheapestIndex];
          if (Tracing) Trace.Script("Cheapest node = " + cheapestNode.Action);
          return cheapestNode;
        }

        /// <summary>
        /// Finds the neighbors of this node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Node[] FindNeighbors(Node node)
        {
          var neighbors = new List<Node>();
          if (Tracing) Trace.Script("Looking for neighboring nodes (actions) for the node: " + node.Description + " with preconditions: " + node.State.ToString());

          // Check for actions that satisfy the preconditions of this node
          foreach (var action in ActionEffectsTable)
          {
            var state = action.Key;
            // If the action satisfies the preconditions, add it as a possible
            // neighbor
            if (state.Satisfies(node.State))
            {
              if (Tracing) Trace.Script(action.Value + " satifies the condition = " + node.State.ToString());
              var preconditions = action.Value.Preconditions;
              neighbors.Add(new Node(node, action.Value.Cost, preconditions, action.Value));
            }
          }

          if (neighbors.Count == 0)
            if (Tracing) Trace.Script("No nodes satisfy the condition!");

          return neighbors.ToArray();
        }

        /// <summary>
        /// Checks whether we are finished
        /// </summary>
        /// <param name="node"></param>
        /// <returns>If there's no preconditions for this action, we are finisnhed</returns>
        bool IsFinished(Node node)
        {
          // If there's no preconditions left or if the current world state already satisfies the precondition...
          //bool alreadySatisfied = DestinationNode.State.Satisfies(node.State);
          if (node.State.IsEmpty || DestinationNode.State.Satisfies(node.State))
          {
            if (Tracing) Trace.Script("No preconditions left to fulfill for node: " + node.Description);
            return true;
          }
          return false;
        }

        /// <summary>
        /// Builds the path given the current node
        /// </summary>
        /// <returns></returns>
        Path BuildPath(Node node)
        {
          var path = new Path();

          var currentNode = node;
          while (currentNode != null)
          {
            if (currentNode.Action != null)
              path.Add(currentNode.Action);

            currentNode = currentNode.Parent;
          }

          return path;
        }

      }
    }
  }

}