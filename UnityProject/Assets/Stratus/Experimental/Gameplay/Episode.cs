using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using UnityEngine.Events;
using UnityEngine.AI;

namespace Stratus
{
  /// <summary>
  /// A gameplay episode system, consisting of multiple segments
  /// </summary>
  [ExecuteInEditMode]
  public class Episode : Multiton<Episode>
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

    /// <summary>
    /// A callback consisting of the segment that has been jumped to, and the index of the checkpoint
    /// </summary>
    [System.Serializable]
    public class SegmentCallback : UnityEvent<Segment, int>
    {
    }

    /// <summary>
    /// Signals that a gameplay segment has been jumped to
    /// </summary>
    public class JumpToSegmentEvent : Stratus.Event
    {
      public JumpToSegmentEvent(Segment segment, int checkpointIndex, Vector3 position)
      {
        this.segment = segment;
        this.checkpointIndex = checkpointIndex;
        this.position = position;
      }

      public Segment segment { get; private set; }
      public int checkpointIndex { get; private set; }
      public Vector3 position { get; private set; }      
    }

    /// <summary>
    /// Signals that this episode should be started
    /// </summary>
    public class BeginEvent : Stratus.Event
    {

    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("The segments within this episode")]
    public List<Segment> segments;
    [Tooltip("The initial segment")]
    public Segment initialSegment;
    [Tooltip("Whether to enter the first segment on awake")]
    public bool enterOnAwake = false;
    [Tooltip("How segments are managed within this episode")]
    public SegmentManagementMode mode;


    [Header("Jump Configuration")]
    [Tooltip("What to do on a segment jump")]
    public JumpMechanism mechanism;
    public Transform targetTransform;
    public SegmentCallback onJump = new SegmentCallback();

    [Header("Debug")]
    public bool debug = false;
    [DrawIf("debug", true, ComparisonType.Equals)]
    public Color debugTextColor = Color.white;
    public StratusGUI.Anchor windowAnchor = StratusGUI.Anchor.BottomRight;
    public InputField nextSegmentInput = new InputField(KeyCode.PageDown);
    public InputField previousSegmentInput = new InputField(KeyCode.PageUp);
    [SerializeField]
    private string debugTextColorHex;
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The currently active episode
    /// </summary>
    public static Episode currentEpisode { get; private set; }
    /// <summary>
    /// The currently active segment in this episode
    /// </summary>
    public Segment currentSegment => segments[currentSegmentIndex];
    /// <summary>
    /// The previous segment
    /// </summary>
    public Segment previousSegment => currentSegmentIndex > 0 ? segments[currentSegmentIndex - 1] : null;
    /// <summary>
    /// The next segment
    /// </summary>
    public Segment nextSegment => currentSegmentIndex < (segments.Count - 1) ? segments[currentSegmentIndex + 1] : null;
    /// <summary>
    /// The index of the current segment
    /// </summary>
    private int currentSegmentIndex { get; set; }


    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      currentSegmentIndex = GetSegmentIndex(initialSegment);
    }

    private void Start()
    {
      if (Application.isPlaying)
      {
        if (enterOnAwake)
        {
          Enter(initialSegment, true);
        }
      }
    }

    protected override void OnMultitonEnable()
    {
    }

    protected override void OnMultitonDisable()
    {
    }
    
    private void Update()
    {
      if (debug && Application.isPlaying)
        CheckDebugInput();
    }

    private void OnGUI()
    {
      if (debug && Application.isPlaying)
        DrawVisualization();
    }

    private void Reset()
    {      
    }

    private void OnValidate()
    {
      debugTextColorHex = debugTextColor.ToHex();
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

    }

    /// <summary>
    /// Enters the specified (by label) segment within this episode if its present
    /// </summary>
    /// <param name="label">The label for the segment</param>
    /// <param name="jump">Whether to teleport the player to the checkpoint</param>
    /// <param name="checkpointIndex"></param>
    public bool Enter(string label, bool jump, int checkpointIndex = 0)
    {
      if (!Segment.available.ContainsKey(label))
      {
        Trace.Error($"The segment labeled {label} is not present at this time!", this);
        return false;
      }

      Segment segment = Segment.available[label];
      Enter(segment, jump, checkpointIndex);
      return true;
    }

    /// <summary>
    /// Jumps to the specified segment within this episode
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="jump">Whether to teleport the player to the checkpoint</param>
    /// <param name="checkpointIndex">The checkpoint to jump to</param>
    public void Enter(Segment segment, bool jump, int checkpointIndex = 0)
    {
      if (!Application.isPlaying)
        return;

      //if (debug)
      //  Trace.Script($"Entering the segment {segment}, checkpoint {checkpointIndex}", this);
      
      // Enter the segment
      currentSegmentIndex = GetSegmentIndex(segment);
      segment.Enter();

      // Now warp the player to it
      if (jump)
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
            Scene.Dispatch<JumpToSegmentEvent>(new JumpToSegmentEvent(segment, checkpointIndex, position));
            break;
          default:
            break;
        }
      }
    }
    
    /// <summary>
    /// Exits this segment
    /// </summary>
    /// <param name="segment"></param>
    public void Exit(Segment segment)
    {
      segment.Exit();
    }

    /// <summary>
    /// Sets the initial segment for this episode
    /// </summary>
    /// <param name="segment"></param>
    public void SetInitialSegment(Segment initialSegment)
    {
      //Trace.Script($"Initial segment = {initialSegment}");

      // Toggle the selected segment
      this.initialSegment = initialSegment;
      initialSegment.Toggle(true);

      // Toggle off all other ones
      foreach (var segment in segments)
      {
        if (segment != null && initialSegment)
          segment.Toggle(false);
      }
    }

    /// <summary>
    /// Gets the index of a given segment, provided its part of this episode
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    public int GetSegmentIndex(Segment segment)
    {
      return segments.FindIndex(x => x == segment);
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
      Rect rect = StratusGUI.CalculateAnchoredPositionOnScreen(windowAnchor, size);
      GUILayout.BeginArea(rect);
      GUILayout.Label(msg, StratusGUIStyles.header);
      GUILayout.EndArea();
    }


  }

}