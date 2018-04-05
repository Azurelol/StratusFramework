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
    public Type baseType { get; private set; }
    public Type[] subTypes { get; private set; }
    public Type selectedClass => subTypes[currentIndex];
    private string selectedClassName => selectedClass.Name;
    public string[] displayedOptions { get; private set; }
    public bool showHint { get; private set; }
    public bool isValidIndex => showHint ? selectedIndex > 0 : true;
    public int currentIndex => showHint ? selectedIndex - 1 : selectedIndex;
    public string selectionHint { get; private set; }
    //{
    //  get { return selectionHintField; }
    //  set
    //  {
    //    selectionHintField = value;
    //    if (showHint)
    //      displayedOptions[0] = value;
    //  }
    // }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private int selectedIndex = 0;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public TypeSelector(Type baseType, bool sortAlphabetically = false)
    {
      this.baseType = baseType;
      this.subTypes = Reflection.GetSubclass(baseType);
      this.displayedOptions = subTypes.Names((Type type) => type.Name);
      
      if (sortAlphabetically)
      {
        Array.Sort(displayedOptions);
        Array.Sort(subTypes, (Type left, Type right) => { return left.Name.CompareTo(right.Name); });
      }
    }

    public TypeSelector(Type baseType, Type interfaceType, bool sortAlphabetically = false)
    {
      this.baseType = baseType;
      this.subTypes = Reflection.GetInterfaces(baseType, interfaceType);
      this.displayedOptions = subTypes.Names((Type type) => type.Name);

      if (sortAlphabetically)
      {
        Array.Sort(displayedOptions);
        Array.Sort(subTypes, (Type left, Type right) => { return left.Name.CompareTo(right.Name); });
      }
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

    private void SetHint(string hint)
    {
      showHint = true;
      selectionHint = hint;
      displayedOptions = displayedOptions.AddFront(selectionHint);
    }





  }

}