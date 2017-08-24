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

      private static Reflection.ClassList Composites;
      private static Reflection.ClassList Actions;
      private static Reflection.ClassList Decorators;

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      [MenuItem("Stratus/Experimental/AI/Behavior Browser %g")]
      public static void Open()
      {
        EditorWindow.GetWindow(typeof(BehaviorBrowserWindow), false, Title);
        Composites = Reflection.GenerateClassList<Composite>();
        Actions = Reflection.GenerateClassList<Action>();
        Decorators = Reflection.GenerateClassList<Decorator>();
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

      private void ShowClasses(string header, Reflection.ClassList list)
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
