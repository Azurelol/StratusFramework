using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	public abstract class StratusTreeAsset<T> : ScriptableObject where T : StratusTreeElement
	{
		//------------------------------------------------------------------------/
		// Static Fields
		//------------------------------------------------------------------------/
		static int IDCounter;
		static int minNumChildren = 5;
		static int maxNumChildren = 10;
		static float probabilityOfBeingLeaf = 0.5f;

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		public List<T> elements = new List<T>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public bool hasElements => this.elements != null && this.elements.Count > 0;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Generates a random tree 
		/// </summary>
		/// <param name="numTotalElements"></param>
		/// <returns></returns>
		public static List<T> GenerateRandomTree(int numTotalElements)
		{
			int numRootChildren = numTotalElements / 4;
			IDCounter = 0;
			var treeElements = new List<T>(numTotalElements);


			T root = default(T);
			root.name = "Root";
			root.depth = -1;
			root.id = IDCounter;

			treeElements.Add(root);
			for (int i = 0; i < numRootChildren; ++i)
			{
				int allowedDepth = 6;
				AddChildrenRecursive(root, Random.Range(minNumChildren, maxNumChildren), true, numTotalElements, ref allowedDepth, treeElements);
			}

			return treeElements;
		}


		/// <summary>
		/// Adds childrne recursively to the given list of nodes
		/// </summary>
		/// <param name="element"></param>
		/// <param name="numChildren"></param>
		/// <param name="force"></param>
		/// <param name="numTotalElements"></param>
		/// <param name="allowedDepth"></param>
		/// <param name="treeElements"></param>
		private static void AddChildrenRecursive(T element, int numChildren, bool force, int numTotalElements, ref int allowedDepth, List<T> treeElements)
		{
			if (element.depth >= allowedDepth)
			{
				allowedDepth = 0;
				return;
			}

			for (int i = 0; i < numChildren; ++i)
			{
				if (IDCounter > numTotalElements)
					return;

				T child = default(T);
				child.name = $"Element {IDCounter}";
				child.depth = element.depth + 1;
				child.id = IDCounter++;

				treeElements.Add(child);

				if (!force && Random.value < probabilityOfBeingLeaf)
					continue;

				AddChildrenRecursive(child, Random.Range(minNumChildren, maxNumChildren), false, numTotalElements, ref allowedDepth, treeElements);
			}
		}

	}

}