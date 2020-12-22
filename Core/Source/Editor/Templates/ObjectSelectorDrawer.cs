using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
  public abstract class ObjectPropertySelectorDrawer<T> : StratusPropertyDrawer
  {
    protected abstract string objectPropertyName { get; }
    protected override void OnDrawProperty(Rect position, SerializedProperty property)
    {
      SerializedProperty objectProperty = property.FindPropertyRelative(objectPropertyName);
      EditorGUI.BeginChangeCheck();
      EditorGUI.PropertyField(position, objectProperty, GUIContent.none);
      bool changed = EditorGUI.EndChangeCheck();
      if (objectProperty.objectReferenceValue != null && !changed)
      {
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        DrawSelector(position, property, property.GetValue<T>());
      }
    }

    protected override float GetPropertyHeight(SerializedProperty property)
    {
      SerializedProperty objectProperty = property.FindPropertyRelative(objectPropertyName);
      return EditorGUIUtility.singleLineHeight * (objectProperty.objectReferenceValue != null ? 2f : 1f);
    }

    protected abstract void DrawSelector(Rect position, SerializedProperty property, T target);

    //protected abstract string[] GetSelectionList

  }

}