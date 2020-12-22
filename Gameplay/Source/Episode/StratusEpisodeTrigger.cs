using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
  /// <summary>
  /// A trigger for events within the episode system
  /// </summary>
  public class StratusEpisodeTrigger : StratusTriggerBehaviour
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public StratusEpisodeBehaviour episode;
    [Tooltip("The segment to check against")]
    public StratusSegmentBehaviour segment;
    [Tooltip("What type to event to listen for")]
    public StratusSegmentBehaviour.EventType eventType;
    //[Tooltip("How to refer to the segment ")]
    //public Segment.ReferenceType referenceType = Segment.ReferenceType.Reference;
    //[Tooltip("The label of the segment to check against")]
    //[DrawIf("referenceType", Segment.ReferenceType.Label, ComparisonType.Equals)]
    //public string segmentLabel;
    //[DrawIf("referenceType", Segment.ReferenceType.Reference, ComparisonType.Equals)]

    public override string automaticDescription
    {
      get
      {
        if (segment != null)
          return $"On {eventType} {episode.label}.{segment.label}";
        return string.Empty;
      }
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      if (eventType == StratusSegmentBehaviour.EventType.Enter)
        StratusScene.Connect<StratusSegmentBehaviour.EnteredEvent>(this.OnSegmentEnteredEvent);
      else if (eventType == StratusSegmentBehaviour.EventType.Exit)
        StratusScene.Connect<StratusSegmentBehaviour.ExitedEvent>(this.OnSegmentExitedEvent);
    }

    protected override void OnReset()
    {
      segment = GetComponent<StratusSegmentBehaviour>();
      if (segment)
        episode = segment.episode;
    }    

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/

    void OnSegmentEnteredEvent(StratusSegmentBehaviour.EnteredEvent e)
    {
      if (ValidateSegment(e.segment))
        Activate();
    }

    void OnSegmentExitedEvent(StratusSegmentBehaviour.ExitedEvent e)
    {
      if (ValidateSegment(e.segment))
        Activate();
    }

    bool ValidateSegment(StratusSegmentBehaviour segment)
    {
      return this.segment == segment;
      //if (referenceType == ReferenceType.Label)
      //  return this.segmentLabel == segment.label;
      //else if (referenceType == ReferenceType.Reference)
      //  return this.segment = segment;
      //return false;
    }


  }

}