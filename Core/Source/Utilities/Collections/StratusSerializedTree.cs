using System;
using System.Collections.Generic;
using System.Linq;
using Stratus.OdinSerializer;
using UnityEngine;

namespace Stratus
{
	///// <summary>
	///// Utility class for building a tree to be used with the TreeView
	///// </summary>
	///// <typeparam name="TreeElementType"></typeparam>  
	//public class TreeBuilder<TreeElementType, DataType>
	//	where TreeElementType : TreeElement<DataType>, new()
	//	where DataType : class, IStratusLabeled
	//{
	//	//------------------------------------------------------------------------/
	//	// Fields
	//	//------------------------------------------------------------------------/
	//	[OdinSerialize]
	//	private List<TreeElementType> tree = new List<TreeElementType>();
	//	[SerializeField]
	//	private int idCounter = 0;

	//	//------------------------------------------------------------------------/
	//	// Properties
	//	//------------------------------------------------------------------------/
	//	public bool hasRoot { get; private set; }
	//	private System.Action<TreeElementType, DataType> setData { get; set; }

	//	//------------------------------------------------------------------------/
	//	// Methods
	//	//------------------------------------------------------------------------/
	//	public TreeBuilder()
	//	{
	//		this.AddRoot();
	//	}

	//	private void AddRoot()
	//	{
	//		TreeElementType root = new TreeElementType
	//		{
	//			name = "Root",
	//			depth = -1,
	//			id = this.idCounter++
	//		};
	//		this.tree.Add(root);
	//		this.hasRoot = true;
	//	}

	//	public void AddChild(DataType childData, int depth)
	//	{
	//		TreeElementType child = new TreeElementType
	//		{
	//			id = this.idCounter++,
	//			depth = depth
	//		};
	//		child.Set(childData);
	//		this.tree.Add(child);
	//	}

	//	public void AddChildren(DataType[] childrenData, int depth)
	//	{
	//		foreach (DataType childData in childrenData)
	//		{
	//			this.AddChild(childData, depth);
	//		}
	//	}

	//	public List<TreeElementType> ToTree()
	//	{
	//		return this.tree;
	//	}

	//}

	/// <summary>
	/// A serialized tree
	/// </summary>
	/// <typeparam name="TreeElementType"></typeparam>
	[Serializable]
	public class StratusSerializedTree<TreeElementType, DataType> : ISerializationCallbackReceiver
	  where TreeElementType : TreeElement<DataType>, new()
	  where DataType : class, IStratusLabeled
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[OdinSerialize]
		public List<TreeElementType> elements = new List<TreeElementType>();
		[SerializeField]
		private int idCounter = 0;
		[NonSerialized]
		private TreeElementType _root;
		[SerializeField]
		private int _maxDepth;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/    
		/// <summary>
		/// Returns the root of the tree (internally parsing and setting up children based on depth information)
		/// </summary>
		public TreeElementType root
		{
			get
			{
				if (!this.valid)
				{
					this.BuildRootFromElements();
				}

				return this._root;
			}
		}
		private bool valid => this._root != null;
		public bool hasElements => this.elements.Count > 1;
		public int maxDepth => this._maxDepth;
		public static int rootDepth { get; } = -1;
		public static int defaultDepth { get; } = 0;
		public bool hasRoot => this.hasElements && this.elements[0].depth == rootDepth;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusSerializedTree(IEnumerable<TreeElementType> elements)
		{
			this.elements.AddRange(elements);
			BuildRootFromElements();
		}

		public StratusSerializedTree(IEnumerable<DataType> values) : this()
		{
			AddElements(values, 0);
		}

		public StratusSerializedTree()
		{
			this.AddRoot();
		}

		private void BuildRootFromElements()
		{
			this._root = StratusTreeElement.ListToTree(this.elements);
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			this.BuildRootFromElements();
		}

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/
		public void Assert()
		{
			StratusTreeElement.Assert(this.elements);
		}

		public void Repair()
		{
			//this.Clear();
			if (!this.hasRoot)
			{
				this.AddRoot();
			}

			StratusTreeElement.UpdateDepthValues(this.root);
		}

		public Exception Validate()
		{
			Exception exception = StratusTreeElement.Validate(this.elements);
			return exception;
		}

		public void AddElement(DataType data)
		{
			this.AddElement(data, defaultDepth);
		}

		public TreeElementType AddChildElement(DataType data, TreeElementType parent)
		{
			// Insert element below the last child
			TreeElementType element = this.CreateElement(data, parent.depth + 1);
			int insertionIndex = this.FindLastChildIndex(parent) + 1;
			this.elements.Insert(insertionIndex, element);
			return element;
		}

