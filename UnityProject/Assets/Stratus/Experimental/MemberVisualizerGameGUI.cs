using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;


namespace Stratus
{
  [Singleton("Member Visualizer Game GUI", true, true)]
  public class MemberVisualizerGameGUI : LayoutGameViewWindow<MemberVisualizerGameGUI>
  {
    protected override bool draw => MemberVisualizer.gameGUIDrawCount > 0;
    protected override string title { get; } = "Member Visualizer";
    
    protected override void OnAwake()
    {
      
    }

    protected override void OnGUILayout(Rect position)
    {
      foreach (var drawList in MemberVisualizer.gameGUIDrawLists)
      {
        foreach (var dl in drawList.Value)
        {
          GameObject go = dl.Key;
          GUILayout.Label($"{go.name}", StratusGUIStyles.header);
          foreach (var member in dl.Value)
          {
            //if (useCustomColors)
            //  GUILayout.Label($"<color={member.hexColor}>{member.description}</color>");
            //else
            GUILayout.Label($"{member.description}", StratusGUIStyles.miniText);
          }
        }
      }
    }

    public void Add(MemberVisualizer mv) { }
    public void Remove(MemberVisualizer mv) { }

  }

}