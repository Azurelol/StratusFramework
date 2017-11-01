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
      ComparedField = property.serializedObject.FindProperty(DrawIf.ComparedPropertyName);
      // Get the value of the compared field
      object comparedFieldValue = ComparedField.GetValue<object>();
      // References to the values as numeric types
      INumeric numericComparedFieldValue = null;
      INumeric numericComparedValue = null;

      // Try to set the numeric types
      try
      {
        numericComparedFieldValue = new INumeric(comparedFieldValue);
        numericComparedValue = new INumeric(DrawIf.ComparedValue);
      }
      catch (NumericTypeExpectedException)
      {
        if (DrawIf.Comparison != ComparisonType.Equals && DrawIf.Comparison != ComparisonType.NotEqual)
        {
          Trace.Error("The only comparsion types available to type '" + comparedFieldValue.GetType() + "' are Equals and NotEqual. (On object '" + property.serializedObject.targetObject.name + "')", null, true);
         return;
        }
      }

      // Whether the condition has been met
      bool conditionMet = false;

      // Compare the values to see if the condition has been met
      switch (DrawIf.Comparison)
      {
        case ComparisonType.Equals:
          if (comparedFieldValue.Equals(DrawIf.ComparedValue))
            conditionMet = true;
          break;

        case ComparisonType.NotEqual:
          if (!comparedFieldValue.Equals(DrawIf.ComparedValue))
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

      // The height of the property should be defaulted to the default height
      PropertyHeight = base.GetPropertyHeight(property, label);

      // If the condition is met, draw the field
      if (conditionMet)
        EditorGUI.PropertyField(position, property);
      // Otherwise use the default ebhavior
      else
      {
        if (DrawIf.DefaultBehavior == PropertyDrawingType.ReadOnly)
        {
          UnityEngine.GUI.enabled = false;
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