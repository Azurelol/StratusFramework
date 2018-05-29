using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// A trigger for events within the episode system
  /// </summary>
  public class EpisodeTrigger : Trigger
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public Episode episode;
    [Tooltip("The segment to check against")]
    public Segment segment;
    [Tooltip("What type to event to listen for")]
    public Segment.EventType eventType;
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
      if (eventType == Segment.EventType.Enter)
        Scene.Connect<Segment.EnteredEvent>(this.OnSegmentEnteredEvent);
      else if (eventType == Segment.EventType.Exit)
        Scene.Connect<Segment.ExitedEvent>(this.OnSegmentExitedEvent);
    }

    protected override void OnReset()
    {
      segment = GetComponent<Segment>();
      episode = segment.episode;
    }    

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/

    void OnSegmentEnteredEvent(Segment.EnteredEvent e)
    {
      if (ValidateSegment(e.segment))
        Activate();
    }

    void OnSegmentExitedEvent(Segment.ExitedEvent e)
    {
      if (ValidateSegment(e.segment))
        Activate();
    }

    bool ValidateSegment(Segment segment)
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