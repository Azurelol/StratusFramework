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
    public sealed class LayoutSceneViewDisplayAttribute : Attribute
    {
      public LayoutSceneViewDisplayAttribute(string title, float width, float height, Overlay.Anchor anchor, Overlay.Dimensions dimensions)
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
      private Overlay.Anchor anchor { get; set; }
      private Overlay.Dimensions dimensions { get; set; }
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public string title { get; set; }
    public Vector2 size { get; set; }
    public Overlay.Anchor anchor { get; set; }
    public Overlay.Dimensions dimensions { get; set; }
    public Vector2 relativeDimensions { get; set; }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private bool isInitialized { get; set; }

    //------------------------------------------------------------------------/
    // Methods: Interface
    //------------------------------------------------------------------------/
    protected abstract void OnGUILayout(Rect position);
    protected abstract void OnGUI(Rect position);

    //------------------------------------------------------------------------/
    // Methods: Override
    //------------------------------------------------------------------------/
    protected override void OnInitialize()
    {
      Type type = GetType();
      var settings = AttributeUtility.FindAttribute<LayoutSceneViewDisplayAttribute>(type);
      if (settings == null)
      {
        throw new MissingReferenceException("Missing [LayoutSceneViewDisplay] attribute declaration for the class '" + type.Name + "'");
      }
      
      // Read and set the properties fron the attribute
      this.title = settings.GetProperty<string>("title");
      this.size = new Vector2(settings.GetProperty<float>("width"), settings.GetProperty<float>("height"));
      this.anchor = settings.GetProperty<Overlay.Anchor>("anchor");
      this.dimensions = settings.GetProperty<Overlay.Dimensions>("dimensions");

      // If the size is relative...
      if (this.dimensions == Overlay.Dimensions.Relative)
        relativeDimensions = size;

      // Invoke the hierarchy function for the first time
      OnHierarchyWindowChanged();
    }

    /// <summary>
    /// Invoked by the Unity's Scene window. This is where we hook in our own drawing code
    /// </summary>
    /// <param name="sceneView"></param>
    protected override void OnSceneGUI(SceneView sceneView)
    {
      if (!IsValid(sceneView))
      {
        //Trace.Script("Cannot display " + this.title);
        return;
      }
      
      if (!isInitialized && dimensions == Overlay.Dimensions.Relative)
      {
        size = Overlay.FindRelativeDimensions(relativeDimensions, sceneView.position.size);
        isInitialized = true;
      }

      // Draw the default GUI
      OnGUI(sceneView.position);

      // Draw the provided layout 2D GUI Block
      Rect layoutPosition = Overlay.CalculateAnchoredPositionOnScreen(this.anchor, this.size, sceneView.position.size);
      Handles.BeginGUI();
      GUILayout.BeginArea(layoutPosition, title, GUI.skin.window);
      this.OnGUILayout(layoutPosition);
      GUILayout.EndArea();
      Handles.EndGUI();

      //Trace.Script("SceneView = " + sceneView.position + ", LayoutPos = " + layoutPosition);
    }

    /// <summary>
    /// Whether this display should be shown
    /// </summary>
    /// <param name="sceneView"></param>
    /// <returns></returns>
    protected bool IsValid(SceneView sceneView)
    {
      return (enabled && isValid && sceneView.camera != null);
    }   


  }
}
