using UnityEngine;
using System.Collections;
using Stratus;
using UnityEditor;

namespace Genitus.Models
{
  [CustomPropertyDrawer(typeof(ScopeTargeting))]
  public class ScopeTargetingDrawer : StratusPropertyDrawer
  {
    // Draw the property inside the given rect
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //  // Using BeginProperty / EndProperty on the parent property means that
    //  // prefab override logic works on the entire property.
    //  EditorGUI.BeginProperty(position, label, property);
    //
    //  // Draw label
    //  //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    //
    //  // Draw the navigation property
    //  var propertyScope = property.FindPropertyRelative(nameof(TargetingScope.scope));
    //  EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.range)));
    //  EditorGUILayout.PropertyField(propertyScope, new GUIContent("Scope"));
    //
    //  //EditorGUI.indentLevel = 1;
    //
    //  // Draw fields
    //  var scope = (TargetingScope.Type)propertyScope.enumValueIndex;
    //  switch (scope)
    //  {
    //    case TargetingScope.Type.Single:
    //      break;
    //    case TargetingScope.Type.Radius:
    //      EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.length)), new GUIContent("Radius"));
    //      break;
    //    case TargetingScope.Type.Line:
    //      EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.length)), new GUIContent("Length"));
    //      EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.width)), new GUIContent("Width"));
    //      break;
    //  }
    //
    //  //EditorGUI.indentLevel = 0;
    //
    //  EditorGUI.EndProperty();
    //}

    protected override void OnDrawProperty(Rect position, SerializedProperty property)
    {
      var scopeProperty = property.FindPropertyRelative(nameof(ScopeTargeting.scope));
      var rangeProperty = property.FindPropertyRelative(nameof(ScopeTargeting.range));
      DrawPropertiesInSingleLine(position, scopeProperty, rangeProperty);

      //EditorGUI.indentLevel = 1;

      // Draw fields
      var scope = (ScopeTargeting.Type)scopeProperty.enumValueIndex;
      switch (scope)
      {
        case ScopeTargeting.Type.Single:
          break;
        case ScopeTargeting.Type.Radius:
          EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(ScopeTargeting.length)), new GUIContent("Radius"));
          break;
        case ScopeTargeting.Type.Line:
          EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(ScopeTargeting.length)), new GUIContent("Length"));
          EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(ScopeTargeting.width)), new GUIContent("Width"));
          break;
      }
    }
  }

} 