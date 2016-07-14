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

/**************************************************************************/
/*!
@class ComponentExtensions 
*/
/**************************************************************************/
public static class ComponentExtensions
{
  /**************************************************************************/
  /*!
  @brief Copies a component, constructing a new one from the same type and
         copying the values from the original wholesale.
  @param component The new component.
  @param otherComponent The original component.
  @return A reference to the new component.
  */
  /**************************************************************************/
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
    foreach(var property in propertiesInfo)
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
    foreach(var field in fieldsInfo)
    {
      field.SetValue(component, field.GetValue(otherComponent));
    }

    return component as T;
  }
	
}
