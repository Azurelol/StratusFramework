using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

namespace Stratus
{
  public abstract class MultiColumnTreeView<TreeElementType, ColumnType> : TreeViewWithTreeModel<TreeElementType>
    where TreeElementType : TreeElement
    where ColumnType : struct, IConvertible
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public class TreeViewColumn : MultiColumnHeaderState.Column
    {
      /// <summary>
      /// The function used to select a value for this column
      /// </summary>
      public Func<TreeViewItem<TreeElementType>, string> selectorFunction;
      /// <summary>
      /// An unique icon for this column
      /// </summary>
      public Texture2D icon;

      public TreeViewColumn(string label, Func<TreeViewItem<TreeElementType>, string> selectorFunction)
      {
        this.headerContent = new GUIContent(label);
        this.headerTextAlignment = TextAlignment.Center;
        this.width = this.minWidth = GUI.skin.label.CalcSize(this.headerContent).x;
        this.selectorFunction = selectorFunction;
      }

      public TreeViewColumn()
      {
      }

    }


    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public MultiColumnHeaderState multiColumnHeaderState;
    public bool showControls = true;
    float rowHeights = 20f;
    float toggleWidth = 18f;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public TreeAsset<TreeElementType> treeAsset { get; private set; }
    protected TreeViewColumn[] columns { get; private set; }
    public SearchField search { get; private set; }
    public bool initialized { get; private set; }

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/    
    //protected abstract TreeViewColumn[] BuildColumns();
    protected abstract TreeViewColumn BuildColumn(ColumnType columnType);
    protected abstract void DrawColumn(Rect cellRect, TreeViewItem<TreeElementType> item, ColumnType column, ref RowGUIArgs args);
    protected abstract ColumnType GetColumn(int index);
    protected abstract int GetColumnIndex(ColumnType columnType);
    protected abstract void OnItemContextMenu(GenericMenu menu, TreeElementType treeElement);

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public MultiColumnTreeView(TreeViewState state, IList<TreeElementType> data)
    : base(state, new TreeModel<TreeElementType>(data))
    {
      this.columns = this.BuildColumns();
      MultiColumnHeaderState headerState = BuildMultiColumnHeaderState(columns);
      this.multiColumnHeader = new MultiColumnHeader(headerState);
      this.InitializeMultiColumnTreeView();
      this.Reload();
    }

    public MultiColumnTreeView(TreeViewState state, TreeModel<TreeElementType> model)
    : base(state, model)
    {
      this.columns = this.BuildColumns();
      MultiColumnHeaderState headerState = BuildMultiColumnHeaderState(columns);
      this.multiColumnHeader = new MultiColumnHeader(headerState);
      this.InitializeMultiColumnTreeView();
      this.Reload();
    }


    protected void InitializeMultiColumnTreeView()
    {
      this.columnIndexForTreeFoldouts = 2;
      this.showAlternatingRowBackgrounds = true;
      this.showBorder = true;

      // Center foldout in the row since we also center content. See RowGUI
      this.rowHeight = rowHeights;
      this.customFoldoutYOffset = (this.rowHeight - EditorGUIUtility.singleLineHeight) * 0.5f;
      this.extraSpaceBeforeIconAndLabel = this.toggleWidth;

      // Search
      this.search = new SearchField();
      this.search.downOrUpArrowKeyPressed += this.SetFocusAndEnsureSelectedItem;

      // Callbacks
      this.multiColumnHeader.sortingChanged += this.OnSortingChanged;
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnSortingChanged(MultiColumnHeader multiColumnHeader)
    {
      this.SortIfNeeded(this.rootItem, this.GetRows());
    }

    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
      var rows = base.BuildRows(root);
      this.SortIfNeeded(root, rows);
      return rows;
    }

    protected override void RowGUI(RowGUIArgs args)
    {
      var item = (TreeViewItem<TreeElementType>)args.item;
      for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
      {
        Rect cellRect = args.GetCellRect(i);
        this.CenterRectUsingSingleLineHeight(ref cellRect);
        this.DrawColumn(cellRect, item, this.GetColumn(args.GetColumn(i)), ref args);

      }
    }

    protected override void ContextClickedItem(int id)
    {
      base.ContextClickedItem(id);
      TreeViewItem<TreeElementType> treeItem = (TreeViewItem<TreeElementType>)this.FindItem(id, this.rootItem);
      TreeElementType treeElement = treeItem.item;

      GenericMenu menu = new GenericMenu();
      this.OnItemContextMenu(menu, treeElement);
      menu.ShowAsContext();

      //Trace.Script($"Context click on {treeElement.name}");
    }

    protected override bool CanRename(TreeViewItem item)
    {
      // Only allow rename if we can showw the rename overlay with a certain width 
      // (label might be clipped by other columns)
      Rect renameRect = GetRenameRect(this.treeViewRect, 0, item);
      return renameRect.width > 30;
    }

    protected override void RenameEnded(RenameEndedArgs args)
    {
      // Set the backend name and reload the tree to reflect the new model
      if (args.acceptedRename)
      {
        var element = this.treeModel.Find(args.itemID);
        element.name = args.newName;
        Reload();
      }
    }

    protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
    {
      Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
      CenterRectUsingSingleLineHeight(ref cellRect);
      return base.GetRenameRect(cellRect, row, item);
    }

    protected override bool CanMultiSelect(TreeViewItem item) => true;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/

