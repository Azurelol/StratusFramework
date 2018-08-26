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
    public DropdownList<Type> subTypes { get; private set; }

    //public Type[] subTypes { get; private set; }
    public Type selectedClass => subTypes.selected;
    private string selectedClassName => selectedClass.Name;
    public int selectedIndex => subTypes.selectedIndex;
    public string[] displayedOptions => subTypes.displayedOptions;
    //public string[] displayedOptions { get; private set; }
    //public bool isValidIndex => currentIndex > 0;
    public System.Action onSelectionChanged { get; set; }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public TypeSelector(Type baseType, bool includeAbstract, bool sortAlphabetically = false)
    {
      this.baseType = baseType;      
      this.subTypes = new DropdownList<Type>(Reflection.GetSubclass(baseType), (Type type) => type.Name);
      //this.displayedOptions = subTypes.Names(;
      
      if (sortAlphabetically)
      {
        this.subTypes.Sort();
      }
    }

    public TypeSelector(Type baseType, Type interfaceType, bool sortAlphabetically = false)
    {
      this.baseType = baseType;
      this.subTypes = new DropdownList<Type>(Reflection.GetInterfaces(baseType, interfaceType), (Type type) => type.Name);

      if (sortAlphabetically)
      {
        this.subTypes.Sort();
      }
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public void ResetSelection(int index = 0)
    {
      if (selectedIndex == index)
        return;

      subTypes.selectedIndex = index;
      OnSelectionChanged();
    }

    public Type AtIndex(int index) => subTypes.AtIndex(index);

    protected virtual void OnSelectionChanged()
    {
      onSelectionChanged?.Invoke();
    }

    public virtual void DrawGUILayout(string label)
    {
      StratusEditorGUI.GUILayoutPopup(label, selectedIndex, displayedOptions, ResetSelection);
    }
    

    //public virtual bool DrawGUILayout(string label)
    //{
    //  bool changed = StratusEditorUtility.CheckControlChange(() =>
    //  {
    //    selectedIndex = StratusEditorGUI.GUILayoutPopup(label, selectedIndex, displayedOptions);
    //  });
    //
    //  if (changed)
    //    OnSelectionChanged();      
    //
    //  return changed;
    //}


    //
    //public bool GUILayoutPopup(GUIStyle style)
    //{
    //  bool changed = StratusEditorUtility.CheckControlChange(() =>
    //  {
    //    selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions, style);
    //  });
    //  return changed && isValidIndex;
    //}
    //
    //public bool GUILayoutPopup(GUIStyle style, params GUILayoutOption[] options)
    //{
    //  bool changed = StratusEditorUtility.CheckControlChange(() =>
    //  {
    //    selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions, style, options);
    //  });
    //  return changed && isValidIndex;
    //}
    //
    //public bool GUILayoutPopup(string label, GUIStyle style)
    //{
    //  bool changed = StratusEditorUtility.CheckControlChange(() =>
    //  {
    //    selectedIndex = EditorGUILayout.Popup(label, selectedIndex, displayedOptions, style);
    //  });
    //  return changed && isValidIndex;
    //}

  }

}