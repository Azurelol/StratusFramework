using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;
using UnityEditor;

namespace Stratus
{
  public abstract class HierarchicalTreeView<TreeElementType> : TreeViewWithTreeModel<TreeElementType>
    where TreeElementType : TreeElement
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public HierarchicalTreeView(TreeViewState state, TreeModel<TreeElementType> model) : base(state, model)
    {
      this.InitializeHierarchicalTreeView();
    }

    public HierarchicalTreeView(TreeViewState state, IList<TreeElementType> data) : base(state, new TreeModel<TreeElementType>(data))
    {
      this.InitializeHierarchicalTreeView();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void InitializeHierarchicalTreeView()
    {
      this.Reload();
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnMainGUI(Rect rect)
    {
      GUI.DrawTexture(rect, EditorStyles.toolbar.normal.background);
      base.OnMainGUI(rect);
    }

    protected override void DoubleClickedItem(int id)
    {      
      this.SetExpanded(id, !this.IsExpanded(id));
    }

    //override Row




  }
}
