using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [LayoutViewDisplayAttribute("Member Visualizer", 225f, 200f, StratusGUI.Anchor.BottomRight, StratusGUI.Dimensions.Absolute)]
  public class MemberVisualizerSceneDisplay : LayoutSceneViewDisplay
  {
    //private bool useCustomColors = false;
    protected override bool isValid => MemberVisualizer.sceneGUIDrawCount > 0;
    
    private GUIStyle textStyle;

    protected override void OnInitializeDisplay()
    {
      base.OnInitializeDisplay();
    }

    protected override void OnGUI(Rect position)
    {
      
    }

    protected override void OnGUILayout(Rect position)
    {
      
      foreach(var drawList in MemberVisualizer.sceneGUIDrawLists)
      {
        foreach (var dl in drawList.Value)
        {
          GameObject go = dl.Key;
          GUILayout.Label($"{go.name}", EditorStyles.centeredGreyMiniLabel);
          foreach (var member in dl.Value)
          {            
            //if (useCustomColors)
            //  GUILayout.Label($"<color={member.hexColor}>{member.description}</color>");
            //else
              GUILayout.Label($"{member.description}", StratusGUIStyles.miniText);
          }
        }
      }      
      
      //useCustomColors = GUILayout.Toggle(useCustomColors, "Custom Colors");
    }

    protected override void OnInitializeState()
    {
      
    }
  }

}