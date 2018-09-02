using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OdinSerializer;

namespace Stratus
{
  /// <summary>
  /// A serialized element of a Tree Model
  /// </summary>
  [Serializable]
  public class TreeElement
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/ 

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/ 
    [SerializeField] public int id;
    [SerializeField] public string name;
    [SerializeField] public int depth;

    [NonSerialized] public TreeElement parent;
    [NonSerialized] public List<TreeElement> children;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/ 
    /// <summary>
    /// Whether this tree element has children
    /// </summary>
    public bool hasChildren => children != null && children.Count > 0;
    /// <summary>
    /// The root node must have a depth of -1
    /// </summary>
    public bool isRoot => depth == -1;
    /// <summary>
    /// Howw many children this element has
    /// </summary>
    public int childrenCount => children != null ? children.Count : 0;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/ 
    public TreeElement()
    {
    }

    public TreeElement(string name, int depth, int id)
    {
      this.name = name;
      this.depth = depth;
      this.id = id;
    }

    public override string ToString()
    {
      return $"{name} id({id}) depth({depth})";
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/ 
    /// <summary>
    /// Fills out the list from the given root node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    /// <param name="list"></param>
    public static void TreeToList<T>(T root, IList<T> list) where T : TreeElement
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

    public static TreeElementType MakeRoot<TreeElementType>()
      where TreeElementType : TreeElement, new()
    {
      TreeElementType root = new TreeElementType();
      root.name = "Root";
      root.depth = -1;
      root.id = 0;
      return root;
    }

    public static void AddElement<TreeElementType>(IList<TreeElementType> list, TreeElementType element)
    {
      //foreach(var)
    }

    ///// <summary>
    ///// Fills out the list from the given root node
    ///// </summary>
    ///// <param name="root"></param>
    ///// <param name="result"></param>
    //public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
    //{
    //  if (root == null)
    //    throw new NullReferenceException("root");
    //  if (result == null)
    //    throw new NullReferenceException("result");

    //  result.Clear();

    //  if (root.children == null)
    //    return;

    //  Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
    //  for (int i = root.children.Count - 1; i >= 0; i--)
    //    stack.Push(root.children[i]);

    //  while (stack.Count > 0)
    //  {
    //    TreeViewItem current = stack.Pop();
    //    result.Add(current);

    //    if (current.hasChildren && current.children[0] != null)
    //    {
    //      for (int i = current.children.Count - 1; i >= 0; i--)
    //      {
    //        stack.Push(current.children[i]);
    //      }
    //    }
    //  }
    //} 

    /// <summary>
    /// Returns the root of the tree parsed from the list (always the first element)
    /// Note: The first element is requried to have a depth value of -1, with the rest
    /// of the elements at a depth >= 0
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T ListToTree<T>(IList<T> list) where T : TreeElement
    {
      // Validate input
      TreeElement.Assert(list);

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
        List<TreeElement> children = null;
        if (childCount != 0)
        {
          children = new List<TreeElement>(childCount);
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
    /// Validates the state of the input list, throwing an exception on a failed assertion
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Assert<T>(IList<T> list) where T : TreeElement
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
    /// Updates the depth values below any given element (after reparenting elements)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    public static void UpdateDepthValues<T>(T root) where T : TreeElement
    {
      if (root == null)
        throw new ArgumentNullException(nameof(root), "The root is null");

      if (!root.hasChildren)
        return;

      Stack<TreeElement> stack = new Stack<TreeElement>();
      stack.Push(root);
      while (stack.NotEmpty())
      {
        TreeElement current = stack.Pop();
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
    public static bool IsChildOf<T>(T child, IList<T> elements) where T : TreeElement
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
    public static IList<T> FindCommonAncestorsWithinList<T>(IList<T> elements) where T : TreeElement
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
    public static IList<T> FindChildrenWithinList<T>(IList<T> elements) where T : TreeElement
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
      where DataType : class, INamed
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

  }

  /// <summary>
  /// Generic class for a tree element with one primary data member
  /// </summary>
  /// <typeparam name="DataType"></typeparam>
  public abstract class TreeElement<DataType> : TreeElement, ISerializationCallbackReceiver
    where DataType : class, INamed
  {
    //----------------------------------------------------------------------/
    // Fields
    //----------------------------------------------------------------------/
    [OdinSerialize]
    public DataType data;
    [OdinSerialize]
    public string dataTypeName;

    //----------------------------------------------------------------------/
    // Properties
    //----------------------------------------------------------------------/    
    public bool hasData => data != null;
    public Type dataType => data.GetType();
    public DataType[] childrenData { get; private set; }
    //public IList<TreeElement<DataType>> childrenDerived { get; private set; }

    //----------------------------------------------------------------------/
    // Messages
    //----------------------------------------------------------------------/    
    public void OnBeforeSerialize()
    {
      if (hasData)
        this.UpdateName();
    }

    public void OnAfterDeserialize()
    {
      //this.childrenDerived = (IList<TreeElement<DataType>>)this.children;
      this.childrenData = this.GetChildrenData();
    }

    //----------------------------------------------------------------------/
    // Methods
    //----------------------------------------------------------------------/

    public void Set(DataType data)
    {
      this.data = data;
      this.dataTypeName = data.GetType().Name;
      this.UpdateName();
    }

    public void UpdateName()
    {
      this.name = this.GetName();
    }

    protected virtual string GetName()
    {
      return data.name;
    }

    public DataType[] GetChildrenData()
    {
      if (!this.hasChildren)
        return null;

      List<DataType> children = new List<DataType>();
      foreach (var child in this.children)
        children.Add(((TreeElement<DataType>)child).data);
      return children.ToArray();
    }

    public TreeElement<DataType> GetChild(int index) => (TreeElement<DataType>)children[index];
    public T GetParent<T>() where T : TreeElement => (T)parent;





  }

}