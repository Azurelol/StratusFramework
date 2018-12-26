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
    public T item { get; set; }

    public TreeViewItem(int id, int depth, string displayName, T item) : base(id, depth, displayName)
    {
      this.item = item;
    }

  }

  public abstract class TreeViewWithTreeModel<TreeElementType> : TreeView 
    where TreeElementType: TreeElement
  {
    //------------------------------------------------------------------------/
    // Declaration
    //------------------------------------------------------------------------/ 
    public struct SelectionData
    {
      public TreeElementType element;
      public int index;
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/ 
    protected TreeModel<TreeElementType> treeModel;
    protected readonly List<TreeViewItem> rows = new List<TreeViewItem>(100);
    private const int hiddenRootDepth = -1;
    private const string genericDragId = "GenericDragColumnDragging";

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/ 
    public event System.Action<IList<TreeElementType>> onSelectionChanged;
    public event System.Action<IList<int>> onSelectionIdsChanged;
    public event System.Action onTreeChanged;
    public event Action<IList<TreeViewItem>> onBeforeDroppingDraggedItems;
    public SearchField search { get; protected set; }
    public TreeAsset<TreeElementType> treeAsset { get; private set; }

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/ 
    protected abstract void OnItemContextMenu(GenericMenu menu, TreeElementType treeElement);
    protected abstract void OnContextMenu(GenericMenu menu);

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/ 
    public TreeViewWithTreeModel(TreeViewState state, TreeModel<TreeElementType> model) 
      : base(state)
    {
      this.InitializeTreeViewWithModel(model);
    }

    public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<TreeElementType> model) 
      : base(state, multiColumnHeader)
    {
      this.InitializeTreeViewWithModel(model);
    }

    protected void InitializeTreeViewWithModel(TreeModel<TreeElementType> model)
    {
      this.treeModel = model;
      this.treeModel.onModelChanged += this.OnModelChanged;
      this.search = new SearchField();
      this.search.downOrUpArrowKeyPressed += this.SetFocusAndEnsureSelectedItem;      
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/ 
    protected override TreeViewItem BuildRoot()
    {
      return new TreeViewItem<TreeElementType>(this.treeModel.root.id, hiddenRootDepth, this.treeModel.root.name, this.treeModel.root);      
    }

    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
      if (this.treeModel.root == null)
      {
        StratusDebug.Error($"Tree model root is null. Was the data set?");
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

    protected override void ContextClickedItem(int id)
    {
      base.ContextClickedItem(id);
      TreeViewItem<TreeElementType> treeItem = (TreeViewItem<TreeElementType>)this.FindItem(id, this.rootItem);
      TreeElementType treeElement = treeItem.item;

      GenericMenu menu = new GenericMenu();
      this.OnItemContextMenu(menu, treeElement);
      menu.ShowAsContext();
      UnityEngine.Event.current.Use();
    }

    protected override void ContextClicked()
    {
      base.ContextClicked();
      GenericMenu menu = new GenericMenu();
      this.OnContextMenu(menu);
      if (menu.GetItemCount() > 0)
        menu.ShowAsContext();
      UnityEngine.Event.current.Use();
    }

    protected override void SelectionChanged(IList<int> selectedIds)
    {
      base.SelectionChanged(selectedIds);

      List<TreeElementType> elements = GetElements(selectedIds);
      onSelectionChanged?.Invoke(elements);
      onSelectionIdsChanged?.Invoke(selectedIds);
    }

    public TreeElementType GetElement(int id)
    {
      TreeViewItem<TreeElementType> treeItem = (TreeViewItem<TreeElementType>)this.FindItem(id, this.rootItem);
      return treeItem.item;
    }

    public List<TreeElementType> GetElements(IList<int> ids)
    {
      List<TreeElementType> elements = new List<TreeElementType>();
      foreach (var id in ids)
      {        
        elements.Add(GetElement(id));
      }
      return elements;
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
    // Methods: Public
    //------------------------------------------------------------------------/ 
    /// <summary>
    /// Draws the multicolumn tree view GUI
    /// </summary>
    /// <param name="rect"></param>
    public void TreeViewGUI(Rect rect)
    {
      Rect searchRect = this.GetSearchBarRect(rect);
      this.searchString = this.search.OnGUI(searchRect, this.searchString);

      Rect mainRect = this.GetMainRect(rect);
      this.OnMainGUI(mainRect);
    }

    protected virtual void OnMainGUI(Rect rect)
    {			
      this.OnGUI(rect);
    }

    /// <summary>
    /// Sets the current tree asset at runtime
    /// </summary>
    /// <param name="asset"></param>
    public void SetTreeAsset(TreeAsset<TreeElementType> asset)
    {
      this.treeAsset = asset;
      this.SetTree(asset.elements);
    }

    /// <summary>
    /// Sets the data for this tree, also initializing it
    /// </summary>
    /// <param name="tree"></param>
    public void SetTree(IList<TreeElementType> tree)
    {
      this.treeModel.SetData(tree);
      this.Reload();
    }

    /// <summary>
    /// Calculates the rect to be used by the search bar
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    protected virtual Rect GetSearchBarRect(Rect position)
    {
      return new Rect(position.x + 20f,
                      position.y + 10f,
                      position.width - 40f,
                      20f);
    }

    /// <summary>
    /// Calculates the rect to be used by the multi-column tree view
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    protected virtual Rect GetMainRect(Rect position)
    {
      return new Rect(position.x + 20f,
                      position.y + 30f,
                      position.width - 40,
                      position.height - 60);
    }

    //------------------------------------------------------------------------/
    // Methods: Search
    //------------------------------------------------------------------------/ 
    private void Search(TreeElementType from, string search, List<TreeViewItem> result)
    {
      if (string.IsNullOrEmpty(search))
        throw new ArgumentException("Invalid search: cannot be null or empty", nameof(search));

      // Tree is flattened when searhcing
      const int kItemDepth = 0;

      // Search for matching elements starting from the given element
      Stack<TreeElementType> stack = new Stack<TreeElementType>();
      foreach (var element in from.children)
        stack.Push((TreeElementType)element);
      while (stack.Count > 0)
      {
        TreeElementType current = stack.Pop();

        // If matches the search...
        if (current.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          result.Add(new TreeViewItem<TreeElementType>(current.id, kItemDepth, current.name, current));
        }
        
        if (current.hasChildren)
        {
          foreach(var element in current.children)          
            stack.Push((TreeElementType)element);          
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

    private void AddChildrenRecursive(TreeElementType parent, int depth, IList<TreeViewItem> newRows)
    {
      foreach(TreeElementType child in parent.children)
      {
        TreeViewItem<TreeElementType> item = new TreeViewItem<TreeElementType>(child.id, depth, child.name, child);
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
              TreeElementType parentData = ((TreeViewItem<TreeElementType>)args.parentItem).item;
              if (IsParentValid(parentData))
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

    public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, TreeElementType parent, int insertIndex)
    {
      this.onBeforeDroppingDraggedItems?.Invoke(draggedRows);

      //if (this.onBeforeDroppingDraggedItems != null)
      //  beforeDroppingDraggedItems(draggedRows);

      var draggedElements = new List<TreeElement>();
      foreach (var x in draggedRows)
        draggedElements.Add(((TreeViewItem<TreeElementType>)x).item);

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

    protected virtual bool IsParentValid(TreeElementType parent) => true;

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    /// <summary>
    /// Fills out the list from the given root node
    /// </summary>
    /// <param name="root"></param>
    /// <param name="result"></param>
    public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
    {
      if (root == null)
        throw new NullReferenceException("root");
      if (result == null)
        throw new NullReferenceException("result");

      result.Clear();

      if (root.children == null)
        return;

      Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
      for (int i = root.children.Count - 1; i >= 0; i--)
        stack.Push(root.children[i]);

      while (stack.Count > 0)
      {
        TreeViewItem current = stack.Pop();
        result.Add(current);

        if (current.hasChildren && current.children[0] != null)
        {
          for (int i = current.children.Count - 1; i >= 0; i--)
          {
            stack.Push(current.children[i]);
          }
        }
      }
    }

  }

}