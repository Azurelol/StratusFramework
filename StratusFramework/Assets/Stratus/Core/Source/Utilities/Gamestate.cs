using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  ///  An abstract template for setting up a Gamestate class for your project
  /// </summary>
  /// <typeparam name="State"></typeparam>
  public abstract class Gamestate<State> where State : struct
  {
    public class ChangeEvent : Stratus.Event
    {
      public State State;
      public ChangeEvent(State state) { State = state; }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The current gamestate
    /// </summary>
    public static State Current;
    /// <summary>
    /// The previous gamestate
    /// </summary>
    static State Previous;
    /// <summary>
    /// All event handlers
    /// </summary>
    static List<EventHandler> Handlers = new List<EventHandler>();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private Gamestate() { }

    /// <summary>
    /// Changes the gamestate
    /// </summary>
    /// <param name="state">The new state</param>
    public static void Change(State state)
    {
      Previous = Current;
      Current = state;
      //Trace.Script("State has been changed to '" + Current.ToString() + "'");
      Scene.Dispatch<ChangeEvent>(new ChangeEvent(Current));
      InformHandlers();
    }

    /// <summary>
    /// Reverts to the previous state
    /// </summary>
    public static void Revert()
    {
      Current = Previous;
      //Trace.Script("State has been reverted to '" + Current.ToString() + "'");
      Scene.Dispatch<ChangeEvent>(new ChangeEvent(Current));
      InformHandlers();
    }

    /// <summary>
    /// Informs all handlers of the current state
    /// </summary>
    static void InformHandlers()
    {
      //if (Gamestate.Tracing)
      //  Trace.Script("Informing " + Handlers.Count + " handlers of the state change!");
      foreach (var handler in Handlers)
      {
        handler.Inform(Current);
      }
    }

    //------------------------------------------------------------------------/
    // Subclasses
    //------------------------------------------------------------------------/
    /// <summary>
    /// Handles state changes
    /// </summary>
    public class EventHandler
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
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
      public EventHandler(State state, MonoBehaviour parent = null, bool tracing = false)
      {
        goal = state;
        this.parent = parent;
        this.logging = tracing;
        Gamestate<State>.Handlers.Add(this);

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
        Inform(Gamestate<State>.Current);
      }

      /// <summary>
      /// Sets a callback for a function that will receive a bool signaling whether the target goal state has been reached
      /// </summary>
      /// <param name="onStateChange"></param>
      public void Set(ToggleCallback onStateChange, bool isCalledImmediately = true)
      {
        this.onStateChange = onStateChange;
        if (isCalledImmediately)
          Inform(Gamestate<State>.Current);
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
        if (state.Equals(goal) && !isAtGoalState)
        {
          if (logging)
            Trace.Script("Now at goal state '" + goal.ToString() + "'", parent);

          isAtGoalState = true;
          if (onEnterState != null) onEnterState();
          else if (onStateChange != null) onStateChange(isAtGoalState);
        }
        // If we were at the goal state and the state has been changed to another...
        else if (state.Equals(goal) == false && isAtGoalState)
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
        Gamestate<State>.Handlers.Remove(this);
      }

    }

  }

}