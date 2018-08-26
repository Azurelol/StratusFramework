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
        public BehaviourTreeView(TreeViewState state, IList<BehaviorTree.BehaviorNode> data) : base(state, data)
        {
        }
      }

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      [SerializeField]
      private BehaviourTreeView treeInspector;
      [SerializeField]
      private TreeViewState treeViewState;

      const string folder = "Stratus/Experimental/AI/";
      private TypeSelector behaviorSelector;

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// The behavior tree currently being edited
      /// </summary>
      private BehaviorTree behaviorTree;

      /// <summary>
      /// The node currently being inspected
      /// </summary>
      private BehaviorTree.BehaviorNode currentNode { get; set; }

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      protected override void OnWindowEnable()
      {
        if (this.treeViewState == null)
          this.treeViewState = new TreeViewState();

        TreeBuilder<BehaviorTree.BehaviorNode, Behavior> treeBuilder = new TreeBuilder<BehaviorTree.BehaviorNode, Behavior>(null);
        this.treeInspector = new BehaviourTreeView(treeViewState, treeBuilder.ToTree());
        this.treeInspector.Reload();
        this.behaviorSelector = new TypeSelector(typeof(Behavior), false);
      }

      protected override void OnWindowGUI()
      {
        Rect rect = currentPosition;

        rect.width *= 0.5f;
        DrawHierarchy(rect);

        rect.x += rect.width;
        DrawInspector(rect);        
      }

      //----------------------------------------------------------------------/
      // Procedures
      //----------------------------------------------------------------------/
      private void DrawHierarchy(Rect rect)
      {
        treeInspector?.TreeViewGUI(rect);
      }

      private void DrawInspector(Rect rect)
      {
        EditorGUILayout.LabelField("Inspector", StratusGUIStyles.header);
        GUI.BeginGroup(rect);
        GUILayout.Label("Inspector", StratusGUIStyles.header);
        //GUILayout.BeginHorizontal();
        //{
        //  StratusEditorGUI.GUILayoutPopup("Behaviors", behaviorSelector.subTypes);
        //  if (GUILayout.Button("Add", EditorStyles.miniButtonRight))
        //  {
        //    AddNode(behaviorSelector.selectedClass);
        //  }
        //}
        //GUILayout.EndHorizontal();
        //treeInspector.GetSelection
        GUI.EndGroup();
      }

      //----------------------------------------------------------------------/
      // Methods: Private
      //----------------------------------------------------------------------/
      private void AddNode(Type type)
      {
        behaviorTree.AddBehaviour(type);
      }

      private void RemoveNode(BehaviorTree.BehaviorNode node)
      {
        //behaviorTree.AddBehaviour(type);
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
        OnOpen("Behaviour Tree Editor");
        instance.SetTree(tree);
      }

      public void SetTree(BehaviorTree tree)
      {
        this.behaviorTree = tree;
        if (tree.nodes == null)
          tree.nodes = new List<BehaviorTree.BehaviorNode>();
        this.treeInspector.SetTree(tree.nodes);        
      }


    }
  }

}