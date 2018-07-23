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
      Favorites,
      Inspector
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
      GameObject,
      Component,
      Type,
      Member,
      Value
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
    private int selectedModeIndex = 0;

    [SerializeField]
    private MemberInspectorTreeView memberInspector;

    [SerializeField]
    private TreeViewState treeViewState;

    private Countdown pollTimer;
    //private const float listRatio = 0.25f;
    //private GUILayoutOption listLeftElementWidth;
    //private GUILayoutOption listRightElementWidth;
    //private GUILayoutOption listElementHeight;
    private TreeView treeView;


    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/     
    public InformationMode informationMode { get; private set; }
    public GameObjectInformation currentTargetInformation { get; private set; }
    private string[] toolbarOptions = new string[] { nameof(Mode.Inspector), nameof(Mode.Favorites) };
    private SerializedProperty memberProperty { get; set; }
    private Type gameObjectType { get; set; }
    private bool hasTarget => this.target != null && this.currentTargetInformation != null;
    private int selectedIndex { get; set; }
    private AnimBool[] showComponent { get; set; }
    private Vector2 componentScrollPosition { get; set; }
    private Vector2 watchListScrollPosition { get; set; }
    private DropdownList<ComponentInformation> componentList { get; set; }

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

      GameObjectBookmark.onUpdate += this.onBookmarkUpdate;
      GameObjectBookmark.onFavoritesChanged += this.onBookmarkUpdate;
    }

    protected override void OnWindowGUI()
    {
      //return; 

      EditorGUI.BeginChangeCheck();
      {
        this.selectedModeIndex = GUILayout.Toolbar(this.selectedModeIndex, this.toolbarOptions);
      }
      if (EditorGUI.EndChangeCheck())
      {
        this.SetTreeView();
      }

      if (this.memberInspector == null)
        this.SetTreeView();

      switch (this.selectedModeIndex)
      {
        case 0:
          this.SelectTarget();
          if (this.hasTarget)
          {
            //EditorGUILayout.LabelField($"Components ({this.informationMode})", EditorStyles.centeredGreyMiniLabel);
            //this.componentList.selectedIndex = EditorGUILayout.Popup(this.componentList.selectedIndex, this.componentList.displayedOptions, StratusGUIStyles.popup);
            this.memberInspector.OnTreeViewGUI(this.availablePosition);
          }
          break;

        case 1:
          if (GameObjectBookmark.hasFavorites)
          {
            this.memberInspector.OnTreeViewGUI(this.availablePosition);
          }
          break;
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
          break;
      }

      GameObjectBookmark.UpdateAvailable();
    }

    protected override void OnUpdate()
    {
      // Check whether values need to be updated
      bool updateValues = pollTimer.Update(Time.deltaTime);
      if (pollTimer.isFinished)
      {
        switch (this.selectedModeIndex)
        {
          case 0:
            if (this.hasTarget)
            {
              componentList.selected.UpdateValues();
            }
            break;

          case 1:
            if (GameObjectBookmark.hasAvailableInformation)
            {
              foreach (var targetInfo in GameObjectBookmark.availableInformation)
                targetInfo.UpdateFavoritesValues();
            }
            break;
        }

        // Reset the poll timer
        pollTimer.Reset();
        this.Repaint();
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

    private void onBookmarkUpdate()
    {
      Trace.Script("Bookmarks changed!");
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
              this.targetTemporaryInformation = (GameObjectInformation)this.currentTargetInformation.CloneJSON();
              this.currentTargetInformation = this.targetTemporaryInformation;
              GameObjectBookmark.Remove(this.target);
              this.informationMode = InformationMode.Temporary;
            });
          }
          else
          {
            menu.AddItem(new GUIContent(bookmarkLabel), false, () =>
            {
              GameObjectBookmark bookmark = GameObjectBookmark.Add(this.target);
              bookmark.SetInformation(this.currentTargetInformation);
              this.currentTargetInformation = bookmark.information;
              this.informationMode = InformationMode.Bookmark;
            });
          }

          // 2. Clear Favorites
          menu.AddItem(new GUIContent("Clear Watch List"), false, () =>
          {
            this.currentTargetInformation.ClearWatchList();
          });

          //menu.AddItem(new GUIContent(bookmarkLabel), false, () =>
          //{
          //  if (hasBookmark)
          //  {
          //    this.targetTemporaryInformation = (GameObjectInformation)this.currentTargetInformation.CloneJSON();
          //    this.currentTargetInformation = this.targetTemporaryInformation;
          //    //this.targetTemporaryInformation = this.currentTargetInformation;
          //    GameObjectBookmark.Remove(this.target);
          //    this.informationMode = InformationMode.Temporary;
          //  }
          //  else
          //  {
          //    GameObjectBookmark bookmark = GameObjectBookmark.Add(this.target);
          //    bookmark.SetInformation(this.currentTargetInformation);
          //    this.currentTargetInformation = bookmark.information;
          //    this.informationMode = InformationMode.Bookmark;
          //  }
          //});
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
        else
        {
          this.informationMode = InformationMode.Temporary;
          this.targetTemporaryInformation = new GameObjectInformation(this.target);
          this.currentTargetInformation = this.targetTemporaryInformation;
        }

        this.lastComponentIndex = 0;
        //Trace.Script($"Setting target information for {this.target.name}");
      }

      this.showComponent = this.GenerateAnimBools(this.currentTargetInformation.numberofComponents, false);
      this.componentList = new DropdownList<ComponentInformation>(this.currentTargetInformation.components, (ComponentInformation component) => component.name, this.lastComponentIndex);
      this.SetTreeView();
    }

    //------------------------------------------------------------------------/
    // Methods: Draw
    //------------------------------------------------------------------------/
    private void SetTreeView()
    {
      IList<MemberInspectorTreeElement> members = null;
      switch (this.selectedModeIndex)
      {
        case 0:
          if (this.hasTarget)
            members = MemberInspectorTreeElement.GenerateInspectorTree(this.currentTargetInformation);
          else
            return;
          break;

        case 1:
          members = MemberInspectorTreeElement.GenerateFavoritesTree();
          break;
      }

      //this.favoritesTree = MemberInspectorTreeElement.GenerateFavoritesTree();
      if (this.memberInspector == null)
      {
        this.memberInspector = new MemberInspectorTreeView(this.treeViewState, members);
        Trace.Script("Created member inspector tree view");
      }
      else
      {
        Trace.Script($"Set tree view with ({members.Count}) members");
        this.memberInspector.SetTree(members);
      }
    }

    private void DrawInspector()
    {
      //listLeftElementWidth = GUILayout.Width(position.width * listRatio);
      //listRightElementWidth = GUILayout.Width(position.width * (1f - listRatio));
      //listElementHeight = GUILayout.MinHeight(20f);



      // Scroll list      
      //this.componentScrollPosition = GUILayout.BeginScrollView(this.componentScrollPosition, StratusGUIStyles.background);
      //{
      //  this.DrawComponent(this.componentList.selected);
      //}
      //GUILayout.EndScrollView();
    }


    //private void DrawComponent(ComponentInformation componentInfo)
    //{
    //  if (componentInfo.hasFields)
    //    DrawList("Fields", componentInfo, componentInfo.fields, ref componentInfo.fieldValues, ref componentInfo.favoriteFields);
    //  if (componentInfo.hasProperties)
    //    DrawList("Properties", componentInfo, componentInfo.properties, ref componentInfo.propertyValues, ref componentInfo.favoriteProperties);
    //}

    //private void DrawList(string label, ComponentInformation component, MemberInfo[] members, ref object[] values, ref bool[] favorites)
    //{
    //  int count = members.Length;
    //  EditorGUILayout.LabelField($"{label} ({count})", EditorStyles.whiteLargeLabel);
    //  for (int i = 0; i < count; ++i)
    //  {
    //    MemberInfo member = members[i];
    //    GUILayout.BeginHorizontal();
    //    {
    //      EditorGUI.BeginChangeCheck();
    //      {
    //        favorites[i] = GUILayout.Toggle(favorites[i], string.Empty, StratusGUIStyles.listViewToggle, listElementHeight);
    //      }
    //      if (EditorGUI.EndChangeCheck())
    //      {
    //        if (favorites[i])
    //          this.currentTargetInformation.Watch(member, component, i);
    //        else
    //          this.currentTargetInformation.RemoveWatch(member, component, i);
    //
    //        // Whenever faorites change
    //        GameObjectBookmark.UpdateFavoriteMembers();
    //        //this.OnFavoritesUpdated();
    //      }
    //
    //      GUILayout.Label(new GUIContent(member.Name, null, member.Name), StratusGUIStyles.listViewLabel, listLeftElementWidth, listElementHeight);
    //      EditorGUILayout.SelectableLabel($"{values[i]}", StratusGUIStyles.textField, listRightElementWidth, listElementHeight);
    //    }
    //    GUILayout.EndHorizontal();
    //  }
    //}



  }
}