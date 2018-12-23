using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Analytics
{
  [LayoutViewDisplayAttribute("Stratus Analytics", 225f, 200f, StratusGUI.Anchor.BottomLeft, StratusGUI.Dimensions.Absolute)]
  public class AnalyticsSceneViewDisplay : SingletonSceneViewDisplay<AnalyticsEngine>
  {
    //AnimationCurve curve = new AnimationCurve();

    protected override void OnGUI(Rect position)
    {
      
    }

    protected override void OnGUILayout(Rect position)
    {
      if (UnityEngine.Event.current.type != EventType.Repaint)
        return;

      //GL.Begin(GL.LINES);
      //EditorGUILayout.CurveField("Example", curve);
    }

    protected override void OnInitializeSingletonState()
    {
      
    }
  }

}