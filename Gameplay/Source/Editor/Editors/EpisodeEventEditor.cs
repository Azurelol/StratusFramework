using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusEpisodeEvent))]
  public class EpisodeEventEditor : TriggerableEditor<StratusEpisodeEvent>
  {
    private ObjectDropdownList<StratusSegmentBehaviour> segments;
    private ObjectDropdownList<StratusPositionCheckpoint> checkpoints;
    private SerializedProperty episodeProperty;
    private SerializedProperty segmentProperty;

    protected override void OnTriggerableEditorEnable()
    {
      MakeSegmentList();
      episodeProperty = propertyMap["episode"];
      segmentProperty = propertyMap["segment"];
      propertyChangeCallbacks.Add(episodeProperty, MakeSegmentList);
      propertyChangeCallbacks.Add(segmentProperty, MakeCheckpointList);
    }

    protected override bool DrawDeclaredProperties()
    {
      bool changed = false;
      changed |= DrawSerializedProperty(propertyMap["episode"]);

      if (segments != null)
      {

        EditorGUI.BeginChangeCheck();
        segments.selectedIndex = EditorGUILayout.Popup("Segment", segments.selectedIndex, segments.displayedOptions);
        if (EditorGUI.EndChangeCheck())
        {
          changed = true;
          triggerable.segment = segments.selected;
          MakeCheckpointList();
        }
      }

      if (checkpoints != null)
      {
        EditorGUI.BeginChangeCheck();
        checkpoints.selectedIndex = EditorGUILayout.Popup("Checkpoint", checkpoints.selectedIndex, checkpoints.displayedOptions);
        if (EditorGUI.EndChangeCheck())
        {
          changed = true;
          triggerable.checkpointIndex = checkpoints.selectedIndex;
        }
      }


      changed |= DrawSerializedProperty(propertyMap["eventType"]);
      changed |= DrawSerializedProperty(propertyMap["jump"]);
      return changed;
    }

    private void MakeSegmentList()
    {
      segments = null;

      if (!triggerable.episode)
      {
        triggerable.segment = null;
        return;
      }

      segments = new ObjectDropdownList<StratusSegmentBehaviour>(triggerable.episode.segments, triggerable.segment);
      MakeCheckpointList();
    }

    private void MakeCheckpointList()
    {
      checkpoints = null;
      if (!triggerable.segment)
        return;
      checkpoints = new ObjectDropdownList<StratusPositionCheckpoint>(triggerable.segment.checkpoints, triggerable.checkpointIndex);
    }


  }

}