using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Dependencies.Ludiq.Reflection;
using UnityEngine.Events;

namespace Stratus
{
  /// <summary>
  /// Mantains state information of this object at runtime, saving and loading them at will
  /// </summary>
  public class Stateful : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations: Types
    //------------------------------------------------------------------------/
    public class SerializedStateRecorder
    {
      public List<MemberReference> memberReferences;
      public List<UnityMember> memberFields;
    }

    public class SerializedState
    {
      public string label { get; private set; }
      public int index { get; private set; }
      public object[] values { get; private set; }

      public SerializedState(string label, int index, object[] values)
      {
        this.label = label;
        this.index = index;
        this.values = values;
      }
    }

    //------------------------------------------------------------------------/
    // Declarations: Events
    //------------------------------------------------------------------------/
    public enum EventType
    {
      Save,
      Load,
      SaveInitial,
      LoadInitial,
      LoadLast
    }

    public enum InitialStateConfiguration
    {
      Immediate,
      OnEvent,
      [Tooltip("Invoked after the provided methods to invoke have been invoked")]
      OnCallbackFinished,
      OnDelay,       
    }

    public class StateEvent : Stratus.Event
    {
      public StateEvent(EventType type, string label)
      {
        this.type = type;
        this.label = label;
      }

      public EventType type { get; private set; }
      public string label { get; private set; }

      public bool usesLabel => type == Stateful.EventType.Save || type == Stateful.EventType.Load;
    }
    
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public bool debug = false;
    public Event.Scope scope = Event.Scope.Scene;
    public InitialStateConfiguration initialStateConfiguration = InitialStateConfiguration.Immediate;
    public UnityEvent onInitialState = new UnityEvent();
    public float delay = 0.0f;

    [Header("State")]
    //[DrawIf(nameof(initialStateConfiguration), InitialStateConfiguration.AfterEvent, ComparisonType.Equals)]

    public bool recordTransformState = true;
    [Filter(Methods = false, Properties = true, NonPublic = true, ReadOnly = true, Static = true, Inherited = true, Fields = true, Extension = false)]
    public List<UnityMember> memberFields = new List<UnityMember>();

    private Dictionary<string, SerializedState> stateMap = new Dictionary<string, SerializedState>();
    private List<SerializedState> stateList = new List<SerializedState>();
    private List<MemberReference> memberReferences = new List<MemberReference>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The initial state of this object
    /// </summary>
    public SerializedState initialState { get; private set; }
    /// <summary>
    /// The last state of this object
    /// </summary>
    public SerializedState lastState { get; private set; }
    /// <summary>
    /// The number of states this object has recorded
    /// </summary>
    public int numberOfStates { get; private set; } = 0;
    /// <summary>
    /// The number of members this object is recording
    /// </summary>
    public int memberCount { get; private set; }

    private static Dictionary<string, bool> availableStates { get; set; } = new Dictionary<string, bool>();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      // Optionally, subscribe to scene-wide events
      if (scope == Event.Scope.Scene)
        Scene.Connect<StateEvent>(this.OnStateEvent);

      // Always subscribe to specific requests
      gameObject.Connect<StateEvent>(this.OnStateEvent);

      AddCommonRecorders();
    }

    private void Start()
    {
      switch (initialStateConfiguration)
      {
        case InitialStateConfiguration.Immediate:
          SaveInitialState();
          break;

        case InitialStateConfiguration.OnEvent:
          gameObject.Dispatch<StateEvent>(new StateEvent(EventType.SaveInitial, null));
          break;

        case InitialStateConfiguration.OnCallbackFinished:
          onInitialState.Invoke();
          SaveInitialState();
          break;

        case InitialStateConfiguration.OnDelay:
          Invoke(nameof(SaveInitialState), delay);
          break;

      }      
    }

    private void OnValidate()
    {
      foreach (var member in memberFields)
      {
        if (member == null)
          continue;
        if (member.target != this.gameObject)
          member.target = this.gameObject;
      }
    }

    void OnStateEvent(StateEvent e)
    {
      switch (e.type)
      {
        case EventType.Save:
          SaveState(e.label);
          break;
        case EventType.Load:
          LoadState(e.label);
          break;
        case EventType.SaveInitial:
          SaveInitialState();
          break;
        case EventType.LoadInitial:
          LoadInitialState();
          break;
        case EventType.LoadLast:
          LoadLastState();
          break;
        default:
          break;
      }
    }


    //------------------------------------------------------------------------/
    // Methods: Public
    //------------------------------------------------------------------------/
    public void LoadState(string label)
    {
      if (!stateMap.ContainsKey(label))
        Trace.Error($"The state {label} was not found!", this);

      SerializedState state = stateMap[label];
      LoadState(state);
    }

    public SerializedState SaveState(string label)
    {
      SerializedState state = MakeState(label);

      // Overwrite if found
      if (stateMap.ContainsKey(label))
      {
        stateMap[label] = state;
        stateList.RemoveAt(stateMap[label].index);
        stateList.Add(state);

        if (debug)
          Trace.Script($"The state {label} has been overwritten!", this);
      }
      // Else make a new one
      else
      {
        stateMap.Add(label, state);
        stateList.Add(state);
      }

      return state;
    }

    /// <summary>
    /// Loads the initial state of this object
    /// </summary>
    public void LoadInitialState()
    {
      LoadState(initialState);
    }

    /// <summary>
    /// Loads the last state of this object
    /// </summary>
    public void LoadLastState()
    {
      LoadState(lastState);
    }

    private void SaveInitialState()
    {
      initialState = MakeState("Initial");
    }

    //------------------------------------------------------------------------/
    // Methods: State
    //------------------------------------------------------------------------/
    private void LoadState(SerializedState state)
    {
      // The starting index of objet values
      int index = 0;

      // Set through member reference
      foreach (var member in memberReferences)
        member.Set(state.values[index++]);

      // Set through member field
      foreach (var member in memberFields)
      {
        if (member.isAssigned)
          member.Set(state.values[index++]);
      }

      if (debug)
        Trace.Script($"Loaded the state {state.label} with {memberCount} members!", this);

    }

    private SerializedState MakeState(string label)
    {
      List<object> values = new List<object>();

      // Record from MemberReference
      foreach (var member in memberReferences)
      {
        values.Add(member.Get());
      }

      // Record from MemebrField
      foreach (var member in memberFields)
      {
        if (member.isAssigned)
          values.Add(member.Get());
      }

      SerializedState state = new SerializedState(label, numberOfStates, values.ToArray());
      numberOfStates++;

      if (debug)
        Trace.Script($"Recorded the state {state.label} with {memberCount} members!", this);

      lastState = state;

      return state;
    }

    //------------------------------------------------------------------------/
    // Methods: Recorders
    //------------------------------------------------------------------------/
    private void AddCommonRecorders()
    {
      if (recordTransformState) AddTransformRecorder();
      memberCount = memberReferences.Count + memberFields.Count;
    }

    private void AddTransformRecorder()
    {
      memberReferences.Add(MemberReference.Construct(() => transform.position));
      memberReferences.Add(MemberReference.Construct(() => transform.rotation));
      memberReferences.Add(MemberReference.Construct(() => transform.localScale));
      memberReferences.Add(MemberReference.Construct(() => transform.parent));
    }

    


  }
}