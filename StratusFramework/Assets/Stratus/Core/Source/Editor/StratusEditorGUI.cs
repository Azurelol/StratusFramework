using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  public static class StratusEditorGUI
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public static float standardPadding { get; } = 8f;
    private static TextAlignment currentAlignGroup { get; set; }
    private static bool alignGroupActive { get; set; }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public static void GUILayoutPopup(string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
    {
      SearchablePopup.Popup(label, selectedindex, displayedOptions, onSelected);
    }

    public static void GUILayoutPopup(string label, DropdownList dropdownList)
    {
      SearchablePopup.Popup(label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUILayoutPopup(DropdownList dropdownList)
    {
      SearchablePopup.Popup(dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, string label, DropdownList dropdownList)
    {
      SearchablePopup.Popup(position, label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, DropdownList dropdownList)
    {
      SearchablePopup.Popup(position, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
    {
      SearchablePopup.Popup(position, label, selectedindex, displayedOptions, onSelected);
    }

    public static int GUILayoutPopup(string label, int selectedindex, string[] displayedOptions)
    {
      return EditorGUILayout.Popup(label, selectedindex, displayedOptions);
    }

    public static bool EditorGUILayoutProperty(SerializedProperty serializedProperty, string label)
    {
      EditorGUI.BeginChangeCheck();
      EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label));
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(serializedProperty.serializedObject.targetObject, serializedProperty.displayName);
        serializedProperty.serializedObject.ApplyModifiedProperties();
        return true;
      }
      return false;
    }

    public static void EnumToolbar<T>(ref T enumValue)
    {
      string[] options = SearchableEnum.GetEnumDisplayNames((Enum)(object)enumValue);
      enumValue = (T)(object)GUILayout.Toolbar(Convert.ToInt32(enumValue), options, GUILayout.ExpandWidth(false));
    }

    public static void BeginAligned(TextAlignment alignment)
    {
      currentAlignGroup = alignment;
      GUILayout.BeginHorizontal();
      switch (alignment)
      {
        case TextAlignment.Left:
          GUILayout.FlexibleSpace();
          break;

        case TextAlignment.Center:
          GUILayout.FlexibleSpace();
          break;

        case TextAlignment.Right:
          GUILayout.FlexibleSpace();
          break;
      }
      alignGroupActive = true;
    }

    public static void EndAligned()
    {
      if (!alignGroupActive)
        throw new ArgumentException("Missing a matching BeginAligned call!");

      switch (currentAlignGroup)
      {
        case TextAlignment.Left:
          GUILayout.FlexibleSpace();
          break;

        case TextAlignment.Center:
          GUILayout.FlexibleSpace();
          break;

        case TextAlignment.Right:
          break;
      }
      GUILayout.EndHorizontal();
      alignGroupActive = false;
    }




  }

}