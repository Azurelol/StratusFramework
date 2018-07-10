using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Dependencies.Ludiq.Reflection;
using System;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor.IMGUI.Controls;
using UnityEditor.AnimatedValues;

namespace Stratus
{
  /// <summary>
  /// A window used for inspecting the members of an object at runtime
  /// </summary>
  public class MemberInspectorWindow : StratusEditorWindow<MemberInspectorWindow>
  {
    /// <summary>
    /// Information about a component
    /// </summary>
    public class ComponentInfo
    {
      public Component component;
      public Type type;
      public FieldInfo[] fields;
      public PropertyInfo[] properties;
      public object[] fieldValues, propertyValues;
      private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      public int fieldCount => fields.Length;
      public int propertyCount => properties.Length;
      public bool hasFields => fieldCount > 0;
      public bool hasProperties => propertyCount > 0;

      public ComponentInfo(Component component)
      {
        this.component = component;
        this.type = this.component.GetType();
        this.fields = this.type.GetFields(bindingFlags);
        this.fieldValues = new object[this.fields.Length];
        this.properties = this.type.GetProperties(bindingFlags);
        this.propertyValues = new object[this.properties.Length];
      }

      public void UpdateValues()
      {
        // Some properties may fail in editor or in play mode
        for (int f = 0; f < fields.Length; ++f)
        {
          try
          {
            this.fieldValues[f] = this.GetValue(this.fields[f]);
          }
          catch (Exception e)
          {
          }
        }

        for (int p = 0; p < properties.Length; ++p)
        {
          try
          {
            this.propertyValues[p] = this.GetValue(this.properties[p]);
          }
          catch (Exception e)
          {
          }
        }


      }

      public object GetValue(FieldInfo field) => field.GetValue(component);
      public object GetValue(PropertyInfo property) => property.GetValue(component);
    }

    /// <summary>
    /// Information about a gameobject and all its components
    /// </summary>
    public class GameObjectInfo
    {
      public GameObject target;
      public ComponentInfo[] componentInformation;
      public int numberofComponents { get; private set; }

      public GameObjectInfo(GameObject target)
      {
        // Set target
        this.target = target;

        // Set components
        List<ComponentInfo> components = new List<ComponentInfo>();
        foreach (var component in target.GetComponents<Component>())
        {
          components.Add(new ComponentInfo(component));
        }
        this.componentInformation = components.ToArray();
        this.numberofComponents = this.componentInformation.Length;
      }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/    
    public MemberField member;
    private TreeView treeView;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float pollSpeed = 0.5f;

    private Countdown pollTimer;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/    
    private SerializedProperty memberProperty { get; set; }
    private Type gameObjectType { get; set; }
    private bool hasTarget => target != null;
    private GameObjectInfo targetInformation { get; set; }
    private int selectedIndex { get; set; }
    private AnimBool[] showComponent { get; set; }
    private Vector2 scrollPosition { get; set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnWindowEnable()
    {
      this.member = new MemberField();
      this.pollTimer = new Countdown(this.pollSpeed);
      this.memberProperty = this.serializedObject.FindProperty(nameof(member));
      this.gameObjectType = typeof(GameObject);
      this.CheckTarget();
    }

    protected override void OnWindowGUI()
    {
      this.SelectTarget();

      if (this.hasTarget)
        this.InspectTarget();
    }

    protected override void OnPlayModeStateChange(PlayModeStateChange stateChange)
    {
      switch (stateChange)
      {
        case PlayModeStateChange.EnteredEditMode:
          break;
        case PlayModeStateChange.ExitingEditMode:
          break;
        case PlayModeStateChange.EnteredPlayMode:
          break;
        case PlayModeStateChange.ExitingPlayMode:
          //this.CheckTarget();
          break;
      }
    }

    protected override void OnUpdate()
    {
      if (this.hasTarget)
      {
        // Check whether values need to be updated
        bool updateValues = pollTimer.Update(Time.deltaTime);
        if (pollTimer.isFinished)
        {
          for (int c = 0; c < this.targetInformation.numberofComponents; ++c)
          {
            ComponentInfo componentInfo = this.targetInformation.componentInformation[c];
            // Update the values if the component is being shown
            bool show = this.showComponent[c].target;
            if (show && updateValues)
              componentInfo.UpdateValues();
          }
          pollTimer.Reset();
        }
      }
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Core/Member Inspector")]
    private static void Open() => OnOpen("Members");

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
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Target", StratusGUIStyles.header);
      bool changed = StratusEditorUtility.CheckControlChange(() =>
      {
        this.target = (GameObject)EditorGUILayout.ObjectField(this.target, this.gameObjectType, true);
      });

      if (changed)
        this.CheckTarget();
    }

    private void SelectTarget(GameObject target)
    {
      this.target = target;
      this.OnTargetSelected();
    }

    private void CheckTarget()
    {
      if (this.target)
        this.OnTargetSelected();
    }

    private void OnTargetSelected()
    {
      this.targetInformation = new GameObjectInfo(this.target);
      this.showComponent = this.GenerateAnimBools(this.targetInformation.numberofComponents, false);
      this.member.SetTarget(this.target);
    }

    private void InspectTarget()
    {


      EditorGUILayout.LabelField("Components", EditorStyles.centeredGreyMiniLabel);
      this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
      {
        // Show fields and proeprties for every component
        for (int c = 0; c < this.targetInformation.numberofComponents; ++c)
        {
          ComponentInfo componentInfo = this.targetInformation.componentInformation[c];
          StratusEditorUtility.DrawVerticalFadeGroup(this.showComponent[c], componentInfo.type.Name, () => this.DrawComponent(componentInfo), EditorStyles.helpBox, EditorBuildSettings.scenes.Length > 0);
        }
      }
      EditorGUILayout.EndScrollView();
    }

    private void DrawComponent(ComponentInfo componentInfo)
    {
      if (componentInfo.hasFields)
        DrawFields(componentInfo);
      if (componentInfo.hasProperties)
        DrawProperties(componentInfo);
    }

    private void DrawFields(ComponentInfo componentInfo)
    {
      EditorGUILayout.LabelField($"Fields ({componentInfo.fieldCount})", EditorStyles.whiteLargeLabel);
      for (int f = 0; f < componentInfo.fieldCount; ++f)
      {
        EditorGUILayout.LabelField($"- <b>{componentInfo.fields[f].Name}: </b> {componentInfo.fieldValues[f]}", StratusGUIStyles.skin.label);
      }
    }

    private void DrawProperties(ComponentInfo componentInfo)
    {
      EditorGUILayout.LabelField($"Properties ({componentInfo.propertyCount})", EditorStyles.whiteLargeLabel);
      for (int p = 0; p < componentInfo.propertyCount; ++p)
      {
        EditorGUILayout.LabelField($"- <b>{componentInfo.properties[p].Name}: </b>{componentInfo.propertyValues[p]}", StratusGUIStyles.skin.label);
      }
    }

  }
}