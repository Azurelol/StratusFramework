using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  public static class StratusEditorGUI
  {
    public static void GUILayoutPopup(string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
    {
      SearchablePopup.Popup(label, selectedindex, displayedOptions, onSelected);
    }

    public static void GUILayoutPopup(string label, DropdownList dropdownList)
    {
      SearchablePopup.Popup(label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, string label, DropdownList dropdownList)
    {
      SearchablePopup.Popup(position, label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
    {
      SearchablePopup.Popup(position, label, selectedindex, displayedOptions, onSelected);
    }

    public static int GUILayoutPopup(string label, int selectedindex, string[] displayedOptions)
    {
      return EditorGUILayout.Popup(label, selectedindex, displayedOptions);
    }
  }

}