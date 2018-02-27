/******************************************************************************/
/*!
@file   SerializedPropertyExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
  public static class SerializedPropertyExtensions
  {
    /// <summary>
    /// Gets the owning object of a specific type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property"></param>
    /// <param name="fieldInfo"></param>
    /// <returns></returns>
    public static T GetObject<T>(this SerializedProperty property, FieldInfo fieldInfo) where T : class
    {
      var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
      var type = obj.GetType();
      if (obj == null) { return null; }

      T actualObject = null;
      //if (typeof(IEnumerable).IsAssignableFrom(obj.GetType()))
      if (type.IsArray)// || type.IsGenericType)
      {
        var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
        actualObject = ((T[])obj)[index];
      }
      else
      {
        actualObject = obj as T;
      }
      return actualObject;
    }

    /// <summary>
    /// Returns the value of a given property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property"></param>
    /// <returns></returns>
    public static T GetValue<T>(this SerializedProperty property)
    {
      return Reflection.GetNestedObject<T>(property.serializedObject.targetObject, property.propertyPath);
    }

    /// This is a way to get a field name string in such a manner that the compiler will
    /// generate errors for invalid fields.  Much better than directly using strings.
    /// Usage: instead of
    /// <example>
    /// "m_MyField";
    /// </example>
    /// do this:
    /// <example>
    /// MyClass myclass = null;
    /// SerializedPropertyHelper.PropertyName( () => myClass.m_MyField);
    /// </example>
    public static string PropertyName(Expression<Func<object>> exp)
    {
      var body = exp.Body as MemberExpression;
      if (body == null)
      {
        var ubody = (UnaryExpression)exp.Body;
        body = ubody.Operand as MemberExpression;
      }
      return body.Member.Name;
    }

    /// Usage: instead of
    /// <example>
    /// mySerializedObject.FindProperty("m_MyField");
    /// </example>
    /// do this:
    /// <example>
    /// MyClass myclass = null;
    /// mySerializedObject.FindProperty( () => myClass.m_MyField);
    /// </example>
    public static SerializedProperty FindProperty(this SerializedObject obj, Expression<Func<object>> exp)
    {
      return obj.FindProperty(PropertyName(exp));
    }

    /// Usage: instead of
    /// <example>
    /// mySerializedProperty.FindPropertyRelative("m_MyField");
    /// </example>
    /// do this:
    /// <example>
    /// MyClass myclass = null;
    /// mySerializedProperty.FindPropertyRelative( () => myClass.m_MyField);
    /// </example>
    public static SerializedProperty FindPropertyRelative(this SerializedProperty obj, Expression<Func<object>> exp)
    {
      return obj.FindPropertyRelative(PropertyName(exp));
    }
    


  }

}