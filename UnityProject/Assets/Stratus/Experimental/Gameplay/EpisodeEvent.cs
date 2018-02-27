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
    
    protected override void OnAwake()
    {
    }

    protected override void OnReset()
    {
      
    }

    protected override void OnTrigger()
    {
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
  }

}