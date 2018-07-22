using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using UnityEditor;

namespace Stratus
{  
  public class MemberInspectorTreeAsset : TreeAsset<MemberInspectorTreeElement>
  {
  }

  [Serializable]
  public class MemberInspectorTreeElement : TreeElement
  {
    public GameObjectInformation.MemberReference member;

    public void Set(GameObjectInformation.MemberReference member)
    {
      this.member = member;
      this.name = member.name;
    }
  }


  public class MemberInspectorTreeView : MultiColumnTreeView<MemberInspectorTreeElement, MemberInspectorWindow.Column>  
  {
    public MemberInspectorTreeView(TreeViewState state, TreeViewColumn[] columns, TreeModel<MemberInspectorTreeElement> model) : base(state, columns, model)
    {
    }

    public MemberInspectorTreeView(TreeViewState state, TreeViewColumn[] columns, IList<MemberInspectorTreeElement> data) : base(state, columns, data)
    {
    }

    protected static TreeViewColumn[] BuildColumns()
    {
      var columns = new TreeViewColumn[]
      {
        new TreeViewColumn
        {
          headerContent = new GUIContent("GameObject"),
          headerTextAlignment = TextAlignment.Left,
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Right,
          width = 100,
          minWidth = 100,
          maxWidth = 120,
          autoResize = false,
          allowToggleVisibility = true,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.data.member.gameObjectName
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Component"),
          headerTextAlignment = TextAlignment.Left,
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Right,
          width = 200,
          minWidth = 150,
          maxWidth = 250,
          autoResize = false,
          allowToggleVisibility = true,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.data.member.componentName
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Type"),
          headerTextAlignment = TextAlignment.Left,
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Center,
          width = 60,
          minWidth = 60,
          autoResize = false,
          allowToggleVisibility = false,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.data.member.type.ToString()
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Member"),
          headerTextAlignment = TextAlignment.Left,
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Center,
          width = 100,
          minWidth = 80,
          maxWidth = 120,
          autoResize = false,
          allowToggleVisibility = false,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.data.member.name
        },
        new TreeViewColumn
        {
          headerContent = new GUIContent("Value", "In sed porta ante. Nunc et nulla mi."),
          headerTextAlignment = TextAlignment.Left,
          sortedAscending = true,
          sortingArrowAlignment = TextAlignment.Left,
          width = 200,
          minWidth = 150,
          maxWidth = 250,
          autoResize = true,
          selectorFunction = (TreeViewItem<MemberInspectorTreeElement> element) => element.data.member.latestValueString
        }        
      };

      return columns;
    }

    protected override void DrawColumn(Rect cellRect, TreeViewItem<MemberInspectorTreeElement> item, MemberInspectorWindow.Column column, ref RowGUIArgs args)
    {
      switch (column)
      {
        case MemberInspectorWindow.Column.GameObject:
          DefaultGUI.Label(cellRect, item.data.member.gameObjectName, args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Component:
          DefaultGUI.Label(cellRect, item.data.member.componentName, args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Type:
          DefaultGUI.Label(cellRect, item.data.member.type.ToString(), args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Member:
          DefaultGUI.Label(cellRect, item.data.member.name, args.selected, args.focused);
          break;
        case MemberInspectorWindow.Column.Value:
          DefaultGUI.Label(cellRect, item.data.member.latestValueString, args.selected, args.focused);
          break;
      }
    }

    protected override MemberInspectorWindow.Column GetColumn(int index) => (MemberInspectorWindow.Column)index;

    public static MemberInspectorTreeView Create(IList<MemberInspectorTreeElement> tree, TreeViewState state = null)
    {
      TreeViewColumn[] columns = BuildColumns();
      MemberInspectorTreeView treeView = new MemberInspectorTreeView(state, columns, tree);
      return treeView;

      // Columns
      //// Header state
      //MultiColumnHeaderState headerState = BuildMultiColumnHeaderState(columns);
      //// Header
      //MultiColumnHeader header = new MultiColumnHeader(headerState);
      // Model
      //TreeModel<MemberInspectorTreeElement> treeModel = new TreeModel<MemberInspectorTreeElement>(tree);      

    }

  }
}