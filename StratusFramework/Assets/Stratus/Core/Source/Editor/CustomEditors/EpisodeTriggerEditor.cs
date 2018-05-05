using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  [CustomEditor(typeof(EpisodeTrigger))]
  public class EpisodeTriggerEditor : BehaviourEditor<EpisodeTrigger>
  {
    private ObjectDropdownList<Segment> segments;
    private SerializedProperty episodeProperty;

    protected override void OnStratusEditorEnable()
    {
      MakeSegmentList();
      episodeProperty = propertyMap["episode"];
      propertyChangeCallbacks.Add(episodeProperty, MakeSegmentList);
    }

    protected override bool DrawDeclaredProperties()
    {
      bool changed = false;
      changed |= DrawSerializedProperty(propertyMap["episode"]);

      if (segments != null)
      {
        segments.selectedIndex = EditorGUILayout.Popup("Segment", segments.selectedIndex, segments.displayedOptions);
        changed |= DrawSerializedProperty(propertyMap["eventType"]);
        target.segment = segments.selected;
      }
      return changed;
    }
    
    private void MakeSegmentList()
    {
      segments = null;

      if (!target.episode)
      {
        target.segment = null;
        return;
      }

      segments = new ObjectDropdownList<Segment>(target.episode.segments, target.segment);
    }

  }

}