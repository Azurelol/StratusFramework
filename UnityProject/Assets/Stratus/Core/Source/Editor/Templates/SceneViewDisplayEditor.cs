using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Stratus.Utilities;

namespace Stratus
{
  [InitializeOnLoad]
  public class SceneViewDisplayEditorWindow : EditorWindow
  {
    protected const string displayTitle = "Scene View Displays";
    private Dictionary<SceneViewDisplay, bool> expanded;
    private bool globalExpanded;

    static SceneViewDisplayEditorWindow()
    {
    }

    [MenuItem("Stratus/Windows/Scene View Display")]
    public static void Open()
    {
      EditorWindow.GetWindow(typeof(SceneViewDisplayEditorWindow), true, displayTitle);
    }

    private void OnEnable()
    {
      expanded = new Dictionary<SceneViewDisplay, bool>();
      foreach (var display in SceneViewDisplay.displays)
      {
        expanded[display] = false;
      }
    }

    private void OnGUI()
    {
      EditorGUILayout.BeginVertical(StratusGUIStyles.box);
      globalExpanded = EditorGUILayout.Foldout(globalExpanded, "Global");
      if (globalExpanded)
      {
        EditorGUI.indentLevel++;
        SceneViewDisplay.InspectGlobal();
        EditorGUI.indentLevel--;
      }
     EditorGUILayout.EndVertical();

      EditorGUILayout.BeginVertical(StratusGUIStyles.box);
      foreach (var display in SceneViewDisplay.displays)
      {
        expanded[display] = EditorGUILayout.Foldout(expanded[display], display.name);
        if (expanded[display])
        {
          EditorGUI.indentLevel++;
          EditorGUILayout.BeginVertical(StratusGUIStyles.box);
          display.Inspect();
          EditorGUILayout.EndVertical();
          EditorGUI.indentLevel--;
        }
      }
      EditorGUILayout.EndVertical();
    }

  }

}