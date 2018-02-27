using Stratus.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;

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

    /// <summary>
    /// Whether this display has been initialized
    /// </summary>
    protected bool initializedDisplay { get; private set; }

    /// <summary>
    /// Whether the display for this state has been initialized
    /// </summary>
    protected bool initializedState { get; private set; }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    protected abstract void OnInitializeDisplay();
    protected abstract void OnSceneGUI(SceneView view);
    protected abstract void OnInitializeState();

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
      //SceneView.onSceneGUIDelegate += OnFirstOnSceneGUICall;
      EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
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
        displays.Add(Activator.CreateInstance(displayType) as PersistentSceneViewDisplay);
      }

      // Now initialize them
      foreach (var display in displays)
      {
        display.InitializeDisplay();
        SceneView.onSceneGUIDelegate += display.SceneGUI;
        //EditorApplication.hierarchyWindowChanged += display.OnHierarchyWindowChanged;
        //EditorApplication.playModeStateChanged += display.OnPlayModeStateChanged;
      }
    }

    //private static void OnFirstOnSceneGUICall(SceneView sceneView)
    //{
    //  SceneView.onSceneGUIDelegate -= OnFirstOnSceneGUICall;
    //
    //  foreach (var display in displays)
    //  {
    //    display.InitializeState();
    //  }
    //}


    private static void OnHierarchyWindowChanged()
    {
      foreach (var display in displays)
      {
        display.InitializeState();
      }
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
    {
      foreach (var display in displays)
      {
        display.InitializeState();
      }
    }

    //------------------------------------------------------------------------/
    // Methods: Private
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes this display
    /// </summary>
    private void InitializeDisplay()
    {
      this.OnInitializeDisplay();
      initializedDisplay = true;
    }

    /// <summary>
    /// Initializes the state for the display, when valid
    /// </summary>
    private void InitializeState()
    {
      if (isValid)
      {
        OnInitializeState();
        initializedState = true;
      }
      else
      {
        initializedState = false;
      }
    }

    /// <summary>
    /// Receives the onSceneGUI callback from ScemeVoew
    /// </summary>
    /// <param name="sceneView"></param>
    private void SceneGUI(SceneView sceneView)
    {
      if (!initializedState)
        InitializeState();

      if (IsValid(sceneView))
        OnSceneGUI(sceneView);
    }

    /// <summary>
    /// Whether this display should be shown
    /// </summary>
    /// <param name="sceneView"></param>
    /// <returns></returns>
    protected bool IsValid(SceneView sceneView)
    {
      return enabled && isValid && sceneView.camera != null && initializedState;
    }

    //------------------------------------------------------------------------/
    // Methods: Public Static
    //------------------------------------------------------------------------/   
    //public static void DrawWireCubes<T>(T[] components, Color color) where T : MonoBehaviour
    //{
    //  Handles.color = color;
    //  foreach (var component in components)
    //  {
    //    Transform transform = component.transform;
    //    Vector3 pos = transform.position;
    //    Vector3 scale = transform.localScale;
    //    Handles.DrawWireCube(pos, scale);
    //  }
    //}

  }

}