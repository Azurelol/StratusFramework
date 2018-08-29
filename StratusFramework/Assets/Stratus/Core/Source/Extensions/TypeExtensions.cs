using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Utilities;

namespace Stratus
{
  public static partial class Extensions
  {
    /// <summary>
    /// Retrieves a specific attribute from the given type, if it is present
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static T GetAttribute<T>(this Type type) where T : Attribute
    {
      return AttributeUtility.FindAttribute<T>(type);
    }

    public static Dictionary<Type, Attribute> MapAttributes(this Type type)
    {
      return AttributeUtility.MapAttributes(type);
    }

    public static bool IsArrayOrList(this Type listType)
    {
      return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
    }

    public static Type GetArrayOrListElementType(this Type listType)
    {
      if (listType.IsArray)
      {
        return listType.GetElementType();
      }
      if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
      {
        return listType.GetGenericArguments()[0];
      }

      return null;
    }

  }

}