		public void AddParentElement(DataType data, TreeElementType element)
		{
			// Insert element below the last child
			TreeElementType parentElement = this.CreateElement(data, element.depth);
			element.depth++;
			parentElement.parent = element.parent;

			int insertionIndex = this.FindIndex(element);

			foreach (TreeElementType child in this.FindChildren(element))
			{
				child.depth++;
			}

			this.elements.Insert(insertionIndex, parentElement);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(TreeElementType parentElement, int insertionIndex, List<TreeElementType> elements)
		{
			this.MoveElements(parentElement, insertionIndex, elements.ToArray());
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void Reparent(StratusTreeElement parentElement, params StratusTreeElement[] elements)
		{
			StratusTreeElement.Parent(parentElement, elements);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void Reparent(StratusTreeElement parentElement, List<StratusTreeElement> elements)
		{
			StratusTreeElement.Parent(parentElement, elements.ToArray());
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(TreeElementType parentElement, int insertionIndex, params TreeElementType[] elements)
		{
			if (insertionIndex < 0)
			{
				throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");
			}

			// Invalid reparenting input
			if (parentElement == null)
			{
				return;
			}

			// We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
			if (insertionIndex > 0)
			{
				insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);
			}

			// Remove draggedItems from their parents
			foreach (TreeElementType draggedItem in elements)
			{
				draggedItem.parent.children.Remove(draggedItem);  // remove from old parent
				draggedItem.parent = parentElement;         // set new parent
			}

			if (parentElement.children == null)
			{
				parentElement.children = new List<StratusTreeElement>();
			}

			// Insert dragged items under new parent
			parentElement.children.InsertRange(insertionIndex, elements);

			StratusTreeElement.UpdateDepthValues(this.root);
			//TreeElement.TreeToList(this.root, this.data);
		}

		public void RemoveElement(TreeElementType element)
		{
			// Remove all children first
			if (element.hasChildren)
			{
				foreach (StratusTreeElement child in element.allChildren)
				{
					this.elements.Remove((TreeElementType)child);
				}
			}

			this.elements.Remove(element);
		}

		public void RemoveElementExcludeChildren(TreeElementType element)
		{
			StratusTreeElement parent = element.parent != null ? element.parent : this.root;

			// Reparent all children first
			if (element.hasChildren)
			{
				this.Reparent(parent, element.children);
			}

			this.elements.Remove(element);
		}

		public void ReplaceElement(TreeElementType originalElement, DataType replacementData)
		{
			TreeElementType replacementElement = this.AddChildElement(replacementData, (TreeElementType)originalElement.parent);
			if (originalElement.hasChildren)
			{
				this.Reparent(replacementElement, originalElement.children);
			}

			this.RemoveElement(originalElement);
		}

		public void AddElements(IEnumerable<DataType> elementsData, int depth)
		{
			foreach (DataType data in elementsData)
			{
				this.AddElement(data, depth);
			}
		}

		public void Iterate(System.Action<TreeElementType> action)
		{
			if (!this.valid)
			{
				this.BuildRootFromElements();
			}

			foreach (TreeElementType element in this.elements)
			{
				action(element);
			}
		}

		public void Clear()
		{
			this.elements.Clear();
			this.idCounter = 0;
			this.AddRoot();
		}

		//------------------------------------------------------------------------/
		// Methods: Private
		//------------------------------------------------------------------------/
		private void AddElement(DataType data, int depth)
		{
			TreeElementType element = this.CreateElement(data, depth);
			this.elements.Add(element);
		}

		private TreeElementType GetElement(int index)
		{
			return this.elements[index];
		}

		private TreeElementType CreateElement(DataType data, int depth)
		{
			TreeElementType element = new TreeElementType
			{
				id = this.idCounter++,
				depth = depth
			};
			element.Set(data);

			if (depth > this._maxDepth)
			{
				this._maxDepth = element.depth;
			}

			return element;
		}

		private void AddRoot()
		{
			TreeElementType root = new TreeElementType
			{
				name = "Root",
				depth = -1,
				id = this.idCounter++
			};
			this.elements.Insert(0, root);
		}



		private int FindIndex(TreeElementType element)
		{
			int index = this.elements.IndexOf(element);
			return index;
		}

		private int FindLastChildIndex(TreeElementType element)
		{
			int index = this.FindIndex(element);
			int lastIndex = index + element.totalChildrenCount;
			return lastIndex;
		}

		private TreeElementType[] FindChildren(TreeElementType element)
		{
			int index = this.FindIndex(element);
			return this.elements.GetRange(index, element.totalChildrenCount).ToArray();
		}


	}

}