using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Stratus.Utilities;
using System;

namespace Stratus
{
  namespace AI
  {
    public class BehaviorBrowserWindow: EditorWindow
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      private Vector2 ScrollPosition;
      private static string Title = "Behavior Browser";

      private static StratusReflection.ClassList Composites;
      private static StratusReflection.ClassList Actions;
      private static StratusReflection.ClassList Decorators;

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      [MenuItem("Stratus/Experimental/AI/Behavior Browser %g")]
      public static void Open()
      {
        EditorWindow.GetWindow(typeof(BehaviorBrowserWindow), false, Title);
        Composites = StratusReflection.GenerateClassList<StratusAIComposite>();
        Actions = StratusReflection.GenerateClassList<StratusAITask>();
        Decorators = StratusReflection.GenerateClassList<StratusAIService>();
      }

      private void OnGUI()
      {
        EditorGUILayout.BeginVertical();
        this.ScrollPosition = EditorGUILayout.BeginScrollView(this.ScrollPosition, false, false);
        {
          ShowClasses("Composites", Composites);
          ShowClasses("Actions", Actions);
          ShowClasses("Decorators", Decorators);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
      }

      private void ShowClasses(string header, StratusReflection.ClassList list)
      {
        GUILayout.Label(header, EditorStyles.boldLabel);
        foreach (var e in list)
        {
          GUILayout.Label(e.Key);
        }
      }

    } 
  } 
}
