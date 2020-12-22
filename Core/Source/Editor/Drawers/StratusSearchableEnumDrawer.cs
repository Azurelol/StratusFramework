using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
  public class SearchableEnumDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      label = EditorGUI.BeginProperty(position, label, property);
      {
        StratusSearchableEnum.EnumPopup(position, label, property);
      }
      EditorGUI.EndProperty();
    }
  }
}