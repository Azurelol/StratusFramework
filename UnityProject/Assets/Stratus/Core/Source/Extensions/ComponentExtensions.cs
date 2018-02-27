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
    /// Copies the values from another component of the same type onto this one
    /// </summary>
    /// <typeparam name="T">The component class</typeparam>
    /// <param name="self">The component to copy into.</param>
    /// <param name="source">The component being copied</param>
    /// <returns>A reference to the new component</returns>
    public static T CopyFrom<T>(this T self, T source) where T : Component
    {
      return CopyFrom((Component)self, (Component)source) as T;
    }


    /// <summary>
    /// Copies the values from another component of the same type onto this one
    /// </summary>
    /// <typeparam name="T">The component class</typeparam>
    /// <param name="self">The component to copy into.</param>
    /// <param name="source">The component being copied</param>
    /// <returns>A reference to the new component</returns>
    public static Component CopyFrom(this Component self, Component source)
    {
      //Type componentType = self.GetType();
      JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), self);
      return self;

      // Check that they are matching types
      //if (componentType != source.GetType())
      //  return null;
      //
      //BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
      //  | BindingFlags.Default;
      //
      //// Copy properties over
      //PropertyInfo[] propertiesInfo = componentType.GetProperties(flags);
      //foreach (var property in propertiesInfo)
      //{
      //  if (property.CanWrite)
      //  {
      //    try
      //    {
      //      // Check whether the property type is value type, enum, or string type
      //      if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
      //      {
      //        property.SetValue(self, property.GetValue(source, null), null);
      //      }
      //      // Else the property type is object/complex types so need to recursively call this method
      //      // until the end of the tree is reached
      //      else
      //      {
      //        object objPropertyValue = property.GetValue(source, null);
      //        if (objPropertyValue == null)
      //          property.SetValue(self, null, null);
      //        else
      //        {
      //          property.SetValue(self, objPropertyValue.Clone(), null);
      //        }
      //      }
      //    }
      //    catch
      //    {
      //      // In case of NotImplementedException being thrown.
      //    }
      //  }
      //}
      //
      //// Copy fields over
      //FieldInfo[] fieldsInfo = componentType.GetFields(flags);
      //foreach (var field in fieldsInfo)
      //{
      //  Type fieldType = field.FieldType;
      //  bool canAssign = fieldType.IsAbstract || fieldType.IsValueType || fieldType.IsEnum || fieldType.Equals(typeof(System.String));
      //  //string canAssignVal = canAssign ? "assignment" : "deep copy" ;
      //  //Trace.Script($"Doing {canAssignVal} of type {field.FieldType.Name}");
      //  if (canAssign)
      //    field.SetValue(self, field.GetValue(source));
      //  else
      //  {
      //    object value = field.GetValue(source);
      //    field.SetValue(self, value);
      //  }
      //}      
      //
      //return self;
    }

    /// <summary>
    /// Copies the values from another scriptable object of the same type onto this one
    /// </summary>
    /// <typeparam name="T">The component class</typeparam>
    /// <param name="self">The component to copy into.</param>
    /// <param name="source">The component being copied</param>
    /// <returns>A reference to the new component</returns>
    public static ScriptableObject CopyFrom(this ScriptableObject self, ScriptableObject source)
    {
      //Type componentType = self.GetType();
      JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), self);
      return self;
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