using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using UnityEditor;

namespace Stratus
{  
  [Serializable]
  public class MemberInspectorTreeElement : TreeElement<GameObjectInformation.MemberReference>
  {
    protected override string GetName() => this.data.name;

    /// <summary>
    /// Generates a tree of all current favorited members from bookmarked GameObjects
    /// </summary>
    /// <returns></returns>
    public static List<MemberInspectorTreeElement> GenerateFavoritesTree()
    {
      var elements = MemberInspectorTreeElement.GenerateFlatTree<MemberInspectorTreeElement, GameObjectInformation.MemberReference>(Set, GameObjectBookmark.favorites);      
      return elements;
    }

    /// <summary>
    /// Given a target, generates a tree for its members
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static List<MemberInspectorTreeElement> GenerateInspectorTree(GameObjectInformation target)
    {
      var treeBuilder = new TreeBuilder<MemberInspectorTreeElement, GameObjectInformation.MemberReference>(Set);
      treeBuilder.AddRoot();
      treeBuilder.AddChildren(target.memberReferences, 0);
      return treeBuilder.ToTree();
    }
  } 


  public class MemberInspectorTreeView : MultiColumnTreeView<MemberInspectorTreeElement, MemberInspectorWindow.Column>  
  {
    public MemberInspectorTreeView(TreeViewState state, TreeModel<MemberInspectorTreeElement> model) : base(state, model)
    {
    }

    public MemberInspectorTreeView(TreeViewState state, IList<MemberInspectorTreeElement> data) : base(state, data)
    {
    }

    protected override TreeViewColumn[] BuildColumns()
    {
      var columns = new TreeViewColumn[]
      {
        new TreeViewColumn
        {
          headerContent = new GUIContent("GameObject"),
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Right,
          width = 100,
          minWidth = 100,
          maxWidth = 120,
          autoResize = false,
          allowToggleVisibility = true,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.gameObjectName
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Component"),
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Right,
          width = 200,
          minWidth = 150,
          maxWidth = 250,
          autoResize = false,
          allowToggleVisibility = true,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.componentName
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Type"),
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Center,
          width = 60,
          minWidth = 60,
          autoResize = false,
          allowToggleVisibility = true,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.type.ToString()
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Member"),
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Center,
          width = 100,
          minWidth = 80,
          maxWidth = 120,
          autoResize = false,
          allowToggleVisibility = false,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.name
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Value"),
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Left,
          width = 200,
          minWidth = 150,
          maxWidth = 250,
          autoResize = true,
          allowToggleVisibility = false,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.latestValueString
        }        
      };

      return columns;
    }

    protected override void DrawColumn(Rect cellRect, TreeViewItem<MemberInspectorTreeElement> item, MemberInspectorWindow.Column column, ref RowGUIArgs args)
    {
      switch (column)
      {
        case MemberInspectorWindow.Column.GameObject:
          DefaultGUI.Label(cellRect, item.item.data.gameObjectName, args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Component:
          DefaultGUI.Label(cellRect, item.item.data.componentName, args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Type:
          DefaultGUI.Label(cellRect, item.item.data.type.ToString(), args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Member:
          DefaultGUI.Label(cellRect, item.item.data.name, args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Value:
          DefaultGUI.Label(cellRect, item.item.data.latestValueString, args.selected, args.focused);
          break;
      }
    }

    protected override MemberInspectorWindow.Column GetColumn(int index) => (MemberInspectorWindow.Column)index;

    protected override void OnItemContextMenu(GenericMenu menu, MemberInspectorTreeElement treeElement)
    {
      GameObjectInformation.MemberReference member = treeElement.data;
      bool isFavorite = member.isFavorite;
      if (isFavorite)
        menu.AddItem(new GUIContent("Remove Watch"), false, () => member.gameObjectInfo.RemoveWatch(member));
      else
        menu.AddItem(new GUIContent("Watch"), false, () => member.gameObjectInfo.Watch(member));        
    }

  }
}