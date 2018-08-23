using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  /// <summary>
  /// Draws enums using a popup window that includes a search field
  /// Credit to (https://github.com/roboryantron/UnityEditorJunkie)
  /// </summary>
  public static class SearchableEnum 
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    private static int hash { get; } = nameof(SearchableEnum).GetHashCode();
    private static Dictionary<Type, string[]> enumDisplayNames { get; set; }
    private static Dictionary<Type, Array> enumValues{ get; set; }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Displays an searchable enum popup
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static Enum EnumPopup(Rect position, string label, Enum selected)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);

      // Prefix
      GUIContent labelContent = new GUIContent(label);
      position = EditorGUI.PrefixLabel(position, id, labelContent);

      int index = 0;

      // Enum Drawer
      Type enumType = selected.GetType();
      string[] displayedOptions = GetEnumDisplayNames(selected);
      GUIContent enumValueContent = new GUIContent(selected.ToString());
      if (DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelect = i =>
        {
          index = i;
        };
        SearchablePopup.EditorGUI(position, index, displayedOptions, onSelect);
      }

      return GetEnumValue(enumType, index);
    }

    /// <summary>
    /// Displays an searchable enum popup
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static int EnumPopup(Rect position, string label, Type enumType, int selectedIndex)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);

      // Prefix
      GUIContent labelContent = new GUIContent(label);
      position = EditorGUI.PrefixLabel(position, id, labelContent);

      // Enum Drawer
      string[] displayedOptions = GetEnumDisplayNames(enumType);
      GUIContent enumValueContent = new GUIContent(displayedOptions[selectedIndex]);
      if (DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelect = i =>
        {
          selectedIndex = i;
        };
        SearchablePopup.EditorGUI(position, selectedIndex, displayedOptions, onSelect);
      }

      return selectedIndex;
    }

    /// <summary>
    /// Displays an searchable enum popup
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void EnumPopup(Rect position, string label, SerializedProperty property)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);

      // Prefix
      GUIContent labelContent = new GUIContent(label);
      position = EditorGUI.PrefixLabel(position, id, labelContent);

      // Enum Drawer
      string[] displayedOptions = property.enumDisplayNames;
      GUIContent enumValueContent = new GUIContent(displayedOptions[property.enumValueIndex]);
      if (DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelect = i =>
        {
          property.enumValueIndex = i;
          property.serializedObject.ApplyModifiedProperties();
        };
        SearchablePopup.EditorGUI(position, property.enumValueIndex, displayedOptions, onSelect);
      }
    }


    /// <summary>
    /// A custom button drawer that allows for a controlID so that we can
    /// sync the button ID and the label ID to allow for keyboard
    /// navigation like the built-in enum drawers.
    /// </summary>
    private static bool DropdownButton(int id, Rect position, GUIContent content)
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
          EditorStyles.popup.Draw(position, content, id, false);
          break;
      }
      return false;
    }

    private static string[] GetEnumDisplayNames(Enum value)
    {
      Type type = value.GetType();
      return enumDisplayNames.GetValueAddIfMissing(type, Enum.GetNames);
    }

    private static string[] GetEnumDisplayNames(Type enumType)
    {
      return enumDisplayNames.GetValueAddIfMissing(enumType, Enum.GetNames);
    }

    private static Array GetEnumValues(Type enumType)
    {
      return enumValues.GetValueAddIfMissing(enumType, Enum.GetValues);
    }

    private static Enum GetEnumValue(Type enumType, int index)
    {
      return (Enum)GetEnumValues(enumType).GetValue(index);
    }

  }

}