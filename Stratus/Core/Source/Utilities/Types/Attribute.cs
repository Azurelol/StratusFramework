using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  // @TODO: Use an array/map for modifiers?
  /// <summary>
  /// A simple interface for managing an attribute  
  /// </summary>
  [Serializable]
  public struct FloatAttribute
  {
    float Base;
    float Minimum;
    float Modifier;
    float Current_;

    public float current { get { return Current_; } }
    /// <summary>
    /// The maximum value of this attribute. Base + modifiers.
    /// </summary>
    public float maximum { get { return Base + Modifier; } }
    /// <summary>
    /// The current ratio of the attribute when compared to its maximum as a percentage
    /// </summary>
    public float percentage { get { return (current / maximum) * 100.0f; } }
    /// <summary>
    /// Whether this attribute's current value is at its maximum value
    /// </summary>
    public bool isAtMaximum { get { return current == maximum; } }
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">The base value of the attribute</param>
    /// <param name="minimum">The minimum value for this attribute</param>
    public FloatAttribute(float value, float minimum = 0.0f)
    {
      Base = value;
      Minimum = minimum;
      Modifier = 0.0f;
      Current_ = value;
    }
    /// <summary>
    /// Resets the current value of this attribute back to maximum
    /// </summary>
    public void Reset()
    {
      Current_ = maximum;
    }
    /// <summary>
    /// Adds to the current value of this attribute, up to the maximum value
    /// </summary>
    /// <param name="value"></param>
    public void Add(float value)
    {
      Current_ += value;
      if (current > maximum) Current_ = maximum;
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
      Current_ -= value;
      if (current < Minimum) Current_ = Minimum;
      float percentageLost = previousPercentage - percentage;
      return percentageLost;
    }
    /// <summary>
    /// Adds a positive temporary modifier to this attribute
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(float modifier)
    {
      Modifier += modifier;
    }
    /// <summary>
    /// Sets the modifier of this attribute to a flat value
    /// </summary>
    /// <param name="modifier"></param>
    public void SetModifier(float modifier)
    {
      Modifier = modifier;
    }
    /// <summary>
    /// Clears all modifiers for this attribute
    /// </summary>
    public void ClearModifiers()
    {
      Modifier = 0.0f;
    }

  }
}