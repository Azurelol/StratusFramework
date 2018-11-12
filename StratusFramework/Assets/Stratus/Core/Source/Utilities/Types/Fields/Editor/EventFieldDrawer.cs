using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(EventField))]
  public class EventFieldDrawer : StratusPropertyDrawer
  {

    float height = 0;

    protected override float GetPropertyHeight(SerializedProperty property)
    {
      //return base.GetPropertyHeight(property);
      return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      SerializedProperty typeProp = property.FindPropertyRelative(nameof(EventField.type));
      SerializedProperty scopeProperty = property.FindPropertyRelative(nameof(EventField.scope));
      StratusEvent.Scope scope = GetEnumValue<StratusEvent.Scope>(scopeProperty);

      //SerializedProperty typeProp = property.FindPropertyRelative("Type");
      //Type eventType = typeProp.objectReferenceValue as Type;

      label = EditorGUI.BeginProperty(position, label, property);
      EditorGUI.PrefixLabel(position, label);

      
        float initialHeight = position.y;
      DrawPropertiesVertical(ref position, typeProp, scopeProperty);
    
      // Scope
      if (scope == StratusEvent.Scope.GameObject)
      {
        DrawSingleProperty(ref position, property.FindPropertyRelative(nameof(EventField.targets)));
        //AddLine(ref position);
      }
      height = position.y - initialHeight;
      EditorGUI.EndProperty();
    }

    protected override void OnDrawProperty(Rect position, SerializedProperty property)
    {
      SerializedProperty typeProp = property.FindPropertyRelative(nameof(EventField.type));
      SerializedProperty scopeProperty = property.FindPropertyRelative(nameof(EventField.scope));
      StratusEvent.Scope scope = GetEnumValue<StratusEvent.Scope>(scopeProperty);

      //SerializedProperty typeProp = property.FindPropertyRelative("Type");
      //Type eventType = typeProp.objectReferenceValue as Type;
      DrawPropertiesVertical(ref position, typeProp, scopeProperty);
      
      // Scope
      if (scope == StratusEvent.Scope.GameObject)
      {
        EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(EventField.targets)));
        AddLine(ref position);
      }



    }
  }
}