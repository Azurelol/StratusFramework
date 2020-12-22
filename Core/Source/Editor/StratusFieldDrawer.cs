using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace Stratus
{
  public partial class StratusSerializedEditorObject
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
      public StratusSerializedFieldType propertyType { get; private set; }
      public bool isArray { get; private set; }
      public Dictionary<Type, Attribute> attributesByName { get; private set; } = new Dictionary<Type, Attribute>();

      //------------------------------------------------------------------------/
      // Properties: Static
      //------------------------------------------------------------------------/
      public static Type attributeType { get; } = typeof(System.Attribute);
      public static Type rangeAttributeType { get; } = typeof(RangeAttribute);
      public static Type hideInInspectorAttribute { get; } = typeof(HideInInspector);

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public FieldDrawer(FieldInfo field)
      {
        this.field = field;
        this.type = this.field.FieldType;
        this.name = ObjectNames.NicifyVariableName(field.Name);

        this.attributesByName.AddRange((Attribute attribute) => attribute.GetType(), this.field.GetCustomAttributes(attributeType));
        this.propertyType = SerializedFieldTypeExtensions.Deduce(field);
        this.isPrimitive = OdinSerializer.FormatterUtilities.IsPrimitiveType(this.type);
        this.isArray = typeof(IList).IsAssignableFrom(this.type); 
        // Drawable unless its meant to be hidden
        this.isDrawable = !this.attributesByName.ContainsKey(hideInInspectorAttribute);        
        // All currently fields are eventually of a single line
        this.height = StratusEditorUtility.lineHeight;
      }

      public override bool DrawEditorGUILayout(object target, bool isChild = false)
      {
        EditorGUI.BeginChangeCheck();
        switch (propertyType)
        {
          case StratusSerializedFieldType.ObjectReference:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.ObjectField(name, this.GetValue<UnityEngine.Object>(target), type, true));
            break;
          case StratusSerializedFieldType.Integer:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.IntField(name, this.GetValue<int>(target)));
            break;
          case StratusSerializedFieldType.Boolean:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.Toggle(name, this.GetValue<bool>(target)));
            break;
          case StratusSerializedFieldType.Float:
            OnFloatEditorGUILayout(target);
            break;
          case StratusSerializedFieldType.String:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.TextField(name, this.GetValue<string>(target)));
            break;
          case StratusSerializedFieldType.Color:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.ColorField(name, this.GetValue<Color>(target)));
            break;
          case StratusSerializedFieldType.LayerMask:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.LayerField(name, this.GetValue<LayerMask>(target)));
            break;
          case StratusSerializedFieldType.Enum:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.EnumPopup(name, this.GetValue<Enum>(target)));
            break;
          case StratusSerializedFieldType.Vector2:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.Vector2Field(name, this.GetValue<Vector2>(target)));
            break;
          case StratusSerializedFieldType.Vector3:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.Vector3Field(name, this.GetValue<Vector3>(target)));
            break;
          case StratusSerializedFieldType.Vector4:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.Vector4Field(name, this.GetValue<Vector4>(target)));
            break;
          case StratusSerializedFieldType.Rect:
            this.field.SetValue(target, UnityEditor.EditorGUILayout.RectField(name, this.GetValue<Rect>(target)));
            break;
          default:
            if (isArray)
            {
              EditorGUILayout.Space();
              StratusReorderableList.DrawCachedPolymorphicList(this.field, target);
            }
            else
            {
              UnityEditor.EditorGUILayout.LabelField($"No drawer implementation for {name} of type {type.Name}");
            }
            break;
        }

        if (EditorGUI.EndChangeCheck())
        {

          return true;
        }

        return false;
      }

      //public static void FieldEditorGUILayout(FieldInfo field, SerializedPropertyType propertyType, string name, object target)
      //{
      //  switch (propertyType)
      //  {
      //    case SerializedPropertyType.ObjectReference:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.ObjectField(name, this.GetValue<UnityEngine.Object>(target), type, true));
      //      break;
      //    case SerializedPropertyType.Integer:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.IntField(name, this.GetValue<int>(target)));
      //      break;
      //    case SerializedPropertyType.Boolean:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.Toggle(name, this.GetValue<bool>(target)));
      //      break;
      //    case SerializedPropertyType.Float:
      //      OnFloatEditorGUILayout(target);
      //      break;
      //    case SerializedPropertyType.String:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.TextField(name, this.GetValue<string>(target)));
      //      break;
      //    case SerializedPropertyType.Color:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.ColorField(name, this.GetValue<Color>(target)));
      //      break;
      //    case SerializedPropertyType.LayerMask:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.LayerField(name, this.GetValue<LayerMask>(target)));
      //      break;
      //    case SerializedPropertyType.Enum:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.EnumPopup(name, this.GetValue<Enum>(target)));
      //      break;
      //    case SerializedPropertyType.Vector2:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.Vector2Field(name, this.GetValue<Vector2>(target)));
      //      break;
      //    case SerializedPropertyType.Vector3:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.Vector3Field(name, this.GetValue<Vector3>(target)));
      //      break;
      //    case SerializedPropertyType.Vector4:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.Vector4Field(name, this.GetValue<Vector4>(target)));
      //      break;
      //    case SerializedPropertyType.Rect:
      //      this.field.SetValue(target, UnityEditor.EditorGUILayout.RectField(name, this.GetValue<Rect>(target)));
      //      break;
      //    default:
      //      if (isArray)
      //      {
      //        EditorGUILayout.Space();
      //        StratusReorderableList.DrawCachedPolymorphicList(this.field, target);
      //      }
      //      else
      //      {
      //        UnityEditor.EditorGUILayout.LabelField($"No drawer implementation for {name} of type {type.Name}");
      //      }
      //      break;
      //  }
      //}

      public override bool DrawEditorGUI(Rect position, object target)
      {
        EditorGUI.BeginChangeCheck();
        object value = null;
        switch (propertyType)
        {
          case StratusSerializedFieldType.ObjectReference:
            value = EditorGUI.ObjectField(position, name, GetValue<UnityEngine.Object>(target), type, true);
            break;
          case StratusSerializedFieldType.Integer:
            value = OnIntEditorGUI(position, target);
            break;
          case StratusSerializedFieldType.Boolean:
            value = EditorGUI.Toggle(position, name, GetValue<bool>(target));
            break;
          case StratusSerializedFieldType.Float:
            value = OnFloatEditorGUI(position, target);
            break;
          case StratusSerializedFieldType.String:
            value = EditorGUI.TextField(position, name, GetValue<string>(target));
            break;
          case StratusSerializedFieldType.Color:
            value = EditorGUI.ColorField(position, name, GetValue<Color>(target));
            break;
          case StratusSerializedFieldType.LayerMask:
            value = EditorGUI.LayerField(position, name, GetValue<LayerMask>(target));
            break;
          case StratusSerializedFieldType.Enum:
            StratusSearchableEnum.EnumPopup(position, name, GetValue<Enum>(target), (Enum selected) => SetValue(target, selected));
            break;
          case StratusSerializedFieldType.Vector2:
            value = EditorGUI.Vector2Field(position, name, GetValue<Vector2>(target));
            break;
          case StratusSerializedFieldType.Vector3:
            value = EditorGUI.Vector3Field(position, name, GetValue<Vector3>(target));
            break;
          case StratusSerializedFieldType.Vector4:
            value = EditorGUI.Vector4Field(position, name, GetValue<Vector4>(target));
            break;
          case StratusSerializedFieldType.Rect:
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
        position = EditorGUI.PrefixLabel(position, new GUIContent(name));
        //position.x += labelWidth;
        //position.width -= labelWidth;

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
        position = EditorGUI.PrefixLabel(position, new GUIContent(name));
        //position.x += labelWidth;
        //position.width -= labelWidth;

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
          this.SetValue(target, UnityEditor.EditorGUILayout.Slider(name, value, range.min, range.max));
        }
        else
        {
          this.SetValue(target, UnityEditor.EditorGUILayout.FloatField(name, this.GetValue<float>(target)));
        }
      }

      public object GetValue(object target) => field.GetValue(target);
      public T GetValue<T>(object target) => (T)field.GetValue(target);
      public void SetValue(object target, object value) => field.SetValue(target, value);
    }
  }


}