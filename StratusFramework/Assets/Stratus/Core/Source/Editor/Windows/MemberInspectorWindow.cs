using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Dependencies.Ludiq.Reflection;
using System;

namespace Stratus
{
  /// <summary>
  /// A window used for inspecting the members of an object at runtime
  /// </summary>
  public class MemberInspectorWindow : StratusEditorWindow<MemberInspectorWindow>
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/    
    public MemberField member;
    public int booly = 5;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public GameObject target { get; private set; }
    private SerializedProperty memberProperty { get; set; }
    private Type gameObjectType { get; set; }
    private bool hasTarget => member.isAssigned;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnWindowEnable()
    {
      this.member = new MemberField(); 
      this.memberProperty = this.serializedObject.FindProperty(nameof(member));
      this.gameObjectType = typeof(GameObject);
    }

    protected override StratusMenuBarDrawer OnSetMenuBar()
    {
      StratusMenuBarDrawer menuBar = new StratusMenuBarDrawer(Coordinates.Orientation.Vertical);
      menuBar.AddItem("Boo", this.DrawTarget, StratusEditorUtility.ContextMenuType.Options);
      return menuBar;
    }

    protected override void OnWindowGUI()
    {
      this.SelectTarget();
      if (this.hasTarget)
        this.DrawTarget();

      var editor = UnityEditor.Editor.CreateEditor(this);
      editor.DrawDefaultInspector();
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Core/Member Inspector")]
    private static void Open() => OnOpen("Member Inspector");

    /// <summary>
    /// Inspects the gameobkect, opening the window
    /// </summary>
    /// <param name="target"></param>
    public static void Inspect(GameObject target)
    {
      Open();
      instance.SelectTarget(target);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void SelectTarget()
    {
      bool changed = StratusEditorUtility.CheckControlChange(() => this.target = (GameObject)EditorGUILayout.ObjectField(this.target, this.gameObjectType, true));
      if (changed)
        this.OnTargetSelected();
    }

    private void SelectTarget(GameObject target)
    {
      Trace.Script($"Now inspecting {target}");
      this.target = target;
      this.OnTargetSelected();
    }

    private void OnTargetSelected()
    {
      this.member.SetTarget(this.target);
    }

    private void DrawTarget()
    {
      //EditorGUILayout.ObjectField(this.member, typeof(MemberField))
    }

  }
}