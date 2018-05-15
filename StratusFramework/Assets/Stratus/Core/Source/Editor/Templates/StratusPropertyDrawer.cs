using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Stratus
{
  /// <summary>
  /// Generic Property Drawer with a added utility functions
  /// </summary>
  public abstract class StratusPropertyDrawer : PropertyDrawer
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public enum Lines
    {
      Single,
      Double,
      Dynamic
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this property is an array
    /// </summary>
    public bool isArray { get; private set; }
    ///// <summary>
    ///// The current height needed to draw this property
    ///// </summary>
    //public float height { get; private set; }
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
    /// Whether this property has multiple values
    /// </summary>
    public bool hasMultipleFields { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      isArray = property.name != fieldInfo.Name;

      float height = 0f;

      if (isArray)
      {
        //fields = fieldInfo.FieldType.GetFields();
        //fieldCount = fields.Length;
        //hasMultipleFields = fieldCount > 1;

        foreach(var child in GetChildren(property))
        {
          height += GetPropertyHeight(child);
        }

        //height = EditorGUI.GetPropertyHeight(property);
        //for (int c = 0; c < property.; ++c)
        //{
        //  height += GetPropertyHeight(property.GetArrayElementAtIndex(c));
        //}
        //height = GetPropertyHeight(property);
      }
      else
      {
        height = GetPropertyHeight(property);
        //fields = fieldInfo.FieldType.GetFields();
        //fieldCount = fields.Length;
        //property = property.serializedObject.FindProperty(fieldInfo.Name);
        //hasMultipleFields = fieldCount > 1;
        //height = property.isExpanded ? fieldCount + 1 : 1;
        //height *= lineHeight;
      }

      return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      //isArray = property.name != fieldInfo.Name;
      isArray = property.isArray;

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
          position = EditorGUI.PrefixLabel(position, label);
          DrawProperty(position, property);
          //DrawMultipleFields(position, property, label);
        }
      }

      EditorGUI.EndProperty();
    }

    protected virtual void DrawProperty(Rect position, SerializedProperty property)
    {
      EditorGUI.PropertyField(position, property);
    }

    protected virtual float GetPropertyHeight(SerializedProperty property)
    {
      float value = 0;
      fields = fieldInfo.FieldType.GetFields();
      fieldCount = fields.Length;
      property = property.serializedObject.FindProperty(fieldInfo.Name);
      hasMultipleFields = fieldCount > 1;
      value = property.isExpanded ? fieldCount + 1 : 1;
      value *= lineHeight;
      return value;
    }

    //protected 

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void DrawArray(Rect position, SerializedProperty property)
    {
      for(int e = 0; e < property.arraySize; ++e)
      {
        SerializedProperty arrayElement = property.GetArrayElementAtIndex(e);
        DrawProperty(position, arrayElement);
      }
      //SerializedProperty[] children = property.arr
      //EditorGUI.PropertyField(position, property, true);
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
        EditorGUI.PropertyField(position, property.FindPropertyRelative(field.Name));
        position.y += lineHeight;
      }
    }

    public T GetEnumValue<T>(SerializedProperty property, string enumPropertyName)
    {
      SerializedProperty enumProperty = property.FindPropertyRelative(enumPropertyName);
      T value = (T)(object)enumProperty.enumValueIndex;
      return value;
    }

    public static void DrawPropertiesInSingleLine(Rect position, SerializedProperty[] children)
    {
      int n = children.Length;
      position.width /= n;
      for (int p = 0; p < n; ++p)
      {
        SerializedProperty property = children[n];
        EditorGUI.PropertyField(position, property, GUIContent.none);
        position.x += position.width;
      }
    }

    public static IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
    {
      property = property.Copy();
      var nextElement = property.Copy();
      bool hasNextElement = nextElement.NextVisible(false);
      if (!hasNextElement)
      {
        nextElement = null;
      }

      property.NextVisible(true);
      while (true)
      {
        if ((SerializedProperty.EqualContents(property, nextElement)))
        {
          yield break;
        }

        yield return property;

        bool hasNext = property.NextVisible(false);
        if (!hasNext)
        {
          break;
        }
      }
    }


  }

}