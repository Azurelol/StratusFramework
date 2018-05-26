using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Stratus
{
  public abstract class StateDrivenToggle : StratusBehaviour
  {
    public enum Validation
    {
      [Tooltip("The object is enabled during any of these states")]
      EnableOn,
      [Tooltip("The object is disabled during any of these states")]
      DisableOn
    }

    /// <summary>
    /// The extent to which this component is toggled by states
    /// </summary>
    public enum Extent
    {
      [Tooltip("A single state")]
      Single,
      [Tooltip("Multiple states")]
      Multiple
    }
  }


  /// <summary>
  /// Given a provided enum class used for defining exclusive global states,
  /// provides a component to handle propagating changes based on the given state
  /// </summary>
  /// <typeparam name="State"></typeparam>
  public abstract class StateDrivenToggle<State> : StateDrivenToggle where State : struct, IConvertible
  {
    /// <summary>
    /// Callback for when the state has changed
    /// </summary>
    /// <param name="state"></param>
    public delegate void OnStateChange(State state);

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Header("States")]
    //[Tooltip("How many states this object responds to")]
    //public Extent extent = Extent.Single;
    ///// <summary>
    ///// The state at which this object is active
    ///// </summary>
    //public State activeState;
    [Tooltip("Defines how this object is toggled")]
    public Validation validation = Validation.EnableOn;
    /// <summary>
    /// The states at which this object is active
    /// </summary>
    [Tooltip("The state at which this object is active")]
    public List<State> states = new List<State>();
    /// <summary>
    /// A delay between toggling this object on and off
    /// </summary>
    [Header("Toggle Response")]
    [Tooltip("A delay between toggling this object on and off")]
    public float delay = 0.0f;
    /// <summary>
    /// Any methods to invoke when enabled
    /// </summary>
    [Space]
    [Tooltip("Any methods to invoke when toggled")]
    public UnityEvent onEnabled = new UnityEvent();
    /// <summary>
    /// Any methods to invoke when disabled
    /// </summary>
    [Tooltip("Any methods to invoke when toggled")]
    public UnityEvent onDisabled = new UnityEvent();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The current global state
    /// </summary>
    public static State currentState { get; private set; }
    /// <summary>
    /// A provided callback for when the state has changed
    /// </summary>
    public static OnStateChange onStateChange { get; set; }
    /// <summary>
    /// The list of all subscribed objects. When states change, these are notified.
    /// </summary>
    public static List<StateDrivenToggle<State>> toggleables { get; private set; } = new List<StateDrivenToggle<State>>();
    /// <summary>
    /// List of all handlers for listening to state changes
    /// </summary>
    private static List<StateHandler> handlers { get; set; } = new List<StateHandler>();
    /// <summary>
    /// Whether the initial state has bene set
    /// </summary>
    private static bool initialized { get; set; } = false;
    /// <summary>
    /// The previous state
    /// </summary>
    private static State previousState;

    //private static bool debug
    //{
    //  get { return PlayerPrefs.GetInt("de")}
    //}

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// When first enabled, adds itself to the list of all objects to be toggled on/off on a state change
    /// </summary>
    private void OnEnable()
    {
      toggleables.Add(this);
      if (initialized)
        Apply(currentState);
    }

    private void OnDestroy()
    {
      toggleables.Remove(this);
    }

    private void Reset()
    {
      states.Add(default(State));
    }

    private void Awake()
    {

    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Changes the global state, notifying all subscribers
    /// </summary>
    /// <param name="nextState"></param>
    public static void Change(State nextState)
    {
      initialized = true;
      previousState = currentState;
      currentState = nextState;
      Trace.Script($"'{previousState}' -> '{ currentState}'");
      foreach (var toggle in toggleables.ToArray())
        toggle.Apply(currentState);
      foreach (var handler in handlers)
        handler.Inform(currentState);
      onStateChange?.Invoke(nextState);

    }

    /// <summary>
    /// Reverts to the previous global state, notifying all subscribers
    /// </summary>
    public static void Revert()
    {
      if (!initialized)
        return;
      //Trace.Script($"Reverting to '{previousState}");
      Change(previousState);
    }

    /// <summary>
    /// Enables this object
    /// </summary>
    public void Toggle(bool toggled)
    {
      //Trace.Script("Toggled = " + toggled, this);

      if (toggled)
        onEnabled?.Invoke();
      else
        onDisabled?.Invoke();

      if (delay > 0.0f)
        this.StartCoroutine(Routines.Call(() => { gameObject.SetActive(toggled); }, this.delay), "Delay");
      else
        gameObject.SetActive(toggled);
    }

    /// <summary>
    /// Applies the state change to the object
    /// </summary>
    /// <param name="state"></param>
    private void Apply(State nextState)
    {
      bool isActive = false;

      if (validation == Validation.EnableOn)
      {
        // If the given state is present among given states, enable this object
        foreach (var state in states)
        {
          bool isEqual = Compare(state, nextState);
          if (isEqual)
          {
            isActive = true;
            break;
          }
        }
      }
      else if (validation == Validation.DisableOn)
      {
        // If the given state is present among given states, disable this object
        foreach (var state in states)
        {
          bool isEqual = Compare(state, nextState);
          if (isEqual)
          {
            isActive = false;
            break;
          }
        }

      }
      Toggle(isActive);
    }

    /// <summary>
    /// Custom comparator since this is a generic type
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static bool Compare(State x, State y)
    {
      return EqualityComparer<State>.Default.Equals(x, y);
    }

    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Informs this object to be notified of specific state changes
    /// </summary>
    public class StateHandler
    {
      public delegate void Callback();
      public delegate void ToggleCallback(bool isAtState);

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The class instance  this handler belongs to
      /// </summary>
      MonoBehaviour parent;
      /// <summary>
      /// The goal state for this handler
      /// </summary>
      State goal;
      /// <summary>
      /// Function called when entering this state
      /// </summary>
      Callback onEnterState;
      /// <summary>
      /// Function called when exiting this state
      /// </summary>
      Callback onExitState;
      /// <summary>
      /// Function called when the state has changed
      /// </summary>
      ToggleCallback onStateChange;
      /// <summary>
      /// Whether this handle is currently at the goal state
      /// </summary>
      bool isAtGoalState = false;
      /// <summary>
      /// Whether to print state changes
      /// </summary>
      public bool logging = false;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="state">The desired goal state for this handler</param>
      /// <param name="onEnter">The function to be called when the goal state has been entered</param>
      /// <param name="onExit">The function to be called when the goal state has been exited</param>
      public StateHandler(State state, MonoBehaviour parent = null, bool log = false)
      {
        goal = state;
        this.parent = parent;
        logging = log;

        handlers.Add(this);
      }

      /// <summary>
      /// DTOR. Unsubscribes this from the list of objects to be notified of state changes.
      /// </summary>
      ~StateHandler()
      {
        Shutdown();
      }

      /// <summary>
      /// Sets callbacks for when the target goal state has been entered and exited
      /// </summary>
      /// <param name="onEnter">The function which will be invoked when the target goal state has been entered</param>
      /// <param name="onExit">The function which will be invoked when the target goal state has been exited</param>
      public void Set(Callback onEnter, Callback onExit)
      {
        onEnterState = onEnter;
        onExitState = onExit;
        if (initialized)
          Inform(currentState);
      }

      /// <summary>
      /// Sets a callback for a function that will receive a bool signaling whether the target goal state has been reached
      /// </summary>
      /// <param name="onStateChange"></param>
      public void Set(ToggleCallback onStateChange, bool isCalledImmediately = true)
      {
        this.onStateChange = onStateChange;
        if (isCalledImmediately && initialized)
          Inform(currentState);
      }

      /// <summary>
      /// Informs the handler of state changes
      /// </summary>
      /// <param name="state"></param>
      internal void Inform(State state)
      {
        //if (Tracing)
        //  Trace.Script("Goal = " + Goal + ", State = " + state, Parent);
        // If the state has been changed to the goal state and we are not there currently...
        if (Compare(state, goal) && !isAtGoalState)
        {
          if (logging)
            Trace.Script("Now at goal state '" + goal.ToString() + "'", parent);

          isAtGoalState = true;
          if (onEnterState != null) onEnterState();
          else if (onStateChange != null) onStateChange(isAtGoalState);
        }
        // If we were at the goal state and the state has been changed to another...
        else if (!Compare(state, goal) && isAtGoalState)
        {
          isAtGoalState = false;
          if (onExitState != null)
          {
            if (logging)
              Trace.Script("Now exiting state '" + state.ToString() + "'", parent);
            onExitState();
          }
          else if (onStateChange != null)
          {
            if (logging)
              Trace.Script("Not at goal state '" + goal.ToString() + "', flipping state to " + isAtGoalState, parent);
            onStateChange(isAtGoalState);
          }
        }
      }

      /// <summary>
      /// Terminates this statehandler
      /// </summary>
      public void Shutdown()
      {
        handlers.Remove(this);
      }

    }

  }

}