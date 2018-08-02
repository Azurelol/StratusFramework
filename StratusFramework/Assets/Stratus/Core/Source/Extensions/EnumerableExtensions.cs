using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Stratus
{
  public static partial class Extensions
  {
    public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
    {
      if (ascending)
      {
        return source.OrderBy(selector);
      }
      else
      {
        return source.OrderByDescending(selector);
      }
    }

    public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
    {
      if (ascending)
      {
        return source.ThenBy(selector);
      }
      else
      {
        return source.ThenByDescending(selector);
      }
    }
  }


  public abstract class ConstrainedEnumParser<TClass> where TClass : class
    // value type constraint S ("TEnum") depends on reference type T ("TClass") [and on struct]
  {
    // internal constructor, to prevent this class from being inherited outside this code
    internal ConstrainedEnumParser()
    {
    }
    // Parse using pragmatic/adhoc hard cast:
    //  - struct + class = enum
    //  - 'guaranteed' call from derived <System.Enum>-constrained type EnumUtils
    public static TEnum Parse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct, TClass
    {
      return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
    }
    public static bool TryParse<TEnum>(string value, out TEnum result, bool ignoreCase = false, TEnum defaultValue = default(TEnum)) where TEnum : struct, TClass // value type constraint S depending on T
    {
      var didParse = Enum.TryParse(value, ignoreCase, out result);
      if (didParse == false)
      {
        result = defaultValue;
      }
      return didParse;
    }
    public static TEnum ParseOrDefault<TEnum>(string value, bool ignoreCase = false, TEnum defaultValue = default(TEnum)) where TEnum : struct, TClass // value type constraint S depending on T
    {
      if (string.IsNullOrEmpty(value)) { return defaultValue; }
      TEnum result;
      if (Enum.TryParse(value, ignoreCase, out result)) { return result; }
      return defaultValue;
    }
  }

  

}