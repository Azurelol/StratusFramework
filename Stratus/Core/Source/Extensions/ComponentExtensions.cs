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
    /// Copies the values from another component of the same type
    /// </summary>
    /// <typeparam name="T">The component class</typeparam>
    /// <param name="component">The component to copy into.</param>
    /// <param name="otherComponent">The component being copied</param>
    /// <returns>A reference to the new component</returns>
    public static T Copy<T>(this Component component, T otherComponent) where T : Component
    {
      Type componentType = component.GetType().DeclaringType;

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

    // Gets the positions of all vertices of this collider in wolrd space
    public static Vector3[] GetVertices(this BoxCollider b)
    {
      Vector3[] vertices = new Vector3[8];
      vertices[0] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f);
      vertices[1] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);
      vertices[2] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);
      vertices[3] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f);
      vertices[4] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f);
      vertices[5] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f);
      vertices[6] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);
      vertices[7] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);

      return vertices;
    }

  }

}