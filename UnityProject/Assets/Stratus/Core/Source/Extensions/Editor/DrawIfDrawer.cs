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
    DrawIfAttribute drawIf;
    // Field being compared
    SerializedProperty comparedField;
    // Height of the property
    float propertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return propertyHeight;
      //return property.isExpanded ? EditorGUI.GetPropertyHeight(property) : 0f;
      ///return EditorGUI.GetPropertyHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Set the global variables
      drawIf = attribute as DrawIfAttribute;

      // Whether the condition has been met
      bool conditionMet = false;

      // If we are doing a property comparison
      if (drawIf.predicate == PredicateMode.PropertyComparison)
      {
        comparedField = property.serializedObject.FindProperty(drawIf.comparedPropertyName);
        // Get the value of the compared field
        object comparedFieldValue = comparedField.GetValue<object>();
        // References to the values as numeric types
        INumeric numericComparedFieldValue = null;
        INumeric numericComparedValue = null;

        // Try to set the numeric types
        try
        {
          numericComparedFieldValue = new INumeric(comparedFieldValue);
          numericComparedValue = new INumeric(drawIf.comparedValue);
        }
        catch (NumericTypeExpectedException)
        {
          if (drawIf.comparison != ComparisonType.Equals && drawIf.comparison != ComparisonType.NotEqual)
          {
            Trace.Error("The only comparsion types available to type '" + comparedFieldValue.GetType() + "' are Equals and NotEqual. (On object '" + property.serializedObject.targetObject.name + "')", null, true);
            return;
          }
        }
        // Compare the values to see if the condition has been met
        switch (drawIf.comparison)
        {
          case ComparisonType.Equals:
            if (comparedFieldValue.Equals(drawIf.comparedValue))
              conditionMet = true;
            break;

          case ComparisonType.NotEqual:
            if (!comparedFieldValue.Equals(drawIf.comparedValue))
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
      else if (drawIf.predicate == PredicateMode.Predicate)
      {
        //var booly = property.serializedObject..GetProperty<bool>(DrawIf.predicateName);
        //SerializedProperty predicateProperty = property.serializedObject.FindProperty(DrawIf.predicateName);
        //if (predicateProperty.propertyType == SerializedPropertyType.Boolean)
        //  conditionMet = predicateProperty.boolValue;

        MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;
        MethodInfo predicateMethod = mb.GetType().GetMethod(drawIf.predicateName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (predicateMethod != null)
        {
          conditionMet = (bool)predicateMethod.Invoke(mb, null);
        }
        else
        {
          PropertyInfo predicateProperty = mb.GetType().GetProperty(drawIf.predicateName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
          if (predicateProperty != null)
            conditionMet = (bool)predicateProperty.GetValue(mb, null);
          else
            throw new System.Exception("The component is missing the predicate" + drawIf.predicateName);
        }



        //// Make sure that the right component is present
        //Component component = Selection.activeGameObject.GetComponent(drawIf.type);
        //if (component == null)
        //  throw new System.Exception("The component of type " + drawIf.type.Name + " is missing from the selected GameObject");
        //
        //// We can now safely invoke the method on the component
        //if (drawIf.isProperty)
        //  conditionMet = (bool)drawIf.predicateProperty.GetValue(component, null);
        //else
        //  conditionMet = (bool)drawIf.predicateMethod.Invoke(component, null);
      }

      // The height of the property should be defaulted to the default height
      propertyHeight = EditorGUI.GetPropertyHeight(property);
      //propertyHeight = base.GetPropertyHeight(property, label);

      // If the condition is met, draw the field
      if (conditionMet)
      {
        //EditorGUILayout.PropertyField(property);
        EditorGUI.PropertyField(position, property, true);
      }
      // Otherwise use the default ebhavior
      else
      {
        if (drawIf.defaultBehavior == PropertyDrawingType.ReadOnly)
        {
          UnityEngine.GUI.enabled = false;
          //EditorGUILayout.PropertyField(property);
          EditorGUI.PropertyField(position, property, true);
          UnityEngine.GUI.enabled = true;
        }
        else
        {
          propertyHeight = 0f;
        }
      }
    }
  }
}

#endif