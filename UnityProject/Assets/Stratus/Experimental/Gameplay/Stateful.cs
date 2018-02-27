using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Dependencies.Ludiq.Reflection;

namespace Stratus
{
  /// <summary>
  /// Mantains state information of this object at runtime, loading them at will
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
      Load
    }

    public enum LoadEventType
    {
      Specific,
      Initial,
      Last
    }

    public abstract class StateEvent : Stratus.Event
    {
      public string label { get; private set; }

      protected StateEvent(string label)
      {
        this.label = label;
      }
    }

    /// <summary>
    /// Saves the state with the given label. If one is present, it will overwrite it
    /// </summary>
    public class SaveEvent : StateEvent
    {
      public SaveEvent(string label) : base(label)
      {
      }
    }

    public class LoadEvent : StateEvent
    {
      public LoadEvent(string label, LoadEventType type) : base(label)
      {
        this.loadEvent = type;
      }

      public LoadEventType loadEvent { get; private set; }

    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public bool debug = false;
    public Event.Scope scope = Event.Scope.Scene;
    [Header("State")]
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
      if (scope == Event.Scope.Scene)
      {
        Scene.Connect<SaveEvent>(this.OnSaveEvent);
        Scene.Connect<LoadEvent>(this.OnLoadEvent);
      }

      // Always subscribe to specific requests
      gameObject.Connect<SaveEvent>(this.OnSaveEvent);
      gameObject.Connect<LoadEvent>(this.OnLoadEvent);

      AddCommonRecorders();
    }

    private void Start()
    {
      SaveInitialState();
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

    void OnSaveEvent(SaveEvent e)
    {
      SaveState(e.label);
    }

    void OnLoadEvent(LoadEvent e)
    {
      switch (e.loadEvent)
      {
        case LoadEventType.Specific:
          LoadState(e.label);
          break;
        case LoadEventType.Initial:
          LoadInitialState();
          break;
        case LoadEventType.Last:
          LoadLastState();
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
        Trace.Script($"Loaded the state {state.label} with {memberCount} members!");

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
        Trace.Script($"Recorded the state {state.label} with {memberCount} members!");

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