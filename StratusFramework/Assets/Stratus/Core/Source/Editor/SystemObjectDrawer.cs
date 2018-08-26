using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using OdinSerializer;

namespace Stratus
{
  public partial class SerializedSystemObject
  {
    /// <summary>
    /// Draws all the fields in a System.Object
    /// </summary>
    public class ObjectDrawer : Drawer
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public ObjectDrawer parent { get; private set; }
      public Drawer[] drawers { get; private set; }
      public FieldInfo[] fields { get; private set; }
      public Dictionary<string, FieldInfo> fieldsByName { get; private set; } = new Dictionary<string, FieldInfo>();
      public bool hasFields => fields.NotEmpty();
      public bool hasDefaultConstructor { get; private set; }
      public int fieldCount => drawers.Length;
      public bool isArray { get; private set; }
      public bool isField { get; private set; }      

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public ObjectDrawer(Type type, ObjectDrawer parent = null)
      {
        this.type = type;
        this.fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        this.fieldsByName.AddRange(this.fields, (FieldInfo field) => field.Name);
        this.height = lineHeight;
        this.drawers = GenerateDrawers(fields);
        this.isDrawable = this.drawers.NotEmpty();
        this.hasDefaultConstructor = (type.GetConstructor(Type.EmptyTypes) != null) || type.IsValueType;
      }

      public void SetParent(ObjectDrawer parent, string fieldName)
      {
        this.parent = parent;
        this.isField = true;
        this.height -= lineHeight;
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

      //------------------------------------------------------------------------/
      // Methods: Draw
      //------------------------------------------------------------------------/
      public override bool DrawEditorGUILayout(object target)
      {
        bool changed = false;
        string content = this.isDrawable ? this.displayName : $"No serialized fields available";
        UnityEditor.EditorGUILayout.LabelField(content);

        if (this.isField)
        {
          UnityEditor.EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        }

        foreach (var drawer in drawers)
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
          UnityEditor.EditorGUILayout.EndVertical();
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
        string content = this.isDrawable ? this.displayName : $"No serialized fields for {type.Name}";
        EditorGUI.LabelField(position, content);

        if (this.isField)
        {
          EditorGUI.indentLevel++;
          //GUI.BeginGroup(position, EditorStyles.helpBox);
          position.y += lineHeight;
        }


        // Draw all drawers
        foreach (var drawer in drawers)
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
          position.y += lineHeight;
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
            {
              drawers.Add(drawer);
              this.height += drawer.height;
            }

          }
          else
          {
            ObjectDrawer drawer = new ObjectDrawer(fieldType);
            drawer.SetParent(this, field.Name);
            if (drawer.isDrawable)
            {
              drawers.Add(drawer);
              this.height += drawer.height;
            }
          }

        }
        return drawers.ToArray();
      }
    }
  }

 }