using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Stratus.Utilities;
using System;

namespace Stratus
{
  /// <summary>
  /// Displays all present derived Stratus events in the assembly
  /// </summary>
  public class EventBrowser : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private Vector2 ScrollPosition;
    private static string Title = "Event Browser";
    private static Reflection.ClassList Events;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Windows/Event Browser")]
    public static void Open()
    {
      EditorWindow.GetWindow(typeof(EventBrowser), false, Title);
      Events = Reflection.GenerateClassList<Stratus.Event>();
    }

    private void OnGUI()
    {
      EditorGUILayout.BeginVertical();
      this.ScrollPosition = EditorGUILayout.BeginScrollView(this.ScrollPosition, false, false);
      ShowEvents();
      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
    }

    //------------------------------------------------------------------------/
    // GUI
    //------------------------------------------------------------------------/
    private void ShowEvents()
    {
      GUILayout.Label("Events", EditorStyles.boldLabel);
      foreach(var e in Events)
      {
        GUILayout.Label(e.Key);
      }
    }

    //------------------------------------------------------------------------/
    // Data
    //------------------------------------------------------------------------/


  }

}