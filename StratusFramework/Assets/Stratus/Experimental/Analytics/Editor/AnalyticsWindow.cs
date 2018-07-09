using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Analytics
{
  public class AnalyticsWindow : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private Vector2 ScrollPosition;
    private AnimationCurve curve = new AnimationCurve();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Experimental/Analytics")]
    public static void Open()
    {
      EditorWindow.GetWindow(typeof(AnalyticsWindow), false, "Stratus Analytics");
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnEnable()
    {
      
    }

    private void OnGUI()
    {
      EditorGUILayout.BeginVertical();
      this.ScrollPosition = EditorGUILayout.BeginScrollView(this.ScrollPosition, false, false);
      {
        DrawVisualization();
      }
      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
    }

    private void DrawVisualization()
    {
      EditorGUILayout.CurveField("Example", curve);      
    }

  }
}