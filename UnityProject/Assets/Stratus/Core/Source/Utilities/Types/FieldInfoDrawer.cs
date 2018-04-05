#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace Stratus
{
  /// <summary>
  /// Edits System.Object types in a completely generic way
  /// </summary>
  public class SerializedSystemObject
  {
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

    public Type type { get; private set; }
    public FieldInfoDrawer[] fieldDrawers { get; private set; }
    public FieldInfo[] fields { get; private set; }
    public object target { get; private set; }
    public bool hasFields => fields.NotEmpty();
    public bool hasDefaultConstructor { get; private set; }
    public bool isDrawable { get; private set; }

    public SerializedSystemObject(Type type, object target)
    {
      this.type = type;
      this.fields = type.GetFields();
      this.fieldDrawers = GenerateDrawers(fields);
      this.target = target;
      this.hasDefaultConstructor = (type.GetConstructor(Type.EmptyTypes) != null) || type.IsValueType;
    }

    public bool DrawEditorGUILayout(FieldInfoDrawer drawer)
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

    public bool DrawFields()
    {
      bool changed = false;
      foreach (var drawer in fieldDrawers)
        changed |= DrawEditorGUILayout(drawer);
      return changed;
    }

    public FieldInfoDrawer[] GenerateDrawers(FieldInfo[] fields)
    {
      int validTypes = 0;
      FieldInfoDrawer[] drawers = new FieldInfoDrawer[fields.Length];
      for (int i = 0; i < fields.Length; ++i)
      {
        drawers[i] = new FieldInfoDrawer(fields[i]);
        if (drawers[i].isValid) validTypes++;
      }

      if (validTypes > 0)
        isDrawable = true;

      return drawers;
    }

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



  }
}


#endif