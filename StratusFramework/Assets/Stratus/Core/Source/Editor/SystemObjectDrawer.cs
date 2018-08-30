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
      public DrawCommand [] drawCalls { get; private set; }
      //public bool[] isField { get; private set; }

      public bool isArray { get; private set; }
      public bool isField { get; private set; }

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public DefaultObjectDrawer(Type type) : base(type)
      {
        this.height = lineHeight;
        this.drawCalls = GenerateDrawCommands(fields);
        this.isDrawable = this.drawCalls.NotEmpty();
      }

      //------------------------------------------------------------------------/
      // Methods: Draw
      //------------------------------------------------------------------------/
      public override bool DrawEditorGUILayout(object target)
      {
        bool changed = false;
        string content = this.isDrawable ? this.displayName : $"No serialized fields available";
        UnityEditor.EditorGUILayout.LabelField(content);

        foreach (var call in drawCalls)
        {
          if (call.hasChildren)
          {
            changed |= call.drawer.DrawEditorGUILayout(target);
          }          
          else
          {
            //UnityEditor.EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            object value = GetValueOrSetDefault(call.field, target);
            changed |= call.drawer.DrawEditorGUILayout(value);
            //UnityEditor.EditorGUILayout.EndVertical();
          }

        }
        return changed;
      }

      public override bool DrawEditorGUI(Rect position, object target)
      {
        bool changed = false;
        string content = this.isDrawable ? this.displayName : $"No serialized fields for {type.Name}";
        EditorGUI.LabelField(position, content);

        // Draw all drawers
        foreach (var call in drawCalls)
        {
          if (call.hasChildren)
          {
            changed |= call.drawer.DrawEditorGUI(position, target);
          }
          else
          {
            //UnityEditor.EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            //position.y += lineHeight;
            object value = GetValueOrSetDefault(call.field, target);
            changed |= call.drawer.DrawEditorGUI(position, value);
            EditorGUI.indentLevel--;
            //UnityEditor.EditorGUILayout.EndVertical();
          }
          position.y += lineHeight;
        }
        return changed;
      }

      private object GetValueOrSetDefault(FieldInfo field, object target)
      {
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



      protected DrawCommand[] GenerateDrawCommands(FieldInfo[] fields)
      {
        List<DrawCommand> drawers = new List<DrawCommand>();
        for (int i = 0; i < fields.Length; ++i)
        {
          FieldInfo field = fields[i];
          Type fieldType = field.FieldType;
          SerializedPropertyType serializedPropertyType = DeducePropertyType(field);

          // Unity is supported by Unity if it's not a generic array
          bool isArray = IsArray(fieldType);
          bool isUnitySupportedType = (serializedPropertyType != SerializedPropertyType.Generic || isArray); //  OdinSerializer.FormatterUtilities.IsPrimitiveType(fieldType);
          Drawer drawer = null;

          
          if (isUnitySupportedType)
          {
            drawer = new FieldDrawer(field);
          }
          else
          {
            drawer = GetDrawer(fieldType); 
          }

          DrawCommand drawCommand = new DrawCommand(drawer, field, isUnitySupportedType);
          if (drawer.isDrawable)
            this.height += drawer.height;
          drawers.Add(drawCommand);
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
        EditorGUI.BeginChangeCheck();
        this.OnDrawEditorGUI(position, (T)target);
        bool changed = EditorGUI.EndChangeCheck();
        return changed;
      }

      public override bool DrawEditorGUILayout(object target)
      {
        EditorGUI.BeginChangeCheck();
        this.OnDrawEditorGUILayout((T)target);
        bool changed = EditorGUI.EndChangeCheck();
        return changed;
      }

    }
  }

}