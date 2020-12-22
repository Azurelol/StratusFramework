using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public partial class StratusTreeElement
	{
		//------------------------------------------------------------------------/
		// Methods: Static
		//------------------------------------------------------------------------/ 
		/// <summary>
		/// Fills out the list from the given root node
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="list"></param>
		public static void TreeToList<T>(T root, IList<T> list) where T : StratusTreeElement
		{
			// Clear the current list
			list.Clear();

			// Add all the children starting from the root to the list in order
			Stack<T> stack = new Stack<T>();
			stack.Push(root);
			while (stack.NotEmpty())
			{
				T current = stack.Pop();
				list.Add(current);

				if (current.hasChildren)
				{
					// Back to front traversal
					for (int c = current.children.Count - 1; c >= 0; c--)
					{
						stack.Push((T)current.children[c]);
					}
				}
			}
		}

		/// <summary>
		/// Returns the root of the tree parsed from the list (always the first element)
		/// Note: The first element is requried to have a depth value of -1, with the rest
		/// of the elements at a depth >= 0
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T ListToTree<T>(IList<T> list) where T : StratusTreeElement
		{
			// Validate input
			StratusTreeElement.Assert(list);

			// Clear old state
			foreach (var element in list)
			{
				element.parent = null;
				element.children = null;
			}

			// Set child and parent references using depth info
			for (int parentIndex = 0; parentIndex < list.Count; parentIndex++)
			{
				T parent = list[parentIndex];

				// Been visited before
				bool alreadyHasValidChildren = parent.children != null;
				if (alreadyHasValidChildren)
					continue;

				// Count children based depth value, lookign at children until its
				// the same depth of this element
				int parentDepth = parent.depth;
				int childCount = 0;
				for (int i = parentIndex + 1; i < list.Count; i++)
				{
					int depth = list[i].depth;
					if (depth == parentDepth + 1)
						childCount++;
					else if (depth <= parentDepth)
						break;
				}

				// Fill the child array for this element
				List<StratusTreeElement> children = null;
				if (childCount != 0)
				{
					children = new List<StratusTreeElement>(childCount);
					childCount = 0;

					for (int i = parentIndex + 1; i < list.Count; i++)
					{
						int depth = list[i].depth;
						if (depth == parentDepth + 1)
						{
							list[i].parent = parent;
							children.Add(list[i]);
							childCount++;
						}

						if (depth <= parentDepth)
							break;
					}
				}

				parent.children = children;
			}

			// Now return the root
			return list[0];
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="children"></param>
		public static void Parent(StratusTreeElement parent, params StratusTreeElement[] children)
		{
			// Invalid reparenting input
			if (parent == null)
				return;

			// Remove draggedItems from their parents
			foreach (var child in children)
			{
				Parent(parent, child);
				UpdateDepthValues(child);
			}

			if (parent.children == null)
				parent.children = new List<StratusTreeElement>();

			// Insert dragged items under new parent
			parent.children.AddRange(children);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="children"></param>
		public static void Reparent(StratusTreeElement oldParent, StratusTreeElement newParent)
		{
			int depthDifference = oldParent.depth - newParent.depth;
			StratusTreeElement[] children = oldParent.allChildren;

			// Remove draggedItems from their parents
			foreach (var child in children)
			{
				Parent(newParent, child);
				UpdateDepthValues(child);
			}

			if (newParent.children == null)
				newParent.children = new List<StratusTreeElement>();

			// Insert dragged items under new parent
			newParent.children.AddRange(children);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="children"></param>
		public static void Parent(StratusTreeElement parent, StratusTreeElement child)
		{
			// Invalid reparenting input
			if (parent == null)
				return;

			// Remove from old parent
			child.parent.children.Remove(child);
			// Set new parent
			child.parent = parent;
			// Update depth value
			child.depth = parent.depth + 1;
			// Insert the child
			if (parent.children == null)
				parent.children = new List<StratusTreeElement>();
			parent.children.Add(child);
		}

		/// <summary>
		/// Validates the state of the input list, throwing an exception on a failed assertion
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void Assert<T>(IList<T> list) where T : StratusTreeElement
		{
			// Verify count
			if (list.Count == 0)
				throw new ArgumentException("The list should have items");

			// Validate depth of first
			if (list[0].depth != -1)
				throw new ArgumentException("The list item at index 0 (first) should have a depth of -1");

			// Validate depth of rest
			for (int i = 0; i < list.Count - 1; i++)
			{
				int depth = list[i].depth;
				int nextDepth = list[i + 1].depth;
				if (nextDepth > depth && nextDepth - depth > 1)
					throw new ArgumentException(string.Format("Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {0} has depth {1} while index {2} has depth {3}", i, depth, i + 1, nextDepth));
			}

			for (int i = 1; i < list.Count; ++i)
			{
				if (list[i].depth < 0)
					throw new ArgumentException($"Invalid depth value for item at index {i}. Only the first item (the root) should have a depth of 0");
			}

			if (list.Count > 1 && list[1].depth != 0)
				throw new ArgumentException("Input list at index 1 is assumed to have a depth of 0", nameof(list));
		}

		/// <summary>
		/// Validate the depth of the tree
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>

		public static bool ValidateDepthValues<T>(IList<T> list) where T : StratusTreeElement
		{
			// Validate depth of first
			if (list[0].depth != -1)
				throw new ArgumentException("The list item at index 0 (first) should have a depth of -1");

			// Validate depth of rest
			for (int i = 0; i < list.Count - 1; i++)
			{
				int depth = list[i].depth;
				int nextDepth = list[i + 1].depth;
				if (nextDepth > depth && nextDepth - depth > 1)
					throw new ArgumentException(string.Format("Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {0} has depth {1} while index {2} has depth {3}", i, depth, i + 1, nextDepth));
			}
			return true;
		}

		/// <summary>
		/// Validates the state of the input list, throwing an exception on a failed assertion
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static Exception Validate<T>(IList<T> list) where T : StratusTreeElement
		{
			// Verify count
			if (list.Count == 0)
				return new ArgumentException("The list should have items");

			// Validate depth of first
			if (list[0].depth != -1)
				return new ArgumentException("The list item at index 0 (first) should have a depth of -1");

			// Validate depth of rest
			for (int i = 0; i < list.Count - 1; i++)
			{
				int depth = list[i].depth;
				int nextDepth = list[i + 1].depth;
				if (nextDepth > depth && nextDepth - depth > 1)
					return new ArgumentException(string.Format("Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {0} has depth {1} while index {2} has depth {3}", i, depth, i + 1, nextDepth));
			}

			for (int i = 1; i < list.Count; ++i)
			{
				if (list[i].depth < 0)
					return new ArgumentException($"Invalid depth value for item at index {i}. Only the first item (the root) should have a depth of 0");
			}

			if (list.Count > 1 && list[1].depth != 0)
				return new ArgumentException("Input list at index 1 is assumed to have a depth of 0", nameof(list));

			return null;
		}

		/// <summary>
		/// Updates the depth values below any given element (after reparenting elements)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		public static void UpdateDepthValues<T>(T root) where T : StratusTreeElement
		{
			if (root == null)
				throw new ArgumentNullException(nameof(root), "The root is null");

			if (!root.hasChildren)
				return;

			Stack<StratusTreeElement> stack = new Stack<StratusTreeElement>();
			stack.Push(root);
			while (stack.NotEmpty())
			{
				StratusTreeElement current = stack.Pop();
				if (current.hasChildren)
				{
					foreach (var child in current.children)
					{
						child.depth = current.depth + 1;
						stack.Push(child);
					}
				}
			}
		}

		/// <summary>
		/// Returns true if there is an ancestor of child in the elements list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="child"></param>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static bool IsChildOf<T>(T child, IList<T> elements) where T : StratusTreeElement
		{
			while (child != null)
			{
				child = (T)child.parent;
				if (elements.Contains(child))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns a list of elements with common ancestors
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static IList<T> FindCommonAncestorsWithinList<T>(IList<T> elements) where T : StratusTreeElement
		{
			// IF there's only one elment...
			if (elements.Count == 1)
				return new List<T>(elements);

			List<T> result = new List<T>(elements);
			result.RemoveAll(g => IsChildOf(g, elements));
			return result;
		}

		/// <summary>
		/// Returns a list of elements with common ancestors
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static IList<T> FindChildrenWithinList<T>(IList<T> elements) where T : StratusTreeElement
		{
			// IF there's only one elment...
			if (elements.Count == 1)
				return new List<T>(elements);

			List<T> result = new List<T>(elements);
			result.RemoveAll(g => !IsChildOf(g, elements));
			return result;
		}


		public static List<TreeElementType> GenerateFlatTree<TreeElementType, DataType>(params DataType[] elements)
		  where TreeElementType : TreeElement<DataType>, new()
		  where DataType : class, IStratusLabeled
		{
			List<TreeElementType> treeList = new List<TreeElementType>();

			int idCounter = 0;

			// Add root
			TreeElementType root = new TreeElementType();
			root.name = "Root";
			root.depth = -1;
			root.id = idCounter++;
			treeList.Add(root);

			// Add the elements right below root
			foreach (var element in elements)
			{
				TreeElementType child = new TreeElementType();
				child.Set(element);
				child.depth = 0;
				child.id = idCounter++;
				treeList.Add(child);
			}

			return treeList;
		}

		public static StratusTreeElement[] GetAllChildren(StratusTreeElement element)
		{
			List<StratusTreeElement> children = new List<StratusTreeElement>();
			GetChildrenRecursive(element, children);
			return children.ToArray();
		}

		private static void GetChildrenRecursive(StratusTreeElement element, List<StratusTreeElement> children)
		{
			foreach (var child in element.children)
			{
				children.Add(child);
				GetChildrenRecursive(child, children);
			}
		}

		public static TreeElementType[] GetChildren<TreeElementType, DataType>(TreeElementType element)
		  where TreeElementType : TreeElement<DataType>, new()
		  where DataType : class, IStratusLabeled
		{
			List<TreeElementType> children = new List<TreeElementType>();
			GetChildrenRecursive<TreeElementType, DataType>(element, children);
			return children.ToArray();
		}

		private static void GetChildrenRecursive<TreeElementType, DataType>(TreeElementType element, List<TreeElementType> children)
		  where TreeElementType : TreeElement<DataType>, new()
		  where DataType : class, IStratusLabeled
		{
			foreach (var child in element.children)
			{
				TreeElementType derivedChild = (TreeElementType)child;
				children.Add(derivedChild);
				GetChildrenRecursive<TreeElementType, DataType>(derivedChild, children);
			}
		}

		public static int GetTotalChildrenCount(StratusTreeElement element)
		{
			int count = 0;
			GetTotalChildrenCount(element, ref count);
			return count;
		}

		private static void GetTotalChildrenCount(StratusTreeElement element, ref int count)
		{
			if (!element.hasChildren)
				return;

			count += element.childrenCount;
			foreach (var child in element.children)
			{
				GetTotalChildrenCount(child, ref count);
			}
		}
	}

}