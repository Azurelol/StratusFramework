using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using OdinSerializer;

namespace Stratus
{
  /// <summary>
  /// Edits System.Object types in a completely generic way
  /// </summary>
  public partial class SerializedSystemObject
  {
    /// <summary>
    /// Base class for all drawers
    /// </summary>
    public class Drawer
    {
      public string label { get; protected set; }
      public Type type { get; protected set; }
    }

    /// <summary>
    /// Draws all the fields in a System.Object
    /// </summary>
    public class SystemObjectDrawer : Drawer
    {
      public FieldInfoDrawer[] fieldDrawers { get; private set; }
      public FieldInfo[] fields { get; private set; }
      public bool hasFields => fields.NotEmpty();
      public bool hasDefaultConstructor { get; private set; }
      public bool isDrawable { get; private set; }
      public int fieldCount => fieldDrawers.Length;
      public bool isArray { get; private set; }

      public SystemObjectDrawer(Type type)
      {
        this.type = type;
        this.fields = type.GetFields();
        this.fieldDrawers = GenerateDrawers(fields);
        this.isDrawable = this.fieldDrawers.NotEmpty();
        this.hasDefaultConstructor = (type.GetConstructor(Type.EmptyTypes) != null) || type.IsValueType;
      }

      public bool DrawEditorGUILayout(object target)
      {
        bool changed = false;
        foreach (var drawer in fieldDrawers)
          changed |= drawer.DrawEditorGUILayout(target);
        return changed;
      }

      public bool DrawEditorGUI(Rect position, object target)
      {
        bool changed = false;
        foreach (var drawer in fieldDrawers)
        {
          //position.height = EditorGUIUtility.singleLineHeight;
          changed |= drawer.DrawEditorGUI(position, target);
          position.y += StratusEditorUtility.lineHeight;
        }
        return changed;
      }

      public static FieldInfoDrawer[] GenerateDrawers(FieldInfo[] fields)
      {
        List<FieldInfoDrawer> drawers = new List<FieldInfoDrawer>();
        for (int i = 0; i < fields.Length; ++i)
        {
          FieldInfoDrawer drawer = new FieldInfoDrawer(fields[i]);
          if (drawer.isValid)
            drawers.Add(drawer);          
        }
        return drawers.ToArray();
      }
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public SystemObjectDrawer drawer { get; private set; }
    public object target { get; private set; }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public SerializedSystemObject(Type type, object target)
    {
      this.drawer = new SystemObjectDrawer(type);
      this.target = target;      
    }

    public bool Draw()
    {
      return this.drawer.DrawEditorGUILayout(this.target);
    }

    public string Serialize()
    {
      string data = JsonUtility.ToJson(target);
      return data;
    }

    public void Deserialize(string data)
    {
      JsonUtility.FromJsonOverwrite(data, target);
    }

    public void Serialize(UnityEngine.Object targetObject, SerializedProperty stringProperty)
    {
      string data = JsonUtility.ToJson(target);
      stringProperty.stringValue = data;
      Undo.RecordObject(targetObject, stringProperty.displayName);
      stringProperty.serializedObject.ApplyModifiedProperties();
    }

    public void Deserialize(SerializedProperty stringProperty)
    {
      string data = stringProperty.stringValue;
      JsonUtility.FromJsonOverwrite(data, target);
    }

    //------------------------------------------------------------------------/
    // Static Methods
    //------------------------------------------------------------------------/
    public static SerializedPropertyType DeducePropertyType(FieldInfo field)
    {      
      Type type = field.FieldType;
      SerializedPropertyType propertyType = SerializedPropertyType.Generic;

      if (type == typeof(UnityEngine.Object))
        propertyType = SerializedPropertyType.ObjectReference;
      else if (type.Equals(typeof(bool)))
        propertyType = SerializedPropertyType.Boolean;
      else if (type.Equals(typeof(int)))
        propertyType = SerializedPropertyType.Integer;
      else if (type.Equals(typeof(float)))
        propertyType = SerializedPropertyType.Float;
      else if (type.Equals(typeof(string)))
        propertyType = SerializedPropertyType.String;
      else if (type.Equals(typeof(Vector2)))
        propertyType = SerializedPropertyType.Vector2;
      else if (type.Equals(typeof(Vector3)))
        propertyType = SerializedPropertyType.Vector3;
      else if (type.Equals(typeof(Vector4)))
        propertyType = SerializedPropertyType.Vector4;
      else if (type.Equals(typeof(Color)))
        propertyType = SerializedPropertyType.Color;
      else if (type.IsEnum)
        propertyType = SerializedPropertyType.Enum;
      else if (type.Equals(typeof(Rect)))
        propertyType = SerializedPropertyType.Rect;
      else if (type.Equals(typeof(LayerMask)))
        propertyType = SerializedPropertyType.LayerMask;
      return propertyType;
    }

    public static bool IsList(object o)
    {
      if (o == null) return false;
      return o is IList &&
             o.GetType().IsGenericType &&
             o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
    }

    public static bool IsList(Type type)
    {
      return type.IsGenericType &&
             type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
    }

    public static bool IsDictionary(object o)
    {
      if (o == null) return false;
      return o is IDictionary &&
             o.GetType().IsGenericType &&
             o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
    }
  }
}