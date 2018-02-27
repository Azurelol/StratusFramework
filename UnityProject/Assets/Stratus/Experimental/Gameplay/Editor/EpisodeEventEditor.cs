using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(EpisodeEvent))]
  public class EpisodeEventEditor : BaseEditor<EpisodeEvent>
  {
    private DropdownList<Segment> segments;
    private DropdownList<Checkpoint> checkpoints;
    private SerializedProperty episodeProperty;
    private SerializedProperty segmentProperty;

    protected override void OnBaseEditorEnable()
    {
      MakeSegmentList();
      episodeProperty = propertyMap["episode"];
      segmentProperty = propertyMap["segment"];
      propertyChangeCallbacks.Add(episodeProperty, MakeSegmentList);
      propertyChangeCallbacks.Add(segmentProperty, MakeCheckpointList);
    }

    protected override void DrawDeclaredProperties()
    {
      DrawSerializedProperty(propertyMap["episode"]);

      if (segments != null)
      {

        EditorGUI.BeginChangeCheck();
        segments.selectedIndex = EditorGUILayout.Popup("Segment", segments.selectedIndex, segments.displayedOptions);
        if (EditorGUI.EndChangeCheck())
        {
          target.segment = segments.selected;
          MakeCheckpointList();
        }
      }

      if (checkpoints != null)
      {
        EditorGUI.BeginChangeCheck();
        checkpoints.selectedIndex = EditorGUILayout.Popup("Checkpoint", checkpoints.selectedIndex, checkpoints.displayedOptions);
        if (EditorGUI.EndChangeCheck())
        {
          target.checkpointIndex = checkpoints.selectedIndex;
        }
      }


      DrawSerializedProperty(propertyMap["eventType"]);
      DrawSerializedProperty(propertyMap["jump"]);

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
      MakeCheckpointList();
    }

    private void MakeCheckpointList()
    {
      checkpoints = null;
      if (!target.segment)
        return;
      checkpoints = new DropdownList<Checkpoint>(target.segment.checkpoints, target.checkpointIndex);
    }

  }

}