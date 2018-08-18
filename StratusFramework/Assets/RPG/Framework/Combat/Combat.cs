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
    public class GainEvent : Stratus.Event { public ValueType value; }
    /// <summary>
    /// If an input has been accepted, and is legal, represents the beginning of an action
    /// </summary>
    public class LossEvent : Stratus.Event { public ValueType value; }
  }
}

namespace Altostratus
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
    // Declarations
    //------------------------------------------------------------------------/
    [Serializable]
    public struct Attribute
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      float baseValue;
      float minimumValue;
      float modifierValue;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The current value of this attribute
      /// </summary>
      public float current { get; private set; }
      /// <summary>
      /// The maximum value of this attribute. Base + modifiers.
      /// </summary>
      public float maximum { get { return baseValue + modifierValue; } }
      /// <summary>
      /// The current ratio of the attribute when compared to its maximum as a percentage
      /// </summary>
      public float percentage { get { return (current / maximum) * 100.0f; } }
      /// <summary>
      /// Whether this attribute's current value is at its maximum value
      /// </summary>
      public bool isAtMaximum { get { return current == maximum; } }
      /// <summary>
      /// Returns an instance with a value of 1
      /// </summary>
      public static Attribute one => new Attribute(1f);

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="value">The base value of the attribute</param>
      /// <param name="minimum">The minimum value for this attribute</param>
      public Attribute(float value, float minimum = 0.0f)
      {
        this.baseValue = value;
        this.minimumValue = minimum;
        this.modifierValue = 0.0f;
        this.current = value;
      }

      /// <summary>
      /// Resets the current value of this attribute back to maximum
      /// </summary>
      public void Reset()
      {
        current = maximum;
      }
      /// <summary>
      /// Adds to the current value of this attribute, up to the maximum value
      /// </summary>
      /// <param name="value"></param>
      public void Add(float value)
      {
        current += value;
        if (current > maximum) current = maximum;
      }
      /// <summary>
      /// Reduces the current value of this attribute, up to its minimum value
      /// </summary>
      /// <param name="value"></param>
      /// <returns>How much was lost, as a percentage of the total value of this attribute</returns>
      public float Reduce(float value)
      {
        if (value < 0f)
          throw new ArgumentException($"The input value for Reduce '{value}' was not negative!");

        float previousPercentage = percentage;
        current -= value;
        if (current < minimumValue) current = minimumValue;
        float percentageLost = previousPercentage - percentage;
        return percentageLost;
      }
      /// <summary>
      /// Adds a positive temporary modifier to this attribute
      /// </summary>
      /// <param name="modifier"></param>
      public void AddModifier(float modifier)
      {
        this.modifierValue += modifier;
      }
      /// <summary>
      /// Sets the modifier of this attribute to a flat value
      /// </summary>
      /// <param name="modifier"></param>
      public void SetModifier(float modifier)
      {
        this.modifierValue = modifier;
      }
      /// <summary>
      /// Clears all modifiers for this attribute
      /// </summary>
      public void ClearModifiers()
      {
        modifierValue = 0.0f;
      }
    }

    //------------------------------------------------------------------------/
    // Events: State
    //------------------------------------------------------------------------/
    /// <summary>
    /// Base class for all combat events
    /// </summary>
    public abstract class BaseCombatEvent : Stratus.Event {}
    /// <summary>
    /// Combat has started
    /// </summary>
    public class StartedEvent : Stratus.Event { public CombatEncounter Encounter; }
    /// <summary>
    /// Combat has ended
    /// </summary>
    public class EndedEvent : Stratus.Event {}

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