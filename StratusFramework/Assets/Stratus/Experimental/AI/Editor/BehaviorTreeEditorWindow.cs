using UnityEngine;
using Stratus;
using UnityEditor;
using Stratus.Editor;
using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Callbacks;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    public class BehaviorTreeEditorWindow : StratusEditorWindow<BehaviorTreeEditorWindow>
    {
      //----------------------------------------------------------------------/
      // Declarations
      //----------------------------------------------------------------------/
      public enum Column
      {
        Hierarchy,
        Inspector
      }

      public class BehaviourTreeView : HierarchicalTreeView<BehaviorTree.BehaviorNode>
      {
        private BehaviorTreeEditorWindow window => BehaviorTreeEditorWindow.instance;
        private BehaviorTree tree => BehaviorTreeEditorWindow.instance.behaviorTree;


        public BehaviourTreeView(TreeViewState state, IList<BehaviorTree.BehaviorNode> data) : base(state, data)
        {
        }

        protected override void OnItemContextMenu(GenericMenu menu, BehaviorTree.BehaviorNode treeElement)
        {
          // Actions
          menu.AddPopup("Add/Actions", BehaviorTreeEditorWindow.actionTypes.displayedOptions, (int index) =>
          {
            window.AddNode(BehaviorTreeEditorWindow.actionTypes.AtIndex(index), treeElement);
          });

          // Composite
          menu.AddPopup("Add/Composites", BehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
          {
            window.AddNode(BehaviorTreeEditorWindow.compositeTypes.AtIndex(index), treeElement);
          });

          // Service
          if (treeElement.data is Composite)
          {
            menu.AddPopup("Add/Service", BehaviorTreeEditorWindow.serviceTypes.displayedOptions, (int index) =>
            {
              window.AddNode(BehaviorTreeEditorWindow.serviceTypes.AtIndex(index), treeElement);
            });
          }

          menu.AddItem("Remove", false, () => window.RemoveNode(treeElement));
        }

        protected override void OnContextMenu(GenericMenu menu)
        {
          // Composite
          menu.AddPopup("Add/Composites", BehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
          {
            window.AddNode(BehaviorTreeEditorWindow.compositeTypes.AtIndex(index));
          });

          // Actions
          menu.AddPopup("Add/Actions", BehaviorTreeEditorWindow.actionTypes.displayedOptions, (int index) =>
          {
            window.AddNode(BehaviorTreeEditorWindow.actionTypes.AtIndex(index));
          });

          menu.AddItem("Clear", false, () => window.RemoveAllNodes());
        }
      }

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      [SerializeField]
      private BehaviourTreeView treeInspector;
      [SerializeField]
      private TreeViewState treeViewState;

      private SerializedSystemObject currentNodeSerializedObject;
      const string folder = "Stratus/Experimental/AI/";
      private Vector2 inspectorScrollPosition, blackboardScrollPosition;
      private SerializedPropertyMap behaviorTreeProperties;
      //private SerializedProperty treeProperty, treeElementsProperty, currentNodeProperty;
      private StratusEditor blackboardEditor;

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// All supported behavior types
      /// </summary>
      public static TypeSelector behaviorTypes { get; } = TypeSelector.FilteredSelector(typeof(Behavior), typeof(Decorator), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector compositeTypes { get; } = new TypeSelector(typeof(Composite), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector actionTypes { get; } = new TypeSelector(typeof(Action), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector decoratorTypes { get; } = new TypeSelector(typeof(Decorator), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector serviceTypes { get; } = new TypeSelector(typeof(Service), false, true);

      /// <summary>
      /// The behavior tree currently being edited
      /// </summary>
      public BehaviorTree behaviorTree { get; private set; }

      /// <summary>
      /// The blackboard being used by the tree
      /// </summary>
      private Blackboard blackboard => behaviorTree.blackboard;

      /// <summary>
      /// The scope of the blackboard being inspected
      /// </summary>
      private Blackboard.Scope scope;

      /// <summary>
      /// The nodes currently being inspected
      /// </summary>
      public IList<BehaviorTree.BehaviorNode> currentNodes { get; private set; }

      /// <summary>
      /// The nodes currently being inspected
      /// </summary>
      public BehaviorTree.BehaviorNode currentNode { get; private set; }

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      protected override void OnWindowEnable()
      {
        if (this.treeViewState == null)
          this.treeViewState = new TreeViewState();

        if (behaviorTree)
        {
          this.treeInspector = new BehaviourTreeView(treeViewState, behaviorTree.tree.elements);
          this.OnTreeSet();
        }
        else
        {
          TreeBuilder<BehaviorTree.BehaviorNode, Behavior> treeBuilder = new TreeBuilder<BehaviorTree.BehaviorNode, Behavior>();
          this.treeInspector = new BehaviourTreeView(treeViewState, treeBuilder.ToTree());
        }
        //this.treeInspector.onSelectionChanged += this.OnSelectionChanged;
        this.treeInspector.onSelectionIdsChanged += this.OnSelectionChanged;
        this.treeInspector.Reload();
        //this.behaviorSelector = new TypeSelector(typeof(Behavior), false);
      }

      protected override void OnWindowGUI()
      {
        Rect rect = currentPosition;
        rect = StratusEditorUtility.Pad(rect);

        // Hierarchy: LEFT
        rect.width *= 0.5f;
        DrawHierarchy(rect);
        // Inspector: TOP-RIGHT
        rect.x += rect.width;
        rect.width -= StratusEditorGUI.standardPadding;
        rect.height *= 0.5f;
        rect.height -= padding * 2f;
        DrawInspector(rect);
        // Blackboard: BOTTOM-RIGHT
        rect.y += rect.height;
        rect.y += padding;
        DrawBlackboard(rect);
      }

      //----------------------------------------------------------------------/
      // Procedures
      //----------------------------------------------------------------------/
      private void DrawHierarchy(Rect rect)
      {
        //if (behaviorTree != null)
        GUILayout.BeginArea(rect);
        GUILayout.Label("Hierarchy", StratusGUIStyles.header);
        rect.y += 10f;
        treeInspector?.TreeViewGUI(rect);
        GUILayout.EndArea();
      }

      private void DrawInspector(Rect rect)
      {
        GUILayout.BeginArea(rect);
        GUILayout.Label("Inspector", StratusGUIStyles.header);

        if (currentNodes != null)
        {
          if (currentNodes.Count == 1)
          {
            GUILayout.Label(currentNode.dataTypeName, EditorStyles.largeLabel);
            this.inspectorScrollPosition = EditorGUILayout.BeginScrollView(this.inspectorScrollPosition, GUI.skin.box);
            bool changed = currentNodeSerializedObject.DrawEditorGUILayout();
            EditorGUILayout.EndScrollView();
          }
          else
          {
            GUILayout.Label("Editing multiple nodes is not supported!", EditorStyles.largeLabel);
          }
        }

        GUILayout.EndArea();
      }

      private void DrawBlackboard(Rect rect)
      {
        GUILayout.BeginArea(rect);
        GUILayout.Label("Blackboard", StratusGUIStyles.header);
        if (this.behaviorTree != null)
        {
          // Set the blackboard
          SerializedProperty blackboardProperty = this.behaviorTreeProperties.GetProperty(nameof(BehaviorTree.blackboard));
          bool changed = this.EditProperty(blackboardProperty, "Asset");
          if (changed && this.blackboard != null)
            this.OnBlackboardSet();

          EditorGUILayout.Space();

          // Draw the blackboard
          if (this.blackboardEditor != null)
          {
            // Controls
            StratusEditorGUI.BeginAligned(TextAlignment.Center);
            StratusEditorGUI.EnumToolbar(ref scope);
            StratusEditorGUI.EndAligned();

            this.blackboardScrollPosition = EditorGUILayout.BeginScrollView(this.blackboardScrollPosition, GUI.skin.box);
            switch (scope)
            {
              case Blackboard.Scope.Local:
                blackboardEditor.DrawSerializedProperty(nameof(Blackboard.locals));
                break;
              case Blackboard.Scope.Global:
                blackboardEditor.DrawSerializedProperty(nameof(Blackboard.globals));
                break;
            }
            EditorGUILayout.EndScrollView();
          }

        }
        GUILayout.EndArea();
      }

      //----------------------------------------------------------------------/
      // Methods: Private
      //----------------------------------------------------------------------/
      private void AddNode(Type type, BehaviorTree.BehaviorNode parent = null)
      {
        if (parent != null)
          behaviorTree.AddBehavior(type, parent);
        else
          behaviorTree.AddBehavior(type);

        Save();
      }

      private void RemoveNode(BehaviorTree.BehaviorNode node)
      {
        if (node == currentNode)
          currentNodeSerializedObject = null;

        this.behaviorTree.RemoveBehavior(node);
        Save();
      }

      private void RemoveAllNodes()
      {
        behaviorTree.ClearBehaviors();
        currentNodeSerializedObject = null;
        Save();
      }

      private void Refresh()
      {
        this.treeInspector.SetTree(this.behaviorTree.tree.elements);
      }

      private void OnTreeSet()
      {
        this.behaviorTreeProperties = new SerializedPropertyMap(this.behaviorTree, typeof(StratusScriptable));
        //this.treeProperty = this.behaviorTreeProperties.GetProperty(nameof(BehaviorTree.tree));
        //this.treeElementsProperty = this.treeElementsProperty.FindPropertyRelative("elements");

        // Blackboard
        this.blackboardEditor = null;
        if (this.blackboard)
          this.OnBlackboardSet();
      }

      private void Save()
      {
        EditorUtility.SetDirty(behaviorTree);
        Refresh();
      }

      private void OnBlackboardSet()
      {
        this.blackboardEditor = StratusEditor.CreateEditor(this.behaviorTree.blackboard) as StratusEditor;
        //this.blackboardEditor.OnInspectorGUI();
      }

      //private void OnSelectionChanged(IList<BehaviorTree.BehaviorNode> elements)
      //{
      //  this.currentNodeSerializedObject = null;
      //  this.currentNodeProperty = null;
      //  //this.currentNodeProperty = this.treeElementsProperty.get
      //
      //  
      //  currentNodes = elements;
      //  if (currentNodes.Count > 0)
      //  {
      //    this.currentNode = currentNodes[0];
      //    this.currentNodeSerializedObject = new SerializedSystemObject(currentNode.data);
      //    //this.currentNodeProperty = this.treeElementsProperty.GetArrayElementAtIndex(0);
      //  }
      //}

      private void OnSelectionChanged(IList<int> ids)
      {
        this.currentNodeSerializedObject = null;
        //this.currentNodeProperty = null;

        this.currentNodes = this.treeInspector.GetElements(ids);
        if (this.currentNodes.Count > 0)
        {
          this.currentNode = currentNodes[0];
          this.currentNodeSerializedObject = new SerializedSystemObject(currentNode.data);
          //SerializedObject boo = new SerializedObject(currentNode.data);
          //this.currentNodeProperty = this.treeElementsProperty.GetArrayElementAtIndex(ids[0]);
        }
      }

      //----------------------------------------------------------------------/
      // Methods: Static
      //----------------------------------------------------------------------/
      [OnOpenAsset]
      public static bool OnOpenAsset(int instanceID, int line)
      {
        var myTreeAsset = EditorUtility.InstanceIDToObject(instanceID) as BehaviorTree;
        if (myTreeAsset != null)
        {
          Open(myTreeAsset);
          return true;
        }
        return false;
      }

      public static void Open(BehaviorTree tree)
      {
        OnOpen("Behavior Tree");
        instance.SetTree(tree);
      }

      public void SetTree(BehaviorTree tree)
      {
        this.behaviorTree = tree;
        this.OnTreeSet();
        this.Refresh();
      }




    }
  }

}