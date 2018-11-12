using Stratus.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stratus
{
  public static partial class Extensions
  {

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

    public static T GetValue<T>(this FieldInfo fieldInfo, object target) => (T)fieldInfo.GetValue(target);
    public static void SetValue<T>(this FieldInfo fieldInfo, object target, object value) => fieldInfo.SetValue(target, value);

  }

}