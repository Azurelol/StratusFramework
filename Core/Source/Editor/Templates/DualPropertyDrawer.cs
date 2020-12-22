using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  /// <summary>
  /// Generic property drawer for classes with two properties that should be displayed side by side.
  /// </summary>
  public abstract class DualPropertyDrawer : StratusPropertyDrawer
  {
    protected abstract string firstProperty { get; }
    protected abstract string secondProperty { get; }

    protected override void OnDrawProperty(Rect position, SerializedProperty property)
    {
      SerializedProperty first = property.FindPropertyRelative(firstProperty);
      SerializedProperty second = property.FindPropertyRelative(secondProperty);
      position.width /= 2f;
      EditorGUI.PropertyField(position, first, GUIContent.none);
      position.x += position.width;
      EditorGUI.PropertyField(position, second, GUIContent.none);
    }

    
  }

  /// <summary>
  /// Generic property drawer for classes with two variable properties that should be displayed side by side.
  /// </summary>
  public abstract class DualDynamicPropertyDrawer : StratusPropertyDrawer
  {
    protected override void OnDrawProperty(Rect position, SerializedProperty property)
    {
      SerializedProperty first = GetFirstProperty(property);
      SerializedProperty second = GetSecondProperty(property);
      position.width /= 2f;
      EditorGUI.PropertyField(position, first, GUIContent.none);
      position.x += position.width;
      EditorGUI.PropertyField(position, second, GUIContent.none);
    }

    protected abstract SerializedProperty GetFirstProperty(SerializedProperty property);
    protected abstract SerializedProperty GetSecondProperty(SerializedProperty property);
  }


}