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

    public static int GUILayoutPopup(string label, int selectedindex, string[] displayedOptions)
    {
      return EditorGUILayout.Popup(label, selectedindex, displayedOptions);
    }
  }

}