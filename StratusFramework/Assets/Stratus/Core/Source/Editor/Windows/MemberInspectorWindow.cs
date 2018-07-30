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
using UnityEditor.Callbacks;

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
    /// <summary>
    /// The current mode for this window
    /// </summary>
    public enum Mode
    {
      Inspector = 0,
      WatchList = 1,
    }

    /// <summary>
    /// How the current information is being stored
    /// </summary>
    public enum InformationMode
    {
      Temporary,
      Bookmark
    }

    public enum Column
    {
      Favorite,
      GameObject,
      Component,
      Member,
      Value,
      Type,
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/   
    [SerializeField]
    private Mode mode = Mode.Inspector;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private GameObjectInformation targetTemporaryInformation;

    [SerializeField]
    private float pollSpeed = 1f;

    [SerializeField]
    private int lastComponentIndex = 0;

    [SerializeField]
    private MemberInspectorTreeView memberInspector;

    [SerializeField]
    private TreeViewState treeViewState;

    private Countdown pollTimer;
    private TreeView treeView;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/     
    public InformationMode informationMode { get; private set; }
    public GameObjectInformation currentTargetInformation { get; private set; }
    private string[] toolbarOptions = new string[]
    {
      nameof(Mode.Inspector),
      nameof(Mode.WatchList),
    };
    private SerializedProperty memberProperty { get; set; }
    private Type gameObjectType { get; set; }
    private bool hasTarget => this.target != null && this.currentTargetInformation != null;
    private int selectedIndex { get; set; }
    private AnimBool[] showComponent { get; set; }
    private Vector2 componentScrollPosition { get; set; }
    private Vector2 watchListScrollPosition { get; set; }
    private DropdownList<ComponentInformation> componentList { get; set; }
    private bool updateTreeView { get; set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnWindowEnable()
    {
      if (this.treeViewState == null)
        this.treeViewState = new TreeViewState();

      this.pollTimer = new Countdown(this.pollSpeed);
      this.gameObjectType = typeof(GameObject);

      this.CheckTarget();

      // Update tree view on assembly reload
      this.updateTreeView = true;
      GameObjectBookmark.onUpdate += this.OnBookmarkUpdate;
      GameObjectInformation.onChanged += this.OnGameObjectInformationChanged;
    }

    protected override void OnWindowGUI()
    {
      StratusEditorUtility.DrawCentered(this.DrawControls);

      switch (this.mode)
      {
        case Mode.Inspector:
          this.SelectTarget();
          if (this.hasTarget)
          {
            EditorGUILayout.LabelField($"({this.informationMode})", EditorStyles.centeredGreyMiniLabel);
            this.memberInspector.OnTreeViewGUI(this.currentPosition);
          }
          break;

        case Mode.WatchList:
          if (GameObjectBookmark.hasWatchList)
          {
            this.memberInspector.OnTreeViewGUI(this.currentPosition);
          }
          break;
      }

    }
    protected override void OnWindowUpdate()
    {
      // Check whether values need to be updated
      bool updateValues = pollTimer.Update(Time.deltaTime);
      if (pollTimer.isFinished)
      {        
        switch (this.mode)
        {
          case Mode.Inspector:
            if (this.hasTarget)
            {
              this.currentTargetInformation.Refresh();
              this.currentTargetInformation.UpdateWatchValues();
            }
            break;

          case Mode.WatchList:
            if (GameObjectBookmark.hasAvailableInformation)
            {
              foreach (var targetInfo in GameObjectBookmark.availableInformation)
              {
                targetInfo.Refresh();
                targetInfo.UpdateWatchValues();
              }
            }
            break;
        }

        // Reset the poll timer
        pollTimer.Reset();
        this.Repaint();
      }
    }

    protected override void OnPlayModeStateChange(PlayModeStateChange stateChange)
    {
      switch (stateChange)
      {
        // Update the tree view only when entering 
        case PlayModeStateChange.EnteredPlayMode:
        case PlayModeStateChange.EnteredEditMode:
          updateTreeView = true;
          if (this.target)
            this.OnTargetSelected();
          break;

        // Don't bother trying to update while exiting
        case PlayModeStateChange.ExitingEditMode:
        case PlayModeStateChange.ExitingPlayMode:
          updateTreeView = false;
          break;
      }
    }

    //protected override StratusMenuBarDrawer OnSetMenuBar()
    //{
    //  StratusMenuBarDrawer menuBar = new StratusMenuBarDrawer(Coordinates.Orientation.Horizontal, GUILayout.ExpandWidth(false));
    //  menuBar.AddItem("Refresh", this.Refresh, "Options");
    //  menuBar.AddItem("Refresh", this.Refresh, "Options 2");
    //  return menuBar;
    //}


    public void OnBeforeSerialize()
    {
      if (this.componentList != null)
        this.lastComponentIndex = this.componentList.selectedIndex;
    }

    public void OnAfterDeserialize()
    {
    }

    private void OnBookmarkUpdate()
    {
      if (updateTreeView)
        this.SetTreeView();
    }

    private void OnGameObjectInformationChanged(GameObjectInformation information, GameObjectInformation.Change change)
    {
      Trace.Script($"Information for {information.target.name}, change = {change}");

      if (change == GameObjectInformation.Change.ComponentsAndWatchList)
        GameObjectBookmark.UpdateWatchList();

      if (updateTreeView)
        this.SetTreeView();
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Core/Member Inspector")]
    private static void Open() => OnOpen("Members");

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
      if (instance == null || instance.memberInspector == null)
        return false;
      return instance.memberInspector.TryOpenAsset(instanceID, line);
    }

    /// <param name="target"></param>
    public static void Inspect(GameObject target)
    {
      Open();
      instance.SelectTarget(target);
    }

    public static void SetBookmark(GameObject target)
    {
      GameObjectBookmark bookmark = GameObjectBookmark.Add(target);
      bookmark.SetInformation(instance.currentTargetInformation);
      instance.currentTargetInformation = bookmark.information;
      instance.informationMode = InformationMode.Bookmark;
    }

    public static void RemoveBookmark(GameObject target)
    {
      instance.targetTemporaryInformation = (GameObjectInformation)instance.currentTargetInformation.CloneJSON();
      instance.targetTemporaryInformation.ClearWatchList();
      instance.currentTargetInformation = instance.targetTemporaryInformation;
      GameObjectBookmark.Remove(target);
      instance.informationMode = InformationMode.Temporary;
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
        StratusEditorUtility.OnLastControlMouseClick(null, () =>
        {
          bool hasBookmark = this.target.HasComponent<GameObjectBookmark>();
          string bookmarkLabel = hasBookmark ? "Remove Bookmark" : "Bookmark";
          GenericMenu menu = new GenericMenu();

          // 1. Bookmark
          if (hasBookmark)
          {
            menu.AddItem(new GUIContent(bookmarkLabel), false, () =>
            {
              RemoveBookmark(target);
            });
          }
          else
          {
            menu.AddItem(new GUIContent(bookmarkLabel), false, () =>
            {
              SetBookmark(target);
            });
          }

          // 2. Clear Favorites
          menu.AddItem(new GUIContent("Clear Watch List"), false, () =>
          {
            this.currentTargetInformation.ClearWatchList();
          });

          menu.ShowAsContext();
        });
      });

      if (changed)
      {
        if (this.target)
        {
          this.OnTargetSelected();
        }
        else
        {
          this.currentTargetInformation = null;

          if (this.informationMode == InformationMode.Temporary)
            this.targetTemporaryInformation = null;
        }
      }
    }

    private void SelectTarget(GameObject target)
    {
      this.target = target;
      this.selectedIndex = 0;
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
        this.currentTargetInformation = this.targetTemporaryInformation = null;
        this.lastComponentIndex = 0;
      }
    }

    private void OnTargetSelected()
    {
      // If there's no target information or if the target is different from the previous
      //if (this.currentTargetInformation == null || this.currentTargetInformation.target != this.target)
      {
        // If the target has as bookmark, use that information instead
        GameObjectBookmark bookmark = this.target.GetComponent<GameObjectBookmark>();
        if (bookmark != null)
        {
          this.informationMode = InformationMode.Bookmark;
          this.currentTargetInformation = bookmark.information;
        }
        // Otherwise recreate the current target information
        else if (this.currentTargetInformation == null || this.currentTargetInformation.target != this.target)
        {
          this.informationMode = InformationMode.Temporary;
          this.targetTemporaryInformation = new GameObjectInformation(this.target);
          this.currentTargetInformation = this.targetTemporaryInformation;
        }

        this.lastComponentIndex = 0;
        //Trace.Script($"Setting target information for {this.target.name}");
      }

      //this.showComponent = this.GenerateAnimBools(this.currentTargetInformation.numberofComponents, false);
      //this.componentList = new DropdownList<ComponentInformation>(this.currentTargetInformation.components, (ComponentInformation component) => component.name, this.lastComponentIndex);
      this.SetTreeView();
    }

    //------------------------------------------------------------------------/
    // Methods: Data
    //------------------------------------------------------------------------/
    private void Refresh()
    {
      this.currentTargetInformation?.Refresh();
      foreach (var targetInformation in GameObjectBookmark.availableInformation)
      {
        targetInformation.Refresh();
      }
    }

    //------------------------------------------------------------------------/
    // Methods: Draw
    //------------------------------------------------------------------------/
    private void DrawControls()
    {
      // Toolbar      
      EditorGUI.BeginChangeCheck();
      {
        int index = (int)this.mode;
        this.mode = (Mode)GUILayout.Toolbar((int)this.mode, this.toolbarOptions, GUILayout.ExpandWidth(false));
      }
      if (EditorGUI.EndChangeCheck() || this.memberInspector == null)
      {
        this.SetTreeView();
      }
    }

    private void SetTreeView()
    {
      IList<MemberInspectorTreeElement> members = null;
      switch (this.mode)
      {
        case Mode.WatchList:
          members = MemberInspectorTreeElement.GenerateFavoritesTree();
          break;

        case Mode.Inspector:
          if (this.hasTarget)
            members = MemberInspectorTreeElement.GenerateInspectorTree(this.currentTargetInformation);
          else
            return;
          break;
      }
      //this.favoritesTree = MemberInspectorTreeElement.GenerateFavoritesTree();
      if (this.memberInspector == null)
      {
        this.memberInspector = new MemberInspectorTreeView(this.treeViewState, members);
        //Trace.Script("Created member inspector tree view");
      }
      else
      {
        Trace.Script($"Set tree view with ({members.Count - 1}) members");
        this.memberInspector.SetTree(members);
      }

      switch (this.mode)
      {
        case Mode.WatchList:
          this.memberInspector.EnableColumn(Column.GameObject);
          this.memberInspector.DisableColumn(Column.Favorite);
          break;

        case Mode.Inspector:
          this.memberInspector.DisableColumn(Column.GameObject);
          this.memberInspector.EnableColumn(Column.Favorite);
          break;
      }

    }

  }
}