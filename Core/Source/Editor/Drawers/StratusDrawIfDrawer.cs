
using System;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	[CustomPropertyDrawer(typeof(DrawIfAttribute))]
	public class DrawIfPropertyDrawer : PropertyDrawer
	{
		//private static Dictionary<Type, >
		public static Type propertyDrawerType { get; } = typeof(PropertyDrawer);
		public static MemberInfo[] propertyDrawerMembers { get; }
			= propertyDrawerType.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// The attribute on the property
		private DrawIfAttribute drawIf;

		// Field being compared
		private SerializedProperty comparedMember;

		// Height of the property
		private float propertyHeight;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return this.propertyHeight;
			//return property.isExpanded ? EditorGUI.GetPropertyHeight(property) : 0f;
			///return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Set the global variables
			this.drawIf = this.attribute as DrawIfAttribute;

			// Whether the condition has been met
			bool conditionMet = false;

			// The actual target needed
			object target = property.GetParent<object>();
			Type targetType = this.fieldInfo.ReflectedType;


			if (this.drawIf.predicate == PredicateMode.PropertyComparison)
			{
				//this.comparedMember = property.serializedObject.FindProperty(this.drawIf.comparedMemberName);
				this.comparedMember = property.serializedObject.FindProperty(this.drawIf.comparedMemberName);
				// Get the value of the compared field
				object comparedFieldValue = target.GetFieldOrPropertyValue<object>(this.drawIf.comparedMemberName); // ( targetType.Getpro this.comparedMember.GetValue<object>();
																													// References to the values as numeric types
				StratusNumeric numericComparedFieldValue = null;
				StratusNumeric numericComparedValue = null;

				// Try to set the numeric types
				try
				{
					numericComparedFieldValue = new StratusNumeric(comparedFieldValue);
					numericComparedValue = new StratusNumeric(this.drawIf.comparedValue);
				}
				catch (StratusNumericTypeExpectedException)
				{
					if (this.drawIf.comparison != ComparisonType.Equals && this.drawIf.comparison != ComparisonType.NotEqual)
					{
						StratusDebug.LogError("The only comparsion types available to type '" + comparedFieldValue.GetType() + "' are Equals and NotEqual. (On object '" + property.serializedObject.targetObject.name + "')", null);
						return;
					}
				}
				// Compare the values to see if the condition has been met
				switch (this.drawIf.comparison)
				{
					case ComparisonType.Equals:
						if (comparedFieldValue.Equals(this.drawIf.comparedValue))
						{
							conditionMet = true;
						}

						break;

					case ComparisonType.NotEqual:
						if (!comparedFieldValue.Equals(this.drawIf.comparedValue))
						{
							conditionMet = true;
						}

						break;

					case ComparisonType.Greater:
						if (numericComparedFieldValue > numericComparedValue)
						{
							conditionMet = true;
						}

						break;

					case ComparisonType.Lesser:
						if (numericComparedFieldValue < numericComparedValue)
						{
							conditionMet = true;
						}

						break;

					case ComparisonType.LesserOrEqual:
						if (numericComparedFieldValue <= numericComparedValue)
						{
							conditionMet = true;
						}

						break;

					case ComparisonType.GreaterOrEqual:
						if (numericComparedFieldValue >= numericComparedValue)
						{
							conditionMet = true;
						}

						break;
				}
			}
			// Else if we are checking a predicate
			else if (this.drawIf.predicate == PredicateMode.Predicate)
			{
				// Method
				MethodInfo predicateMethod = targetType.GetMethod(this.drawIf.comparedMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (predicateMethod != null)
				{
					conditionMet = (bool)predicateMethod.Invoke(target, null);
				}
				// Property
				else
				{
					PropertyInfo predicateProperty = targetType.GetProperty(this.drawIf.comparedMemberName,
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);

					if (predicateProperty != null)
					{
						conditionMet = (bool)predicateProperty.GetValue(target, null);
					}
					else
					{
						throw new System.Exception($"{this.fieldInfo.Name} , {this.fieldInfo.ReflectedType.GetNiceFullName()} : The type {targetType.GetNiceFullName()} is missing the boolean property {this.drawIf.comparedMemberName}");
					}
				}
			}

			// The height of the property should be defaulted to the default height
			this.propertyHeight = EditorGUI.GetPropertyHeight(property);
			//propertyHeight = base.GetPropertyHeight(property, label);

			// If the condition is met, draw the field
			if (conditionMet)
			{
				//base.OnGUI(position, property, label);
				StratusEditorUtility.UseDefaultDrawer(position, property, label, fieldInfo.FieldType);
			}
			// Otherwise use the default ebhavior
			else
			{
				if (this.drawIf.defaultBehavior == PropertyDrawingType.ReadOnly)
				{

					UnityEngine.GUI.enabled = false;
					StratusEditorUtility.UseDefaultDrawer(position, property, label, targetType);
					UnityEngine.GUI.enabled = true;
				}
				else
				{
					this.propertyHeight = 0f;
				}
			}
		}
	}
}

