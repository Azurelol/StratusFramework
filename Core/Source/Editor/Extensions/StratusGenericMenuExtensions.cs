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

    public static void AddItem(this GenericMenu menu, string content, bool on, GenericMenu.MenuFunction menuFunction)
    {
      menu.AddItem(new GUIContent(content), on, menuFunction);
    }

    public static void AddItem(this GenericMenu menu, string content, bool on, GenericMenu.MenuFunction2 menuFunction, object useData)
    {
      menu.AddItem(new GUIContent(content), on, menuFunction, useData); 
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

    public static void AddItems(this GenericMenu menu, string[] displayedOptions, System.Action<int> onSelected)
    {
      for (int i = 0; i < displayedOptions.Length; ++i)
      {
        int index = i;
        menu.AddItem(new GUIContent($"{displayedOptions[i]}"), false, () =>
        {
          onSelected(index);
        });
      }
    }

    public static MessageType Convert(this StratusObjectValidation.Level type)
    {
      switch (type)
      {
        default:
        case StratusObjectValidation.Level.Info: return MessageType.Info;
        case StratusObjectValidation.Level.Warning: return MessageType.Warning;
        case StratusObjectValidation.Level.Error: return MessageType.Error;
      }
    }

    public static StratusObjectValidation.Level Convert(this MessageType type)
    {
      switch (type)
      {
        default:
        case MessageType.Info: return StratusObjectValidation.Level.Info;
        case MessageType.Warning: return StratusObjectValidation.Level.Warning;
        case MessageType.Error: return StratusObjectValidation.Level.Error;
      }
    }


  }

}