using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Utilities;
using UnityEditor;
using System.Linq.Expressions;
using System.Linq;

namespace Stratus
{
  /// <summary>
  /// Allows an easy interface for selecting subclasses from a given type
  /// </summary>
  public class TypeSelector
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public Type baseClass { get; private set; }
    public Type[] subclasses { get; private set; }
    public Type selectedClass => subclasses[currentIndex];
    private string selectedClassName => selectedClass.Name;
    public string[] displayedOptions { get; private set; }
    public bool showHint
    {
      set { showHintField = value; SetHint(value); }
      get { return showHintField; }
    }
    public bool isValidIndex => showHint ? selectedIndex > 0 : true;
    public int currentIndex => showHint ? selectedIndex - 1 : selectedIndex;
    public string selectionHint
    {
      get { return selectionHintField; }
      set
      {
        selectionHintField = value;
        if (showHint)
          displayedOptions[0] = value;
      }
     }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private int selectedIndex = 0;
    private bool showHintField;
    private string selectionHintField;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public TypeSelector(Type baseClass, bool sortAlphabetically = false, bool showHint = false)
    {
      this.baseClass = baseClass;
      this.subclasses = Reflection.GetSubclass(baseClass);
      this.displayedOptions = Reflection.GetSubclassNames(baseClass);
      
      if (sortAlphabetically)
      {
        Array.Sort(displayedOptions);
        Array.Sort(subclasses, (Type left, Type right) => { return left.Name.CompareTo(right.Name); });
      }

      this.showHint = showHint;
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public void ResetSelection(int index = 0)
    {
      selectedIndex = index;
    }

    public bool GUILayoutPopup()
    {
      bool changed = StratusEditorUtility.CheckControlChange(() =>
      {
        selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions);
      });
      return changed && isValidIndex;
    }

    public bool GUILayoutPopup(GUIStyle style)
    {
      bool changed = StratusEditorUtility.CheckControlChange(() =>
      {
        selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions, style);
      });
      return changed && isValidIndex;
    }

    public bool GUILayoutPopup(GUIStyle style, params GUILayoutOption[] options)
    {
      bool changed = StratusEditorUtility.CheckControlChange(() =>
      {
        selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions, style, options);
      });
      return changed && isValidIndex;
    }

    public bool GUILayoutPopup(string label, GUIStyle style)
    {
      bool changed = StratusEditorUtility.CheckControlChange(() =>
      {
        selectedIndex = EditorGUILayout.Popup(label, selectedIndex, displayedOptions, style);
      });
      return changed && isValidIndex;
    }

    private void SetHint(bool set)
    {
      if (set)
      {
        displayedOptions = displayedOptions.AddFirst(selectionHint);
      }
      else
      {
        displayedOptions = displayedOptions.RemoveFirst();
      }
    }





  }

}