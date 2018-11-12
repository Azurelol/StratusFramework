using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using UnityEngine.Events;
using UnityEngine.AI;
using Stratus.Interfaces;
using System;

namespace Stratus.Gameplay
{
  /// <summary>
  /// A gameplay episode system, consisting of multiple segments
  /// </summary>
  [ExecuteInEditMode]
  public class StratusEpisode : Multiton<StratusEpisode>, Debuggable, ValidatorAggregator
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public enum JumpMechanism
    {
      Translate,
      Callback,
      [Tooltip("Dispatches an event signaling the segment has been jumped to")]
      Event
    }

    public enum SegmentManagementMode
    {
      [Tooltip("Only one segment and its trigger system are active a time")]
      Singular,
      [Tooltip("Multiple segments can be active at a given time")]
      Concurrent
    }

    public enum Scope
    {
      Segment,
      Episode
    }

    public enum Mode
    {
      [Tooltip("Episode is fully operational")]
      Live,
      [Tooltip("The behaviours in this episode are disabled")]
      Suspended
    }

    /// <summary>
    /// A callback consisting of the segment that has been jumped to, and the index of the checkpoint
    /// </summary>
    [System.Serializable]
    public class SegmentCallback : UnityEvent<StratusSegment, int> { }

    /// <summary>
    /// Base class for episode events
    /// </summary>
    public abstract class EpisodeEvent : Stratus.StratusEvent
    {
      public StratusEpisode episode { get; internal set; }
    }

    /// <summary>
    /// Signals that this episode has started
    /// </summary>
    public class BeginEvent : EpisodeEvent { }

    /// <summary>
    /// Signals that this episdoe has ended
    /// </summary>
    public class EndEvent : EpisodeEvent { }

    /// <summary>
    /// Signals that a gameplay segment has been jumped to
    /// </summary>
    public class JumpToSegmentEvent : EpisodeEvent
    {
      public StratusSegment segment { get; private set; }
      public int checkpointIndex { get; private set; }
      public Vector3 position { get; private set; }

      public JumpToSegmentEvent(StratusSegment segment, int checkpointIndex, Vector3 position)
      {
        this.segment = segment;
        this.checkpointIndex = checkpointIndex;
        this.position = position;
      }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("The segments within this episode")]
    public List<StratusSegment> segments;
    [Tooltip("The initial segment")]
    public StratusSegment initialSegment;
    [Tooltip("How segments are managed within this episode")]
    public SegmentManagementMode segmentManagement;
    [Tooltip("Whether to begin this episode on awake")]
    public bool beginOnAwake = false;
    [Tooltip("The current mode for this episode")]
    public Mode mode = Mode.Live;

    [Header("Jump Configuration")]
    [Tooltip("What to do on a segment jump")]
    public JumpMechanism mechanism;
    public Transform targetTransform;
    public SegmentCallback onJump = new SegmentCallback();

    [Header("Debug")]
    public bool debugDisplay = false;
    [DrawIf(nameof(StratusEpisode.debugDisplay), true, ComparisonType.Equals)]
    public Color debugTextColor = Color.white;
    [Tooltip("Whether to allow the navigation of segments using input")]
    public bool debugNavigation = false;
    //[DrawIf(nameof(Episode.debugNavigation), true, ComparisonType.Equals)]
    public StratusGUI.Anchor windowAnchor = StratusGUI.Anchor.BottomRight;
    //[DrawIf(nameof(Episode.debugNavigation), true, ComparisonType.Equals)]
    public InputBinding nextSegmentInput = new InputBinding(KeyCode.PageDown);
    //[DrawIf(nameof(Episode.debugNavigation), true, ComparisonType.Equals)]
    public InputBinding previousSegmentInput = new InputBinding(KeyCode.PageUp);
    [SerializeField]
    private string debugTextColorHex;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The currently active episode
    /// </summary>
    public static StratusEpisode current { get; private set; }
    /// <summary>
    /// The currently active segment in this episode
    /// </summary>
    public StratusSegment currentSegment => segments != null ? segments[currentSegmentIndex] : null;
    /// <summary>
    /// The previous segment
    /// </summary>
    public StratusSegment previousSegment => currentSegmentIndex > 0 ? segments[currentSegmentIndex - 1] : null;
    /// <summary>
    /// The next segment
    /// </summary>
    public StratusSegment nextSegment => currentSegmentIndex < (segments.Count - 1) ? segments[currentSegmentIndex + 1] : null;
    /// <summary>
    /// The index of the current segment
    /// </summary>
    private int currentSegmentIndex { get; set; }
    /// <summary>
    /// The amount of episoddes
    /// </summary>
    public static List<StratusEpisode> visible { get; set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      if (initialSegment)
        currentSegmentIndex = GetSegmentIndex(initialSegment);
    }

    private void Start()
    {
      if (Application.isPlaying)
      {
        if (beginOnAwake)
          Begin();
      }
    }

    protected override void OnMultitonEnable()
    {
    }

    protected override void OnMultitonDisable()
    {
    }

    protected override void OnReset()
    {      
    }

    private void Update()
    {
      if (debugNavigation && Application.isPlaying)
        CheckDebugInput();
    }

    private void OnGUI()
    {
      if (debugDisplay && Application.isPlaying)
        DrawVisualization();
    }

    private void Reset()
    {
    }

    private void OnValidate()
    {
      debugTextColorHex = debugTextColor.ToHex();
      if (segments.Count == 1)
        initialSegment = segments[0];
      if (initialSegment)
        SetInitialSegment(initialSegment);
    }

