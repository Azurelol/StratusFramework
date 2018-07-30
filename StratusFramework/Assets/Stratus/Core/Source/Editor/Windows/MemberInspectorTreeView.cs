using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using UnityEditor;

namespace Stratus
{  
  [Serializable]
  public class MemberInspectorTreeElement : TreeElement<ComponentInformation.MemberReference>
  {
    protected override string GetName() => this.data.name;

    public static List<MemberInspectorTreeElement> GenerateFavoritesTree()
    {
      var members = GameObjectBookmark.watchList;
      var elements = MemberInspectorTreeElement.GenerateFlatTree<MemberInspectorTreeElement, ComponentInformation.MemberReference>(Set, members);      
      return elements;
    }

    public static List<MemberInspectorTreeElement> GenerateInspectorTree(GameObjectInformation target)
    {
      var treeBuilder = new TreeBuilder<MemberInspectorTreeElement, ComponentInformation.MemberReference>(Set);
      treeBuilder.AddChildren(target.members, 0);
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

    protected override TreeViewColumn BuildColumn(MemberInspectorWindow.Column columnType)
    {
      TreeViewColumn column = null;
      switch (columnType)
      {
        case MemberInspectorWindow.Column.Favorite: 
          column = new TreeViewColumn
          {
            headerContent = new GUIContent(StratusGUIStyles.starStackIcon, "Watch"),
            headerTextAlignment = TextAlignment.Center,
            sortedAscending = true,
            sortingArrowAlignment = TextAlignment.Right,
            width = 30,
            minWidth = 30,
            maxWidth = 45,
            autoResize = false,
            allowToggleVisibility = false,
            selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.isWatched.ToString()
          };
          break;
        case MemberInspectorWindow.Column.GameObject:
          column = new TreeViewColumn
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
          };
          break;
        case MemberInspectorWindow.Column.Component:
          column = new TreeViewColumn
          {
            headerContent = new GUIContent("Component"),
            sortedAscending = true,
            sortingArrowAlignment = TextAlignment.Right,
            width = 150,
            minWidth = 100,
            maxWidth = 250,
            autoResize = false,
            allowToggleVisibility = true,
            selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.componentName
          };
          break;
        case MemberInspectorWindow.Column.Type:
          column = new TreeViewColumn
          {
            headerContent = new GUIContent("Type"),
            sortedAscending = true,
            sortingArrowAlignment = TextAlignment.Center,
            width = 60,
            minWidth = 60,
            autoResize = false,
            allowToggleVisibility = true,
            selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.item.data.type.ToString()
          };
          break;
        case MemberInspectorWindow.Column.Member:
          column = new TreeViewColumn
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
          };
          break;
        case MemberInspectorWindow.Column.Value:
          column = new TreeViewColumn
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
          };
          break;
      }
      return column;
    }

    protected override void DrawColumn(Rect cellRect, TreeViewItem<MemberInspectorTreeElement> item, MemberInspectorWindow.Column column, ref RowGUIArgs args)
    {
      switch (column)
      {
        case MemberInspectorWindow.Column.Favorite:
          if (item.item.data.isWatched)
            this.DrawIcon(cellRect,StratusGUIStyles.starIcon);
          break;
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

    protected override int GetColumnIndex(MemberInspectorWindow.Column columnType) => (int)columnType;

    protected override void OnItemContextMenu(GenericMenu menu, MemberInspectorTreeElement treeElement)
    {      
      ComponentInformation.MemberReference member = treeElement.data;
      
      // 1. Select
      menu.AddItem(new GUIContent("Select"), false, () => Selection.activeGameObject = member.componentInfo.gameObject);

      if (!Application.isPlaying)
      {
        // 2. Watch
        bool isFavorite = member.isWatched;
        if (isFavorite)
        {
          menu.AddItem(new GUIContent("Remove Watch"), false, () => member.componentInfo.RemoveWatch(member));
        }
        else
        {
          menu.AddItem(new GUIContent("Watch"), false, () =>
          {
            GameObject target = member.componentInfo.gameObject;
            //bool hasBookmark = target.HasComponent<GameObjectBookmark>();
            //if (!hasBookmark)
            //{
            //  MemberInspectorWindow.SetBookmark(member.componentInfo.gameObject);
            //}
            member.componentInfo.Watch(member);
          });        
        }
      }

    }

  }
}