/******************************************************************************/
/*!
@file   DrawIf.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@note   Credit to: Or-Aviram: 
        https://forum.unity3d.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
*/
/******************************************************************************/
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Type of comparison
  /// </summary>
  public enum ComparisonType
  {
    Equals = 1,
    NotEqual = 2,
    Greater = 3,
    Lesser = 4,
    GreaterOrEqual = 5,
    LesserOrEqual = 6
  }

  /// <summary>
  /// What to do with the property if the attribute is not validated
  /// </summary>
  public enum PropertyDrawingType
  {
    ReadOnly =2,
    DontDraw = 3
  }

  /// <summary>
  /// Draws the field/property only if the predicate comparing the property returns true
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
  public class DrawIfAttribute : PropertyAttribute
  {    
    public string ComparedPropertyName { get; private set; }
    public object ComparedValue { get; private set; }
    public ComparisonType Comparison { get; private set; }
    public PropertyDrawingType DefaultBehavior { get; private set; }

    /// <summary>
    /// Only draws the field if the condition is met
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared</param>
    /// <param name="comparedValue">The value the property is being compared to</param>
    /// <param name="comparison">The predicate to use</param>
    /// <param name="default">What should happen if the condition is not met</param>
    public DrawIfAttribute(string comparedPropertyName, object comparedValue, ComparisonType comparison, PropertyDrawingType defaultBehavior)
    {
      this.ComparedPropertyName = comparedPropertyName;
      this.ComparedValue = comparedValue;
      this.Comparison = comparison;
      this.DefaultBehavior = defaultBehavior;      
    }
  }



}
