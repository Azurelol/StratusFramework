using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  /// <summary>
  /// Represents an action taken by the player
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ValueEvent<ValueType> 
  {
    /// <summary>
    /// If an input has been accepted, and is legal, represents the beginning of an action
    /// </summary>
    public class GainEvent : Stratus.StratusEvent { public ValueType value; }
    /// <summary>
    /// If an input has been accepted, and is legal, represents the beginning of an action
    /// </summary>
    public class LossEvent : Stratus.StratusEvent { public ValueType value; }
  }
}

namespace Genitus
{
  /// <summary>
  /// Provides major definitions for combat within the framework
  /// </summary>
  public static class Combat
  {
    //------------------------------------------------------------------------/
    // Enummerations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Targeting parameters for a given command
    /// </summary>
    public enum TargetingParameters { Self, Ally, Enemy, Any }

    //------------------------------------------------------------------------/
    // Events: State
    //------------------------------------------------------------------------/
    /// <summary>
    /// Base class for all combat events
    /// </summary>
    public abstract class BaseCombatEvent : Stratus.StratusEvent {}
    /// <summary>
    /// Combat has started
    /// </summary>
    public class StartedEvent : Stratus.StratusEvent { public CombatEncounter Encounter; }
    /// <summary>
    /// Combat has ended
    /// </summary>
    public class EndedEvent : Stratus.StratusEvent {}

    //------------------------------------------------------------------------/
    // Events: Damage
    //------------------------------------------------------------------------/
    /// <summary>
    /// Signals that the character's health should be fully restored
    /// </summary>
    public class RestoreEvent : BaseCombatEvent {}

    /// <summary>
    /// Base class for health modification events
    /// </summary>
    public abstract class ModifyHealthEvent : BaseCombatEvent
    {
      /// <summary>
      /// The source that is modifying this character's health
      /// </summary>
      public CombatController source;
      /// <summary>
      /// By how much to modify the character's health
      /// </summary>
      public float value;
    }

    public class DamageEvent : ModifyHealthEvent
    {
      public int penetration;
    }
    public class HealEvent : ModifyHealthEvent
    {
    }

    //------------------------------------------------------------------------/
    // Events: Damage
    //------------------------------------------------------------------------/

  }

}