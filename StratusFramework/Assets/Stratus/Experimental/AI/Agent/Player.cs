using Stratus.AI;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Stratus.AI
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
      public class InputEvent : Stratus.StratusEvent { public InputField.State state; }
      /// <summary>
      /// If an input has been accepted, and is legal, represents the beginning of an action
      /// </summary>
      public class StartedEvent : Stratus.StratusEvent { }
      /// <summary>
      /// If an input has been accepted, and is legal, represents the beginning of an action
      /// </summary>
      public class EndedEvent : Stratus.StratusEvent { }
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

    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
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

  }

}