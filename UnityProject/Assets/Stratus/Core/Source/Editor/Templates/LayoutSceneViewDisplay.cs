using Stratus.Utilities;
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// Provides an automatic layout 2D Handle GUI besides default access to the SceneView GUI
  /// </summary>
  public abstract class LayoutSceneViewDisplay : SceneViewDisplay
  {
    /// <summary>
    /// A required attribute for configuring the LayoutSceneViewDisplay
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class LayoutViewDisplayAttributeAttribute : Attribute
    {
      public LayoutViewDisplayAttributeAttribute(string title, float width, float height, StratusGUI.Anchor anchor, StratusGUI.Dimensions dimensions)
      {
        this.title = title;
        this.width = width;
        this.height = height;
        this.anchor = anchor;
        this.dimensions = dimensions;
      }

      private string title { get; set; }
      private float width { get; set; }
      private float height { get; set; }
      private StratusGUI.Anchor anchor { get; set; }
      private StratusGUI.Dimensions dimensions { get; set; }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public string title;
    public Vector2 offset;
    public StratusGUI.Anchor anchor;
    public StratusGUI.Dimensions dimensions;
    public Vector2 size;
    public Vector2 scale;
    private Vector2 scrollPos = Vector2.zero;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public Vector2 currentSize { get; private set; }
    private bool hasCalculatedDimensions { get; set; }

    //------------------------------------------------------------------------/
    // Methods: Interface
    //------------------------------------------------------------------------/
    protected abstract void OnGUILayout(Rect position);
    protected abstract void OnGUI(Rect position);

    //------------------------------------------------------------------------/
    // Methods: Override
    //------------------------------------------------------------------------/
    protected override void OnInitializeDisplay()
    {
    }

    /// <summary>
    /// Invoked by the Unity's Scene window. This is where we hook in our own drawing code
    /// </summary>
    /// <param name="sceneView"></param>
    protected override void OnSceneGUI(SceneView sceneView)
    {      
      //if (!hasCalculatedDimensions && dimensions == StratusGUI.Dimensions.Relative)
      //{
      //  size = StratusGUI.FindRelativeDimensions(scale, sceneView.position.size);
      //  hasCalculatedDimensions = true;
      //}

      // What size to use
      if (dimensions == StratusGUI.Dimensions.Absolute)
        currentSize = size;
      else if (dimensions == StratusGUI.Dimensions.Relative)
        currentSize = StratusGUI.FindRelativeDimensions(scale, sceneView.position.size);

      // Draw the default GUI
      OnGUI(sceneView.position);      

      // Draw the provided layout 2D GUI Block
      Rect layoutPosition = StratusGUI.CalculateAnchoredPositionOnScreen(anchor, currentSize, sceneView.position.size);
      Handles.BeginGUI();
      {
        GUILayout.BeginArea(layoutPosition, title, GUI.skin.window);
        {
          //GUILayout.Label(layoutPosition, "+");
          scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
          this.OnGUILayout(layoutPosition);
          GUILayout.EndScrollView();
        }
        GUILayout.EndArea();
        //if (StratusEditorUtility.IsMousedOver(layoutPosition) && !StratusEditorUtility.currentEventUsed)
        //{
        //  System.Action onRightClick = () =>
        //  {
        //    var menu = new GenericMenu();
        //    menu.AddItem(new GUIContent("Select"), false, () => { Trace.Script("Boop"); });
        //  };
        //  StratusEditorUtility.OnMouseClick(null, onRightClick, null);
        //  
        //}
        //StratusEditorUtility.DisableMouseSelection(layoutPosition);
      }
      Handles.EndGUI();

      //if (StratusEditorUtility.IsMousedOver(layoutPosition))
      //{
      //  int control = GUIUtility.GetControlID(FocusType.Passive);
      //}
      //
      //GUIUtility.hotControl = control;
      

    }

    //------------------------------------------------------------------------/
    // Methods: Private
    //------------------------------------------------------------------------/
    protected override void OnReset()
    {
      Type type = GetType();
      var settings = AttributeUtility.FindAttribute<LayoutViewDisplayAttributeAttribute>(type);
      if (settings == null)
      {
        throw new MissingReferenceException("Missing [LayoutSceneViewDisplay] attribute declaration for the class '" + type.Name + "'");
      }

      // Read and set the properties fron the attribute
      this.title = settings.GetProperty<string>("title");
      this.size = new Vector2(settings.GetProperty<float>("width"), settings.GetProperty<float>("height"));
      this.anchor = settings.GetProperty<StratusGUI.Anchor>("anchor");
      this.dimensions = settings.GetProperty<StratusGUI.Dimensions>("dimensions");      

      // If the size is relative...
      if (this.dimensions == StratusGUI.Dimensions.Relative)
        scale = size;
    }

    protected override void OnInspect()
    {
      anchor = (StratusGUI.Anchor)EditorGUILayout.EnumPopup("Anchor", anchor);
      dimensions = (StratusGUI.Dimensions)EditorGUILayout.EnumPopup("Dimensions", dimensions);
      switch (dimensions)
      {
        case StratusGUI.Dimensions.Relative:
          EditorGUI.indentLevel++;
          scale.x = EditorGUILayout.Slider("Horizontal", scale.x, 0f, 1f);
          scale.y = EditorGUILayout.Slider("Vertical", scale.y, 0f, 1f);
          EditorGUI.indentLevel--;
          //scale = EditorGUILayout.Slider("Scale", scale);
          break;
        case StratusGUI.Dimensions.Absolute:
          EditorGUI.indentLevel++;
          size.x = EditorGUILayout.FloatField("Width", size.x);
          size.y = EditorGUILayout.FloatField("Height", size.y);
          EditorGUI.indentLevel--;
          //size = EditorGUILayout.Vector2Field("Size", size);
          break;
      }
    }

    private void CalculateRelativeDimensions()
    {

    }



  }
}
