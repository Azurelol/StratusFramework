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
  public class StratusSearchablePopup : PopupWindowContent
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
    private const string searchControlName = nameof(StratusSearchablePopup);

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
    public static float height = StratusEditorUtility.lineHeight;    
    public static Rect defaultPosition => GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 50f, height);
    public static GUIStyle toolbarStyle => EditorStyles.toolbar;
    public static Color selectedColor => Color.cyan;
    public static Color defaultColor => Color.white;
    public static UnityEngine.Event currentEvent => UnityEngine.Event.current;
    private static int hash { get; } = nameof(StratusSearchablePopup).GetHashCode();

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    private StratusSearchablePopup(string[] displayedOptions, int selectedIndex, System.Action<int> onSelection)
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
      return new Vector2(base.GetWindowSize().x, Mathf.Min(600, list.entries.Length * rowHeight + toolbarStyle.fixedHeight));
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
      Rect contentRect = new Rect(0, 0, rect.width - GUI.skin.verticalScrollbar.fixedWidth, this.list.currentEntryCount * rowHeight);
      this.scrollPosition = GUI.BeginScrollView(rect, this.scrollPosition, contentRect);
      {
        Rect rowRect = new Rect(0, 0, rect.width, rowHeight);
        for (int i = 0; i < this.list.currentEntryCount; i++)
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
                this.onSelection(this.list.currentEntries[i].index);
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
      if (this.list.currentEntries[index].index == this.selectedIndex)
        StratusGUI.GUIBox(rowRect, selectedColor, SelectionStyle);
      else if (index == this.hoverIndex)
        StratusGUI.GUIBox(rowRect, defaultColor, SelectionStyle);

      rowRect.xMin += rowIndent;
      GUI.Label(rowRect, this.list.currentEntries[index].item.value);
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
            if (this.hoverIndex >= 0 && this.hoverIndex < this.list.currentEntries.Count)
            {
              this.onSelection(this.list.currentEntries[hoverIndex].index);
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
      StratusSearchablePopup window = new StratusSearchablePopup(displayedOptions, selected, onSelection);
      PopupWindow.Show(rect, window);
    }

    /// <summary>
    /// Displays an searchable popup for a list of strings
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void Popup(Rect position, string label, int selectedIndex, string[] displayedOptions, System.Action<int> onSelected)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);

      // Prefix
      position.height = height;
      GUIContent labelContent = new GUIContent(label);
      position = UnityEditor.EditorGUI.PrefixLabel(position, id, labelContent);      

      // Enum Drawer
      GUIContent enumValueContent = new GUIContent(displayedOptions[selectedIndex]);
      if (DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelect = i =>
        {
          onSelected(i);
          //string value = displayedOptions[i];
        };
        StratusSearchablePopup.EditorGUI(position, selectedIndex, displayedOptions, onSelect);
      }
    }

    /// <summary>
    /// Displays an searchable popup for a list of strings
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void Popup(Rect position, int selectedIndex, string[] displayedOptions, System.Action<int> onSelected)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);
      position.height = height;
      //position.width *= 0.5f;
      // Enum Drawer
      GUIContent enumValueContent = new GUIContent(displayedOptions[selectedIndex]);
      if (DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelect = i =>
        {
          onSelected(i);
          //string value = displayedOptions[i];
        };
        StratusSearchablePopup.EditorGUI(position, selectedIndex, displayedOptions, onSelect);
      }
    }

    /// <summary>
    /// Displays an searchable popup for a list of strings
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void Popup(string label, int selectedIndex, string[] displayedOptions, System.Action<int> onSelected)
    {
      Popup(defaultPosition, label, selectedIndex, displayedOptions, onSelected);
    }

    /// <summary>
    /// Displays an searchable popup for a list of strings
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void Popup(int selectedIndex, string[] displayedOptions, System.Action<int> onSelected)
    {
      Popup(defaultPosition, selectedIndex, displayedOptions, onSelected);
    }

    private static void Repaint() => EditorWindow.focusedWindow.Repaint();    
    private void ResetScrollPosition() => this.scrollPosition = Vector2.zero;

    /// <summary>
    /// A custom button drawer that allows for a controlID so that we can
    /// sync the button ID and the label ID to allow for keyboard
    /// navigation like the built-in enum drawers.
    /// </summary>
    public static bool DropdownButton(int id, Rect position, GUIContent content)
    {
      UnityEngine.Event current = UnityEngine.Event.current;
      switch (current.type)
      {
        case EventType.MouseDown:
          if (position.Contains(current.mousePosition) && current.button == 0)
          {
            UnityEngine.Event.current.Use();
            return true;
          }
          break;
        case EventType.KeyDown:
          if (GUIUtility.keyboardControl == id && current.character == '\n')
          {
            UnityEngine.Event.current.Use();
            return true;
          }
          break;
        case EventType.Repaint:
          //UnityEditor.EditorGUI.Popup(position, )
          EditorStyles.popup.Draw(position, content, id, false);
          break;
      }
      return false;
    }

  }

}