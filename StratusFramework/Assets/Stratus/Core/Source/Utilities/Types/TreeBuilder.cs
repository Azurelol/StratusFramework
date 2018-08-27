using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OdinSerializer;

namespace Stratus
{
  /// <summary>
  /// Utility class for building a tree to be used with the TreeView
  /// </summary>
  /// <typeparam name="TreeElementType"></typeparam>  
  public class TreeBuilder<TreeElementType, DataType> 
    where TreeElementType : TreeElement<DataType>, new ()
    where DataType : INamed
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
      //child.data = childData;
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
  public class SerializedTree<TreeElementType, DataType>
    where TreeElementType : TreeElement<DataType>, new()
    where DataType : INamed
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [OdinSerialize]
    public List<TreeElementType> elements = new List<TreeElementType>();
    [SerializeField]
    private int idCounter = 0;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/    

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
    // Methods: Public
    //------------------------------------------------------------------------/
    public void AddElement(DataType data, int depth)
    {
      TreeElementType element = new TreeElementType();
      element.id = idCounter++;
      element.depth = depth;
      element.Set(data);
      elements.Add(element);
    }

    public void AddElements(DataType[] elementsData, int depth)
    {
      foreach (var data in elementsData)
      {
        this.AddElement(data, depth);
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
    private void AddRoot()
    {
      TreeElementType root = new TreeElementType();
      root.name = "Root";
      root.depth = -1;
      root.id = idCounter++;
      elements.Add(root);
    }
  }

}