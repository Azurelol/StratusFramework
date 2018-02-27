using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  [CustomEditor(typeof(EpisodeTrigger))]
  public class EpisodeTriggerEditor : BaseEditor<EpisodeTrigger>
  {
    private DropdownList<Segment> segments;
    private SerializedProperty episodeProperty;

    protected override void OnBaseEditorEnable()
    {
      MakeSegmentList();
      episodeProperty = propertyMap["episode"];
      propertyChangeCallbacks.Add(episodeProperty, MakeSegmentList);
    }

    protected override void DrawDeclaredProperties()
    {
      DrawSerializedProperty(propertyMap["episode"]);

      if (segments != null)
      {
        segments.selectedIndex = EditorGUILayout.Popup("Segment", segments.selectedIndex, segments.displayedOptions);
        DrawSerializedProperty(propertyMap["eventType"]);
        target.segment = segments.selected;
      }
    }
    
    private void MakeSegmentList()
    {
      segments = null;

      if (!target.episode)
      {
        target.segment = null;
        return;
      }

      segments = new DropdownList<Segment>(target.episode.segments, target.segment);
    }

  }

}