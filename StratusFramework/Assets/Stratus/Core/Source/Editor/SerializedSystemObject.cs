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
  public class SerializedSystemObject
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Draws a specific field
    /// </summary>
    public struct FieldInfoDrawer
    {
      public FieldInfo field;
      public SerializedPropertyType propertyType;
      public string label;
      public Type type;
      public bool isValid;

      public FieldInfoDrawer(FieldInfo field) : this()
      {
        this.field = field;
        this.type = this.field.FieldType;
        this.label = ObjectNames.NicifyVariableName(field.Name);
        this.propertyType = DeducePropertyType(field);
        isValid = propertyType != SerializedPropertyType.Generic;
      }

      public object GetValue(object target) => field.GetValue(target);
      public T GetValue<T>(object target) => (T)field.GetValue(target);
      public void SetValue(object target, object value) => field.SetValue(target, value);
    }

    /// <summary>
    /// Draws all the fields in a System.Object
    /// </summary>
    public class SystemObjectDrawer
    {
      public Type type { get; private set; }
      public FieldInfoDrawer[] fieldDrawers { get; private set; }
      public FieldInfo[] fields { get; private set; }
      public bool hasFields => fields.NotEmpty();
      public bool hasDefaultConstructor { get; private set; }
      public bool isDrawable { get; private set; }
      public int fieldCount => fieldDrawers.Length;

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
          changed |= DrawEditorGUILayout(drawer, target);
        return changed;
      }

      public bool DrawEditorGUI(Rect position, object target)
      {
        bool changed = false;
        foreach (var drawer in fieldDrawers)
        {
          //position.height = EditorGUIUtility.singleLineHeight;
          changed |= DrawEditorGUI(position, drawer, target);
          position.y += StratusEditorUtility.lineHeight;
        }
        return changed;
      }

      private bool DrawEditorGUILayout(FieldInfoDrawer drawer, object target)
      {
        EditorGUI.BeginChangeCheck();
        switch (drawer.propertyType)
        {
          case SerializedPropertyType.ObjectReference:
            drawer.field.SetValue(target, EditorGUILayout.ObjectField(drawer.label, drawer.GetValue<UnityEngine.Object>(target), drawer.type, true));
            break;
          case SerializedPropertyType.Integer:
            drawer.field.SetValue(target, EditorGUILayout.IntField(drawer.label, drawer.GetValue<int>(target)));
            break;
          case SerializedPropertyType.Boolean:
            drawer.field.SetValue(target, EditorGUILayout.Toggle(drawer.label, drawer.GetValue<bool>(target)));
            break;
          case SerializedPropertyType.Float:
            drawer.field.SetValue(target, EditorGUILayout.FloatField(drawer.label, drawer.GetValue<float>(target)));
            break;
          case SerializedPropertyType.String:
            drawer.field.SetValue(target, EditorGUILayout.TextField(drawer.label, drawer.GetValue<string>(target)));
            break;
          case SerializedPropertyType.Color:
            drawer.field.SetValue(target, EditorGUILayout.ColorField(drawer.label, drawer.GetValue<Color>(target)));
            break;
          case SerializedPropertyType.LayerMask:
            drawer.field.SetValue(target, EditorGUILayout.LayerField(drawer.label, drawer.GetValue<LayerMask>(target)));
            break;
          case SerializedPropertyType.Enum:
            drawer.field.SetValue(target, EditorGUILayout.EnumPopup(drawer.label, drawer.GetValue<Enum>(target)));
            break;
          case SerializedPropertyType.Vector2:
            drawer.field.SetValue(target, EditorGUILayout.Vector2Field(drawer.label, drawer.GetValue<Vector2>(target)));
            break;
          case SerializedPropertyType.Vector3:
            drawer.field.SetValue(target, EditorGUILayout.Vector3Field(drawer.label, drawer.GetValue<Vector3>(target)));
            break;
          case SerializedPropertyType.Vector4:
            drawer.field.SetValue(target, EditorGUILayout.Vector4Field(drawer.label, drawer.GetValue<Vector4>(target)));
            break;
          case SerializedPropertyType.Rect:
            drawer.field.SetValue(target, EditorGUILayout.RectField(drawer.label, drawer.GetValue<Rect>(target)));
            break;
          default:
            break;
        }

        if (EditorGUI.EndChangeCheck())
          return true;

        return false;
      }

      private bool DrawEditorGUI(Rect position, FieldInfoDrawer drawer, object target)
      {
        EditorGUI.BeginChangeCheck();
        switch (drawer.propertyType)
        {
          case SerializedPropertyType.ObjectReference:
            drawer.field.SetValue(target, EditorGUI.ObjectField(position, drawer.label, drawer.GetValue<UnityEngine.Object>(target), drawer.type, true));
            break;
          case SerializedPropertyType.Integer:
            drawer.field.SetValue(target, EditorGUI.IntField(position, drawer.label, drawer.GetValue<int>(target)));
            break;
          case SerializedPropertyType.Boolean:
            drawer.field.SetValue(target, EditorGUI.Toggle(position, drawer.label, drawer.GetValue<bool>(target)));
            break;
          case SerializedPropertyType.Float:
            drawer.field.SetValue(target, EditorGUI.FloatField(position, drawer.label, drawer.GetValue<float>(target)));
            break;
          case SerializedPropertyType.String:
            drawer.field.SetValue(target, EditorGUI.TextField(position, drawer.label, drawer.GetValue<string>(target)));
            break;
          case SerializedPropertyType.Color:
            drawer.field.SetValue(target, EditorGUI.ColorField(position, drawer.label, drawer.GetValue<Color>(target)));
            break;
          case SerializedPropertyType.LayerMask:
            drawer.field.SetValue(target, EditorGUI.LayerField(position, drawer.label, drawer.GetValue<LayerMask>(target)));
            break;
          case SerializedPropertyType.Enum:
            drawer.field.SetValue(target, EditorGUI.EnumPopup(position, drawer.label, drawer.GetValue<Enum>(target)));
            break;
          case SerializedPropertyType.Vector2:
            drawer.field.SetValue(target, EditorGUI.Vector2Field(position, drawer.label, drawer.GetValue<Vector2>(target)));
            break;
          case SerializedPropertyType.Vector3:
            drawer.field.SetValue(target, EditorGUI.Vector3Field(position, drawer.label, drawer.GetValue<Vector3>(target)));
            break;
          case SerializedPropertyType.Vector4:
            drawer.field.SetValue(target, EditorGUI.Vector4Field(position, drawer.label, drawer.GetValue<Vector4>(target)));
            break;
          case SerializedPropertyType.Rect:
            drawer.field.SetValue(target, EditorGUI.RectField(position, drawer.label, drawer.GetValue<Rect>(target)));
            break;
          default:
            EditorGUI.LabelField(position, $"No supported drawer for type {drawer.type.Name}!");
            break;
        }

        if (EditorGUI.EndChangeCheck())
          return true;

        return false;
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
  }
}