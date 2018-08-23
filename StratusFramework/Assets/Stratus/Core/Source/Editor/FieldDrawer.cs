﻿using System.Collections;
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
    public class FieldDrawer : Drawer
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public FieldInfo field { get; private set; }
      public SerializedPropertyType propertyType { get; private set; }
      public bool isArray { get; private set; }
      public Dictionary<Type, Attribute> attributesByName { get; private set; } = new Dictionary<Type, Attribute>();

      //------------------------------------------------------------------------/
      // Properties: Static
      //------------------------------------------------------------------------/
      public static Type attributeType { get; } = typeof(System.Attribute);
      public static Type rangeAttributeType { get; } = typeof(RangeAttribute);
      //private static Dictionary<Type, System.Func<Attribute, SerializedProperty, bool>> attributeFunctions { get; } = new Dictionary<Type, Func<Attribute, SerializedProperty, bool>>()
      //{
      //  { typeof(RangeAttribute), OnRangeAttribute},
      //};

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public FieldDrawer(FieldInfo field)
      {
        this.field = field;
        this.type = this.field.FieldType;
        this.name = ObjectNames.NicifyVariableName(field.Name);

        this.attributesByName.AddRange(this.field.GetCustomAttributes(attributeType), (Attribute attribute) => attribute.GetType());
        this.propertyType = SerializedSystemObject.DeducePropertyType(field);

        this.isDrawable = propertyType != SerializedPropertyType.Generic;
        this.isPrimitive = OdinSerializer.FormatterUtilities.IsPrimitiveType(this.type);
        this.isArray = typeof(IList).IsAssignableFrom(this.type); //this.type.IsArray || IsList(this.type);
        this.height = StratusEditorUtility.lineHeight;
      }

      public FieldDrawer(FieldInfo field, SerializedPropertyType propertyType)
      {
        this.field = field;
        this.type = this.field.FieldType;
        this.name = ObjectNames.NicifyVariableName(field.Name);
        this.propertyType = propertyType;
        this.isDrawable = propertyType != SerializedPropertyType.Generic;
        this.isPrimitive = OdinSerializer.FormatterUtilities.IsPrimitiveType(this.type);
        this.isArray = typeof(IList).IsAssignableFrom(this.type); //this.type.IsArray || IsList(this.type);
      }

      public override bool DrawEditorGUILayout(object target)
      {
        EditorGUI.BeginChangeCheck();
        switch (propertyType)
        {
          case SerializedPropertyType.ObjectReference:
            field.SetValue(target, EditorGUILayout.ObjectField(name, GetValue<UnityEngine.Object>(target), type, true));
            break;
          case SerializedPropertyType.Integer:
            field.SetValue(target, EditorGUILayout.IntField(name, GetValue<int>(target)));
            break;
          case SerializedPropertyType.Boolean:
            field.SetValue(target, EditorGUILayout.Toggle(name, GetValue<bool>(target)));
            break;
          case SerializedPropertyType.Float:
            OnFloatEditorGUILayout(target);
            break;
          case SerializedPropertyType.String:
            field.SetValue(target, EditorGUILayout.TextField(name, GetValue<string>(target)));
            break;
          case SerializedPropertyType.Color:
            field.SetValue(target, EditorGUILayout.ColorField(name, GetValue<Color>(target)));
            break;
          case SerializedPropertyType.LayerMask:
            field.SetValue(target, EditorGUILayout.LayerField(name, GetValue<LayerMask>(target)));
            break;
          case SerializedPropertyType.Enum:
            field.SetValue(target, EditorGUILayout.EnumPopup(name, GetValue<Enum>(target)));
            break;
          case SerializedPropertyType.Vector2:
            field.SetValue(target, EditorGUILayout.Vector2Field(name, GetValue<Vector2>(target)));
            break;
          case SerializedPropertyType.Vector3:
            field.SetValue(target, EditorGUILayout.Vector3Field(name, GetValue<Vector3>(target)));
            break;
          case SerializedPropertyType.Vector4:
            field.SetValue(target, EditorGUILayout.Vector4Field(name, GetValue<Vector4>(target)));
            break;
          case SerializedPropertyType.Rect:
            field.SetValue(target, EditorGUILayout.RectField(name, GetValue<Rect>(target)));
            break;
          default:
            if (isArray)
            {
              StratusEditorUtility.DrawPolymorphicList(this.field, target, this.name);
              //EditorGUILayout.LabelField($"No drawer implementation for {label} of type {type.Name}");
            }
            else
            {
              EditorGUILayout.LabelField($"No drawer implementation for {name} of type {type.Name}");
            }
            break;
        }

        if (EditorGUI.EndChangeCheck())
        {

          return true;
        }

        return false;
      }

      public override bool DrawEditorGUI(Rect position, object target)
      {
        EditorGUI.BeginChangeCheck();
        object value = null;
        switch (propertyType)
        {
          case SerializedPropertyType.ObjectReference:
            value = EditorGUI.ObjectField(position, name, GetValue<UnityEngine.Object>(target), type, true);
            break;
          case SerializedPropertyType.Integer:
            value = OnIntEditorGUI(position, target);
            break;
          case SerializedPropertyType.Boolean:
            value = EditorGUI.Toggle(position, name, GetValue<bool>(target));
            break;
          case SerializedPropertyType.Float:
            value = OnFloatEditorGUI(position, target);
            break;
          case SerializedPropertyType.String:
            value = EditorGUI.TextField(position, name, GetValue<string>(target));
            break;
          case SerializedPropertyType.Color:
            value = EditorGUI.ColorField(position, name, GetValue<Color>(target));
            break;
          case SerializedPropertyType.LayerMask:
            value = EditorGUI.LayerField(position, name, GetValue<LayerMask>(target));
            break;
          case SerializedPropertyType.Enum:
            //value = EditorGUI.EnumPopup(position, name, GetValue<Enum>(target));
            SearchableEnum.EnumPopup(position, name, GetValue<Enum>(target), (Enum selected) => SetValue(target, selected));
            break;
          case SerializedPropertyType.Vector2:
            value = EditorGUI.Vector2Field(position, name, GetValue<Vector2>(target));
            break;
          case SerializedPropertyType.Vector3:
            value = EditorGUI.Vector3Field(position, name, GetValue<Vector3>(target));
            break;
          case SerializedPropertyType.Vector4:
            value = EditorGUI.Vector4Field(position, name, GetValue<Vector4>(target));
            break;
          case SerializedPropertyType.Rect:
            value = EditorGUI.RectField(position, name, GetValue<Rect>(target));
            break;
          default:
            EditorGUI.LabelField(position, $"No supported drawer for type {type.Name}!");
            break;
        }

        if (EditorGUI.EndChangeCheck())
        {
          SetValue(target, value);
          return true;
        }

        return false;
      }

      public object OnFloatEditorGUI(Rect position, object target)
      {
        EditorGUI.PrefixLabel(position, new GUIContent(name));
        position.x += labelWidth;
        position.width -= labelWidth;

        float value = GetValue<float>(target);
        if (attributesByName.ContainsKey(rangeAttributeType))
        {
          RangeAttribute range = attributesByName[rangeAttributeType] as RangeAttribute;
          value = EditorGUI.Slider(position, value, range.min, range.max);
        }
        else
        {
          value = EditorGUI.FloatField(position, value);
        }
        return value;
      }

      public int OnIntEditorGUI(Rect position, object target)
      {
        EditorGUI.PrefixLabel(position, new GUIContent(name));
        position.x += labelWidth;
        position.width -= labelWidth;

        int value = GetValue<int>(target);
        if (attributesByName.ContainsKey(rangeAttributeType))
        {
          RangeAttribute range = attributesByName[rangeAttributeType] as RangeAttribute;
          value = EditorGUI.IntSlider(position, value, (int)range.min, (int)range.max);
        }
        else
        {
          value = EditorGUI.IntField(position, value);
        }
        return value;
      }

      public void OnFloatEditorGUILayout(object target)
      {
        float value = GetValue<float>(target);
        if (attributesByName.ContainsKey(rangeAttributeType))
        {
          RangeAttribute range = attributesByName[rangeAttributeType] as RangeAttribute;
          SetValue(target, EditorGUILayout.Slider(name, value, range.min, range.max));
        }
        else
        {
          SetValue(target, EditorGUILayout.FloatField(name, GetValue<float>(target)));
        }
      }

      public object GetValue(object target) => field.GetValue(target);
      public T GetValue<T>(object target) => (T)field.GetValue(target);
      public void SetValue(object target, object value) => field.SetValue(target, value);
    }
  }


}