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
    }

  }
}
