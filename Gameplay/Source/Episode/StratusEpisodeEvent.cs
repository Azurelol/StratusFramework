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
    public StratusEpisodeBehaviour episode;
    [Tooltip("The segment to check against")]
    public StratusSegmentBehaviour segment;
    [Tooltip("What type to event to listen for")]
    public StratusSegmentBehaviour.EventType eventType;
    [Tooltip("Whether to translate the player onto the segment")]
    [DrawIf("eventType", StratusSegmentBehaviour.EventType.Enter, ComparisonType.Equals)]
    public bool jump = true;
    [Tooltip("To what checkpoint within the segment to transport the player to")]
    [DrawIf("eventType", StratusSegmentBehaviour.EventType.Enter, ComparisonType.Equals)]
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
        StratusDebug.LogError($"No segment has been set for this event!", this);
        return;
      }

      switch (eventType)
      {
        case StratusSegmentBehaviour.EventType.Enter:
          episode.Enter(segment, jump, checkpointIndex);
          break;
        case StratusSegmentBehaviour.EventType.Exit:
          episode.Exit(segment);
          break;
        default:
          break;
      }
    }

    public override StratusObjectValidation Validate()
    {
      if (segment == null)
      {
        return new StratusObjectValidation(ComposeLog("There is no segment set!"), StratusObjectValidation.Level.Error, this);
      }

      return null;
    }
  }

}