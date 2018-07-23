using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using UnityEditor.AnimatedValues;

namespace Stratus
{
  public abstract class StratusEditorWindow : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    protected const string rootMenu = "Stratus/";
    protected SerializedObject serializedObject;
  }

  public abstract class StratusEditorWindow<T> : StratusEditorWindow where T : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/    

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    protected static T instance { get; private set; }
    protected StratusMenuBarDrawer menuBarDrawer { get; set; }

    /// <summary>
    /// Computes the current avaialble position within the window, after taking into account
    /// the height consumed by the latest control
    /// </summary>
    protected Rect availablePosition
    {
      get
      {
        Rect available = this.position;
        Rect lastRect = GUILayoutUtility.GetLastRect();
        available.y = lastRect.height + lastRect.y;
        available.height -= lastRect.y;
        return available;
      }
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected abstract void OnWindowEnable();
    protected abstract void OnWindowGUI();
    protected virtual StratusMenuBarDrawer OnSetMenuBar() => null;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnEnable()
    {
      this.serializedObject = new SerializedObject(this);
      this.menuBarDrawer = this.OnSetMenuBar();
      this.OnWindowEnable();
      EditorApplication.playModeStateChanged += this.OnPlayModeStateChange;
    }

    private void OnGUI()
    {
      this.menuBarDrawer?.Draw();
      this.OnWindowGUI();
    }

    private void Update()
    {
      this.OnUpdate();
    }

    protected virtual void OnPlayModeStateChange(PlayModeStateChange stateChange)
    {
    }

    protected virtual void OnUpdate()
    {
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/    
    protected static void OnOpen(string title = null, bool utlity = false)
    {
      Type type = typeof(T);
      title = title != null ? title : type.Name;
      instance = (T)EditorWindow.GetWindow(type, utlity, title);
    }

    //------------------------------------------------------------------------/
    // Methods: Drawing
    //------------------------------------------------------------------------/
    private void DrawMultiColumns()
    {

    }

    //------------------------------------------------------------------------/
    // Methods: Setup
    //------------------------------------------------------------------------/
    protected AnimBool[] GenerateAnimBools(int count, bool value)
    {
      List<AnimBool> bools = new List<AnimBool>();      
      for(int i = 0; i < count; ++i)
      {
        AnimBool animBool = new AnimBool(value);
        animBool.valueChanged.AddListener(this.Repaint);
        bools.Add(animBool);
      }
      return bools.ToArray();
    }


  }

}