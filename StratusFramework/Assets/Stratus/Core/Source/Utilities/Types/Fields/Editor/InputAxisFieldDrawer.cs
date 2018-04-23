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
        //SerializedProperty index = property.FindPropertyRelative("index");

        EditorGUI.BeginProperty(position, label, axis);
        int index = InputManagerUtility.GetIndex(axis.stringValue);
        index = EditorGUI.Popup(position, label.text, index, InputManagerUtility.axesNames);
        axis.stringValue = index >= 0 ? InputManagerUtility.axesNames[index] : "";
        EditorGUI.EndProperty();
      }

    } 

  }
}