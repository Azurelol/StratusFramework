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
    /// <summary>
    /// Base class for all drawers
    /// </summary>
    public abstract class Drawer
    {
      public string name { get; protected set; }
      public string displayName { get; protected set; }
      public Type type { get; protected set; }
      public abstract bool DrawEditorGUILayout(object target);
      public abstract bool DrawEditorGUI(Rect position, object target);
      public bool isDrawable { get; protected set; }
      public bool isPrimitive { get; protected set; }
      public float height { get; protected set; }
      public static float lineHeight => StratusEditorUtility.lineHeight;
      public static float labelWidth => StratusEditorUtility.labelWidth;
    }

    /// <summary>
    /// Base class for all drawers
    /// </summary>
    public abstract class ObjectDrawer : Drawer
    {
      public FieldInfo[] fields { get; private set; }
      public Dictionary<string, FieldInfo> fieldsByName { get; private set; } = new Dictionary<string, FieldInfo>();
      public bool hasFields => fields.NotEmpty();
      public bool hasDefaultConstructor { get; private set; }

      public ObjectDrawer(Type type)
      {
        this.type = type;
        MemberInfo[] members = OdinSerializer.FormatterUtilities.GetSerializableMembers(type, OdinSerializer.SerializationPolicies.Unity);
        this.fields = members.OfType<FieldInfo>().ToArray();
        this.fieldsByName.AddRange(this.fields, (FieldInfo field) => field.Name);
        this.hasDefaultConstructor = (type.GetConstructor(Type.EmptyTypes) != null) || type.IsValueType;
      }
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public ObjectDrawer drawer { get; private set; }
    public object target { get; private set; }

    //------------------------------------------------------------------------/
    // Properties: Static
    //------------------------------------------------------------------------/
    private static Dictionary<Type, ObjectDrawer> objectDrawers { get; set; } = new Dictionary<Type, ObjectDrawer>();
    private static Dictionary<FieldInfo, FieldDrawer> fieldDrawers { get; set; } = new Dictionary<FieldInfo, FieldDrawer>();
    private static Type[] customObjectDrawers { get; set; } = Reflection.GetSubclass<CustomObjectDrawer>(false);

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public SerializedSystemObject(Type type, object target)
    {
      this.drawer = GetDrawer(type);
      this.target = target;
    }

    public SerializedSystemObject(object target)
    {
      this.drawer = GetDrawer(target.GetType());
      this.target = target;
    }

    static SerializedSystemObject()
    {
      foreach (var type in customObjectDrawers)
        objectDrawers.Add(type, (DefaultObjectDrawer)Reflection.Instantiate(type));
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

    //------------------------------------------------------------------------/
    // Static Methods
    //------------------------------------------------------------------------/
    public static SerializedPropertyType DeducePropertyType(FieldInfo field)
    {
      Type type = field.FieldType;
      SerializedPropertyType propertyType = SerializedPropertyType.Generic;

      if (type.IsSubclassOf(typeof(UnityEngine.Object)))
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