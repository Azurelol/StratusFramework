using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// A simple interface for managing an attribute  @TODO: Use an array/map for modifiers?    
  /// </summary>
  [Serializable]
  public struct AttributeInstance
  {
    float _base;
    float minimum;
    float modifier;

    /// <summary>
    /// The current value of this attribute
    /// </summary>
    public float current { get; private set; }
    /// <summary>
    /// The maximum value of this attribute. Base + modifiers.
    /// </summary>
    public float maximum { get { return _base + modifier; } }
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
    public AttributeInstance(float value, float minimum = 0.0f)
    {
      this._base = value;
      this.minimum = minimum;
      this.modifier = 0.0f;
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
      if (current < minimum) current = minimum;
      float percentageLost = previousPercentage - percentage;
      return percentageLost;
    }
    /// <summary>
    /// Adds a positive temporary modifier to this attribute
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(float modifier)
    {
      this.modifier += modifier;
    }
    /// <summary>
    /// Sets the modifier of this attribute to a flat value
    /// </summary>
    /// <param name="modifier"></param>
    public void SetModifier(float modifier)
    {
      this.modifier = modifier;
    }
    /// <summary>
    /// Clears all modifiers for this attribute
    /// </summary>
    public void ClearModifiers()
    {
      modifier = 0.0f;
    }
  }
}