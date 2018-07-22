using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using UnityEditor;
using System.Linq.Expressions;
using System.Linq;

namespace Stratus
{
  public class TreeViewItem<T> : TreeViewItem where T : TreeElement
  {
    public T data { get; set; }
    public TreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
    {
      this.data = data;
    }
  }

  public class TreeViewWithTreeModel<T> : TreeView where T: TreeElement
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/ 
    protected TreeModel<T> treeModel;
    protected readonly List<TreeViewItem> rows = new List<TreeViewItem>(100);
    private const int hiddenRootDepth = -1;
    private const string genericDragId = "GenericDragColumnDragging";

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/ 
    public event System.Action onTreeChanged;
    public event Action<IList<TreeViewItem>> onBeforeDroppingDraggedItems;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/ 
    public TreeViewWithTreeModel(TreeViewState state, TreeModel<T> model) 
      : base(state)
    {
      this.InitializeTreeViewWithModel(model);
    }

    public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model) 
      : base(state, multiColumnHeader)
    {
      this.InitializeTreeViewWithModel(model);
    }

    protected void InitializeTreeViewWithModel(TreeModel<T> model)
    {
      this.treeModel = model;
      this.treeModel.onModelChanged += this.OnModelChanged;
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/ 
    protected override TreeViewItem BuildRoot()
    {
      return new TreeViewItem<T>(this.treeModel.root.id, hiddenRootDepth, this.treeModel.root.name, this.treeModel.root);      
    }

    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
      if (this.treeModel.root == null)
      {
        Trace.Error($"Tree model root is null. Was the data set?");
      }

      this.rows.Clear();
      // IF there's a search string, build the rows from it
      if (!string.IsNullOrEmpty(searchString))
      {
        Search(this.treeModel.root, this.searchString, this.rows);
      }
      // Build rows from the root
      else
      {
        if (this.treeModel.root.hasChildren)
          this.AddChildrenRecursive(this.treeModel.root, 0, this.rows);
      }

      // The child parent information still has to be set for the rows
      // since the information is used by the treeview internal logic (navigation, dragging, etc)
      TreeView.SetupParentsAndChildrenFromDepths(this.rootItem, this.rows);

      return this.rows;
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/ 
    private void OnModelChanged()
    {
      this.onTreeChanged?.Invoke();
      this.Reload();
    }

    //------------------------------------------------------------------------/
    // Methods: Search
    //------------------------------------------------------------------------/ 
    private void Search(T from, string search, List<TreeViewItem> result)
    {
      if (string.IsNullOrEmpty(search))
        throw new ArgumentException("Invalid search: cannot be null or empty", nameof(search));

      // Tree is flattened when searhcing
      const int kItemDepth = 0;

      // Search for matching elements starting from the given element
      Stack<T> stack = new Stack<T>();
      foreach (var element in from.children)
        stack.Push((T)element);
      while (stack.Count > 0)
      {
        T current = stack.Pop();

        // If matches the search...
        if (current.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          result.Add(new TreeViewItem<T>(current.id, kItemDepth, current.name, current));
        }
        
        if (current.hasChildren)
        {
          foreach(var element in current.children)          
            stack.Push((T)element);          
        }        
      }

      // Now sort the results
      this.SortSearchResult(result);
    }

    protected virtual void SortSearchResult(List<TreeViewItem> rows)
    {
      rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
    }

    protected override IList<int> GetAncestors(int id)
    {
      return this.treeModel.GetAncestors(id);
    }

    protected override IList<int> GetDescendantsThatHaveChildren(int id)
    {
      return this.treeModel.GetDescendantsThatHaveChildren(id);
    }


    private void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
    {
      foreach(T child in parent.children)
      {
        TreeViewItem<T> item = new TreeViewItem<T>(child.id, depth, child.name, child);
        newRows.Add(item);

        if(child.hasChildren)
        {
          if (this.IsExpanded(child.id))
          {
            this.AddChildrenRecursive(child, depth + 1, newRows);
          }
          else
          {
            item.children = TreeView.CreateChildListForCollapsedParent();
          }
        }
      }
    }

    //------------------------------------------------------------------------/
    // Methods: Dragging
    //------------------------------------------------------------------------/ 
    protected override bool CanStartDrag(CanStartDragArgs args)
    {
      return true;
    }

    protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
    {
      if (hasSearch)
        return;

      DragAndDrop.PrepareStartDrag();
      var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
      DragAndDrop.SetGenericData(genericDragId, draggedRows);
      DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
      string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
      DragAndDrop.StartDrag(title);
    }

    protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
    {
      // Check if we can handle the current drag data (could be dragged in from other areas/windows in the editor)
      var draggedRows = DragAndDrop.GetGenericData(genericDragId) as List<TreeViewItem>;
      if (draggedRows == null)
        return DragAndDropVisualMode.None;

      // Parent item is null when dragging outside any tree view items.
      switch (args.dragAndDropPosition)
      {
        case DragAndDropPosition.UponItem:
        case DragAndDropPosition.BetweenItems:
          {
            bool validDrag = ValidDrag(args.parentItem, draggedRows);
            if (args.performDrop && validDrag)
            {
              T parentData = ((TreeViewItem<T>)args.parentItem).data;
              OnDropDraggedElementsAtIndex(draggedRows, parentData, args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
            }
            return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
          }

        case DragAndDropPosition.OutsideItems:
          {
            if (args.performDrop)
              OnDropDraggedElementsAtIndex(draggedRows, this.treeModel.root, this.treeModel.root.children.Count);

            return DragAndDropVisualMode.Move;
          }
        default:
          Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
          return DragAndDropVisualMode.None;
      }
    }

    public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, T parent, int insertIndex)
    {
      this.onBeforeDroppingDraggedItems?.Invoke(draggedRows);

      //if (this.onBeforeDroppingDraggedItems != null)
      //  beforeDroppingDraggedItems(draggedRows);

      var draggedElements = new List<TreeElement>();
      foreach (var x in draggedRows)
        draggedElements.Add(((TreeViewItem<T>)x).data);

      var selectedIDs = draggedElements.Select(x => x.id).ToArray();
      this.treeModel.MoveElements(parent, insertIndex, draggedElements);
      SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
    }


    bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
    {
      TreeViewItem currentParent = parent;
      while (currentParent != null)
      {
        if (draggedItems.Contains(currentParent))
          return false;
        currentParent = currentParent.parent;
      }
      return true;
    }

  }

}