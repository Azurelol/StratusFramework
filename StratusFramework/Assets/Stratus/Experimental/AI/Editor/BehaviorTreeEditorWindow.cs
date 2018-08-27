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

        protected override void OnItemContextMenu(GenericMenu menu, BehaviorTree.BehaviorNode treeElement)
        {
          menu.AddItem("Boo", false, ()=> { });
          //menu.AddItem("Remove", false, () => treeElement.data.);
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

        if (behaviorTree)
        {
          this.treeInspector = new BehaviourTreeView(treeViewState, behaviorTree.tree.elements);
        }
        else
        {
          TreeBuilder<BehaviorTree.BehaviorNode, Behavior> treeBuilder = new TreeBuilder<BehaviorTree.BehaviorNode, Behavior>();
          this.treeInspector = new BehaviourTreeView(treeViewState, treeBuilder.ToTree());
        }
        this.treeInspector.Reload();
        this.behaviorSelector = new TypeSelector(typeof(Behavior), false);
      }

      protected override void OnWindowGUI()
      {
        Rect rect = currentPosition;

        //GUILayout.BeginArea(rect, EditorStyles.inspectorDefaultMargins);
        //GUILayout.EndArea();
        rect.y += StratusEditorUtility.lineHeight;

        rect.width *= 0.5f;
        DrawHierarchy(rect);

        rect.x += rect.width;
        rect.width -= StratusEditorGUI.standardPadding;
        //StratusEditorGUI.GUIPopup(rect, "Behaviors", behaviorSelector.subTypes);
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
        //StratusEditorGUI.GUIPopup(rect, "Behaviors", behaviorSelector.subTypes);
        GUILayout.BeginArea(rect, EditorStyles.inspectorDefaultMargins);        
        GUILayout.Label("Inspector", StratusGUIStyles.header);
        StratusEditorGUI.GUILayoutPopup("Behaviors", behaviorSelector.subTypes);
        //GUILayout.BeginHorizontal();
        {
          //EditorGUILayout.Popup(behaviorSelector.selectedIndex, behaviorSelector.displayedOptions);          
          if (GUILayout.Button("Add", EditorStyles.miniButtonRight))
          {
            AddNode(behaviorSelector.selectedClass);
          }
        }
        //GUILayout.EndHorizontal();
        //treeInspector.GetSelection

        if (GUILayout.Button("Refresh"))
          this.Refresh();
        if (GUILayout.Button("Clear"))
          this.RemoveAllNodes();


        foreach(var element in this.behaviorTree.tree.elements)
        {
          GUILayout.Label(element.ToString());
        }
        GUI.EndGroup();
      }

      //----------------------------------------------------------------------/
      // Methods: Private
      //----------------------------------------------------------------------/
      private void AddNode(Type type)
      {
        behaviorTree.AddBehaviour(type);
        EditorUtility.SetDirty(behaviorTree);
        Refresh();
      }

      private void RemoveNode(BehaviorTree.BehaviorNode node)
      {
        //behaviorTree.AddBehaviour(type);
        EditorUtility.SetDirty(behaviorTree);
        Refresh();
      }

      private void RemoveAllNodes()
      {
        behaviorTree.ClearBehaviors();
        EditorUtility.SetDirty(behaviorTree);
        Refresh();
      }

      public void Refresh()
      {
        this.treeInspector.SetTree(this.behaviorTree.tree.elements);
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
        this.Refresh();
      }

      


    }
  }

}