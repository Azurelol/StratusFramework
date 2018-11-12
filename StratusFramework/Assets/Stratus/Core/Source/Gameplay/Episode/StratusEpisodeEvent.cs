using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
  /// <summary>
  /// An event that handles transitions between segments within an episode
  /// </summary>
  public class StratusEpisodeEvent : StratusTriggerable
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public StratusEpisode episode;
    [Tooltip("The segment to check against")]
    public StratusSegment segment;
    [Tooltip("What type to event to listen for")]
    public StratusSegment.EventType eventType;
    [Tooltip("Whether to translate the player onto the segment")]
    [DrawIf("eventType", StratusSegment.EventType.Enter, ComparisonType.Equals)]
    public bool jump = true;
    [Tooltip("To what checkpoint within the segment to transport the player to")]
    [DrawIf("eventType", StratusSegment.EventType.Enter, ComparisonType.Equals)]
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
        StratusDebug.Error($"No segment has been set for this event!", this);
        return;
      }

      switch (eventType)
      {
        case StratusSegment.EventType.Enter:
          episode.Enter(segment, jump, checkpointIndex);
          break;
        case StratusSegment.EventType.Exit:
          episode.Exit(segment);
          break;
        default:
          break;
      }
    }

    public override ObjectValidation Validate()
    {
      if (segment == null)
      {
        return new ObjectValidation(ComposeLog("There is no segment set!"), ObjectValidation.Level.Error, this);
      }

      return null;
    }
  }

}