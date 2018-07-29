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
    /// <summary>
    /// The active instance for this editor window
    /// </summary>
    protected static T instance { get; private set; }

    protected StratusMenuBarDrawer menuBarDrawer { get; set; }

    /// <summary>
    /// The rect used by this window in GUI space, where top left is at position(0,0)
    /// </summary>
    protected Rect guiPosition => GUIUtility.ScreenToGUIRect(this.position);

    /// <summary>
    /// Computes the current avaialble position within the window, after taking into account
    /// the height consumed by the latest control
    /// </summary>
    protected Rect currentPosition
    {
      get
      {        
        Rect lastRect = GUILayoutUtility.GetLastRect();
        Rect available = guiPosition;
        available.x += lastRect.x;
        available.y += lastRect.height + lastRect.y;
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
      instance = this as T;
      this.serializedObject = new SerializedObject(this);
      this.menuBarDrawer = this.OnSetMenuBar();
      this.OnWindowEnable();
      EditorApplication.playModeStateChanged += this.OnPlayModeStateChange;
    }

    private void OnGUI()
    {
      //EditorGUILayout.BeginVertical();
      this.menuBarDrawer?.Draw();
      this.OnWindowGUI();
      //EditorGUILayout.EndVertical();
    }

    private void Update()
    {
      this.OnWindowUpdate();
    }

    protected virtual void OnPlayModeStateChange(PlayModeStateChange stateChange)
    {
    }

    protected virtual void OnWindowUpdate()
    {
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/    
    protected static void OnOpen(string title = null, bool utility = false)
    {
      Type type = typeof(T);
      title = title != null ? title : type.Name;
      EditorWindow.GetWindow(type, utility, title);
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