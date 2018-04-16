using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// An event that handles transitions between segments within an episode
  /// </summary>
  public class EpisodeEvent : Triggerable
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public Episode episode;
    [Tooltip("The segment to check against")]
    public Segment segment;
    [Tooltip("What type to event to listen for")]
    public Segment.EventType eventType;
    [Tooltip("Whether to translate the player onto the segment")]
    [DrawIf("eventType", Segment.EventType.Enter, ComparisonType.Equals)]
    public bool jump = true;
    [Tooltip("To what checkpoint within the segment to transport the player to")]
    [DrawIf("eventType", Segment.EventType.Enter, ComparisonType.Equals)]
    public int checkpointIndex;


    public override string automaticDescription
    {
      get
      {
        if (segment != null)
          return $"{eventType} {episode.label}.{segment.label}.{segment.checkpoints[checkpointIndex].name}";
        return string.Empty;
      }
    }


    protected override void OnAwake()
    {
    }

    protected override void OnReset()
    {      
    }

    protected override void OnTrigger()
    {
      if (segment == null)
      {
        Trace.Error($"No segment has been set for this event!", this);
        return;
      }

      switch (eventType)
      {
        case Segment.EventType.Enter:
          episode.Enter(segment, jump, checkpointIndex);
          break;
        case Segment.EventType.Exit:
          episode.Exit(segment);
          break;
        default:
          break;
      }
    }

    public override Validation Validate()
    {
      if (segment == null)
      {
        return new Validation(ComposeLog("There is no segment set!"), Validation.Level.Error, this);
      }

      return null;
    }
  }

}