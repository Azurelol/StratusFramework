using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
  public abstract class ObjectPropertySelectorDrawer<T> : StratusPropertyDrawer
  {


    protected abstract Lines mode { get; }
    protected abstract string objectPropertyName { get; }
    protected override void DrawProperty(Rect position, SerializedProperty property)
    {
      SerializedProperty objectProperty = property.FindPropertyRelative(objectPropertyName);
      EditorGUI.PropertyField(position, objectProperty, GUIContent.none);
      position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
      DrawSelector(position, property, property.GetValue<T>());
    }

    protected override float GetPropertyHeight(SerializedProperty property)
    {
      return EditorGUIUtility.singleLineHeight * (mode == Lines.Double? 2f : 1f);
    }

    protected abstract void DrawSelector(Rect position, SerializedProperty property, T target);

    //protected abstract string[] GetSelectionList

  }

}