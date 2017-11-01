/******************************************************************************/
/*!
@file   SerializedPropertyExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using Stratus;
using UnityEditor;

namespace Stratus
{
  namespace Utilities
  {
    public static class SerializedPropertyExtensions
    {
      public static T GetValue<T>(this SerializedProperty property)
      {
        return Reflection.GetNestedObject<T>(property.serializedObject.targetObject, property.propertyPath);
      }
    }
  }
}

#endif