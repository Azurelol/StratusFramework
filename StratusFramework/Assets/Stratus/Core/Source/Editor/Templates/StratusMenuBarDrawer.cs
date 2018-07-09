using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  public class StratusMenuBarDrawer
  {
    private struct ContextMenuOption
    {
      public GUIContent content;
      public GenericMenu.MenuFunction menuFunction;

      public ContextMenuOption(GUIContent content, GenericMenu.MenuFunction menuFunction)
      {
        this.content = content;
        this.menuFunction = menuFunction;
      }
    }

    public Coordinates.Orientation orientation { get; private set; }
    private List<ContextMenuOption> addItems = new List<ContextMenuOption>();
    private List<ContextMenuOption> optionItems = new List<ContextMenuOption>();
    private List<ContextMenuOption> validateItems = new List<ContextMenuOption>();

    public StratusMenuBarDrawer(Coordinates.Orientation orientation)
    {
      this.orientation = orientation;
    }

    public void Draw()
    {
      switch (this.orientation)
      {
        case Coordinates.Orientation.Horizontal:
          EditorGUILayout.BeginHorizontal();
          this.OnDraw();
          EditorGUILayout.EndHorizontal();
          break;

        case Coordinates.Orientation.Vertical:
          EditorGUILayout.BeginVertical();
          this.OnDraw();
          EditorGUILayout.EndVertical();
          break;
      }
    }

    public void AddItem(string content, GenericMenu.MenuFunction menuFunction, StratusEditorUtility.ContextMenuType type)
    {
      this.AddItem(new GUIContent(content), menuFunction, type);
    }

    public void AddItem(GUIContent content, GenericMenu.MenuFunction menuFunction, StratusEditorUtility.ContextMenuType type)
    {
      ContextMenuOption item = new ContextMenuOption(content, menuFunction);
      switch (type)
      {
        case StratusEditorUtility.ContextMenuType.Add:
          addItems.Add(item);
          break;
        case StratusEditorUtility.ContextMenuType.Validation:
          optionItems.Add(item);
          break;
        case StratusEditorUtility.ContextMenuType.Options:
          validateItems.Add(item);
          break;
      }
    }

    private void OnDraw()
    {
      this.DrawContextMenu(StratusEditorUtility.ContextMenuType.Add, this.addItems);
      this.DrawContextMenu(StratusEditorUtility.ContextMenuType.Options, this.optionItems);
      this.DrawContextMenu(StratusEditorUtility.ContextMenuType.Validation, this.validateItems);
    }

    private void DrawContextMenu(StratusEditorUtility.ContextMenuType type, List<ContextMenuOption> options)
    {
      if (options.NotEmpty())
      {
        var menu = new GenericMenu();
        foreach (var option in options)
        {
          menu.AddItem(option.content, false, option.menuFunction);
        }
        StratusEditorUtility.DrawContextMenu(menu, type);
      }
    }
  }
}