    //------------------------------------------------------------------------/
    // Methods: Public
    //------------------------------------------------------------------------/
    /// <summary>
    /// Begins this episode
    /// </summary>
    public void Begin()
    {
      current = this;
      if (debugDisplay)
        StratusDebug.Log($"Beginning this episode at {initialSegment.label}", this);

      Enter(currentSegment, true);
      Scene.Dispatch<BeginEvent>(new BeginEvent() { episode = this });
    }

    /// <summary>
    /// Restarts to the initial segment
    /// </summary>
    public void Restart(bool jump = true)
    {
      Enter(initialSegment, jump);
    }

    /// <summary>
    /// Restarts to the beginning of current segment
    /// </summary>
    public void RestartCurrentSegment(bool jump = true)
    {
      Enter(currentSegment, jump);
      //currentSegment.Restart();
      //if (jump)
      //  Jump(currentSegment);
    }

    /// <summary>
    /// Ends this episode
    /// </summary>
    public void End()
    {
      current = null;
      Scene.Dispatch<EndEvent>(new EndEvent() { episode = this });
    }

    /// <summary>
    /// Enters the specified (by label) segment within this episode if its present
    /// </summary>
    /// <param name="label">The label for the segment</param>
    /// <param name="jump">Whether to teleport the player to the checkpoint</param>
    /// <param name="checkpointIndex"></param>
    public bool Enter(string label, bool jump, int checkpointIndex = 0)
    {
      if (!StratusSegment.available.ContainsKey(label))
      {
        StratusDebug.Error($"The segment labeled {label} is not present at this time!", this);
        return false;
      }

      StratusSegment segment = StratusSegment.available[label];
      Enter(segment, jump, checkpointIndex);
      return true;
    }

    /// <summary>
    /// Jumps to the specified segment within this episode
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="jump">Whether to teleport the player to the checkpoint</param>
    /// <param name="checkpointIndex">The checkpoint to jump to</param>
    public void Enter(StratusSegment segment, bool jump, int checkpointIndex = 0)
    {
      if (!Application.isPlaying)
        return;

      if (debugDisplay)
        StratusDebug.Log($"Entering the segment {segment}, checkpoint {checkpointIndex}", this);

      // Toggle the current segment on
      ToggleCurrentSegment();

      // Enter the segment
      currentSegmentIndex = GetSegmentIndex(segment);
      segment.Enter(mode == Mode.Suspended);

      // Now warp the player to it
      if (jump)
        Jump(segment, checkpointIndex);
    }

    /// <summary>
    /// Jumps the player character onto the segment's checkpoint
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="checkpointIndex"></param>
    public void Jump(StratusSegment segment, int checkpointIndex = 0)
    {
      Vector3 position = segment.checkpoints[checkpointIndex].transform.position;
      switch (mechanism)
      {
        case JumpMechanism.Translate:
          var navigation = targetTransform.GetComponent<NavMeshAgent>();
          if (navigation != null)
            navigation.Warp(position);
          else
            targetTransform.position = position;
          break;

        case JumpMechanism.Callback:
          onJump?.Invoke(segment, checkpointIndex);
          break;

        case JumpMechanism.Event:
          Scene.Dispatch<JumpToSegmentEvent>(new JumpToSegmentEvent(segment, checkpointIndex, position) { episode = this });
          break;
        default:
          break;
      }
    }
    
    /// <summary>
    /// Exits this segment
    /// </summary>
    /// <param name="segment"></param>
    public void Exit(StratusSegment segment)
    {
      segment.Exit();
    }

    /// <summary>
    /// Sets the initial segment for this episode
    /// </summary>
    /// <param name="segment"></param>
    public void SetInitialSegment(StratusSegment initialSegment)
    {
      // Toggle the selected segment
      this.initialSegment = initialSegment;

      foreach (var segment in segments)
      {
        segment.episode = this;
        //if (segment != null && currentSegment)
        //  segment.Toggle(false);
      }

      //ToggleCurrentSegment();

      //initialSegment.Toggle(true);
      //// Toggle off all other ones
      //foreach (var segment in segments)
      //{
      //  segment.episode = this;
      //  if (segment != null && initialSegment)
      //    segment.Toggle(false);
      //}
    }

    /// <summary>
    /// Gets the index of a given segment, provided its part of this episode
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    public int GetSegmentIndex(StratusSegment segment)
    {
      return segments.FindIndex(x => x == segment);
    }

    /// <summary>
    /// Sets the initial segment for this episode back to the very first one listed
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    public void ResetInitialSegment()
    {
      SetInitialSegment(segments.FirstOrNull());
    }

    /// <summary>
    /// Toggles the intial segment on
    /// </summary>
    private void ToggleCurrentSegment()
    {
      currentSegment.Toggle(true);
      
      // Toggle off all other ones
      foreach (var segment in segments)
      {
        if (segment != null && currentSegment)
          segment.Toggle(false);
      }
    }

    ObjectValidation[] ValidatorAggregator.Validate()
    {
      return ObjectValidation.Aggregate(segments);      
    }

    void Interfaces.Debuggable.Toggle(bool toggle)
    {
      debugDisplay = debugNavigation = toggle;
    }

    //------------------------------------------------------------------------/
    // Methods: Private
    //------------------------------------------------------------------------/
    private void CheckDebugInput()
    {
      if (nextSegmentInput.isUp && nextSegment != null)
        Enter(nextSegment, true);
      else if (previousSegmentInput.isUp && previousSegment != null)
        Enter(previousSegment, true);
    }

    private void DrawVisualization()
    {
      GUIContent msg = new GUIContent($"<color=#{debugTextColorHex}>{label}.{currentSegment.label}</color>");
      Vector2 size = StratusGUIStyles.header.CalcSize(msg);
      StratusGUI.GUILayoutArea(windowAnchor, size, (Rect rect) =>
      {
        GUILayout.Label(msg, StratusGUIStyles.header);
      });
    }

  }

}