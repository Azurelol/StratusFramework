using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using UnityEditor;
using System.Reflection;
using System;

namespace Stratus
{
  /// <summary>
  /// A request to draw a serialized property
  /// </summary>
  public class SerializedPropertyModel
  {
    public enum SerializationType
    {
      Unity,
      Odin
    }

    public SerializationType type { get; private set; }
    public SerializedProperty unitySerialized { get; private set; }
    public OdinSerializedProperty odinSerialized { get; private set; }
    public bool isExpanded
    {
      get
      {
        return (type == SerializationType.Unity) ? unitySerialized.isExpanded : odinSerialized.isExpanded;
      }

      set
      {
        switch (this.type)
        {
          case SerializationType.Unity:
            unitySerialized.isExpanded = value;
            break;
          case SerializationType.Odin:
            odinSerialized.isExpanded = value;
            break;
        }
      }
    }

    public string displayName => (type == SerializationType.Unity) ? unitySerialized.displayName : odinSerialized.displayName;

    public SerializedPropertyModel(SerializedProperty serializedProperty)
    {
      this.unitySerialized = serializedProperty;
      this.type = SerializationType.Unity;
    }

    public SerializedPropertyModel(OdinSerializedProperty serializedProperty)
    {
      this.odinSerialized = serializedProperty;
      this.type = SerializationType.Odin;
    }    
  }

  /// <summary>
  /// Manages a property serialized by Odin
  /// </summary>
  public class OdinSerializedProperty
  {
    public FieldInfo field { get; private set; }
    public System.Type type { get; private set; }
    public object target { get; private set; }
    public bool isExpanded { get; set; }
    public string displayName { get; private set; }
    public IList list { get; private set; }
    public bool isArray { get; private set; }

    public OdinSerializedProperty(FieldInfo field, object target)
    {
      this.field = field;
      this.type = this.field.FieldType;
      this.displayName = ObjectNames.NicifyVariableName(this.field.Name);
      this.target = target;
      this.isArray = typeof(IList).IsAssignableFrom(this.type);
      if (this.isArray)
        this.list = this.field.GetValue(target) as IList;
    }

    public void DrawEditorGUILayout(object target)
    {
      StratusEditorUtility.DrawField(this.field, target);
    }

    public object GetArrayElementAtIndex(int index)
    {
      return this.list[index];
    }

  }
}