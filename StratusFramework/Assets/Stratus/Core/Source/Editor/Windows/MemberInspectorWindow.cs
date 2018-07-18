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
  public class MemberInspectorWindow : StratusEditorWindow<MemberInspectorWindow>, ISerializationCallbackReceiver
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/ 
    public enum Mode
    {
      Favorites,
      Inspector
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/    
    public MemberField member;
    private TreeView treeView;

    [SerializeField]
    private Mode mode = Mode.Inspector;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private GameObjectInfo targetInformation;

    [SerializeField]
    private float pollSpeed = 1f;

    [SerializeField]
    private int lastComponentIndex = 0;

    [SerializeField]
    private int selectedModeIndex = 0;

    private Countdown pollTimer;
    private const float listRatio = 0.25f;
    private GUILayoutOption listLeftElementWidth; 
    private GUILayoutOption listRightElementWidth;
    private GUILayoutOption listElementHeight;
    private string[] toolbarOptions = new string[] { nameof(Mode.Inspector), nameof(Mode.Favorites) };

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/    
    private SerializedProperty memberProperty { get; set; }
    private Type gameObjectType { get; set; }
    private bool hasTarget => this.targetInformation != null ;

    private int selectedIndex { get; set; }
    private AnimBool[] showComponent { get; set; }
    private Vector2 componentScrollPosition { get; set; }
    private Vector2 watchListScrollPosition { get; set; }
    private DropdownList<ComponentInfo> componentList { get; set; }
    private List<GameObjectInfo.MemberReference> watchedMembers { get; set; } = new List<GameObjectInfo.MemberReference>();

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
      this.mode = (Mode)GUILayout.Toolbar((int)this.mode, this.toolbarOptions);

      if (this.hasTarget)
      {
        listLeftElementWidth = GUILayout.Width(position.width * listRatio);
        listRightElementWidth = GUILayout.Width(position.width * (1f - listRatio));
        listElementHeight = GUILayout.MinHeight(20f);

        switch (this.mode)
        {
          case Mode.Favorites:
            this.DrawFavorites();
            break;

          case Mode.Inspector:
            this.InspectTarget();
            break;
        }

        //this.DrawGrid();
      }
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
          switch (this.mode)
          {
            case Mode.Favorites:
              this.targetInformation.UpdateFavorites();
              break;

            case Mode.Inspector:
              componentList.selected.UpdateValues();
              break;
          }


          //for (int c = 0; c < this.targetInformation.numberofComponents; ++c)
          //{
          //  ComponentInfo componentInfo = this.targetInformation.components[c];
          //  // Update the values if the component is being shown
          //  bool show = this.showComponent[c].target;
          //  if (show && updateValues)
          //    componentInfo.UpdateValues();
          //}

          // Update display content
          //this.targetInformation.UpdateDisplayContent();

          // Reset the poll timer
          pollTimer.Reset();
          this.Repaint();
        }
      }
    }

    public void OnBeforeSerialize()
    {
      if (this.componentList != null)
        this.lastComponentIndex = this.componentList.selectedIndex;
    }

    public void OnAfterDeserialize()
    {

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
    // Methods: Target Selection
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
      {
        if (this.target)
        {
          this.targetInformation = new GameObjectInfo(this.target);
          this.OnTargetSelected();
        }
        else
        {
          this.targetInformation = null;
        }
      }
      
    }

    private void SelectTarget(GameObject target)
    {
      this.target = target;
      this.targetInformation = new GameObjectInfo(this.target);
      this.OnTargetSelected();
    }

    private void CheckTarget()
    {
      if (this.target)
      {
        this.OnTargetSelected();
      }
      else
      {
        this.targetInformation = null;
        this.lastComponentIndex = 0;
      }
    }

    private void OnTargetSelected()
    {
      //if (this.targetInformation == null || !this.targetInformation.isValid)
      //{
      //  this.targetInformation = new GameObjectInfo(this.target);
      //}
      //else
      //{
      //}
      this.showComponent = this.GenerateAnimBools(this.targetInformation.numberofComponents, false);
      this.componentList = new DropdownList<ComponentInfo>(this.targetInformation.components, (ComponentInfo component) => component.name, this.lastComponentIndex);
      this.member.SetTarget(this.target);
    }

    //------------------------------------------------------------------------/
    // Methods: Draw Target
    //------------------------------------------------------------------------/
    private void InspectTarget()
    {
      EditorGUILayout.LabelField("Components", EditorStyles.centeredGreyMiniLabel);
      this.componentList.selectedIndex = EditorGUILayout.Popup(this.componentList.selectedIndex, this.componentList.displayedOptions, StratusGUIStyles.popup);

      //for (int c = 0; c < this.targetInformation.numberofComponents; ++c)
      //{
      //  ComponentInfo componentInfo = this.targetInformation.components[c];
      //  EditorGUILayout.
      //}

      this.componentScrollPosition = EditorGUILayout.BeginScrollView(this.componentScrollPosition, StratusGUIStyles.background);
      {
        this.DrawComponent(this.componentList.selected);
        //ComponentInfo componentInfo = this.componentList.selected;

        // Show fields and proeprties for every component
        //for (int c = 0; c < this.targetInformation.numberofComponents; ++c)
        //{
        //  ComponentInfo componentInfo = this.targetInformation.components[c];
        //  StratusEditorUtility.DrawVerticalFadeGroup(this.showComponent[c], componentInfo.type.Name, () => this.DrawComponent(componentInfo), EditorStyles.helpBox, EditorBuildSettings.scenes.Length > 0);
        //}
      }
      EditorGUILayout.EndScrollView();
    }

    private void DrawComponent(ComponentInfo componentInfo)
    {
      if (componentInfo.hasFields)
        DrawList("Fields", componentInfo, componentInfo.fields, ref componentInfo.fieldValues, ref componentInfo.favoriteFields);
      if (componentInfo.hasProperties)
        DrawList("Properties", componentInfo, componentInfo.properties, ref componentInfo.propertyValues, ref componentInfo.favoriteProperties);
    }

    private void DrawList(string label, ComponentInfo component, MemberInfo[] members, ref object[] values, ref bool[] favorites)
    {
      int count = members.Length;
      EditorGUILayout.LabelField($"{label} ({count})", EditorStyles.whiteLargeLabel);
      for (int i = 0; i < count; ++i)
      {
        MemberInfo member = members[i];
        EditorGUILayout.BeginHorizontal();
        {
          EditorGUI.BeginChangeCheck();
          {
            favorites[i] = GUILayout.Toggle(favorites[i], string.Empty, StratusGUIStyles.listViewToggle, listElementHeight);
          }
          if (EditorGUI.EndChangeCheck())
          {
            if (favorites[i])
              this.targetInformation.Watch(member, component, i);
              //component.Watch(member);
            else
              this.targetInformation.RemoveWatch(member, component);
          }

          GUILayout.Label(new GUIContent(member.Name, null, member.Name), StratusGUIStyles.listViewLabel, listLeftElementWidth, listElementHeight);
          EditorGUILayout.SelectableLabel($"{values[i]}", StratusGUIStyles.listViewTextField, listRightElementWidth, listElementHeight);
          //{
          //  StratusEditorUtility.OnLastControlMouseClick(null, () =>
          //    {
          //      GenericMenu menu = new GenericMenu();
          //      menu.AddItem(new GUIContent("Watch"), false, () => this.targetInformation.Watch(member, component));
          //      menu.ShowAsContext();
          //    }, null);
          //}
        }
        EditorGUILayout.EndHorizontal();
      }
    }

    private void DrawGrid()
    {
      int numRows = 2;
      string[] content = new string[this.targetInformation.numberofComponents * numRows];
      for (int c = 0; c < this.targetInformation.numberofComponents; ++c)
      {
        ComponentInfo component = this.targetInformation.components[c];
        content[(c * numRows) + 0] = component.type.Name;
        content[(c * numRows) + 1] = "Boo";
      }
      this.selectedIndex = GUILayout.SelectionGrid(this.selectedIndex, content, numRows, EditorStyles.toolbarButton);
    }

    private void DrawFavorites()
    {
      EditorGUILayout.LabelField("Favorites", EditorStyles.centeredGreyMiniLabel);
      this.watchListScrollPosition = EditorGUILayout.BeginScrollView(this.watchListScrollPosition, StratusGUIStyles.background);
      {
        const float ratio = 0.4f;
        GUILayoutOption leftElementWidth = GUILayout.Width(position.width * ratio);
        GUILayoutOption rightElementWidth = GUILayout.Width(position.width * (1f - ratio));
        GUILayoutOption elementHeight = GUILayout.MinHeight(12f);

        foreach (var member in this.targetInformation.favorites)
        {
          EditorGUILayout.BeginHorizontal();
          {
            GUILayout.Label(new GUIContent($"{member.componentName}.{member.name}"), StratusGUIStyles.listViewLabel, leftElementWidth, elementHeight);
            EditorGUILayout.SelectableLabel(member.latestValueString, StratusGUIStyles.listViewTextField, rightElementWidth, elementHeight);
          }
          EditorGUILayout.EndHorizontal();
        }

      }
      EditorGUILayout.EndScrollView();

      //this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
      //{
      //  // Show fields and proeprties for every component
      //  foreach (var member in this.targetInformation.watchList)
      //  {
      //    EditorGUILayout.LabelField($"{member.componentName}.{member.name}");
      //  }
      //}
      //EditorGUILayout.EndScrollView();


    }



    


  }
}