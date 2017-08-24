using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(InputAxisField))]
    public class InputAxesFieldDrawer : PropertyDrawer
    {
      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        SerializedProperty axis = property.FindPropertyRelative("axis");
        SerializedProperty index = property.FindPropertyRelative("index");

        EditorGUI.BeginProperty(position, label, axis);
        index.intValue = EditorGUI.Popup(position, label.text, index.intValue, InputManagerUtility.axes);
        axis.stringValue = InputManagerUtility.axes[index.intValue];
        EditorGUI.EndProperty();
      }

    } 

  }
}