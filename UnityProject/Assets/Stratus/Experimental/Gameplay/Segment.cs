using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using UnityEngine.Events;
using UnityEngine.AI;

//    Features:
//start locations for the player at the beginning of every segment
//cheat allows you to move forward and back through segments
//a trigger for starting a segment
//"move object" triggerable lets you move objects into a location/rotation at the start of a segment
//
//note: triggers, objectives, and triggerables don't need to know what segment they are in. segments are just starting points or jump points

namespace Stratus
{
  /// <summary>
  /// Handles the logic for a gameplay segment in Trappist Landing
  /// </summary>
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  public class Segment : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// How to refer to the given episode or segment
    /// </summary>
    public enum ReferenceType
    {
      Reference,
      Label
    }

    public enum EventType
    {
      Enter,
      Exit
    }

    public enum State
    {
      Inactive,
      Entered,
      Exited
    }

    public class BaseSegmentEvent : Stratus.Event
    {
      public Segment segment { get; protected set; }
    }

    /// <summary>
    /// Signals that this segment has been entered
    /// </summary>
    public class EnteredEvent : BaseSegmentEvent
    {
      public EnteredEvent(Segment segment)
      {
        this.segment = segment;
      }
    }

    /// <summary>
    /// Signals that this segment has been exited
    /// </summary>
    public class ExitedEvent : BaseSegmentEvent
    {
      public ExitedEvent(Segment segment)
      {
        this.segment = segment;
      }      
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("Whether to log debug output")]
    public bool log;
    [Tooltip("The string identifier for this segment")]
    public string label;
    [Tooltip("The trigger system used by this segment")]
    public TriggerSystem triggerSystem;
    [Tooltip("Whether the triggers on the trigger system are toggled whenever this segment is entered/exited")]
    public bool toggleTriggers = true;
    [Tooltip("Whether to load the initial state of the objects in this segment, when entered")]
    public bool restart = false;    

    [Tooltip("The list of checkpoints within this segment")]
    public List<Stratus.Checkpoint> checkpoints = new List<Stratus.Checkpoint>();

    /// <summary>
    /// Any methods to invoke when enabled
    /// </summary>
    [Space]
    [Tooltip("Any methods to invoke when entered")]
    public UnityEvent onEntered = new UnityEvent();
    /// <summary>
    /// Any methods to invoke when disabled
    /// </summary>
    [Tooltip("Any methods to invoke when exited")]
    public UnityEvent onExited = new UnityEvent();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The currently active segment
    /// </summary>
    public static Segment current { get; private set; }
    /// <summary>
    /// The initial checkpoint for this segment
    /// </summary>
    public Stratus.Checkpoint initialCheckpoint => checkpoints[0];
    /// <summary>
    /// All currently active segments, indexed by their labels
    /// </summary>
    public static Dictionary<string, Segment> available { get; private set; } = new Dictionary<string, Segment>();
    /// <summary>
    /// A list of all available segments, by their labels
    /// </summary>
    public string[] availableLabels
    {
      get
      {
        List<string> labels = new List<string>();
        foreach (var segment in available)
          labels.Add(segment.Value.label);
        return labels.ToArray();
      }
    }
    /// <summary>
    /// All currently active segments, unordered
    /// </summary>
    public static List<Segment> availableList = new List<Segment>();
    /// <summary>
    /// Whether there are any available segments
    /// </summary>
    public static bool hasAvailable => availableList.Count > 0;
    /// <summary>
    /// The episode this segment belongs to
    /// </summary>
    public Episode episode { get; set; }
    /// <summary>
    /// The list of stateful objects in this segment
    /// </summary>
    public Stateful[] statefulObjects { get; private set; }
    /// <summary>
    /// The current state of this segment
    /// </summary>
    public State state { get; private set; } = State.Inactive;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnEnable()
    {
      if (Application.isPlaying && !string.IsNullOrEmpty(label))
      {
        if (!available.ContainsKey(label))
          available.Add(label, this);
      }
      availableList.Add(this);
    }

    private void OnDisable()
    {
      if (Application.isPlaying && !string.IsNullOrEmpty(label))
      {
        available.Remove(label);
      }

      availableList.Remove(this);
    }

    private void Awake()
    {
      if (!Application.isPlaying)
        return;

      statefulObjects = GetComponentsInChildren<Stateful>();
      Subscribe();
    }

    private void OnDestroy()
    {
      available.Remove(label);
      availableList.Remove(this);
    }

    private void Reset()
    {
      triggerSystem = GetComponent<TriggerSystem>();
      checkpoints.AddRange(GetComponentsInChildren<Checkpoint>());
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/    
    private void Subscribe()
    {
      gameObject.Connect<EnteredEvent>(OnEnteredEvent);
      gameObject.Connect<ExitedEvent>(OnExitedEvent);
    }

    private void OnEnteredEvent(EnteredEvent e)
    {

    }

    private void OnExitedEvent(ExitedEvent e)
    {

    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/    
    /// <summary>
    /// Toggles this segment on and off
    /// </summary>
    /// <param name="toggle"></param>
    public void Toggle(bool toggle)
    {
      if (triggerSystem != null && toggleTriggers)
        triggerSystem.Toggle(toggle);
    }

    /// <summary>
    /// Enters this segment
    /// </summary>
    public void Enter()
    {
      // Inform the previous segment its been exited
      if (current != null && current != this)
        current.Exit();

      Scene.Dispatch<EnteredEvent>(new EnteredEvent(this));
      current = this;
      if (onEntered != null) onEntered.Invoke();
      Toggle(true);

      if (state != State.Inactive && restart)
        Restart();

      state = State.Entered;

      if (log)
        Trace.Script($"Entering", this);
    }

    /// <summary>
    /// Exits this segment
    /// </summary>
    public void Exit()
    {
      Scene.Dispatch<ExitedEvent>(new ExitedEvent(this));
      if (onExited != null) onExited?.Invoke();
      Toggle(false);
      state = State.Exited;

      if (log)
        Trace.Script($"Exiting", this);
    }

    /// <summary>
    /// Restarts the state of this segment: its trigger system
    /// </summary>
    public void Restart()
    {
      triggerSystem.Restart();
      foreach (var stateful in statefulObjects)
      {        
        stateful.LoadInitialState();
      }
      state = State.Inactive;
    }

    /// <summary>
    /// Translates the given object onto one of the checkpoints
    /// </summary>
    /// <param name="obj"></param>
    public void TranslateToCheckpoint(Transform target, int index)
    {
      if (checkpoints.Count - 1 < index  || checkpoints[index] == null)
      {
        Trace.Error($"No checkpoint available at index {index}", this);
        return;
      }
      TranslateToCheckpoint(target, checkpoints[index]);
    }

    /// <summary>
    /// Translates the given object onto one of the checkpoints
    /// </summary>
    /// <param name="obj"></param>
    public void TranslateToCheckpoint(Transform target, Checkpoint checkpoint)
    {
      Vector3 position = checkpoint.transform.position;
      var navigation = target.GetComponent<NavMeshAgent>();
      if (navigation != null)
        navigation.Warp(position);
      else
        target.position = position;
    }

    /// <summary>
    /// Saves the state of the objects within this segment
    /// </summary>
    private void SaveState()
    {

    }

  }
}