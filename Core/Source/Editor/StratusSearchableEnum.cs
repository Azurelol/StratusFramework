﻿using System.Collections;
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
  public static class StratusSearchableEnum 
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    private static int hash { get; } = nameof(StratusSearchableEnum).GetHashCode();
    private static Dictionary<Type, string[]> enumDisplayNames { get; set; } = new Dictionary<Type, string[]>();
    private static Dictionary<Type, Array> enumValues { get; set; } = new Dictionary<Type, Array>();
    

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
    public static void EnumPopup(Rect position, string label, Enum selected, System.Action<Enum> onSelected)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);

      // Prefix
      GUIContent labelContent = new GUIContent(label);
      position = EditorGUI.PrefixLabel(position, id, labelContent);

      int index = 0;

      // Enum Drawer
      Type enumType = selected.GetType();
      string[] displayedOptions = GetEnumDisplayNames(enumType);
      GUIContent enumValueContent = new GUIContent(selected.ToString());
      if (StratusSearchablePopup.DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelectIndex = i =>
        {
          Enum value = GetEnumValue(enumType, i);
          StratusDebug.Log($"Selected {value}");
          onSelected(value);
        };
        StratusSearchablePopup.EditorGUI(position, index, displayedOptions, onSelectIndex);
      }
    }

    /// <summary>
    /// Displays an searchable enum popup
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void EnumPopup(Rect position, string label, Type enumType, int selectedIndex, System.Action<Enum> onSelected)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);

      // Prefix
      GUIContent labelContent = new GUIContent(label);
      position = EditorGUI.PrefixLabel(position, id, labelContent);

      // Enum Drawer
      string[] displayedOptions = GetEnumDisplayNames(enumType);
      GUIContent enumValueContent = new GUIContent(displayedOptions[selectedIndex]);
      if (StratusSearchablePopup.DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelect = i =>
        {
          Enum value = GetEnumValue(enumType, i);
          StratusDebug.Log($"Selected {value}");
          onSelected(value);
        };
        StratusSearchablePopup.EditorGUI(position, selectedIndex, displayedOptions, onSelect);
      }
    }

    /// <summary>
    /// Displays an searchable enum popup
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void EnumPopup(Rect position, GUIContent label, SerializedProperty property)
    {
      int id = GUIUtility.GetControlID(hash, FocusType.Keyboard, position);

      // Prefix
      position = EditorGUI.PrefixLabel(position, id, label);
      // Enum Drawer
      string[] displayedOptions = property.enumDisplayNames;
      GUIContent enumValueContent = new GUIContent(displayedOptions[property.enumValueIndex]);
      if (StratusSearchablePopup.DropdownButton(id, position, enumValueContent))
      {
        System.Action<int> onSelect = i =>
        {
          property.enumValueIndex = i;
          property.serializedObject.ApplyModifiedProperties();
        };
        StratusSearchablePopup.EditorGUI(position, property.enumValueIndex, displayedOptions, onSelect);
      }
    }

    /// <summary>
    /// Displays an searchable enum popup for a serialized property, wrapping it within BeginProperty
    /// </summary>
    /// <param name="position"></param>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public static void EnumPopup(Rect position, SerializedProperty property)
    {
      GUIContent label = new GUIContent(property.displayName);
      label = EditorGUI.BeginProperty(position, label, property);
      EnumPopup(position, label, property);
      EditorGUI.EndProperty();
    }

    /// <summary>
    /// Displays a searchable enum popup for a serialized property
    /// </summary>
    /// <param name="property"></param>
    public static void EnumPopup(SerializedProperty property)
    {
      EnumPopup(StratusSearchablePopup.defaultPosition, property);
    }

    public static string[] GetEnumDisplayNames(Enum value)
    {
      Type type = value.GetType();
      return enumDisplayNames.GetValueOrAdd(type, Enum.GetNames);
    }

    public static string[] GetEnumDisplayNames(Type enumType)
    {
      return enumDisplayNames.GetValueOrAdd(enumType, Enum.GetNames);
    }

    public static Array GetEnumValues(Type enumType)
    {
      return enumValues.GetValueOrAdd(enumType, Enum.GetValues);
    }

    public static Enum GetEnumValue(Type enumType, int index)
    {
      return (Enum)GetEnumValues(enumType).GetValue(index);
    }

  }

}