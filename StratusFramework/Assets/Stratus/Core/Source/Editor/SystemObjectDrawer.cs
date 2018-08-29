using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using OdinSerializer;
using System.Linq;

namespace Stratus
{
  public partial class SerializedSystemObject
  {
    /// <summary>
    /// Draws all the fields in a System.Object
    /// </summary>
    public class DefaultObjectDrawer : ObjectDrawer
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public DefaultObjectDrawer parent { get; private set; }
      public Drawer[] drawers { get; private set; }
      //public bool[] isField { get; private set; }
      public int fieldCount => drawers.Length;
      public bool isArray { get; private set; }
      public bool isField { get; private set; }

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public DefaultObjectDrawer(Type type) : base(type)
      {
        this.height = lineHeight;
        this.drawers = GenerateDrawers(fields);
        this.isDrawable = this.drawers.NotEmpty();
      }

      public void SetParent(DefaultObjectDrawer parent, string fieldName)
      {
        this.parent = parent;
        this.isField = true;
        this.height -= lineHeight;
        this.name = fieldName;
        this.displayName = ObjectNames.NicifyVariableName(this.name);
      }


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

      protected Drawer[] GenerateDrawers(FieldInfo[] fields)
      {
        List<Drawer> drawers = new List<Drawer>();
        for (int i = 0; i < fields.Length; ++i)
        {
          FieldInfo field = fields[i];
          Type fieldType = field.FieldType;
          SerializedPropertyType serializedPropertyType = DeducePropertyType(field);

          // Unity is supported by Unity if it's not a generic array
          bool isArray = IsArray(fieldType);
          bool isUnitySupportedType = (serializedPropertyType != SerializedPropertyType.Generic || isArray); //  OdinSerializer.FormatterUtilities.IsPrimitiveType(fieldType);
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
            ObjectDrawer drawer = GetDrawer(fieldType); // new ObjectDrawer(fieldType);
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

    /// <summary>
    /// A custom object drawer for a given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CustomObjectDrawer : ObjectDrawer
    {
      public CustomObjectDrawer(Type type) : base(type)
      {
      }
    }

    /// <summary>
    /// A custom object drawer for a given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CustomObjectDrawer<T> : CustomObjectDrawer
    {
      //------------------------------------------------------------------------/
      // Virtual
      //------------------------------------------------------------------------/
      protected abstract void OnDrawEditorGUI(Rect position, T value);
      protected abstract void OnDrawEditorGUILayout(T value);
      protected abstract float GetHeight(T value);

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public CustomObjectDrawer() : base(typeof(T))
      {
      }

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      public override bool DrawEditorGUI(Rect position, object target)
      {
        throw new System.NotImplementedException();
        EditorGUI.BeginChangeCheck();
        //this.OnDrawEditorGUI(position, target);
        bool changed = EditorGUI.EndChangeCheck();
        return changed;
      }

      public override bool DrawEditorGUILayout(object target)
      {
        throw new System.NotImplementedException();
        EditorGUI.BeginChangeCheck();
        //this.OnDrawEditorGUILayout(target);
        bool changed = EditorGUI.EndChangeCheck();
        return changed;
      }

    }
  }

}