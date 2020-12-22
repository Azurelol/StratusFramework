using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Stratus
{
	public enum StratusSearchNodeStatus
	{
		Open,
		Closed,
		Unexplored
	}

	/// <summary>
	/// This planner uses A* to do a search for a valid path of actions that will lead
	/// to the desired state.
	/// </summary>
	public class StratusSearch<ElementType> : IStratusLogger
	{
		/// <summary>
		/// Represents a node in the graph of actions.
		/// </summary>
		public class Node
		{
			/// <summary>
			/// The parent of this node, whose preconditions this node fulfills
			/// </summary>
			public Node parent;
			/// <summary>
			/// Whether this node is on the open or closed list
			/// </summary>
			public StratusSearchNodeStatus status = StratusSearchNodeStatus.Unexplored;
			/// <summary>
			/// The element this node represents
			/// </summary>
			public ElementType element;
			/// <summary>
			/// g(x): How much it costs to get back to the starting node
			/// </summary>
			public float givenCost = 0f;
			/// <summary>
			/// f(x) = g(x) + h(x): If provided, the sum of the given cost and heuristic cost from this node to the goal
			/// </summary>
			public float cost;
			/// <summary>
			/// Everytime we do a search, we increment this. Behaves like a dirty bit.
			/// </summary>
			public int iteration = 0;
			/// <summary>
			/// A description of this node
			/// </summary>
			public string description => element.ToString();

			public Node(Node parent, ElementType element, float givenCost)
			{
				this.parent = parent;
				this.element = element;
				this.givenCost = givenCost;
			}

			public override string ToString()
			{
				return description;
			}
		}

		public class NodeComparer : IEqualityComparer<Node>
		{
			public bool Equals(Node x, Node y)
			{
				return x.element.Equals(y.element);
			}

			public int GetHashCode(Node obj)
			{
				return obj.element.GetHashCode();
			}
		}

		/// <summary>
		/// A priority-queue for use in searches
		/// </summary>
		private class PriorityQueue : StratusPriorityQueue<Node, float>
		{
			public PriorityQueue(float minPriority) : base(minPriority)
			{
			}

			public override void Insert(Node node, float priority)
			{
				base.Insert(node, priority);
				node.status = StratusSearchNodeStatus.Open;
			}

			public void Insert(Node node)
			{
				base.Insert(node, node.cost);
				node.status = StratusSearchNodeStatus.Open;
			}

			public override Node Pop()
			{
				var node = base.Pop();
				node.status = StratusSearchNodeStatus.Closed;
				return node;
			}
		}

		/// <summary>
		/// One of the the results of the search. 
		/// An element within range of the origin along its associated cost.
		/// </summary>
		public class PathResult
		{
			public PathResult(ElementType element, float cost, ElementType[] path)
			{
				this.element = element;
				this.cost = cost;
				this.path = path;
			}

			/// <summary>
			/// The element within range of the search
			/// </summary>
			public ElementType element { get; private set; }
			/// <summary>
			/// The cost from the starting element to this one
			/// </summary>
			public float cost { get; private set; }
			/// <summary>
			/// The path from the starting element to this one
			/// </summary>
			public ElementType[] path { get; private set; }

			public override string ToString()
			{
				return $"{element}: cost({cost}), path({path.JoinToString()})";
			}
		}

		/// <summary>
		/// Base class for searches
		/// </summary>
		public abstract class SearchBase
		{
			/// <summary>
			/// Whether to log debug output
			/// </summary>
			public bool debug { get; set; }
			/// <summary>
			/// The starting element for the algorithm to use
			/// </summary>
			public ElementType startElement { get; set; }
			/// <summary>
			/// A function to calculate the distance between two elements
			/// </summary>
			public Func<ElementType, ElementType, float> distanceFunction { get; set; }
			/// <summary>
			/// If provided, a function that checks whether the given cell is traversable
			/// </summary>
			public Predicate<ElementType> traversableFunction { get; set; }
			/// <summary>
			/// A function to gather the neighbors of a cell
			/// </summary>
			public Func<ElementType, ElementType[]> neighborFunction { get; set; }

			///// <summary>
			///// Initiates the search.
			///// </summary>
			///// <returns></returns>
			//public abstract ElementType[] Search();

			protected ElementType[] BuildPath(Node node)
			{
				List<ElementType> path = new List<ElementType>();

				Node currentNode = node;
				while (currentNode != null)
				{
					path.Add(currentNode.element);
					currentNode = currentNode.parent as Node;
				}

				path.Reverse();
				return path.ToArray();
			}
		}

		public abstract class GoalSearch : SearchBase
		{
			/// <summary>
			/// The element to traverse to
			/// </summary>
			public ElementType targetElement { get; set; }
			/// <summary>
			/// Returns true if the search should be exited
			/// </summary>
			protected bool IsFinished(Node node) => node.element.Equals(targetElement);
			protected float CalculateHeuristicCost(Node node, Node target) => distanceFunction(node.element, target.element);

		}

		public class PathSearch : GoalSearch
		{
			public ElementType[] Search()
			{		
				PriorityQueue openList = new PriorityQueue(0);
				int currentIteration = 0;
				var startingNode = new Node(null, startElement, 0f);
				var destinationNode = new Node(null, targetElement, 0f);
				openList.Insert(startingNode, 0);

				//PutOnList(openList, startingNode, StratusSearchNodeStatus.Open, debug);

				while (!openList.Empty())
				{
					// Pop the cheapest node off the open list
					//var parent = FindCheapest(openList, debug);
					var parent = openList.Pop();
					if (debug) StratusDebug.Log("Iteration #" + currentIteration + " | Parent = " + parent.description);

					// if the a route to the starting node was found...
					if (IsFinished(parent))
					{
						if (debug) StratusDebug.Log("Valid path found!");
						return BuildPath(parent);
					}

					// For all neighboring child nodes...
					Node[] neighbors = FindNeighbors(parent, this);
					foreach (Node child in neighbors)
					{
						// If the node is unexplored
						if (child.status == StratusSearchNodeStatus.Unexplored)
						{
							child.cost = child.givenCost + CalculateHeuristicCost(child, destinationNode);
							child.parent = parent;
							openList.Insert(child, child.cost);
							//PutOnList(openList, child, StratusSearchNodeStatus.Open, debug);
						}
						// Else if the node is on the open or closed list
						else
						{
							// If the new cost is lesser
							float cost = child.givenCost + CalculateHeuristicCost(child, destinationNode);
							if (cost < child.cost)
							{
								child.parent = parent;
								child.cost = cost;
							}
						}
					}

					// Place the parent node on the closed list
					//PutOnList(openList, parent, StratusSearchNodeStatus.Closed, debug);
					currentIteration++;
				}

				// If the open list is empty, no path was found
				if (debug)
				{
					StratusDebug.Log("No valid path found!");
				}
				return null;
			}
		}


		public class RangeSearch : SearchBase
		{
			/// <summary>
			/// How far we can travel from the starting element.
			/// A value of 0 will continue the search.
			/// </summary>
			public float range { get; set; }

			public ElementType[] Search()
			{
				Node[] nodes = GetNodes();
				List<ElementType> result = new List<ElementType>();
				for (int i = 0; i < nodes.Length; i++)
				{
					Node node = nodes[i];
					result.Add(node.element);
				}
				return result.ToArray();
			}

			public Dictionary<ElementType, float> SearchWithCosts()
			{
				Node[] nodes = GetNodes();

				//List<ElementType> elements = new List<ElementType>();
				Dictionary<ElementType, float> costs = new Dictionary<ElementType, float>();

				for (int i = 0; i < nodes.Length; i++)
				{
					Node node = nodes[i];
					costs.Add(node.element, node.givenCost);
					//result.Add(node.element);
				}

				return costs;
			}

			public Dictionary<ElementType, PathResult> SearchPaths()
			{
				Dictionary<ElementType, PathResult> results = new Dictionary<ElementType, PathResult>();

				Dictionary<Node, Node> map = GetMap();
				Node[] nodes = map.Keys.ToArray();

				for (int i = 0; i < nodes.Length; i++)
				{
					Node node = nodes[i];
					PathResult result = new PathResult(node.element, node.givenCost, BuildPath(node));
					results.Add(node.element, result);
				}

				return results;
			}

			private Node[] GetNodes()
			{
				return GetMap().Keys.ToArray();
			}

			private Dictionary<Node, Node> GetMap()
			{
				bool IsWithinRange(Node x)
				{
					if (x.givenCost <= range)
					{
						return true;
					}
					if (debug)
					{
						StratusDebug.Log($"{x} Is out of range {x.givenCost} < ({range}) from {startElement}");
					}
					return false;
				}

				Dictionary<Node, Node> map = BuildDjkstraMap(this, null, IsWithinRange);
				return map;
			}
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static readonly NodeComparer nodeComparer = new NodeComparer();

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		protected StratusSearch()
		{
		}

		//------------------------------------------------------------------------/
		// Static Functions
		//------------------------------------------------------------------------/
		/// <summary>
		/// Finds the neighbors of this node
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private static Node[] FindNeighbors(Node node, SearchBase arguments)
		{
			List<Node> neighbors = new List<Node>();
			if (arguments.debug)
			{
				StratusDebug.Log("Looking for neighboring nodes for the node: " + node.description);
			}

			// Check for actions that satisfy the preconditions of this node
			ElementType[] neighborElements = arguments.neighborFunction(node.element);
			foreach (ElementType element in neighborElements)
			{
				// Don't add the parent
				if (node.parent != null && node.parent.element.Equals(element))
				{
					if (arguments.debug)
					{
						StratusDebug.Log($"Skipping neighbor node {element} since it's the parent {node.parent.element}");
					}
					continue;
				}

				// Optionally, check if the element is traversable
				if (arguments.traversableFunction != null && !arguments.traversableFunction(element))
				{
					continue;
				}

				float distance = arguments.distanceFunction(node.element, element);
				float givenCost = node.givenCost + distance;

				Node neighborNode = new Node(node, element, givenCost);
				neighbors.Add(neighborNode);

				if (arguments.debug)
				{
					StratusDebug.Log($"Added neighbor node {neighborNode} with given cost: {givenCost}. (Distance =  {distance})");
				}
			}

			if (arguments.debug)
			{
				StratusDebug.Log($"Found {neighbors.Count} neighbors");
			}
			return neighbors.ToArray();
		}

		/// <summary>
		/// Breadth-first Search
		/// </summary>
		/// <param name="args"></param>
		/// <param name="frontierPredicate"></param>
		/// <returns></returns>
		private static Dictionary<Node, Node> BuildBFSMap(SearchBase args, 
			Predicate<Node> goalFunction,
			Predicate<Node> frontierPredicate)
		{
			Dictionary<Node, Node> map = new Dictionary<Node, Node>(nodeComparer);

			int iterations = 0;
			Node startingNode = new Node(null, args.startElement, 0f);

			PriorityQueue frontier = new PriorityQueue(0);
			frontier.Insert(startingNode);
			map.Add(startingNode, null);

			while (frontier.NotEmpty())
			{
				Node current = frontier.Pop();
				if (goalFunction != null && goalFunction(current))
				{
					break;
				}

				Node[] neighbors = FindNeighbors(current, args);
				foreach (Node next in neighbors)
				{
					if (frontierPredicate != null && !frontierPredicate(next))
					{
						continue;
					}

					if (!map.ContainsKey(next))
					{
						frontier.Insert(next);
						map.Add(next, current);
					}
				}
				iterations++;
			}

			return map;
		}

		/// <summary>
		/// Djkstra Algorithm
		/// </summary>
		/// <param name="args"></param>
		/// <param name="frontierPredicate"></param>
		/// <returns></returns>
		private static Dictionary<Node, Node> BuildDjkstraMap(SearchBase args, Predicate<Node> goalFunction, Predicate<Node> frontierPredicate)
		{
			Dictionary<Node, Node> map = new Dictionary<Node, Node>(nodeComparer);

			int iterations = 0;
			Node startingNode = new Node(null, args.startElement, 0f);

			PriorityQueue frontier = new PriorityQueue(0);
			frontier.Insert(startingNode);
			map.Add(startingNode, null);


			while (frontier.NotEmpty())
			{
				Node current = frontier.Pop();
				if (goalFunction != null && goalFunction(current))
				{
					break;
				}

				Node[] neighbors = FindNeighbors(current, args);
				foreach (Node next in neighbors)
				{
					if (frontierPredicate != null && !frontierPredicate(next))
					{
						continue;
					}

					bool add = false;
					// If the node has not been visited
					if (!map.ContainsKey(next))
					{
						add = true;
					}
					// If it has been visited and this new cost is cheaper than previous
					else
					{
						Node existing = map[next];
						if (next.givenCost < existing.givenCost)
						{
							map.Remove(existing);
							add = true;
						}
					}

					if (add)
					{
						frontier.Insert(next, next.givenCost);
						map.Add(next, current);
					}
				}
				iterations++;
			}

			return map;
		}



	}
}