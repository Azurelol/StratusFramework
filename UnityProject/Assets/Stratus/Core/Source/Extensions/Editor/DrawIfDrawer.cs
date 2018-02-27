/******************************************************************************/
/*!
@file   DrawIfDrawer.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;
using System.Reflection;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(DrawIfAttribute))]
  public class DrawIfPropertyDrawer : PropertyDrawer
  {
    // The attribute on the property
    DrawIfAttribute DrawIf;
    // Field being compared
    SerializedProperty ComparedField;
    // Height of the property
    float PropertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return PropertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Set the global variables
      DrawIf = attribute as DrawIfAttribute;

      // Whether the condition has been met
      bool conditionMet = false;

      // If we are doing a property comparison
      if (DrawIf.predicate == PredicateMode.PropertyComparison)
      {
        ComparedField = property.serializedObject.FindProperty(DrawIf.comparedPropertyName);
        // Get the value of the compared field
        object comparedFieldValue = ComparedField.GetValue<object>();
        // References to the values as numeric types
        INumeric numericComparedFieldValue = null;
        INumeric numericComparedValue = null;

        // Try to set the numeric types
        try
        {
          numericComparedFieldValue = new INumeric(comparedFieldValue);
          numericComparedValue = new INumeric(DrawIf.comparedValue);
        }
        catch (NumericTypeExpectedException)
        {
          if (DrawIf.comparison != ComparisonType.Equals && DrawIf.comparison != ComparisonType.NotEqual)
          {
            Trace.Error("The only comparsion types available to type '" + comparedFieldValue.GetType() + "' are Equals and NotEqual. (On object '" + property.serializedObject.targetObject.name + "')", null, true);
           return;
          }
        }
        // Compare the values to see if the condition has been met
        switch (DrawIf.comparison)
        {
          case ComparisonType.Equals:
            if (comparedFieldValue.Equals(DrawIf.comparedValue))
              conditionMet = true;
            break;

          case ComparisonType.NotEqual:
            if (!comparedFieldValue.Equals(DrawIf.comparedValue))
              conditionMet = true;
            break;

          case ComparisonType.Greater:
            if (numericComparedFieldValue > numericComparedValue)
              conditionMet = true;
            break;

          case ComparisonType.Lesser:
            if (numericComparedFieldValue < numericComparedValue)
              conditionMet = true;
            break;

          case ComparisonType.LesserOrEqual:
            if (numericComparedFieldValue <= numericComparedValue)
              conditionMet = true;
            break;

          case ComparisonType.GreaterOrEqual:
            if (numericComparedFieldValue >= numericComparedValue)
              conditionMet = true;
            break;
        }
      }
      // Else if we are checking a predicate
      else if (DrawIf.predicate == PredicateMode.Predicate)
      {
        //var booly = property.serializedObject..GetProperty<bool>(DrawIf.predicateName);
        //SerializedProperty predicateProperty = property.serializedObject.FindProperty(DrawIf.predicateName);
        //if (predicateProperty.propertyType == SerializedPropertyType.Boolean)
        //  conditionMet = predicateProperty.boolValue;

        // Make sure that the right component is present
        Component component = Selection.activeGameObject.GetComponent(DrawIf.type);
        if (component == null)
          throw new System.Exception("The component of type " + DrawIf.type.Name + " is missing from the selected GameObject");

        // We can now safely invoke the method on the component
        if (DrawIf.isProperty)
          conditionMet = (bool)DrawIf.predicateProperty.GetValue(component, null);
        else
          conditionMet = (bool)DrawIf.predicateMethod.Invoke(component, null);
      }

      // The height of the property should be defaulted to the default height
      PropertyHeight = base.GetPropertyHeight(property, label);

      // If the condition is met, draw the field
      if (conditionMet)
      {
        //EditorGUILayout.PropertyField(property);
        EditorGUI.PropertyField(position, property);
      }
      // Otherwise use the default ebhavior
      else
      {
        if (DrawIf.defaultBehavior == PropertyDrawingType.ReadOnly)
        {
          UnityEngine.GUI.enabled = false;
          //EditorGUILayout.PropertyField(property);
          EditorGUI.PropertyField(position, property);
          UnityEngine.GUI.enabled = true;
        }
        else
        {
          PropertyHeight = 0f;
        }
      }
    }
  }
}

#endif