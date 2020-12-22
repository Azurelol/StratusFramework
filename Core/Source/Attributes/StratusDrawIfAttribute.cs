using System;
using UnityEngine;

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
		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public PredicateMode predicate { get; private set; }
		public string comparedMemberName { get; private set; }
		public object comparedValue { get; private set; }
		public ComparisonType comparison { get; private set; }
		public PropertyDrawingType defaultBehavior { get; private set; }

		//--------------------------------------------------------------------------------------------/
		// CTOR
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Only draws the field if the condition is met
		/// </summary>
		/// <param name="memberName">The name of the property that is being compared</param>
		/// <param name="comparedValue">The value the property is being compared to</param>
		/// <param name="comparison">The predicate to use</param>
		/// <param name="default">What should happen if the condition is not met</param>
		public DrawIfAttribute(string memberName, object comparedValue, ComparisonType comparison, PropertyDrawingType defaultBehavior = PropertyDrawingType.DontDraw)
		{
			this.comparedMemberName = memberName;
			this.predicate = PredicateMode.PropertyComparison;
			this.comparedValue = comparedValue;
			this.comparison = comparison;
			this.defaultBehavior = defaultBehavior;
		}

		/// <summary>
		/// Only draws the field if the boolean member is true
		/// </summary>
		public DrawIfAttribute(string memberName, PropertyDrawingType defaultBehavior = PropertyDrawingType.DontDraw)
		{
			this.comparedMemberName = memberName;
			this.predicate = PredicateMode.Predicate;
			this.comparison = ComparisonType.Equals;
			this.comparedValue = true;
			this.defaultBehavior = defaultBehavior;
		}
	}



}
