/******************************************************************************/
/*!
@file   ComponentExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System;
using System.Reflection;

namespace Stratus
{
  public static partial class Extensions
  {

    /// <summary>
    /// Copies a component, constructing a new one from the same type and
    /// copying the values from the original wholesale.
    /// </summary>
    /// <typeparam name="T">The component class</typeparam>
    /// <param name="component">The component to copy into.</param>
    /// <param name="otherComponent">The component being copied</param>
    /// <returns>A reference to the new component</returns>
    public static T Copy<T>(this Component component, T otherComponent) where T : Component
    {
      Type componentType = component.GetType();

      // Check that they are matching types
      if (componentType != otherComponent.GetType())
        return null;

      BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        | BindingFlags.Default | BindingFlags.DeclaredOnly;

      // Copy properties over
      PropertyInfo[] propertiesInfo = componentType.GetProperties(flags);
      foreach (var property in propertiesInfo)
      {
        if (property.CanWrite)
        {
          try
          {
            // Copy the value over
            property.SetValue(component, property.GetValue(otherComponent, null), null);
          }
          catch
          {
            // In case of NotImplementedException being thrown.
          }
        }
      }

      // Copy fields over
      FieldInfo[] fieldsInfo = componentType.GetFields(flags);
      foreach (var field in fieldsInfo)
      {
        field.SetValue(component, field.GetValue(otherComponent));
      }

      return component as T;
    }

    /// <summary>
    /// Gets or if not present, adds the specified component to the Transform.
    /// </summary>
    public static T GetOrAddComponent<T>(this Component child) where T : Component
    {
      T result = child.GetComponent<T>();
      if (result == null)
      {
        result = child.gameObject.AddComponent<T>();
      }
      return result;
    }

  }

}