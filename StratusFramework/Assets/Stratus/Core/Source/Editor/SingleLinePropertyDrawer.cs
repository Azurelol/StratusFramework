using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  /// <summary>
  /// Used for quick setup of property drawers for classes that want to be drawn in a single line
  /// </summary>
  public abstract class SingleLinePropertyDrawer : PropertyDrawer
  {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      //label = EditorGUI.BeginProperty(position, label, property);
      //Rect contentPosition = EditorGUI.PrefixLabel(position, label);
      //{
      //  Draw(property);
      //}
      //EditorGUI.EndProperty();
    }

    protected abstract void Draw(SerializedProperty property);

  }

  public abstract class SingleLineVariablePropertyDrawer<EnumType>: SingleLinePropertyDrawer where EnumType : struct
  {
    protected abstract string typePropertyName { get; }
    protected abstract void Draw(SerializedProperty property, EnumType type);

    protected override void Draw(SerializedProperty property)
    {
      // 1. Modify the type
      //SerializedProperty typeProperty = property.FindPropertyRelative("typePropertyName");
      //contentPosition.width = width * typeWidth;
      //EditorGUI.PropertyField(contentPosition, typeProp, GUIContent.none);

      //EnumType type = (EnumType)Convert.ChangeType(typeProperty.enumValueIndex, typeof(EnumType)); // (EnumType)typeProperty.enumValueIndex;      
    }

    


  }

}