using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Utility class for building a tree to be used with the TreeView
  /// </summary>
  /// <typeparam name="TreeElementType"></typeparam>
  public class TreeBuilder<TreeElementType, DataType> 
    where TreeElementType : TreeElement, new ()
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private List<TreeElementType> tree = new List<TreeElementType>();
    private int idCounter = 0;
    private System.Action<TreeElementType, DataType> setData;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public TreeBuilder(System.Action<TreeElementType, DataType> setData)
    {
      this.setData = setData;
    }

    public void AddRoot()
    {
      TreeElementType root = new TreeElementType();
      root.name = "Root";
      root.depth = -1;
      root.id = idCounter++;
      tree.Add(root);
    }

    public void AddChild(DataType childData, int depth)
    {
      TreeElementType child = new TreeElementType();
      child.id = idCounter++;
      child.depth = depth;
      setData(child, childData);
      tree.Add(child);
    }

    public void AddChildren(DataType[] childrenData, int depth)
    {
      foreach (var childData in childrenData)
      {
        this.AddChild(childData, depth);
      }
    }

    public List<TreeElementType> ToTree() { return this.tree; }

  }

}