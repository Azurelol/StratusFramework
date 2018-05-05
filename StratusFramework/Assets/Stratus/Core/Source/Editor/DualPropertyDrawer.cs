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

    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //  label = EditorGUI.BeginProperty(position, label, property);
    //  position = EditorGUI.PrefixLabel(position, label);
    //  {
    //    SerializedProperty first = property.FindPropertyRelative(firstProperty);
    //    SerializedProperty second = property.FindPropertyRelative(secondProperty);
    //    position.width /= 2f;
    //    EditorGUI.PropertyField(position, first, GUIContent.none);
    //    position.x += position.width;
    //    EditorGUI.PropertyField(position, second, GUIContent.none);
    //  }
    //  EditorGUI.EndProperty();
    //}

    protected override void DrawProperty(Rect position, SerializedProperty property)
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
    protected override void DrawProperty(Rect position, SerializedProperty property)
    {
      SerializedProperty first = GetFirstProperty(property);
      SerializedProperty second = GetSecondProperty(property);
      position.width /= 2f;
      EditorGUI.PropertyField(position, first, GUIContent.none);
      position.x += position.width;
      EditorGUI.PropertyField(position, second, GUIContent.none);
    }

    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //  label = EditorGUI.BeginProperty(position, label, property);
    //  position = EditorGUI.PrefixLabel(position, label);
    //  {
    //    SerializedProperty first = GetFirstProperty(property);
    //    SerializedProperty second = GetSecondProperty(property);
    //    position.width /= 2f;
    //    EditorGUI.PropertyField(position, first, GUIContent.none);
    //    position.x += position.width;
    //    EditorGUI.PropertyField(position, second, GUIContent.none);
    //
    //  }
    //  EditorGUI.EndProperty();
    //}

    protected abstract SerializedProperty GetFirstProperty(SerializedProperty property);
    protected abstract SerializedProperty GetSecondProperty(SerializedProperty property);
  }


}