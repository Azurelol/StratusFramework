using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace Stratus
{
  public partial class SerializedSystemObject
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Draws a specific field
    /// </summary>
    public class FieldInfoDrawer
    {
      public FieldInfo field { get; private set; }
      public SerializedPropertyType propertyType { get; private set; }
      public string label { get; private set; }
      public Type type { get; private set; }
      public bool isValid { get; private set; }
      public bool isArray { get; private set; }
      public bool isPrimitive { get; private set; }

      public FieldInfoDrawer(FieldInfo field)
      {
        this.field = field;
        this.type = this.field.FieldType;
        this.label = ObjectNames.NicifyVariableName(field.Name);
        this.propertyType = SerializedSystemObject.DeducePropertyType(field);
        isValid = propertyType != SerializedPropertyType.Generic;
        this.isPrimitive = OdinSerializer.FormatterUtilities.IsPrimitiveType(this.type);
        this.isArray = typeof(IList).IsAssignableFrom(this.type); //this.type.IsArray || IsList(this.type);
      }

      public bool DrawEditorGUILayout(object target)
      {
        EditorGUI.BeginChangeCheck();
        switch (propertyType)
        {
          case SerializedPropertyType.ObjectReference:
            field.SetValue(target, EditorGUILayout.ObjectField(label, GetValue<UnityEngine.Object>(target), type, true));
            break;
          case SerializedPropertyType.Integer:
            field.SetValue(target, EditorGUILayout.IntField(label, GetValue<int>(target)));
            break;
          case SerializedPropertyType.Boolean:
            field.SetValue(target, EditorGUILayout.Toggle(label, GetValue<bool>(target)));
            break;
          case SerializedPropertyType.Float:
            field.SetValue(target, EditorGUILayout.FloatField(label, GetValue<float>(target)));
            break;
          case SerializedPropertyType.String:
            field.SetValue(target, EditorGUILayout.TextField(label, GetValue<string>(target)));
            break;
          case SerializedPropertyType.Color:
            field.SetValue(target, EditorGUILayout.ColorField(label, GetValue<Color>(target)));
            break;
          case SerializedPropertyType.LayerMask:
            field.SetValue(target, EditorGUILayout.LayerField(label, GetValue<LayerMask>(target)));
            break;
          case SerializedPropertyType.Enum:
            field.SetValue(target, EditorGUILayout.EnumPopup(label, GetValue<Enum>(target)));
            break;
          case SerializedPropertyType.Vector2:
            field.SetValue(target, EditorGUILayout.Vector2Field(label, GetValue<Vector2>(target)));
            break;
          case SerializedPropertyType.Vector3:
            field.SetValue(target, EditorGUILayout.Vector3Field(label, GetValue<Vector3>(target)));
            break;
          case SerializedPropertyType.Vector4:
            field.SetValue(target, EditorGUILayout.Vector4Field(label, GetValue<Vector4>(target)));
            break;
          case SerializedPropertyType.Rect:
            field.SetValue(target, EditorGUILayout.RectField(label, GetValue<Rect>(target)));
            break;
          default:
            if (isArray)
            {
              StratusEditorUtility.DrawPolymorphicList(this.field, target, this.label);
              //EditorGUILayout.LabelField($"No drawer implementation for {label} of type {type.Name}");
            }
            else
            {
              EditorGUILayout.LabelField($"No drawer implementation for {label} of type {type.Name}");
            }
            break;
        }

        if (EditorGUI.EndChangeCheck())
          return true;

        return false;
      }

      public bool DrawEditorGUI(Rect position, object target)
      {
        EditorGUI.BeginChangeCheck();
        switch (propertyType)
        {
          case SerializedPropertyType.ObjectReference:
            field.SetValue(target, EditorGUI.ObjectField(position, label, GetValue<UnityEngine.Object>(target), type, true));
            break;
          case SerializedPropertyType.Integer:
            field.SetValue(target, EditorGUI.IntField(position, label, GetValue<int>(target)));
            break;
          case SerializedPropertyType.Boolean:
            field.SetValue(target, EditorGUI.Toggle(position, label, GetValue<bool>(target)));
            break;
          case SerializedPropertyType.Float:
            field.SetValue(target, EditorGUI.FloatField(position, label, GetValue<float>(target)));
            break;
          case SerializedPropertyType.String:
            field.SetValue(target, EditorGUI.TextField(position, label, GetValue<string>(target)));
            break;
          case SerializedPropertyType.Color:
            field.SetValue(target, EditorGUI.ColorField(position, label, GetValue<Color>(target)));
            break;
          case SerializedPropertyType.LayerMask:
            field.SetValue(target, EditorGUI.LayerField(position, label, GetValue<LayerMask>(target)));
            break;
          case SerializedPropertyType.Enum:
            field.SetValue(target, EditorGUI.EnumPopup(position, label, GetValue<Enum>(target)));
            break;
          case SerializedPropertyType.Vector2:
            field.SetValue(target, EditorGUI.Vector2Field(position, label, GetValue<Vector2>(target)));
            break;
          case SerializedPropertyType.Vector3:
            field.SetValue(target, EditorGUI.Vector3Field(position, label, GetValue<Vector3>(target)));
            break;
          case SerializedPropertyType.Vector4:
            field.SetValue(target, EditorGUI.Vector4Field(position, label, GetValue<Vector4>(target)));
            break;
          case SerializedPropertyType.Rect:
            field.SetValue(target, EditorGUI.RectField(position, label, GetValue<Rect>(target)));
            break;
          default:
            EditorGUI.LabelField(position, $"No supported drawer for type {type.Name}!");
            break;
        }

        if (EditorGUI.EndChangeCheck())
          return true;

        return false;
      }

      public object GetValue(object target) => field.GetValue(target);
      public T GetValue<T>(object target) => (T)field.GetValue(target);
      public void SetValue(object target, object value) => field.SetValue(target, value);
    }
  }

  
}