    protected static MultiColumnHeaderState BuildMultiColumnHeaderState(TreeViewColumn[] columns)
    {
      Assert.AreEqual(columns.Length, Enum.GetValues(typeof(ColumnType)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");
      MultiColumnHeaderState state = new MultiColumnHeaderState(columns);
      return state;
    }

    private TreeViewColumn[] BuildColumns()
    {
      int numberOfColumns = Enum.GetValues(typeof(ColumnType)).Length;
      TreeViewColumn[] columns = new TreeViewColumn[numberOfColumns];
      for (int c = 0; c < numberOfColumns; ++c)
      {
        ColumnType columnType = this.GetColumn(c);
        columns[c] = this.BuildColumn(columnType);
        if (columns[c] == null)
        {
          throw new Exception($"Column implementation missing for {columnType.ToString()}");
        }
      }
      return columns;
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Checks whether the given instance id is a valid asset for this tree view,
    /// if so it sets it
    /// </summary>
    /// <param name="instanceID"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public bool TryOpenAsset(int instanceID, int line)
    {
      TreeAsset<TreeElementType> treeAsset = EditorUtility.InstanceIDToObject(instanceID) as TreeAsset<TreeElementType>;
      if (treeAsset != null)
      {
        this.SetTreeAsset(treeAsset);
        return true;
      }

      return false;
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


    //------------------------------------------------------------------------/
    // Methods: GUI
    //------------------------------------------------------------------------/
    /// <summary>
    /// Draws the multicolumn tree view GUI
    /// </summary>
    /// <param name="rect"></param>
    public void OnTreeViewGUI(Rect rect)
    {
      this.SearchGUI(rect);
      this.OnGUI(this.GetMultiColumnTreeViewRect(rect));
    }

    /// <summary>
    /// Draws the GUI for the search bar
    /// </summary>
    /// <param name="rect"></param>
    protected void SearchGUI(Rect rect)
    {
      this.searchString = this.search.OnGUI(this.GetToolbarRect(rect), this.searchString);
    }

    /// <summary>
    /// Calculates the rect to be used by the multi-column tree view
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    protected virtual Rect GetMultiColumnTreeViewRect(Rect position)
    {
      return new Rect(20, position.y + 30, position.width - 40, position.height - 60);
    }

    /// <summary>
    /// Calculates the rect to be used by the search bar
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    protected virtual Rect GetToolbarRect(Rect position)
    {
      return new Rect(20f, position.y + 10f, position.width - 40f, 20f);
    }

    public void ToggleColumn(ColumnType column)
    {      
    }

    //------------------------------------------------------------------------/
    // Methods: Sorting
    //------------------------------------------------------------------------/
    private void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
    {
      // If there's only one row orr if there's no columns to sort of
      if (rows.Count <= 1 || this.multiColumnHeader.sortedColumnIndex == -1)
        return;

      // Sort the roots of the existing tree items
      this.SortByMultipleColumns();
      TreeElement.TreeToList(root, rows);
      this.Repaint();
    }

    private void SortByMultipleColumns()
    {
      int[] sortedColumns = multiColumnHeader.state.sortedColumns;
      if (sortedColumns.Empty())
        return;

      var types = rootItem.children.Cast<TreeViewItem<TreeElementType>>();
      var orderedQuery = GetInitialOrder(types, sortedColumns);
      for (int c = 1; c < sortedColumns.Length; ++c)
      {
        int index = sortedColumns[c];
        TreeViewColumn column = this.columns[index];
        bool ascending = this.multiColumnHeader.IsSortedAscending(index);
        orderedQuery = orderedQuery.ThenBy(l => column.selectorFunction(l), ascending);
      }

      this.rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
    }

    IOrderedEnumerable<TreeViewItem<TreeElementType>> GetInitialOrder(IEnumerable<TreeViewItem<TreeElementType>> types, int[] history)
    {
      int index = history[0];
      TreeViewColumn column = this.columns[index];
      bool ascending = multiColumnHeader.IsSortedAscending(index);

      // If a sorting function was provided
      if (column.selectorFunction != null)
        return types.Order(l => column.selectorFunction(l), ascending);

      // Default
      return types.Order(l => l.item.name, ascending);
    }



    //------------------------------------------------------------------------/
    // Utility Methods: Sorting
    //------------------------------------------------------------------------/
    protected void DrawIcon(Rect rect, Texture2D icon)
    {
      GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
    }

    protected void DrawToggle(Rect rect, TreeViewItem<TreeElementType> item, ref bool toggle, ref RowGUIArgs args)
    {
      Rect toggleRect = rect;
      toggleRect.x = GetContentIndent(item);
      toggleRect.width = toggleWidth;

      // Hide when outside cell rect
      if (toggleRect.xMax < rect.xMax)
        toggle = EditorGUI.Toggle(toggleRect, toggle);

      args.rowRect = rect;
      base.RowGUI(args);
    }

    protected void DrawSlider(Rect cellRect, ref float value, float min, float max)
    {
      // When showing controls, make some extra spacing
      const float spacing = 5f;
      cellRect.xMin += spacing;
      value = EditorGUI.Slider(cellRect, GUIContent.none, value, min, max);
    }

    protected void DrawValue(Rect cellRect, string value, bool selected, bool focused)
    {
      DefaultGUI.Label(cellRect, value, selected, focused);
    }

  }

}