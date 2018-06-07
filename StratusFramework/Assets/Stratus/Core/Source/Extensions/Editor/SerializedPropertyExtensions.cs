/******************************************************************************/
/*!
@file   SerializedPropertyExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using Stratus.Utilities;
using UnityEngine;

namespace Stratus
{
  public static class SerializedPropertyExtensions
  {
    /// <summary>
    /// Gets the owning object of a specific type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property"></param>
    /// <param name="fieldInfo"></param>
    /// <returns></returns>
    public static T GetObject<T>(this SerializedProperty property, FieldInfo fieldInfo) where T : class
    {
      var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
      var type = obj.GetType();
      if (obj == null) { return null; }

      T actualObject = null;
      //if (typeof(IEnumerable).IsAssignableFrom(obj.GetType()))
      if (type.IsArray)// || type.IsGenericType)
      {
        var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
        actualObject = ((T[])obj)[index];
      }
      else
      {
        actualObject = obj as T;
      }
      return actualObject;
    }

    /// <summary>
    /// Get the object the serialized property holds by using reflection
    /// </summary>
    /// <typeparam name="T">The object type that the property contains</typeparam>
    /// <param name="property"></param>
    /// <returns>Returns the object type T if it is the type the property actually contains</returns>
    public static T GetValue<T>(this SerializedProperty property)
    {
      return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property));
    }

    /// <summary>
    /// Set the value of a field of the property with the type T
    /// </summary>
    /// <typeparam name="T">The type of the field that is set</typeparam>
    /// <param name="property">The serialized property that should be set</param>
    /// <param name="value">The new value for the specified property</param>
    /// <returns>Returns if the operation was successful or failed</returns>
    public static bool SetValue<T>(this SerializedProperty property, T value)
    {

      object obj = GetSerializedPropertyRootComponent(property);
      //Iterate to parent object of the value, necessary if it is a nested object
      string[] fieldStructure = property.propertyPath.Split('.');
      for (int i = 0; i < fieldStructure.Length - 1; i++)
      {
        obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
      }
      string fieldName = fieldStructure.Last();

      return SetFieldOrPropertyValue(fieldName, obj, value);

    }

    /// <summary>
    /// Get the component of a serialized property
    /// </summary>
    /// <param name="property">The property that is part of the component</param>
    /// <returns>The root component of the property</returns>
    public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
    {
      return (Component)property.serializedObject.targetObject;
    }

    /// <summary>
    /// Iterates through objects to handle objects that are nested in the root object
    /// </summary>
    /// <typeparam name="T">The type of the nested object</typeparam>
    /// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
    /// <param name="obj">The root object from which this path leads to the property</param>
    /// <param name="includeAllBases">Include base classes and interfaces as well</param>
    /// <returns>Returns the nested object casted to the type T</returns>
    public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
    {
      foreach (string part in path.Split('.'))
      {
        obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
      }
      return (T)obj;
    }

    public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
    {
      FieldInfo field = obj.GetType().GetField(fieldName, bindings);
      if (field != null) return (T)field.GetValue(obj);

      PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
      if (property != null) return (T)property.GetValue(obj, null);

      if (includeAllBases)
      {

        foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
        {
          field = type.GetField(fieldName, bindings);
          if (field != null) return (T)field.GetValue(obj);

          property = type.GetProperty(fieldName, bindings);
          if (property != null) return (T)property.GetValue(obj, null);
        }
      }

      return default(T);
    }

    public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
    {
      FieldInfo field = obj.GetType().GetField(fieldName, bindings);
      if (field != null)
      {
        field.SetValue(obj, value);
        return true;
      }

      PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
      if (property != null)
      {
        property.SetValue(obj, value, null);
        return true;
      }

      if (includeAllBases)
      {
        foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
        {
          field = type.GetField(fieldName, bindings);
          if (field != null)
          {
            field.SetValue(obj, value);
            return true;
          }

          property = type.GetProperty(fieldName, bindings);
          if (property != null)
          {
            property.SetValue(obj, value, null);
            return true;
          }
        }
      }
      return false;
    }

    public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
    {
      List<Type> allTypes = new List<Type>();

      if (includeSelf) allTypes.Add(type);

      if (type.BaseType == typeof(object))
      {
        allTypes.AddRange(type.GetInterfaces());
      }
      else
      {
        allTypes.AddRange(
                Enumerable
                .Repeat(type.BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                .Distinct());
      }

      return allTypes;
    }
  

  /// This is a way to get a field name string in such a manner that the compiler will
  /// generate errors for invalid fields.  Much better than directly using strings.
  /// Usage: instead of
  /// <example>
  /// "m_MyField";
  /// </example>
  /// do this:
  /// <example>
  /// MyClass myclass = null;
  /// SerializedPropertyHelper.PropertyName( () => myClass.m_MyField);
  /// </example>
  public static string PropertyName(Expression<Func<object>> exp)
    {
      var body = exp.Body as MemberExpression;
      if (body == null)
      {
        var ubody = (UnaryExpression)exp.Body;
        body = ubody.Operand as MemberExpression;
      }
      return body.Member.Name;
    }

    /// Usage: instead of
    /// <example>
    /// mySerializedObject.FindProperty("m_MyField");
    /// </example>
    /// do this:
    /// <example>
    /// MyClass myclass = null;
    /// mySerializedObject.FindProperty( () => myClass.m_MyField);
    /// </example>
    public static SerializedProperty FindProperty(this SerializedObject obj, Expression<Func<object>> exp)
    {
      return obj.FindProperty(PropertyName(exp));
    }

    /// Usage: instead of
    /// <example>
    /// mySerializedProperty.FindPropertyRelative("m_MyField");
    /// </example>
    /// do this:
    /// <example>
    /// MyClass myclass = null;
    /// mySerializedProperty.FindPropertyRelative( () => myClass.m_MyField);
    /// </example>
    public static SerializedProperty FindPropertyRelative(this SerializedProperty obj, Expression<Func<object>> exp)
    {
      return obj.FindPropertyRelative(PropertyName(exp));
    }

    /// <summary>
    /// Retrieves the FieldInfo for the field behind this serialized property
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static FieldInfo GetFieldInfo(this SerializedProperty property)
    {
      Type objectType = property.serializedObject.targetObject.GetType();
      return objectType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
    }

    /// <summary>
    /// Retrieves the Type of the field behind this serialized property
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static Type GetFieldType(this SerializedProperty property)
    {
      Type objectType = property.serializedObject.targetObject.GetType();
      Type type = objectType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;
      return type;
    }

    /// <summary>
    /// Retrieves all the Attributes for the field behind this serialized property
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static Attribute[] GetFieldAttributes(this SerializedProperty property)
    {
      FieldInfo fi = property.GetFieldInfo();
      Attribute[] attributes = fi.GetCustomAttributes<Attribute>().ToArray();
      return attributes;
    }



  }

}