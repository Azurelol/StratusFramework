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

    public class DrawCommand
    {
      public SerializedProperty property;
      public bool drawContent;

      public DrawCommand(SerializedProperty property)
      {
        this.property = property;
      }

      public virtual void Draw(Rect position)
      {
        if (drawContent)
          EditorGUI.PropertyField(position, property);
        else
          EditorGUI.PropertyField(position, property, GUIContent.none);
      }
    }

    public class DrawPopUp : DrawCommand
    { 
      public string[] values;

      public DrawPopUp(SerializedProperty property, string[] values) : base(property)
      {
        this.values = values;
      }

      public override void Draw(Rect position)
      {
        DrawPopup(position, property, values);
      }
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
    /// Standard vertical spacing betwween controls
    /// </summary>
    public float verticalSpacing => EditorGUIUtility.standardVerticalSpacing;
    /// <summary>
    /// Whether this property has multiple values
    /// </summary>
    public bool hasMultipleFields { get; private set; }   
    /// <summary>
    /// The current height for this property
    /// </summary>
    public float propertyHeight { get; protected set; }
    /// <summary>
    /// The parent property in an array
    /// </summary>
    protected SerializedProperty parent{ get; private set; }
    /// <summary>
    /// The inspected object that this property belongs to
    /// </summary>
    protected Object target { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      isArray = property.name != fieldInfo.Name;

      float height = 0f;
      height += verticalSpacing;

      if (isArray)
      {
        foreach(var child in GetChildren(property))
        {
          height += GetPropertyHeight(child);
        }

        //fields = fieldInfo.FieldType.GetFields();
        //fieldCount = fields.Length;
        //hasMultipleFields = fieldCount > 1;


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

      target = property.serializedObject.targetObject;
      isArray = property.isArray;
      propertyHeight = 0f;

      position.y += verticalSpacing;
      position.height = EditorGUIUtility.singleLineHeight;
      label = EditorGUI.BeginProperty(position, label, property);
      {
        if (isArray)
        {
          //EditorGUI.indentLevel++;
          DrawArray(position, property);
          //EditorGUI.indentLevel--;
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

    protected abstract void DrawProperty(Rect position, SerializedProperty property);
    //{
    //  EditorGUI.PropertyField(position, property);
    //}

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
      parent = property;

      for (int e = 0; e < property.arraySize; ++e)
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

    protected void DrawSingleProperty(ref Rect position, SerializedProperty property)
    {
      EditorGUI.PropertyField(position, property);
      position.y += lineHeight;
      propertyHeight += lineHeight;
    }

    public T GetEnumValue<T>(SerializedProperty property, string enumPropertyName)
    {
      SerializedProperty enumProperty = property.FindPropertyRelative(enumPropertyName);
      T value = (T)(object)enumProperty.enumValueIndex;
      return value;
    }

    /// <summary>
    ///  Doesn't work for flags apparently
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumProperty"></param>
    /// <returns></returns>
    public T GetEnumValue<T>(SerializedProperty enumProperty)
    {
      bool isFlag = typeof(T).IsDefined(typeof(System.FlagsAttribute), inherit: false);
      if (isFlag)
        return (T)(object)enumProperty.intValue;

      return (T)(object)enumProperty.enumValueIndex;
    }

    public static void DrawPropertiesInSingleLine(Rect position, params SerializedProperty[] children)
    {
      int n = children.Length;
      position.width /= n;
      for (int p = 0; p < n; ++p)
      {
        SerializedProperty property = children[p];
        EditorGUI.PropertyField(position, property, GUIContent.none);
        position.x += position.width;
      }
    }

    public static void DrawPropertiesInSingleLineLabeled(Rect position, params SerializedProperty[] children)
    {
      int n = children.Length;
      position.width /= n;
      for (int p = 0; p < n; ++p)
      {
        SerializedProperty property = children[p];
        EditorGUI.PropertyField(position, property);
        position.x += position.width;
      }
    }

    public static void DrawPropertiesInSingleLine(Rect position, params DrawCommand[] drawCommands)
    {
      int n = drawCommands.Length;
      position.width /= n;
      for (int p = 0; p < n; ++p)
      {
        drawCommands[p].Draw(position);
        position.x += position.width;
      }
    }

    public static void DrawPopup(Rect position, SerializedProperty stringProperty, string[] values)
    {
      int index = values.FindIndex(stringProperty.stringValue);
      index = EditorGUI.Popup(position, index, values);
      stringProperty.stringValue = values.AtIndexOrDefault(index, string.Empty);
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