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
      public enum Mode
      {
        Editor = 0,
        Debugger = 1,
      }

      public class BehaviourTreeView : HierarchicalTreeView<BehaviorTree.BehaviorNode>
      {
        private BehaviorTreeEditorWindow window => BehaviorTreeEditorWindow.instance;
        private BehaviorTree tree => BehaviorTreeEditorWindow.instance.behaviorTree;


        public BehaviourTreeView(TreeViewState state, IList<BehaviorTree.BehaviorNode> data) : base(state, data)
        {
        }

        protected override bool IsParentValid(BehaviorTree.BehaviorNode parent)
        {
          //Trace.Script($"Parent type = {parent.dataType}");
          if (parent.data == null)
            return false;
          if (parent.data is Composite)
            return true;
          else if (parent.data is Decorator && !parent.hasChildren)
            return true;

          return false;
        }

        protected override void OnItemContextMenu(GenericMenu menu, BehaviorTree.BehaviorNode treeElement)
        {
          // Tasks
          if (treeElement.data is Task)
          {
            BehaviorTree.BehaviorNode parent = treeElement.GetParent<BehaviorTree.BehaviorNode>();
            menu.AddItem("Duplicate", false, () => window.AddNode((Behavior)treeElement.data.Clone(), parent));
                        
            menu.AddPopup("Add/Decorator", BehaviorTreeEditorWindow.decoratorTypes.displayedOptions, (int index) =>
            {
              window.AddParentNode(BehaviorTreeEditorWindow.decoratorTypes.AtIndex(index), treeElement);
            });
          }
          // Composites
          else if (treeElement.data is Composite)
          {
            menu.AddPopup("Add/Tasks", BehaviorTreeEditorWindow.taskTypes.displayedOptions, (int index) =>
            {
              window.AddChildNode(BehaviorTreeEditorWindow.taskTypes.AtIndex(index), treeElement);
            });
            
            menu.AddPopup("Add/Composites", BehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
            {
              window.AddChildNode(BehaviorTreeEditorWindow.compositeTypes.AtIndex(index), treeElement);
            });  
            
            menu.AddPopup("Add/Decorator", BehaviorTreeEditorWindow.decoratorTypes.displayedOptions, (int index) =>
            {
              window.AddChildNode(BehaviorTreeEditorWindow.decoratorTypes.AtIndex(index), treeElement);
            });
          }
          // Decorators
          else if (treeElement.data is Decorator)
          {
            if (!treeElement.hasChildren)
            {
              menu.AddPopup("Add/Tasks", BehaviorTreeEditorWindow.taskTypes.displayedOptions, (int index) =>
              {
                window.AddChildNode(BehaviorTreeEditorWindow.taskTypes.AtIndex(index), treeElement);
              });

              menu.AddPopup("Add/Composites", BehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
              {
                window.AddChildNode(BehaviorTreeEditorWindow.compositeTypes.AtIndex(index), treeElement);
              });
            }
          }


          // Common
          menu.AddItem("Remove", false, () => window.RemoveNode(treeElement));
        }

        protected override void OnContextMenu(GenericMenu menu)
        {
          // Composite
          menu.AddPopup("Add/Composites", BehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
          {
            window.AddChildNode(BehaviorTreeEditorWindow.compositeTypes.AtIndex(index));
          });

          // Actions
          menu.AddPopup("Add/Tasks", BehaviorTreeEditorWindow.taskTypes.displayedOptions, (int index) =>
          {
            window.AddChildNode(BehaviorTreeEditorWindow.taskTypes.AtIndex(index));
          });

          menu.AddItem("Clear", false, () => window.RemoveAllNodes());
        }

        protected override void OnBeforeRow(Rect rect, TreeViewItem<BehaviorTree.BehaviorNode> treeViewItem)
        {
          if (treeViewItem.item.data is Composite)
          {
            StratusGUI.GUIBox(rect, Composite.color);
          }
          else if (treeViewItem.item.data is Task)
          {
            StratusGUI.GUIBox(rect, Task.color);
          }
          else if (treeViewItem.item.data is Decorator)
          {
            StratusGUI.GUIBox(rect, Decorator.color);
          }
        }
      }

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      [SerializeField]
      private BehaviourTreeView treeInspector;
      [SerializeField]
      private TreeViewState treeViewState;
      [SerializeField]
      private Mode mode = Mode.Editor;
      [SerializeField]
      private Agent debugTarget;

      private SerializedSystemObject currentNodeSerializedObject;
      const string folder = "Stratus/Experimental/AI/";
      private Vector2 inspectorScrollPosition, blackboardScrollPosition;
      private SerializedPropertyMap behaviorTreeProperties;
      private StratusEditor blackboardEditor;
      private string[] toolbarOptions = new string[]
      {
        nameof(Mode.Editor),
        nameof(Mode.Debugger),
      };

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      private static Type compositeType { get; } = typeof(Composite);
      private static Type decoratorType { get; } = typeof(Decorator);

      /// <summary>
      /// All supported behavior types
      /// </summary>
      public static TypeSelector behaviorTypes { get; } = TypeSelector.FilteredSelector(typeof(Behavior), typeof(Service), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector compositeTypes { get; } = new TypeSelector(typeof(Composite), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector taskTypes { get; } = new TypeSelector(typeof(Task), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector serviceTypes { get; } = new TypeSelector(typeof(Service), false, true);

      /// <summary>
      /// All supported decorator types
      /// </summary>
      public static TypeSelector decoratorTypes { get; } = new TypeSelector(typeof(Decorator), false, true);

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

      public bool hasSelection => currentNodes != null;

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
        StratusEditorUtility.DrawAligned(this.DrawModeControl, TextAlignment.Center);
        GUILayout.Space(padding);
        Rect rect = this.currentPosition;
        rect = StratusEditorUtility.Pad(rect);
        switch (this.mode)
        {
          case Mode.Editor:
            this.DrawEditor(rect);
            break;
          case Mode.Debugger:
            this.DrawDebugger(rect);
            break;
        }

      }

      //----------------------------------------------------------------------/
      // Procedures
      //----------------------------------------------------------------------/
      private void DrawModeControl()
      {
        //EditorGUI.BeginChangeCheck();
        {
          this.mode = (Mode)GUILayout.Toolbar((int)this.mode, this.toolbarOptions, GUILayout.ExpandWidth(false));
        }
        //if (EditorGUI.EndChangeCheck())
        //{          
        //}
      }

      private void DrawEditor(Rect rect)
      {
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

      private void DrawDebugger(Rect rect)
      {
        EditorGUILayout.LabelField("Target", StratusGUIStyles.header);
        //EditProperty(nameof(debugTarget));
        EditorGUI.BeginChangeCheck();
        this.debugTarget = (Agent)EditorGUILayout.ObjectField(this.debugTarget, typeof(Agent), true);
        if (EditorGUI.EndChangeCheck())
        {
          EditorUtility.SetDirty(this);
        }
      }

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

        if (hasSelection)
        {
          if (currentNodes.Count == 1)
          {
            GUILayout.Label(currentNode.dataTypeName, EditorStyles.largeLabel);
            this.inspectorScrollPosition = EditorGUILayout.BeginScrollView(this.inspectorScrollPosition, GUI.skin.box);
            currentNodeSerializedObject.DrawEditorGUILayout();
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
      private void AddChildNode(Type type, BehaviorTree.BehaviorNode parent = null)
      {
        if (parent != null)
          behaviorTree.AddBehavior(type, parent);
        else
          behaviorTree.AddBehavior(type);

        Save();
      }

      private void AddParentNode(Type type, BehaviorTree.BehaviorNode child)
      {
        behaviorTree.AddParentBehavior(type, child);
        Save();
      }

      private void AddNode(Behavior behavior, BehaviorTree.BehaviorNode parent = null)
      {
        if (parent != null)
          behaviorTree.AddBehavior(behavior, parent);
        else
          behaviorTree.AddBehavior(behavior);

        Save();
      }
      private void RemoveNode(BehaviorTree.BehaviorNode node)
      {
        //if (node == currentNode)
        currentNodeSerializedObject = null;
        currentNodes = null;

        this.behaviorTree.RemoveBehavior(node);
        Save();
      }

      private void RemoveAllNodes()
      {
        behaviorTree.ClearBehaviors();
        currentNodeSerializedObject = null;
        currentNodes = null;
        Save();
      }

      private void Refresh()
      {
        this.treeInspector.SetTree(this.behaviorTree.tree.elements);
        this.Repaint();
      }

      private void OnTreeSet()
      {
        this.behaviorTree.Assert();
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
        Undo.RecordObject(behaviorTree, "Behavior Tree Edit");
        Refresh();
      }

      private void OnBlackboardSet()
      {
        this.blackboardEditor = StratusEditor.CreateEditor(this.behaviorTree.blackboard) as StratusEditor;
        //this.blackboardEditor.OnInspectorGUI();
      }

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