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
using System.Reflection;

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
    ReadOnly = 2,
    DontDraw = 3
  }
  
  /// <summary>
  /// How the drawing of this attribute is decided
  /// </summary>
  public enum PredicateMode
  {
    PropertyComparison,
    Predicate
  }

  /// <summary>
  /// Draws the field/property only if the predicate comparing the property returns true
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
  public class DrawIfAttribute : PropertyAttribute
  {
    public PredicateMode predicate { get; private set; }
    public string comparedPropertyName { get; private set; }
    public object comparedValue { get; private set; }
    public ComparisonType comparison { get; private set; }
    public PropertyDrawingType defaultBehavior { get; private set; }
    public string predicateName { get; private set; }
    //public Type type { get; private set; }    
    //public MethodInfo predicateMethod { get; private set; }
    //public PropertyInfo predicateProperty { get; private set; }
    //public bool isProperty { get; private set; }

    /// <summary>
    /// Only draws the field if the condition is met
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared</param>
    /// <param name="comparedValue">The value the property is being compared to</param>
    /// <param name="comparison">The predicate to use</param>
    /// <param name="default">What should happen if the condition is not met</param>
    public DrawIfAttribute(string comparedPropertyName, object comparedValue, ComparisonType comparison, PropertyDrawingType defaultBehavior = PropertyDrawingType.DontDraw)
    {
      this.predicate = PredicateMode.PropertyComparison;
      this.comparedPropertyName = comparedPropertyName;
      this.comparedValue = comparedValue;
      this.comparison = comparison;
      this.defaultBehavior = defaultBehavior;      
    }

    /// <summary>
    /// Only draws the field if the condition is met
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared</param>
    /// <param name="comparedValue">The value the property is being compared to</param>
    /// <param name="comparison">The predicate to use</param>
    /// <param name="default">What should happen if the condition is not met</param>
    public DrawIfAttribute(string predicateFunctionName, PropertyDrawingType defaultBehavior = PropertyDrawingType.DontDraw)
    {
      this.predicate = PredicateMode.Predicate;
      this.predicateName = predicateFunctionName;

      //this.type = type;
      //
      //this.predicateMethod = type.GetMethod(predicateName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      //if (this.predicateMethod != null)
      //  isProperty = false;
      //
      //this.predicateProperty = type.GetProperty(predicateName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
      //if (this.predicateProperty != null)
      //  isProperty = true;      
      //
      //if (predicateMethod == null && predicateProperty == null)
      //  throw new System.Exception($"The predicate method or property {predicateName} is missing!");

      this.defaultBehavior = defaultBehavior;
    }
    
    ///// <summary>
    ///// Only draws the field if the condition is met
    ///// </summary>
    ///// <param name="comparedPropertyName">The name of the property that is being compared</param>
    ///// <param name="comparedValue">The value the property is being compared to</param>
    ///// <param name="comparison">The predicate to use</param>
    ///// <param name="default">What should happen if the condition is not met</param>
    //public DrawIfAttribute(string predicatePropertyName, PropertyDrawingType defaultBehavior = PropertyDrawingType.DontDraw)
    //{
    //  this.predicate = PredicateMode.Predicate;
    //  this.predicateName = predicatePropertyName;
    //  this.defaultBehavior = defaultBehavior;
    //}
  }



}
