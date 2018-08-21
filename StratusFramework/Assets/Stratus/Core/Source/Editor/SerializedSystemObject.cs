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
    public abstract class Drawer
    {
      public string name { get; protected set; }
      public string displayName { get; protected set; }
      public Type type { get; protected set; }
      public abstract bool DrawEditorGUILayout(object target);
      public abstract bool DrawEditorGUI(Rect position, object target);
      public bool isDrawable { get; protected set; }
      public bool isPrimitive { get; protected set; }
    }

    /// <summary>
    /// Draws all the fields in a System.Object
    /// </summary>
    public class SystemObjectDrawer : Drawer
    {
      public SystemObjectDrawer parent { get; private set; }
      public Drawer[] fieldDrawers { get; private set; }
      public FieldInfo[] fields { get; private set; }
      public Dictionary<string, FieldInfo> fieldsByName { get; private set; } = new Dictionary<string, FieldInfo>();
      public bool hasFields => fields.NotEmpty();
      public bool hasDefaultConstructor { get; private set; }
      public int fieldCount => fieldDrawers.Length;
      public bool isArray { get; private set; }
      public bool isField { get; private set; }

      public SystemObjectDrawer(Type type, SystemObjectDrawer parent = null)
      {
        this.type = type;
        this.fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        this.fieldsByName.AddRange(this.fields, (FieldInfo field) => field.Name);
        this.fieldDrawers = GenerateDrawers(fields);
        this.isDrawable = this.fieldDrawers.NotEmpty();
        this.hasDefaultConstructor = (type.GetConstructor(Type.EmptyTypes) != null) || type.IsValueType;
      }

      public void SetParent(SystemObjectDrawer parent, string fieldName)
      {
        this.parent = parent;
        this.isField = true;
        this.name = fieldName;
        this.displayName = ObjectNames.NicifyVariableName(this.name);
      }

      //public static object GetDefaultValueForProperty(PropertyInfo property)
      //{
      //  var defaultAttr = property.GetCustomAttribute(typeof(DefaultValueAttribute));
      //  if (defaultAttr != null)
      //    return (defaultAttr as DefaultValueAttribute).Value;
      //
      //  var propertyType = property.PropertyType;
      //  return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
      //}

      public override bool DrawEditorGUILayout(object target)
      {
        bool changed = false;
        string content = this.isDrawable ? this.displayName : $"There are no serialized fields for {type.Name}";
        EditorGUILayout.LabelField(content);

        if (this.isField)
        {
          EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        }

        foreach (var drawer in fieldDrawers)
        {
          // If this is a member inside a member
          if (this.isField)
          {
            object value = GetValueOrSetDefault(target);
            changed |= drawer.DrawEditorGUILayout(value);
          }
          else
          {
            changed |= drawer.DrawEditorGUILayout(target);
          }
        }

        if (this.isField)
        {
          EditorGUILayout.EndVertical();
        }

        return changed;
      }

      private object GetValueOrSetDefault(object target)
      {
        FieldInfo field = this.parent.fieldsByName[this.name];
        // Try to get the value from the taret
        object value;
        value = field.GetValue(target);
        // If the field hasn't been instantiated
        if (value == null)
        {
          value = Activator.CreateInstance(this.type);
          field.SetValue(target, value);
        }
        return value;
      }

      public override bool DrawEditorGUI(Rect position, object target)
      {
        bool changed = false;
        string content = this.isDrawable ? this.displayName : $"There are no serialized fields for {type.Name}";
        EditorGUI.LabelField(position, content);

        if (this.isField)
        {
          EditorGUI.indentLevel++;
          //GUI.BeginGroup(position, EditorStyles.helpBox);
        }

        position.y += StratusEditorUtility.lineHeight;

        // Draw all drawers
        foreach (var drawer in fieldDrawers)
        {
          // If this is a member inside a member
          if (this.isField)
          {
            object value = GetValueOrSetDefault(target);
            changed |= drawer.DrawEditorGUI(position, value);
          }
          else
          {
            changed |= drawer.DrawEditorGUI(position, target);
          }
          position.y += StratusEditorUtility.lineHeight;
        }

        if (this.isField)
        {
          EditorGUI.indentLevel--;
          //GUI.EndGroup();
        }

        return changed;
      }

      private Drawer[] GenerateDrawers(FieldInfo[] fields)
      {
        List<Drawer> drawers = new List<Drawer>();
        for (int i = 0; i < fields.Length; ++i)
        {
          FieldInfo field = fields[i];
          Type fieldType = field.FieldType;
          SerializedPropertyType serializedPropertyType = DeducePropertyType(field);

          bool isUnitySupportedType = serializedPropertyType != SerializedPropertyType.Generic; //  OdinSerializer.FormatterUtilities.IsPrimitiveType(fieldType);
          if (isUnitySupportedType)
          {
            FieldDrawer drawer = new FieldDrawer(field);
            if (drawer.isDrawable)
              drawers.Add(drawer);
          }
          else
          {
            SystemObjectDrawer drawer = new SystemObjectDrawer(fieldType);
            drawer.SetParent(this, field.Name);
            if (drawer.isDrawable)
              drawers.Add(drawer);
          }

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