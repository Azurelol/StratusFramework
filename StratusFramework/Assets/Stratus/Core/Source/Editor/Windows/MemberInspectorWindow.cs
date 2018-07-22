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

    //[SerializeField]
    private IList<MemberInspectorTreeElement> inspectorTree, favoritesTree;

    private Countdown pollTimer;
    private const float listRatio = 0.25f;
    private GUILayoutOption listLeftElementWidth;
    private GUILayoutOption listRightElementWidth;
    private GUILayoutOption listElementHeight;
    private TreeView treeView;


    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/     
    public InformationMode informationMode { get; private set; }
    public GameObjectInformation currentTargetInformation { get; private set; }
    private string[] toolbarOptions = new string[] { nameof(Mode.Inspector), nameof(Mode.Favorites) };
    private SerializedProperty memberProperty { get; set; }
    private Type gameObjectType { get; set; }
    private bool hasTarget => this.target != null;
    private int selectedIndex { get; set; }
    private AnimBool[] showComponent { get; set; }
    private Vector2 componentScrollPosition { get; set; }
    private Vector2 watchListScrollPosition { get; set; }
    private DropdownList<ComponentInfo> componentList { get; set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnWindowEnable()
    {      
      this.pollTimer = new Countdown(this.pollSpeed);
      this.gameObjectType = typeof(GameObject);
      this.CheckTarget();
      GameObjectBookmark.onUpdate += this.onBookmarkUpdate;
      GameObjectBookmark.onFavoritesChanged += this.onBookmarkUpdate;
    }
    protected override void OnWindowGUI()
    {
      this.selectedModeIndex = GUILayout.Toolbar(this.selectedModeIndex, this.toolbarOptions);

      switch (this.selectedModeIndex)
      {
        case 0:
          this.SelectTarget();
          if (this.hasTarget)
            this.DrawInspector();
          break;

        case 1:
          if (GameObjectBookmark.hasFavorites)
            this.DrawFavorites();
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
              componentList.selected.UpdateValues();
            break;

          case 1:
            foreach (var targetInfo in GameObjectBookmark.availableInformation)
              targetInfo.UpdateFavorites();
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
      this.SetFavoritesInspector();
    }

    private void SetFavoritesInspector()
    {
      this.favoritesTree = this.GenerateFavoritesTree();
      if (this.memberInspector == null)
      {
        if (this.treeViewState == null)
          this.treeViewState = new TreeViewState();
        this.memberInspector = MemberInspectorTreeView.Create(this.favoritesTree, this.treeViewState);
      }
      this.memberInspector.SetTree(this.favoritesTree);
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Core/Member Inspector")]
    private static void Open() => OnOpen("Members");

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
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
          menu.AddItem(new GUIContent(bookmarkLabel), false, () =>
          {
            if (hasBookmark)
            {
              this.targetTemporaryInformation = (GameObjectInformation)this.currentTargetInformation.CloneJSON();
              this.currentTargetInformation = this.targetTemporaryInformation;
              //this.targetTemporaryInformation = this.currentTargetInformation;
              GameObjectBookmark.Remove(this.target);
              this.informationMode = InformationMode.Temporary;
            }
            else
            {
              GameObjectBookmark bookmark = GameObjectBookmark.Add(this.target);
              bookmark.SetInformation(this.currentTargetInformation);
              this.currentTargetInformation = bookmark.information;
              this.informationMode = InformationMode.Bookmark;
            }
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
      if (this.currentTargetInformation == null || this.currentTargetInformation.target != this.target)
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
      }

      this.showComponent = this.GenerateAnimBools(this.currentTargetInformation.numberofComponents, false);
      this.componentList = new DropdownList<ComponentInfo>(this.currentTargetInformation.components, (ComponentInfo component) => component.name, this.lastComponentIndex);
    }

    //------------------------------------------------------------------------/
    // Methods: Draw
    //------------------------------------------------------------------------/
    private void DrawInspector()
    {
      listLeftElementWidth = GUILayout.Width(position.width * listRatio);
      listRightElementWidth = GUILayout.Width(position.width * (1f - listRatio));
      listElementHeight = GUILayout.MinHeight(20f);

      EditorGUILayout.LabelField($"Components ({this.informationMode})", EditorStyles.centeredGreyMiniLabel);

      // Select component
      this.componentList.selectedIndex = EditorGUILayout.Popup(this.componentList.selectedIndex, this.componentList.displayedOptions, StratusGUIStyles.popup);

      // Scroll list      
      this.componentScrollPosition = GUILayout.BeginScrollView(this.componentScrollPosition, StratusGUIStyles.background);
      {
        this.DrawComponent(this.componentList.selected);
      }
      GUILayout.EndScrollView();
    }

    private void DrawFavorites()
    {
      if (this.memberInspector == null)
        this.SetFavoritesInspector();

      //Rect favoriteRect = this.position;
      //Rect lastRect = GUILayoutUtility.GetLastRect();
      //favoriteRect.y = lastRect.height;
      //this.memberInspector.OnTreeViewGUI(favoriteRect);
      this.memberInspector.OnTreeViewGUI(this.availablePosition);
      //GUILayout.EndArea();
      return;

      EditorGUILayout.LabelField(nameof(Mode.Favorites), EditorStyles.centeredGreyMiniLabel);
      this.watchListScrollPosition = GUILayout.BeginScrollView(this.watchListScrollPosition, StratusGUIStyles.background);
      {
        const float ratio = 0.4f;
        GUILayoutOption leftElementWidth = GUILayout.Width(position.width * ratio);
        GUILayoutOption rightElementWidth = GUILayout.Width(position.width * (1f - ratio));
        GUILayoutOption elementHeight = GUILayout.MinHeight(12f);

        foreach (var targetInformation in GameObjectBookmark.availableInformation)
        {
          foreach (var member in targetInformation.favorites)
          {
            GUILayout.BeginHorizontal();
            {
              GUILayout.Label(new GUIContent($"{targetInformation.target.name}.{member.componentName}.{member.name}"), StratusGUIStyles.listViewLabel, leftElementWidth, elementHeight);
              EditorGUILayout.SelectableLabel(member.latestValueString, StratusGUIStyles.textField, rightElementWidth, elementHeight);
            }
            GUILayout.EndHorizontal();
          }
        }

      }
      GUILayout.EndScrollView();

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
        GUILayout.BeginHorizontal();
        {
          EditorGUI.BeginChangeCheck();
          {
            favorites[i] = GUILayout.Toggle(favorites[i], string.Empty, StratusGUIStyles.listViewToggle, listElementHeight);
          }
          if (EditorGUI.EndChangeCheck())
          {
            if (favorites[i])
              this.currentTargetInformation.Watch(member, component, i);
            else
              this.currentTargetInformation.RemoveWatch(member, component, i);

            // Whenever faorites change
            GameObjectBookmark.UpdateFavoriteMembers();
            //this.OnFavoritesUpdated();
          }

          GUILayout.Label(new GUIContent(member.Name, null, member.Name), StratusGUIStyles.listViewLabel, listLeftElementWidth, listElementHeight);
          EditorGUILayout.SelectableLabel($"{values[i]}", StratusGUIStyles.textField, listRightElementWidth, listElementHeight);
        }
        GUILayout.EndHorizontal();
      }
    }

    private void DrawGrid()
    {
      int numRows = 2;
      string[] content = new string[this.targetTemporaryInformation.numberofComponents * numRows];
      for (int c = 0; c < this.targetTemporaryInformation.numberofComponents; ++c)
      {
        ComponentInfo component = this.targetTemporaryInformation.components[c];
        content[(c * numRows) + 0] = component.type.Name;
        content[(c * numRows) + 1] = "Boo";
      }
      this.selectedIndex = GUILayout.SelectionGrid(this.selectedIndex, content, numRows, EditorStyles.toolbarButton);
    }

    //------------------------------------------------------------------------/
    // Methods: TreeView
    //------------------------------------------------------------------------/
    private List<MemberInspectorTreeElement> GenerateFavoritesTree()
    {
      var elements = MemberInspectorTreeElement.GenerateFlatTree<MemberInspectorTreeElement, GameObjectInformation.MemberReference>(this.SetTreeElement, GameObjectBookmark.favorites);
      Trace.Script($"Generated favorites tree view ({GameObjectBookmark.favorites.Length})");
      return elements;
    }

    private void SetTreeElement(MemberInspectorTreeElement treeElement, GameObjectInformation.MemberReference member) => treeElement.Set(member);





  }
}