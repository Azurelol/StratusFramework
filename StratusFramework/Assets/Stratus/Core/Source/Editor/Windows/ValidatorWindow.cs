using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Stratus.Editor;
using System;
using Stratus.Interfaces;

namespace Stratus
{
  public class ValidatorWindow : EditorWindow
  {
    public class ValidatorTreeView : TreeView
    {
      public ValidatorTreeView(TreeViewState treeViewState)
    : base(treeViewState)
      {
        Reload();
      }

      protected override TreeViewItem BuildRoot()
      {
        // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
        // are created from data. Here we create a fixed set of items. In a real world example,
        // a data model should be passed into the TreeView and the items created from the model.

        // This section illustrates that IDs should be unique. The root item is required to 
        // have a depth of -1, and the rest of the items increment from that.
        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
        var allItems = new List<TreeViewItem>
        {
            new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
            new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
            new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
            new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
            new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
            new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
            new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
            new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
            new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
        };

        // Utility method that initializes the TreeViewItem.children and .parent for all items.
        SetupParentsAndChildrenFromDepths(root, allItems);

        // Return root of the tree
        return root;
      }
    }

    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private string header;
    private Validation[] messages;
    public static ValidatorWindow window;
    private Vector2 scrollPos, sidebarScrollPos;
    [SerializeField]
    private TreeViewState treeViewState;
    private TreeView treeView;
    //private TypeSelector validatorTypes;
    //private TypeSelector validatorAggregatorTypes;
    //private GUISplitter sidebar;
    //private GUISplitter splitter;

    //private Type[] validatorTypes = new Type[] {  };
    //private DropdownList<Type> validatorTypes;
    private bool onFirstTime;

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    public static void Open(string header, Validation[] messages)
    {
      window = (ValidatorWindow)EditorWindow.GetWindow(typeof(ValidatorWindow), true, "Stratus Validator");      
      window.ShowValidation(header, messages);
    }

    [MenuItem("Stratus/Windows/Validator")]
    public static void Open()
    {
      window = (ValidatorWindow)EditorWindow.GetWindow(typeof(ValidatorWindow), true, "Stratus Validator");
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnEnable()
    {
      if (treeViewState == null)
        treeViewState = new TreeViewState();
      treeView = new ValidatorTreeView(treeViewState);

      //validatorTypes = new DropdownList<Type>(new Type[]
      //{
      //  typeof(Validator),
      //  typeof(ValidatorAggregator)
      //}, (Type type) => type.Name, 0);



      //Trace.Script($"ValidatorTypes = {validatorTypes.subTypes.Length}");
      //validatorTypes = new TypeSelector(typeof(MonoBehaviour), typeof(Interfaces.Validator), true);
      //validatorAggregatorTypes = new TypeSelector(typeof(MonoBehaviour), typeof(Interfaces.ValidatorAggregator), true);
    }

    //protected override void OnMultiColumnEditorEnable(MenuBar menu, GUISplitter columns)
    //{
    //  //sidebar = new GUISplitter(this, GUISplitter.OrientationType.Vertical);
    //  columns.Add(0.25f, DrawSidebar);
    //  columns.Add(0.75f, DrawMessages);
    //}


    private void OnGUI()
    {
      if (!onFirstTime)
      {
        EditorStyles.helpBox.richText = true;
        onFirstTime = true;
      }
      //DrawTree();
      //EditorGUILayout.BeginVertical(EditorStyles.toolbar);
      //
      //StratusGUIStyles.DrawBackgroundColor(position, StratusGUIStyles.Colors.cinnabar);
      //GUI.backgroundColor = StratusGUIStyles.Colors.aquaIsland;
      //GUI.DrawTexture(position, StratusGUIStyles.GetColorTexture(StratusGUIStyles.Colors.aquaIsland), ScaleMode.StretchToFill);
      EditorGUILayout.BeginHorizontal();
      {
        DrawControls(position);
        GUILayout.FlexibleSpace();
        //EditorGUILayout.Separator();
        DrawMessages(position);
        GUILayout.FlexibleSpace();
      }
      EditorGUILayout.EndHorizontal();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void DrawControls(Rect rect)
    {
      EditorGUILayout.BeginVertical(StratusGUIStyles.box, GUILayout.ExpandHeight(true));
      {
        // Validation
        if (GUILayout.Button(StratusGUIStyles.validateTexture, StratusGUIStyles.smallLayout))
        {
          var menu = new GenericMenu();
          menu.AddItem(new GUIContent("Validator"), false, () => ShowValidation("Validator", Interfaces.Global.Validate()));
          menu.AddItem(new GUIContent("Validator Aggregator"), false, () => ShowValidation("Validator", Interfaces.Global.ValidateAggregate()));
          menu.ShowAsContext();
        }

        // Clear
        if (GUILayout.Button(StratusGUIStyles.trashTexture, StratusGUIStyles.smallLayout))
        {
          messages = null;
        }
      }
      EditorGUILayout.EndVertical();
    }

    private void DrawMessages(Rect rect)
    {
      //EditorGUILayout.LabelField("Messages", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
      
      //EditorGUILayout.Separator();

      EditorGUILayout.BeginVertical(StratusGUIStyles.box, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
      EditorGUILayout.Separator();
      EditorGUILayout.LabelField($"{header} Messages", StratusGUIStyles.header);

      if (messages != null)
      {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var message in messages)
        {
          EditorGUILayout.HelpBox(message.message, message.type.Convert());
          if (message.hasContext)
          {
            StratusEditorUtility.OnMouseClick(GUILayoutUtility.GetLastRect(), null, () =>
            {
              var menu = new GenericMenu();

              if (message.target)
                menu.AddItem(new GUIContent("Select"), false, () => { Selection.activeObject = message.target; });
              if (message.onSelect != null)
                menu.AddItem(new GUIContent("Select"), false, () => { message.onSelect(); });

              menu.ShowAsContext();
            });

          }
        }
        EditorGUILayout.EndScrollView();
      }






      EditorGUILayout.EndVertical();
    }

    private void DrawTree()
    {
      treeView.OnGUI(new Rect(0, 0, position.width, position.height));
    }

    private void ShowValidation(string header, Validation[] messages)
    {
      this.header = header;
      this.messages = messages;
    }

    private void Validate(Type validationType)
    {
      if (validationType == typeof(Validator))
      {
        ShowValidation("Validator", Interfaces.Global.Validate());
      }
      else if (validationType == typeof(Validator))
      {
        ShowValidation("Validator Aggregator", Interfaces.Global.ValidateAggregate());
      }

    }


  }

}