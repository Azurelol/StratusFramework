using UnityEngine;
using Stratus;
using UnityEditor;
using Stratus.Editor;
using System;

namespace Stratus
{
  namespace AI
  {
    public class BehaviorTreeEditorWindow : MultiColumnEditorWindow 
    {
      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      const string Folder = "Stratus/Experimental/AI/";
      const string Title = "Behavior Tree Editor";

      /// <summary>
      /// The editor used to edit the behavior tree's nodes
      /// </summary>
      private BehaviorTreeNodeEditor NodeEditor = new BehaviorTreeNodeEditor();

      /// <summary>
      /// The behavior tree currently being edited
      /// </summary>
      private BehaviorTree BehaviorTree;

      /// <summary>
      /// The behavior tree currently being edited
      /// </summary>
      private Blackboard Blackboard;

      /// <summary>
      /// The sidebar containing the node inspector and blackboard
      /// </summary>
      private GUISplitter SideBar;

      /// <summary>
      /// The node currently being inspected
      /// </summary>
      private BehaviorTreeNode CurrentNode { get { return NodeEditor.SelectedNode; } }

     
      //----------------------------------------------------------------------/
      // Methods: Static
      //----------------------------------------------------------------------/      
      [MenuItem(Folder + Title)]
      private static void Open()
      {
        Instantiate();
      }

      /// <summary>
      /// Opens the behavior tree node editor, with a selected tree
      /// </summary>
      /// <param name="tree"></param>
      public static void Open(BehaviorTree tree)
      {
        Instantiate().OpenTree(tree);
      }

      private static BehaviorTreeEditorWindow Instantiate()
      {
        return GetWindow<BehaviorTreeEditorWindow>(Title);
      }

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      protected override void OnMultiColumnEditorEnable(MenuBar menu, GUISplitter columns)
      {
        // Node editor
        var nodeSettings = new BehaviorTreeNodeEditor.Settings();
        NodeEditor.Initialize(this, nodeSettings);

        // Sidebar
        SideBar = new GUISplitter(this, GUISplitter.OrientationType.Vertical);
        SideBar.Add(0.5f, this.DrawInspector);
        SideBar.Add(0.5f, this.DrawBlackboard);

        // Columns
        columns.Add(0.25f, DrawSidebar);
        columns.Add(0.75f, DrawNodes);

        // Menu
        var file = menu.Add("File");
        file.AddItem(new GUIContent("New Tree"), false, Dogs);
        file.AddItem(new GUIContent("Open Tree"), false, Dogs);
        file.AddItem(new GUIContent("Save Tree"), false, Cats);

        var behaviors = menu.Add("Behaviors");
        behaviors.AddItem(new GUIContent("Composites"), false, Dogs);
        behaviors.AddItem(new GUIContent("Decorators"), false, Cats);
        behaviors.AddItem(new GUIContent("Actions"), false, Cats);

        var settings = menu.Add("Settings");
        settings.AddItem(new GUIContent("Blackboard"), false, Dogs);
      }

      //----------------------------------------------------------------------/
      // Methods: Static
      //----------------------------------------------------------------------/
      void OpenTree(BehaviorTree tree)
      {
        BehaviorTree = tree;
        Blackboard = tree.BlackboardAsset;
        ChangeTitle(BehaviorTree.name);
      }

      void ChangeTitle(string title)
      {
        titleContent = new GUIContent(Title + " - " + title);
      }

      //----------------------------------------------------------------------/
      // Methods: Draw
      //----------------------------------------------------------------------/
      void DrawNodes(Rect position)
      {
        NodeEditor.Draw(position);
      }      

      void DrawSidebar(Rect position)
      {        
        SideBar.Draw(position);
      }

      //----------------------------------------------------------------------/
      // Methods: Sidebar
      //----------------------------------------------------------------------/
      void DrawInspector(Rect position)
      {
        GUILayout.Label("Inspector", EditorStyles.boldLabel);        

        if (CurrentNode == null)
          return;


        //EditorGUILayout.LabelField("Position");
        CurrentNode.Name = EditorGUILayout.TextField("Name", CurrentNode.Name);
        EditorGUILayout.RectField("Position", CurrentNode.Rect);
      }

      void DrawBlackboard(Rect position)
      {
        GUILayout.Label("Blackboard", EditorStyles.boldLabel);

        if (Blackboard == null)
        {
          GUILayout.Label("No blackboard selected");
          return;
        }

        // Show globals
        foreach (var key in Blackboard.Globals)
        {
          GUILayout.Label(key.Key);
        }

        // Show locals
        foreach (var key in Blackboard.Globals)
        {
          GUILayout.Label(key.Key);
        }
      }

      //----------------------------------------------------------------------/
      // Methods: Menu Bar
      //----------------------------------------------------------------------/
      void Dogs()
      {
        Trace.Script("Dogs");
      }

      void Cats()
      {
        Trace.Script("Cats");
      }

      


    }
  }

}