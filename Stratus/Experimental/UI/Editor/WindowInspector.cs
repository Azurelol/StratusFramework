/******************************************************************************/
/*!
@file   WindowInspector.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Stratus.UI;

namespace Stratus
{
  /// <summary>
  /// Allows Stratus.UI Windows to be inspected
  /// </summary>
  public class WindowInspector : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The current window being inspected
    /// </summary>
    private Window Target;

    /// <summary>
    /// The previous window selected
    /// </summary>
    private Window PreviousTarget;

    /// <summary>
    /// The windows currently available to inspect
    /// </summary>
    private Window[] AvailableWindows;

    /// <summary>
    /// The width of the left panel
    /// </summary>
    float LeftPanelWidth;

    /// <summary>
    /// The width of the right panel
    /// </summary>
    float RightPanelWidth;

    /// <summary>
    /// The height of the window
    /// </summary>
    //private float Height;

    /// <summary>
    /// The left panel
    /// </summary>
    Rect LeftPanel;
    
    /// <summary>
    /// The right panel
    /// </summary>
    Rect RightPanel;

    /// <summary>
    /// The current position of the scrollbar
    /// </summary>
    Vector2 ScrollingPosition;

    GUIStyle SelectableStyle;
    GUIStyle ButtonStyle;
    GUIStyle TitleStyle;
    GUIStyle HeaderStyle;

    /// <summary>
    /// The margin to use for content inside panels
    /// </summary>
    float Margin = 0.025f;

    private static string Title = "Stratus UI | Window Inspector";

    //------------------------------------------------------------------------/
    // Menu
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Experimental/UI/Window Inspector")]
    public static void Open()
    {
      var window = (WindowInspector)EditorWindow.GetWindow(typeof(WindowInspector), false, Title);
      window.Show();
      window.SelectableStyle = EditorStyles.helpBox;
      window.SelectableStyle.onActive = EditorStyles.toolbarButton.onActive;
      window.ButtonStyle = EditorStyles.toolbarButton;
      window.ButtonStyle.margin.left = 5;
      window.ButtonStyle.margin.right = 5;
      window.TitleStyle = EditorStyles.whiteLargeLabel;
      window.HeaderStyle = EditorStyles.boldLabel;
    }

    private void OnEnable()
    {
      EditorApplication.playmodeStateChanged += OnApplicationStateChanged;
      Reset();
    }

    private void OnDisable()
    {
      EditorApplication.playmodeStateChanged -= OnApplicationStateChanged;
    }

    void OnApplicationStateChanged()
    {
      if (Application.isPlaying)
      {
        //Trace.Script("Playing again!");
        Reset();
      }
    }

    void Reset()
    {
      //Trace.Script("Resetting. Finding new windows");
      Target = null;
      FindAllWindows();
    }
    
    //------------------------------------------------------------------------/
    // GUI
    //------------------------------------------------------------------------/
    private void OnGUI()
    {
      if (!IsPlaying()) return;
      //Height = position.height;
      DrawLeftPanel();
      DrawRightPanel();
    }

    private void DrawLeftPanel()
    {
      LeftPanelWidth = position.width * 0.25f;
      LeftPanel = new Rect(0, 0, LeftPanelWidth, position.height);
      GUILayout.BeginArea(LeftPanel, EditorStyles.helpBox);
      GUILayout.BeginVertical();
      SelectWindow();
      GUILayout.EndVertical();
      GUILayout.EndArea();
    }

    private void DrawRightPanel()
    {
      RightPanelWidth = position.width * 0.75f;
      RightPanel = new Rect((LeftPanelWidth) + (position.width * Margin),
                      (position.height * Margin),
                      RightPanelWidth - (position.width * (Margin * 2)),
                      position.height);
      GUILayout.BeginArea(RightPanel);

      // If there's selected window
      if (Target)
      {
        ShowWindowDetails();
        ShowOptions();
      }

      GUILayout.EndArea();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Displays all available windows in a scrolling list, allowing any given one to be selected
    /// </summary>
    void SelectWindow()
    {
      ScrollingPosition = GUILayout.BeginScrollView(ScrollingPosition, false, false, 
                                                          GUILayout.Width(LeftPanelWidth), 
                                                          GUILayout.Height(position.height));
      foreach (var window in AvailableWindows)
      {
        if (window == null)
          continue;

        if (GUILayout.Button(window.name, SelectableStyle))
        {
          SelectWindow(window);
        }
      }
      GUILayout.EndScrollView();
    }

    /// <summary>
    /// Shows details about the currently selected window
    /// </summary>
    void ShowWindowDetails()
    {
      // Header
      GUILayout.Label(Target.name, TitleStyle);
      // Options
      GUILayout.Label("Properties", HeaderStyle);
      Target.Tracing = EditorGUILayout.Toggle("Tracing", Target.Tracing);
      EditorGUILayout.Toggle("Is Accepting Input", Target.IsAcceptingInput);
      // Links
      if (Target.Controller != null)
      {
        GUILayout.Label("Links", HeaderStyle);
        foreach(var link in Target.Controller.Links)
          GUILayout.Label(link.name);
      }
      // Current Link
      if (Target.CurrentLink != null)
      {
        GUILayout.Label("Current Link", HeaderStyle);
        GUILayout.Label(Target.CurrentLink.name);
      }

    }

    /// <summary>
    /// Shows all available options for this window
    /// </summary>
    void ShowOptions()
    {
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Open", ButtonStyle))
        OpenWindow(); 
      GUILayout.Space(2.5f);
      if (GUILayout.Button("Close", ButtonStyle))
        CloseWindow();
        
      GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Finds all available windows in the scene
    /// </summary>
    void FindAllWindows()
    {
      AvailableWindows = (Window[])FindObjectsOfType(typeof(Window));
    }

    /// <summary>
    /// Selects the current window
    /// </summary>
    void SelectWindow(Window window)
    {
      // If another window was previously selected, close it
      //if (Target != null && Target != window)
      //  CloseWindow();

      Target = window;
    }

    /// <summary>
    /// Opens the target window
    /// </summary>
    void OpenWindow()
    {
      Target.RequestOpen();
    }

    /// <summary>
    /// Closes the target window
    /// </summary>
    void CloseWindow()
    {
      Trace.Script("Close");
      Target.RequestClose();
    }

    /// <summary>
    /// Detects whether the editor is currently in playmode. If not, shows an error notification until is
    /// </summary>
    /// <returns></returns>
    bool IsPlaying()
    {
      if (Application.isPlaying)
        return true;

      ShowNotification(new GUIContent("You need to be in playmode in order to inspect windows!"));

      return false;
    }

  }
}