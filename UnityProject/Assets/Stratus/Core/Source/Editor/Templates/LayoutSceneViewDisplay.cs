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
  public abstract class LayoutSceneViewDisplay : PersistentSceneViewDisplay
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
    // Properties
    //------------------------------------------------------------------------/
    public string title { get; set; }
    public Vector2 size { get; set; }
    public StratusGUI.Anchor anchor { get; set; }
    public StratusGUI.Dimensions dimensions { get; set; }
    public Vector2 relativeDimensions { get; set; }
    private Vector2 scrollPos { get; set; } = Vector2.zero;

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
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
        relativeDimensions = size;
    }

    /// <summary>
    /// Invoked by the Unity's Scene window. This is where we hook in our own drawing code
    /// </summary>
    /// <param name="sceneView"></param>
    protected override void OnSceneGUI(SceneView sceneView)
    {      
      if (!hasCalculatedDimensions && dimensions == StratusGUI.Dimensions.Relative)
      {
        size = StratusGUI.FindRelativeDimensions(relativeDimensions, sceneView.position.size);
        hasCalculatedDimensions = true;
      }

      // Draw the default GUI
      OnGUI(sceneView.position);

      // Draw the provided layout 2D GUI Block
      Rect layoutPosition = StratusGUI.CalculateAnchoredPositionOnScreen(this.anchor, this.size, sceneView.position.size);
      Handles.BeginGUI();
      GUILayout.BeginArea(layoutPosition, title, GUI.skin.window);
      scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
      this.OnGUILayout(layoutPosition);
      GUILayout.EndScrollView();
      GUILayout.EndArea();
      Handles.EndGUI();
    }



  }
}
