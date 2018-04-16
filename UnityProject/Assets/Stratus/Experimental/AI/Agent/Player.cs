using Stratus.AI;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// Base class for the player' avatar logic
  /// </summary>
  public abstract class Player<T> : Agent where T : MonoBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Event Declarations
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Represents an action taken by the player
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ActionEvent<U>
    {
      /// <summary>
      /// An input event represents a request to perform a specific action
      /// </summary>
      public class InputEvent : Stratus.Event { public InputAxisField.State state; }
      /// <summary>
      /// If an input has been accepted, and is legal, represents the beginning of an action
      /// </summary>
      public class StartedEvent : Stratus.Event { }
      /// <summary>
      /// If an input has been accepted, and is legal, represents the beginning of an action
      /// </summary>
      public class EndedEvent : Stratus.Event { }
    }

    /// <summary>
    /// Signals that the player has been revived
    /// </summary>
    public class ReviveEvent : Stratus.Event
    {
    }

    /// <summary>
    ///  Signals that the player has entered combat
    /// </summary>
    public class EnterCombatEvent : Stratus.Event
    {
    }

    /// <summary>
    /// Signals that hte player has ended combat
    /// </summary>
    public class ExitCombatEvent : Stratus.Event
    {
    }

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Returns a reference to the first player
    /// </summary>
    public static T first => all.Count > 0 ? all[0] : null;
    /// <summary>
    /// Container for all players
    /// </summary>
    public static List<T> all { get; set; } = new List<T>();
    
    public override Blackboard blackboard { get { throw new NotImplementedException("The player does not use a blackboard!"); } }


    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
    protected abstract void OnRevive();
    protected abstract void OnPlayerAwake();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAgentAwake()
    {
      all.Add(this as T);
      OnPlayerAwake();
    }

    protected override void OnAgentDestroy()
    {
      all.Remove(this as T);
    }

    protected override void OnCombatEnter()
    {
      //Trace.Script("Entered combat!", this);
      Scene.Dispatch<EnterCombatEvent>(new EnterCombatEvent());
    }
    protected override void OnCombatExit()
    {
      //Trace.Script("Exited combat!", this);
      Scene.Dispatch<ExitCombatEvent>(new ExitCombatEvent());
    }

    protected override void OnSubscribe()
    {
      this.gameObject.Connect<ReviveEvent>(this.OnReviveEvent);
    }

    void OnReviveEvent(ReviveEvent e)
    {
      this.OnRevive();
    }


  }

}