using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using OdinSerializer;
using Stratus.Utilities;
using System.Linq;

namespace Stratus
{
  /// <summary>
  /// Edits System.Object types in a completely generic way
  /// </summary>
  public partial class SerializedSystemObject
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public Type type { get; private set; }
    public OdinSerializedProperty[] properties { get; private set; }
    public Dictionary<string, OdinSerializedProperty> propertiesByName { get; private set; }
    public ObjectDrawer drawer { get; private set; }
    public object target { get; private set; }    

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public SerializedSystemObject(Type type, object target)
    {
      this.type = type;
      this.target = target;
      this.drawer = GetDrawer(type);
      this.GenerateProperties();
    }

    public SerializedSystemObject(object target)
    {
      this.drawer = GetDrawer(target.GetType());
      this.target = target;
    }

    static SerializedSystemObject()
    {
      foreach (var drawerType in customObjectDrawers)
      {
        ObjectDrawer drawer = (ObjectDrawer)Reflection.Instantiate(drawerType);
        Type objectType = drawer.type;
        objectDrawers.Add(objectType, drawer);
      }
    }

    /// <summary>
    /// Gets the object drawer for the given type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ObjectDrawer GetDrawer(Type type)
    {
      if (!objectDrawers.ContainsKey(type))
        objectDrawers.Add(type, new DefaultObjectDrawer(type));
      return objectDrawers[type];
    }

    /// <summary>
    /// Gets the object drawer for the given type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ObjectDrawer GetDrawer(FieldInfo field)
    {
      Type type = field.FieldType;
      if (!objectDrawers.ContainsKey(type))
        objectDrawers.Add(type, new DefaultObjectDrawer(field, type));
      ObjectDrawer drawer = objectDrawers[type];
      //drawer.SetDisplayName(field.Name);
      return drawer;
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public bool DrawEditorGUILayout()
    {
      return this.drawer.DrawEditorGUILayout(this.target);
    }

    public bool DrawEditorGUI(Rect position)
    {
      return this.drawer.DrawEditorGUI(position, this.target);
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

    public void Serialize(SerializedProperty stringProperty)
    {
      string data = JsonUtility.ToJson(target);
      stringProperty.stringValue = data;
      Undo.RecordObject(stringProperty.serializedObject.targetObject, stringProperty.displayName);
      stringProperty.serializedObject.ApplyModifiedProperties();
    }

    public void Deserialize(SerializedProperty stringProperty)
    {
      string data = stringProperty.stringValue;
      JsonUtility.FromJsonOverwrite(data, target);
    }

    private void GenerateProperties()
    {
      FieldInfo[] fields = GetSerializedFields(this.type);
      List<OdinSerializedProperty> properties = new List<OdinSerializedProperty>();
      foreach(var field in fields)
      {
        OdinSerializedProperty property = new OdinSerializedProperty(field, this.target);
        propertiesByName.Add(property.name, property);
        properties.Add(property);
      }
      this.properties = properties.ToArray();
    }

    //------------------------------------------------------------------------/
    // Static Methods
    //------------------------------------------------------------------------/
    private static Dictionary<Type, SerializedPropertyType> propertyTypesMap { get; set; } = new Dictionary<Type, SerializedPropertyType>()
    {
      { typeof(bool), SerializedPropertyType.Boolean },
      { typeof(int), SerializedPropertyType.Integer},
      { typeof(float), SerializedPropertyType.Float },
      { typeof(string), SerializedPropertyType.String },
      { typeof(Vector2), SerializedPropertyType.Vector2 },
      { typeof(Vector3), SerializedPropertyType.Vector3},
      { typeof(Vector4), SerializedPropertyType.Vector4},
      { typeof(Color), SerializedPropertyType.Color },
      { typeof(Rect), SerializedPropertyType.Rect},
      { typeof(LayerMask), SerializedPropertyType.LayerMask},
    };

    public static SerializedPropertyType DeducePropertyType(FieldInfo field)
    {
      Type type = field.FieldType;
      SerializedPropertyType propertyType = SerializedPropertyType.Generic;

      if (type.IsEnum)
        propertyType = SerializedPropertyType.Enum;
      else if (propertyTypesMap.ContainsKey(type))
        propertyType = propertyTypesMap[type];
      else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
        propertyType = SerializedPropertyType.ObjectReference;
      return propertyType;
    }

    public static FieldInfo[] GetSerializedFields(Type type) => Reflection.GetSerializedFields(type);

    public static ObjectDrawer GetObjectDrawer(object element)
    {
      Type elementType = element.GetType();
      if (!objectDrawers.ContainsKey(elementType))
        objectDrawers.Add(elementType, new SerializedSystemObject.DefaultObjectDrawer(elementType));
      ObjectDrawer drawer = objectDrawers[elementType];
      return drawer;
    }

    public static ObjectDrawer GetObjectDrawer(Type elementType)
    {
      if (!objectDrawers.ContainsKey(elementType))
        objectDrawers.Add(elementType, new SerializedSystemObject.DefaultObjectDrawer(elementType));
      ObjectDrawer drawer = objectDrawers[elementType];
      return drawer;
    }

    public static FieldDrawer GetFieldDrawer(FieldInfo field)
    {
      if (!fieldDrawers.ContainsKey(field))
        fieldDrawers.Add(field, new SerializedSystemObject.FieldDrawer(field));
      return fieldDrawers[field];
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

    public static bool IsArray(Type type)
    {
      return typeof(IList).IsAssignableFrom(type);
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
