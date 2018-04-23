using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  public static class GenericMenuExtensions
  {
    /// <summary>
    /// Adds a menu item that will allow the modification of a boolean property
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="property"></param>
    public static void AddBooleanToggle(this GenericMenu menu, SerializedProperty property)
    {
      menu.AddItem(new GUIContent(property.displayName), property.boolValue, () =>
      {
        property.boolValue = !property.boolValue;
        property.serializedObject.ApplyModifiedProperties();
      });
    }

    /// <summary>
    /// Adds a menu item that will allow the modification of an enum property
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="property"></param>
    public static void AddEnumToggle<T>(this GenericMenu menu, SerializedProperty property) where T : struct
    {
      var enumValues = Enum.GetValues(typeof(T));
      for(int i = 0; i < enumValues.Length; ++i)
      {
        int index = i;
        object value = enumValues.GetValue(i);
        menu.AddItem(new GUIContent($"{property.displayName}/{value.ToString()}"), property.enumValueIndex == index ? true : false, () =>
        {
          property.enumValueIndex = index;
          property.serializedObject.ApplyModifiedProperties();
        });
      }
    }

    /// <summary>
    /// Adds a menu item that will allow the modification of an enum property
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="property"></param>
    public static void AddPopup(this GenericMenu menu, string label, string[] displayedOptions, System.Action<int> onSelected)
    {
      for (int i = 0; i < displayedOptions.Length; ++i)
      {
        int index = i;
        menu.AddItem(new GUIContent($"{label}/{displayedOptions[i]}"), false, () =>
        {
          onSelected(index);
        });
      }

    }

    public static MessageType Convert(this Validation.Level type)
    {
      switch (type)
      {
        default:
        case Validation.Level.Info: return MessageType.Info;
        case Validation.Level.Warning: return MessageType.Warning;
        case Validation.Level.Error: return MessageType.Error;
      }
    }

    public static Validation.Level Convert(this MessageType type)
    {
      switch (type)
      {
        default:
        case MessageType.Info: return Validation.Level.Info;
        case MessageType.Warning: return Validation.Level.Warning;
        case MessageType.Error: return Validation.Level.Error;
      }
    }


  }

}