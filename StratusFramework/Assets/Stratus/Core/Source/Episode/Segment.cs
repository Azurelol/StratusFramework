using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using UnityEngine.Events;
using UnityEngine.AI;
using Stratus.Interfaces;

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
  //[DisallowMultipleComponent]
  public class Segment : StratusBehaviour, Debuggable, ValidatorAggregator
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
      public bool restarted = false;

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
    public bool debug = false;
    [Tooltip("The string identifier for this segment")]
    public string label;
    [Tooltip("The trigger system used by this segment")]
    public TriggerSystem triggerSystem;
    [Tooltip("Whether the triggers on the trigger system are controlled by this segment")]
    public bool controlTriggers = true;
    [Tooltip("Whether selected objects are toggled by this segment")]
    public bool toggleObjects = false;
    [Tooltip("Whether to load the initial state of the objects in this segment, if already entered")]
    public bool restart = true;

    [Tooltip("The list of checkpoints within this segment")]
    public List<Stratus.Checkpoint> checkpoints = new List<Stratus.Checkpoint>();
    [Tooltip("Objects to be toggled on and off by this segment")]
    public List<GameObject> toggledObjects = new List<GameObject>();

    /// <summary>
    /// Any methods to invoke when enabled
    /// </summary>
    [Space]
    [Tooltip("Any methods to invoke when entered")]
    public UnityEvent onEntered = new UnityEvent();
    ///// <summary>
    ///// Any methods to invoke when restarted
    ///// </summary>
    //[Space]
    //[Tooltip("Any methods to invoke when entered")]
    //public UnityEvent onRestarted = new UnityEvent();
    /// <summary>
    /// Any methods to invoke when disabled
    /// </summary>
    [Tooltip("Any methods to invoke when exited")]
    public UnityEvent onExited = new UnityEvent();

    [SerializeField]
    private Episode episode_;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The currently active segment
    /// </summary>
    public static Segment current { get; protected set; }
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
    public Episode episode { get { return episode_; } internal set { episode_ = value; } }
    /// <summary>
    /// The list of stateful objects in this segment
    /// </summary>
    public Stateful[] statefulObjects { get; private set; }
    /// <summary>
    /// The current state of this segment
    /// </summary>
    public State state { get; protected set; } = State.Inactive;

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
      // If an episode has susbcribed
      episode?.segments.Remove(this);

      available.Remove(label);
      availableList.Remove(this);
    }

    private void Reset()
    {
      triggerSystem = GetComponent<TriggerSystem>();
      checkpoints.AddRange(GetComponentsInChildren<Checkpoint>());
    }

    void Debuggable.Toggle(bool toggle)
    {
      debug = toggle;
    }

    Validation[] ValidatorAggregator.Validate()
    {
      return Validation.Aggregate(triggerSystem);
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
      ToggleTriggers(toggle);
      ToggleObjects(toggle);
    }

    /// <summary>
    /// Suspends the behaviors in this system
    /// </summary>
    /// <param name="suspend"></param>
    private void ToggleTriggers(bool toggle)
    {
      if (triggerSystem != null && controlTriggers)
        triggerSystem.ToggleTriggers(toggle);
    }

    /// <summary>
    /// Toggles the objects on/off
    /// </summary>
    /// <param name="toggle"></param>
    private void ToggleObjects(bool toggle)
    {
      if (!toggleObjects)
        return;

      foreach (var go in toggledObjects)
      {
        if (go)
          go.SetActive(toggle);
      }
    }

    /// <summary>
    /// Enters this segment
    /// </summary>
    public virtual void Enter(bool suspend = false)
    {
      // We will announce that this segment has been entered in a moment...
      EnteredEvent e = new EnteredEvent(this);

      // Inform the previous segment its been exited
      if (current != null && current != this)
        current.Exit();

      // If the segment is currently active, let's do a restart instead
      // This will set everything back to the initial state at this segment
      if (state != State.Inactive && restart)
      {
        Restart();
        e.restarted = true;
      }

      if (debug)
        Trace.Script($"Entering", this);

      // Invoke the optional callbacks
      if (onEntered != null) onEntered.Invoke();      

      // If not suspended, toggle this segment
      Toggle(!suspend);

      // This is now the current segment
      Scene.Dispatch<EnteredEvent>(e);
      state = State.Entered;
      current = this;
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

      if (debug)
        Trace.Script($"Exiting", this);
    }

    /// <summary>
    /// Restarts to the initial state of this segment, as if recently entered
    /// </summary>
    public virtual void Restart()
    {
      if (debug)
        Trace.Script($"Restarting", this);
      
      // Restart the trigger system' triggers back to their initial state
      if (controlTriggers)
        triggerSystem.Restart();

      // Load the initial state of all registered stateful objects for this segment
      foreach (var stateful in statefulObjects)
        stateful.LoadInitialState();      

      //state = State.Inactive;
    }

    /// <summary>
    /// Translates the given object onto one of the checkpoints
    /// </summary>
    /// <param name="obj"></param>
    public void TranslateToCheckpoint(Transform target, int index)
    {
      if (checkpoints.Count - 1 < index || checkpoints[index] == null)
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