using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
  public class OnValueChangedDrawer : PropertyDrawer
  {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      EditorGUI.BeginChangeCheck();
      EditorGUI.PropertyField(position, property, true);
      if (EditorGUI.EndChangeCheck())
      {
        var onValueChanged = attribute as OnValueChangedAttribute;
        MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;
        MethodInfo method = mb.GetType().GetMethod(onValueChanged.method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        method.Invoke(mb, null);
      }
      EditorGUI.EndProperty();
    }


  }

}