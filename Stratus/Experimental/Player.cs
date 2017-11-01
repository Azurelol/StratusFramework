using Stratus.AI;
using System;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Base class for the player' avatar logic
  /// </summary>
  public abstract class Player : Agent
  {
    //--------------------------------------------------------------------------------------------/
    // Event Declarations
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Represents an action taken by the player
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ActionEvent<T>
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
    public override Blackboard blackboard { get { throw new NotImplementedException("The player does not use a blackboard!"); } }

    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
    protected abstract void OnRevive();
    protected abstract void OnPlayerSubscribe();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
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
      this.OnPlayerSubscribe();
    }

    void OnReviveEvent(ReviveEvent e)
    {
      this.OnRevive();
    }


  }

}