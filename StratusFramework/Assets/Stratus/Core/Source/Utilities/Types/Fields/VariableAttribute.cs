using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  [Serializable]
  public struct VariableAttribute
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [SerializeField]
    private float baseValue;
    [SerializeField]
    private float minimumValue;
    [SerializeField]
    private float modifierValue;
    [SerializeField]
    private float currentValue;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The current value of this parameter
    /// </summary>
    public float current => currentValue;
    /// <summary>
    /// The maximum value of this parameter. Base + modifiers.
    /// </summary>
    public float maximum { get { return baseValue + modifierValue; } }
    /// <summary>
    /// The current ratio of the parameter when compared to its maximum as a percentage
    /// </summary>
    public float percentage { get { return (currentValue / maximum) * 100.0f; } }
    /// <summary>
    /// Whether this parameter's current value is at its maximum value
    /// </summary>
    public bool isAtMaximum { get { return currentValue == maximum; } }
    /// <summary>
    /// Returns an instance with a value of 1
    /// </summary>
    public static VariableAttribute one => new VariableAttribute(1f);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">The base value of the parameter</param>
    /// <param name="minimum">The minimum value for this parameter</param>
    public VariableAttribute(float value, float minimum = 0.0f)
    {
      this.baseValue = value;
      this.minimumValue = minimum;
      this.modifierValue = 0.0f;
      this.currentValue = value;
    }

    /// <summary>
    /// Resets the current value of this parameter back to maximum
    /// </summary>
    public void Reset()
    {
      currentValue = maximum;
    }

    /// <summary>
    /// Adds to the current value of this parameter, up to the maximum value
    /// </summary>
    /// <param name="value"></param>
    public void Increase(float value)
    {
      currentValue += value;
      if (currentValue > maximum) currentValue = maximum;
    }

    /// <summary>
    /// Reduces the current value of this parameter, up to its minimum value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>How much was lost, as a percentage of the total value of this parameter</returns>
    public float Decrease(float value)
    {
      if (value < 0f)
        throw new ArgumentException($"The input value for Reduce '{value}' was not negative!");

      float previousPercentage = percentage;
      currentValue -= value;
      if (currentValue < minimumValue) currentValue = minimumValue;
      float percentageLost = previousPercentage - percentage;
      return percentageLost;
    }
    /// <summary>
    /// Adds a positive temporary modifier to this parameter
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(float modifier)
    {
      this.modifierValue += modifier;
    }
    /// <summary>
    /// Sets the modifier of this parameter to a flat value
    /// </summary>
    /// <param name="modifier"></param>
    public void SetModifier(float modifier)
    {
      this.modifierValue = modifier;
    }
    /// <summary>
    /// Clears all modifiers for this parameter
    /// </summary>
    public void ClearModifiers()
    {
      modifierValue = 0.0f;
    }
  }
}