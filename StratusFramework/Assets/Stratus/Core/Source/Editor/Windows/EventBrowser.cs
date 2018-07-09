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
    private Vector2 scrollPosition;
    private static string displayTitle = "Event Browser";
    private Type[] events;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Core/Event Browser")]
    public static void Open()
    {
      EditorWindow.GetWindow(typeof(EventBrowser), false, displayTitle);
    }

    private void OnEnable()
    {
      events = Reflection.GetSubclass<Stratus.Event>();      
    }

    private void OnGUI()
    {
      EditorGUILayout.BeginVertical();
      this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, false, false);
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
      foreach(var e in events)
      {
        GUILayout.Label(e.Name);
      }
    }

    //------------------------------------------------------------------------/
    // Data
    //------------------------------------------------------------------------/


  }

}