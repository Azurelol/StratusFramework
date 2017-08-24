using Stratus.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Derive from this class to create a scene view display
  /// </summary>
  [InitializeOnLoad]
  public abstract class PersistentSceneViewDisplay
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this window should currently show
    /// </summary>
    public bool enabled { get; protected set; } = true;

    /// <summary>
    /// Whether the current window should be displayed in the SceneView
    /// </summary>
    protected abstract bool isValid { get; }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    protected abstract void OnInitialize();
    protected abstract void OnSceneGUI(SceneView view);
    protected abstract void OnHierarchyWindowChanged();

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private static List<PersistentSceneViewDisplay> displays = new List<PersistentSceneViewDisplay>();

    //------------------------------------------------------------------------/
    // Procedures: Static
    //------------------------------------------------------------------------/
    static PersistentSceneViewDisplay()
    {
      ConstructAllDisplays();
    }

    /// <summary>
    /// Constructs and initializes all declared non-abstract derived displays.
    /// This will add them to the editor's SceneView GUI delegate
    /// </summary>
    private static void ConstructAllDisplays()
    {
      // Get a list of all display classes, then construct them
      Type[] displayClasses = Reflection.GetSubclass<PersistentSceneViewDisplay>();
      foreach (var displayType in displayClasses)
      {
        //Trace.Script("Adding display for " + displayType.Name);
        displays.Add(Activator.CreateInstance(displayType) as PersistentSceneViewDisplay);
      }
      // Now initialize them
      foreach (var display in displays)
      {
        display.Initialize();
        SceneView.onSceneGUIDelegate += display.OnSceneGUI;
        EditorApplication.hierarchyWindowChanged += display.OnHierarchyWindowChanged;
        EditorApplication.playmodeStateChanged += display.OnHierarchyWindowChanged;
      }
    }

    //------------------------------------------------------------------------/
    // Procedures: Static
    //------------------------------------------------------------------------/
    private void Initialize()
    {
      this.OnInitialize();
    }    
    
    //------------------------------------------------------------------------/
    // Methods: Public Static
    //------------------------------------------------------------------------/   
    public static void DrawWireCubes<T>(T[] components, Color color) where T : MonoBehaviour
    {
      Handles.color = color;
      foreach (var component in components)
      {
        Transform transform = component.transform;
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;
        //var rot = transform.rotation;

        Handles.DrawWireCube(pos, scale);
      }
    }

  }

}