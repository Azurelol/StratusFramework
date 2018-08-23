using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  /// <summary>
  /// A popup windoww that displays a list of options alongside a search field
  /// in order to filter the displayed content.
  /// Credit to Ryan Hippie's work (https://github.com/roboryantron/UnityEditorJunkie)
  /// </summary>
  public class SearchablePopup : PopupWindowContent
  {
    //------------------------------------------------------------------------/
    // Fields: Static/Const
    //------------------------------------------------------------------------/
    /// <summary>
    /// Height of each element in the popup list
    /// </summary>
    private const float rowHeight = 16f;
    /// <summary>
    /// How far to indent list entries
    /// </summary>
    private const float rowIndent = 8f;
    /// <summary>
    /// Name to use for the text field for search
    /// </summary>
    private const string searchControlName = nameof(SearchablePopup);

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private System.Action<int> onSelection;
    private int selectedIndex;
    private FilteredStringList list;
    private Vector2 scrollPosition;
    private int hoverIndex;
    private int scrollToIndex;
    private float scrollOffset;

    // Styles
    private static GUIStyle SearchBoxStyle = "ToolbarSeachTextField";
    private static GUIStyle CancelButtonStyle = "ToolbarSeachCancelButton";
    private static GUIStyle DisabledCancelButtonStyle = "ToolbarSeachCancelButtonEmpty";
    private static GUIStyle SelectionStyle = "SelectionRect";

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public static GUIStyle toolbarStyle => EditorStyles.toolbar;
    public static Color selectedColor => Color.cyan;
    public static Color defaultColor => Color.white;
    public static UnityEngine.Event currentEvent => UnityEngine.Event.current;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    private SearchablePopup(string[] displayedOptions, int selectedIndex, System.Action<int> onSelection)
    {
      this.list = new FilteredStringList(displayedOptions);
      this.selectedIndex = this.hoverIndex = this.scrollToIndex = selectedIndex;
      this.onSelection = onSelection;
      this.scrollOffset = this.GetWindowSize().y - rowHeight * 2f;
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public override void OnOpen()
    {
      base.OnOpen();
      EditorApplication.update += Repaint;
    }

    public override void OnClose()
    {
      base.OnClose();
      EditorApplication.update -= Repaint;
    }

    public override Vector2 GetWindowSize()
    {
      return new Vector2(base.GetWindowSize().x, Mathf.Min(600, list.items.Length * rowHeight + toolbarStyle.fixedHeight));
    }

    public override void OnGUI(Rect rect)
    {
      Rect searchRect = new Rect(0, 0, rect.width, toolbarStyle.fixedHeight);
      Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);
            
      ProcessEvent(StratusEditorUtility.currentEvent);
      DrawSearch(searchRect);
      DrawSelectionArea(scrollRect);
    }

    //------------------------------------------------------------------------/
    // Procedures
    //------------------------------------------------------------------------/
    private void DrawSearch(Rect rect)
    {
      if (currentEvent.type == EventType.Repaint)
        EditorStyles.toolbar.Draw(rect, false, false, false, false);

      rect.xMin += 6;
      rect.xMax -= 6;
      rect.y += 2;
      rect.width -= CancelButtonStyle.fixedWidth;

      GUI.FocusControl(searchControlName);
      GUI.SetNextControlName(searchControlName);
      string newText = GUI.TextField(rect, this.list.filter, SearchBoxStyle);

      if (this.list.UpdateFilter(newText))
      {
        this.hoverIndex = 0;
        this.ResetScrollPosition();
      }

      rect.x = rect.xMax;
      rect.width = CancelButtonStyle.fixedWidth;

      if (string.IsNullOrEmpty(this.list.filter))
        GUI.Box(rect, GUIContent.none, DisabledCancelButtonStyle);
      else if (GUI.Button(rect, "x", CancelButtonStyle))
      {
        this.list.UpdateFilter("");
        this.ResetScrollPosition();
      }

    }

    private void DrawSelectionArea(Rect rect)
    {
      Rect contentRect = new Rect(0, 0, rect.width - GUI.skin.verticalScrollbar.fixedWidth, this.list.entryCount * rowHeight);
      this.scrollPosition = GUI.BeginScrollView(rect, this.scrollPosition, contentRect);
      {
        Rect rowRect = new Rect(0, 0, rect.width, rowHeight);
        for (int i = 0; i < this.list.entryCount; i++)
        {
          if (scrollToIndex == i && 
            (currentEvent.type == EventType.Repaint || currentEvent.type == EventType.Layout))
          {
            Rect r = new Rect(rowRect);
            r.y += this.scrollOffset;
            GUI.ScrollTo(r);
            this.scrollToIndex = -1;
            this.scrollPosition.x = 0;
          }

          if (rowRect.Contains(currentEvent.mousePosition))
          {
            switch (currentEvent.type)
            {
              case EventType.MouseMove:
              case EventType.ScrollWheel:
                this.hoverIndex = i;
                break;

              case EventType.MouseDown:
                this.onSelection(this.list.entries[i].index);
                EditorWindow.focusedWindow.Close();
                break;
            }
          }

          DrawRow(rowRect, i);
          rowRect.y = rowRect.yMax;
        }
      }
      GUI.EndScrollView();
    }

    private void DrawRow(Rect rowRect, int index)
    {
      if (this.list.entries[index].index == this.selectedIndex)
        StratusGUI.GUIBox(rowRect, selectedColor, SelectionStyle);
      else if (index == this.hoverIndex)
        StratusGUI.GUIBox(rowRect, defaultColor, SelectionStyle);

      rowRect.xMin += rowIndent;
      GUI.Label(rowRect, this.list.entries[index].item.value);
    }

    private void ProcessEvent(UnityEngine.Event currentEvent)
    {
      if (currentEvent.type == EventType.KeyDown)
      {
        switch (currentEvent.keyCode)
        {
          case KeyCode.DownArrow:
            this.scrollToIndex = this.hoverIndex = Mathf.Min(this.list.maxIndex, this.hoverIndex + 1);
            this.scrollOffset = rowHeight;
            currentEvent.Use();
            break;

          case KeyCode.UpArrow:
            this.scrollToIndex = this.hoverIndex = Mathf.Max(0, this.hoverIndex - 1);
            this.scrollOffset = -rowHeight;
            currentEvent.Use();
            break;

          case KeyCode.Return:
            if (this.hoverIndex >= 0 && this.hoverIndex < this.list.entries.Count)
            {
              this.onSelection(this.list.entries[hoverIndex].index);
              EditorWindow.focusedWindow.Close();
            }
            break;

          case KeyCode.Escape:
            EditorWindow.focusedWindow.Close();
            break;
        }
      }
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    public static void EditorGUI(Rect rect, int selected, string[] displayedOptions, System.Action<int> onSelection)
    {
      SearchablePopup window = new SearchablePopup(displayedOptions, selected, onSelection);
      PopupWindow.Show(rect, window);
    }

    private static void Repaint() => EditorWindow.focusedWindow.Repaint();    
    private void ResetScrollPosition() => this.scrollPosition = Vector2.zero;

  }

}