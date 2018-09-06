using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OdinSerializer;
using System.Linq;

namespace Stratus
{
  /// <summary>
  /// Utility class for building a tree to be used with the TreeView
  /// </summary>
  /// <typeparam name="TreeElementType"></typeparam>  
  public class TreeBuilder<TreeElementType, DataType> 
    where TreeElementType : TreeElement<DataType>, new ()
    where DataType : class, INamed
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [OdinSerialize]
    private List<TreeElementType> tree = new List<TreeElementType>();
    [SerializeField]
    private int idCounter = 0;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public bool hasRoot { get; private set; }
    private System.Action<TreeElementType, DataType> setData { get; set; }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public TreeBuilder()
    {
      this.AddRoot();
    }

    private void AddRoot()
    {
      TreeElementType root = new TreeElementType();
      root.name = "Root";
      root.depth = -1;
      root.id = idCounter++;
      tree.Add(root);
      this.hasRoot = true;
    }

    public void AddChild(DataType childData, int depth)
    {
      TreeElementType child = new TreeElementType();
      child.id = idCounter++;
      child.depth = depth;
      child.Set(childData);
      tree.Add(child);
    }

    public void AddChildren(DataType[] childrenData, int depth)
    {
      foreach (var childData in childrenData)
      {
        this.AddChild(childData, depth);
      }
    }

    public List<TreeElementType> ToTree() => this.tree; 
    public SerializedTree<TreeElementType, DataType> ToSerializedTree()
    {
      SerializedTree<TreeElementType, DataType> serializedTree = new SerializedTree<TreeElementType, DataType>(this.tree, this.idCounter);
      return serializedTree;
    }

  }

  /// <summary>
  /// A serialized tree
  /// </summary>
  /// <typeparam name="TreeElementType"></typeparam>
  [Serializable]
  public class SerializedTree<TreeElementType, DataType> : ISerializationCallbackReceiver
    where TreeElementType : TreeElement<DataType>, new()
    where DataType : class, INamed
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
        if (!parsed)
          Parse();
        return _root;
      }
    }
    private bool parsed => _root != null;
    public bool hasElements => elements.Count > 1;
    public int maxDepth => _maxDepth;
    public static int rootDepth { get; } = -1;
    public static int defaultDepth { get; } = 0;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public SerializedTree(List<TreeElementType> tree, int idCounter)
    {
      this.elements = tree;
      this.idCounter = idCounter;      
    }

    public SerializedTree()
    {
      this.AddRoot();
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public void OnBeforeSerialize()
    {      
    }

    public void OnAfterDeserialize()
    {
      this.Parse(); 
    }

    //------------------------------------------------------------------------/
    // Methods: Public
    //------------------------------------------------------------------------/
    public void Assert()
    {
      TreeElement.Assert(this.elements);
    }

    public void Repair()
    {
      TreeElement.UpdateDepthValues(this.root);
    }

    public Exception Validate()
    {
      Exception exception = TreeElement.Validate(this.elements);
      return exception;
    }

    public void AddElement(DataType data)
    {
      this.AddElement(data, defaultDepth);
    }

    public void AddChildElement(DataType data, TreeElementType parent)
    {
      // Insert element below the last child
      TreeElementType element = CreateElement(data, parent.depth + 1);
      int insertionIndex = FindLastChildIndex(parent) + 1;      
      this.elements.Insert(insertionIndex, element);
    }

    public void AddParentElement(DataType data, TreeElementType element)
    {
      // Insert element below the last child
      TreeElementType parentElement = CreateElement(data, element.depth);
      element.depth++;
      parentElement.parent = element.parent;

      int insertionIndex = FindIndex(element);

      foreach(var child in this.FindChildren(element))
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
      MoveElements(parentElement, insertionIndex, elements.ToArray());
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
        throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

      // Invalid reparenting input
      if (parentElement == null)
        return;

      // We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
      if (insertionIndex > 0)
        insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);

      // Remove draggedItems from their parents
      foreach (var draggedItem in elements)
      {
        draggedItem.parent.children.Remove(draggedItem);  // remove from old parent
        draggedItem.parent = parentElement;         // set new parent
      }

      if (parentElement.children == null)
        parentElement.children = new List<TreeElement>();

      // Insert dragged items under new parent
      parentElement.children.InsertRange(insertionIndex, elements);

      TreeElement.UpdateDepthValues(root);
      //TreeElement.TreeToList(this.root, this.data);
    }

    public void RemoveElement(TreeElementType element)
    {
      // Remove all children first
      if (element.hasChildren)
      {
        foreach(var child in element.children)
        {
          this.elements.Remove((TreeElementType)child);
        }
      }

      this.elements.Remove(element);
    }

    public void AddElements(DataType[] elementsData, int depth)
    {
      foreach (var data in elementsData)
      {
        this.AddElement(data, depth);
      }
    }

    public void Iterate(System.Action<TreeElementType> action)
    {
      if (!parsed)
        Parse();

      foreach (var element in this.elements)
        action(element);
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
      TreeElementType element = CreateElement(data, depth);
      this.elements.Add(element);      
    }

    private TreeElementType GetElement(int index) => this.elements[index];

    private TreeElementType CreateElement(DataType data, int depth)
    {
      TreeElementType element = new TreeElementType();
      element.id = idCounter++;
      element.depth = depth;
      element.Set(data);

      if (depth > this._maxDepth)
        this._maxDepth = element.depth;

      return element;
    }

    private void AddRoot()
    {
      TreeElementType root = new TreeElementType();
      root.name = "Root";
      root.depth = -1;
      root.id = idCounter++;
      elements.Add(root);
    }

    private void Parse()
    {
      _root = TreeElement.ListToTree(this.elements);
    }

    private int FindIndex(TreeElementType element)
    {
      int index = this.elements.IndexOf(element);
      return index;
    }

    private int FindLastChildIndex(TreeElementType element)
    {
      int index = FindIndex(element);
      int lastIndex = index + element.totalChildrenCount;
      return lastIndex;
    }

    private TreeElementType[] FindChildren(TreeElementType element)
    {
      int index = FindIndex(element);
      return this.elements.GetRange(index, element.totalChildrenCount).ToArray();
    }


  }

}