using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Stratus.Utilities;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(SerializableClass), true)]
  public class SerializableClassDrawer : PropertyDrawer
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The current height needed to draw this property
    /// </summary>
    public float height { get; private set; }
    /// <summary>
    /// The number of fields in this property
    /// </summary>
    public float fieldCount { get; private set; }
    /// <summary>
    /// An array of the types of fields in this property
    /// </summary>
    public FieldInfo[] fields { get; private set; }
    /// <summary>
    /// The line height to be used
    /// </summary>
    public float lineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    /// <summary>
    /// Whether this property is an array
    /// </summary>
    public bool isArray { get; private set; }
    /// <summary>
    /// Whether this property has multiple values
    /// </summary>
    public bool hasMultipleFields { get; private set; }
    /// <summary>
    /// Whether to allow foldout of the field in the inspector
    /// </summary>
    public bool foldout { get; private set; } = false;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      isArray = property.name != fieldInfo.Name;

      if (isArray)
      {
        height = EditorGUI.GetPropertyHeight(property);
      }
      else
      {
        fields = fieldInfo.FieldType.GetFields();
        fieldCount = fields.Length;
        property = property.serializedObject.FindProperty(fieldInfo.Name);
        hasMultipleFields = fieldCount > 1;
        height = property.isExpanded ? fieldCount + 1 : 1;
        height *= lineHeight;
      }

      //Trace.Script($"Height for {property.name} = {height}");
      return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

      //Trace.Script($"Drawing properties for property {property.name} of field {fieldInfo.Name}");
      position.height = EditorGUIUtility.singleLineHeight;

      label = EditorGUI.BeginProperty(position, label, property);
      {
        if (isArray)
        {
          EditorGUI.indentLevel++;
          DrawArray(position, property);
          EditorGUI.indentLevel--;
        }
        else
        {
          DrawMultipleFields(position, property, label);
        }
      }

      EditorGUI.EndProperty();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void DrawArray(Rect position, SerializedProperty property)
    {
      EditorGUI.PropertyField(position, property, true);
    }

    private void DrawMultipleFields(Rect position, SerializedProperty property, GUIContent label)
    {
      property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

      if (property.isExpanded)
      {
        position.y += lineHeight;
        DrawFields(position, property);
      }
    }

    private void DrawFields(Rect position, SerializedProperty property)
    {
      foreach (var field in fields)
      {
        //Trace.Script($"- Drawing {field.Name}");
        EditorGUI.PropertyField(position, property.FindPropertyRelative(field.Name));
        position.y += lineHeight;
      }
    }  





  }

}