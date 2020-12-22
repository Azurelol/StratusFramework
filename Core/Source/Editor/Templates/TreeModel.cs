using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Stratus
{
  /// <summary>
  /// Utlity class for working on a list of serializable TreeElements where the order
  /// and depth of each tree element define the tree structure. 
  /// The TreeModel itself is not serializable but the input list is.
  /// The tree representation (parent and children references) are built internally using 
  /// an utility function to convert the list to tree using the depth values of the elements.
  /// The first element of the input list is required to have a depth of -1 (the hidden root),
  /// and the rest a depth of >= 0.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class TreeModel<T> where T : StratusTreeElement
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/ 
    private IList<T> data;
    private int maxID;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/     
    public T root { get; set; }
    public event System.Action onModelChanged;
    public int numberOfDataElements => data.Count;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/ 
    public TreeModel(IList<T> data)
    {
      this.SetData(data);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/ 
    /// <summary>
    /// Sets the data for this tree model
    /// </summary>
    /// <param name="data"></param>
    public void SetData(IList<T> data)
    {
      if (data == null)
        throw new ArgumentNullException("No input data given!");

      this.data = data;
      if (this.numberOfDataElements > 0)
      {
        this.root = StratusTreeElement.ListToTree(this.data);
        this.maxID = this.data.Max(d => d.id);
      }

    }

    /// <summary>
    /// Finds the given element by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T Find(int id) => this.data.FirstOrDefault(element => element.id == id);

    /// <summary>
    /// Generates an unique id for a tree element
    /// </summary>
    /// <returns></returns>
    public int GenerateUniqueID() => ++this.maxID;

    /// <summary>
    /// Gets the ids of all ancestors to the element of given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IList<int> GetAncestors(int id)
    {
      var parents = new List<int>();
      StratusTreeElement T = Find(id);
      if (T != null)
      {
        while (T.parent != null)
        {
          parents.Add(T.parent.id);
          T = T.parent;
        }
      }
      return parents;
    }

    /// <summary>
    /// Gets the ids of all descendants to the element of given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IList<int> GetDescendantsThatHaveChildren(int id)
    {
      T searchFromThis = Find(id);
      if (searchFromThis != null)
      {
        return GetParentsBelowStackBased(searchFromThis);
      }
      return new List<int>();
    }

    /// <summary>
    /// Adds the given elements to the tree model
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="parent"></param>
    /// <param name="insertPosition"></param>
    public void AddElements(IList<T> elements, StratusTreeElement parent, int insertPosition)
    {
      if (elements == null)
        throw new ArgumentNullException("elements", "elements is null");
      if (elements.Count == 0)
        throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
      if (parent == null)
        throw new ArgumentNullException("parent", "parent is null");

      if (parent.children == null)
        parent.children = new List<StratusTreeElement>();

      parent.children.InsertRange(insertPosition, elements.Cast<StratusTreeElement>());
      foreach (var element in elements)
      {
        element.parent = parent;
        element.depth = parent.depth + 1;
        StratusTreeElement.UpdateDepthValues(element);
      }

      StratusTreeElement.TreeToList(this.root, this.data);

      OnChanged();
    }

    /// <summary>
    /// Adds an element onto the tree
    /// </summary>
    /// <param name="element"></param>
    /// <param name="parent"></param>
    /// <param name="insertPosition"></param>
    public void AddElement(T element, StratusTreeElement parent, int insertPosition)
    {
      if (element == null)
        throw new ArgumentNullException("element", "element is null");
      if (parent == null)
        throw new ArgumentNullException("parent", "parent is null");

      if (parent.children == null)
        parent.children = new List<StratusTreeElement>();

      parent.children.Insert(insertPosition, element);
      element.parent = parent;

      StratusTreeElement.UpdateDepthValues(parent);
      StratusTreeElement.TreeToList(this.root, this.data);

      OnChanged();
    }

    /// <summary>
    /// Adds the root element to this model
    /// </summary>
    /// <param name="root"></param>
    public void AddRoot(T root)
    {
      if (root == null)
        throw new ArgumentNullException("root", "root is null");

      if (this.data == null)
        throw new InvalidOperationException("Internal Error: data list is null");

      if (this.data.Count != 0)
        throw new InvalidOperationException("AddRoot is only allowed on empty data list");

      root.id = GenerateUniqueID();
      root.depth = -1;
      this.data.Add(root);
    }

    /// <summary>
    /// Removes all elements with the given id
    /// </summary>
    /// <param name="elementIDs"></param>
    public void RemoveElements(IList<int> elementIDs)
    {
      IList<T> elements = this.data.Where(element => elementIDs.Contains(element.id)).ToArray();
      RemoveElements(elements);
    }

    /// <summary>
    /// Removes the given elements 
    /// </summary>
    /// <param name="elements"></param>
    public void RemoveElements(IList<T> elements)
    {
      foreach (var element in elements)
        if (element == this.root)
          throw new ArgumentException("It is not allowed to remove the root element");

      var commonAncestors = StratusTreeElement.FindCommonAncestorsWithinList(elements);

      foreach (var element in commonAncestors)
      {
        element.parent.children.Remove(element);
        element.parent = null;
      }

      StratusTreeElement.TreeToList(this.root, this.data);

      OnChanged();
    }

    /// <summary>
    /// Reparents the given elements
    /// </summary>
    /// <param name="parentElement"></param>
    /// <param name="insertionIndex"></param>
    /// <param name="elements"></param>
    public void MoveElements(StratusTreeElement parentElement, int insertionIndex, List<StratusTreeElement> elements)
    {
      MoveElements(parentElement, insertionIndex, elements.ToArray());
    }

    /// <summary>
    /// Reparents the given elements
    /// </summary>
    /// <param name="parentElement"></param>
    /// <param name="insertionIndex"></param>
    /// <param name="elements"></param>
    public void MoveElements(StratusTreeElement parentElement, int insertionIndex, params StratusTreeElement[] elements)
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
        parentElement.children = new List<StratusTreeElement>();

      // Insert dragged items under new parent
      parentElement.children.InsertRange(insertionIndex, elements);

      StratusTreeElement.UpdateDepthValues(root);
      StratusTreeElement.TreeToList(this.root, this.data);

      OnChanged();
    }

    /// <summary>
    /// Invoked when the model is changed
    /// </summary>
    private void OnChanged()
    {
      if (this.onModelChanged!= null)
        onModelChanged();
    }

    //------------------------------------------------------------------------/
    // Methods: Private
    //------------------------------------------------------------------------/ 
    private IList<int> GetParentsBelowStackBased(StratusTreeElement searchFromThis)
    {
      Stack<StratusTreeElement> stack = new Stack<StratusTreeElement>();
      stack.Push(searchFromThis);

      var parentsBelow = new List<int>();
      while (stack.Count > 0)
      {
        StratusTreeElement current = stack.Pop();
        if (current.hasChildren)
        {
          parentsBelow.Add(current.id);
          foreach (var T in current.children)
          {
            stack.Push(T);
          }
        }
      }

      return parentsBelow;
    }

  }

  #region Tests
 

  #endregion




}