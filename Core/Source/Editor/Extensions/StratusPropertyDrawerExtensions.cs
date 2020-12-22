using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
  public static partial class Extensions
  {
    public static float GetPropertyHeightSafe(this PropertyDrawer drawer, SerializedProperty property, GUIContent label)
    {
      return (float)StratusReflection.GetReflectedMethod("GetPropertyHeightSafe", typeof(UnityEditor.PropertyDrawer), false, drawer).Invoke(drawer, new object[] { property, label });
    }

    public static void OnGUISafe(this PropertyDrawer drawer, Rect position, SerializedProperty property, GUIContent label)
    {
      StratusReflection.GetReflectedMethod("OnGUISafe", typeof(UnityEditor.PropertyDrawer), false, drawer).Invoke(drawer, new object[] { position, property, label });
    }

    public static void SetFieldInfo(this PropertyDrawer drawer, FieldInfo info)
    {
      StratusReflection.SetField("m_FieldInfo", typeof(PropertyDrawer), info, false, drawer);
    }

    public static void SetAttribute(this PropertyDrawer drawer, PropertyAttribute attrib)
    {
      StratusReflection.SetField("m_Attribute", typeof(PropertyDrawer), attrib, false, drawer);
    }

    public static void SetAttribute(this DecoratorDrawer drawer, PropertyAttribute attrib)
    {
      StratusReflection.SetField("m_Attribute", typeof(DecoratorDrawer), attrib, false, drawer);
    }

    public static Type GetHiddenType(this CustomPropertyDrawer prop)
    {
      return StratusReflection.GetField<Type>("m_Type", typeof(CustomPropertyDrawer), false, prop);
    }

    public static bool GetUseForChildren(this CustomPropertyDrawer prop)
    {
      return StratusReflection.GetField<bool>("m_UseForChildren", typeof(CustomPropertyDrawer), false, prop);
    }

  }
}