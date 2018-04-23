using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using System;
using System.Reflection;
using System.Linq;

namespace Stratus
{
  /// <summary>
  /// Holds a reference to a given variable
  /// </summary>
  public class MemberReference 
  {
    public enum MemberType
    {
      Field,
      Property
    }

    /// <summary>
    /// The name for this mmeber
    /// </summary>
    public string name { get; private set; }
    /// <summary>
    /// The object instance on which this variable resides
    /// </summary>
    public object target { get; private set; }
    /// <summary>
    /// Information about the variable if it's a field type
    /// </summary>
    public FieldInfo field { get; private set; }
    /// <summary>
    /// Information about the variable if it's a property type
    /// </summary>
    public PropertyInfo property { get; private set; }
    /// <summary>
    /// The type of this member
    /// </summary>
    public Type type { get; private set; }
    /// <summary>
    /// The type of member, whether a field or property
    /// </summary>
    public MemberType memberType { get; private set; }
    /// <summary>
    /// Returns the current value of this member
    /// </summary>
    public object value => Get();

    ///// <summary>
    ///// Constructs a reference to the given member
    ///// </summary>
    ///// <param name="varExpr"></param>
    //public MemberReference(Expression<Func<object>> varExpr)
    //{
    //  // Use expressions to find the underlying owner object
    //  var memberExpr = varExpr.Body as MemberExpression;
    //  var inst = memberExpr.Expression;
    //  var targetObj = Expression.Lambda<Func<object>>(inst).Compile()();
    //
    //  // Get the name of the variable
    //  var variableName = memberExpr.Member.Name;
    //  this.name = variableName;
    //  this.target = targetObj;
    //
    //  // Check if it's a property
    //  var property = targetObj.GetType().GetProperty(variableName);
    //  if (property != null)
    //  {
    //    this.property = property;
    //    this.type = property.PropertyType;
    //    this.memberType = MemberType.Property;
    //  }
    //
    //  // Check if it's a field
    //  var field = targetObj.GetType().GetField(variableName);
    //  if (field != null)
    //  {
    //    this.field = field;
    //    this.type = field.FieldType;
    //    this.memberType = MemberType.Field;
    //  }
    //
    //  // Invalid
    //  throw new ArgumentException("The given variable is neither a property or a field!");
    //}

    public static MemberReference Construct<T>(Expression<Func<T>> expression)
    {
      // Use expressions to find the underlying owner object
      var memberExpr = expression.Body as MemberExpression;
      var inst = memberExpr.Expression;
      var targetObj = Expression.Lambda<Func<object>>(inst).Compile()();
      var variableName = memberExpr.Member.Name;

      // Construct the member reference object
      MemberReference memberReference = new MemberReference();
      memberReference.name = variableName;
      memberReference.target = targetObj;

      // Check if it's a property
      var property = targetObj.GetType().GetProperty(variableName);
      if (property != null)
      {
        memberReference.property = property;
        memberReference.type = property.PropertyType;
        memberReference.memberType = MemberType.Property;
        return memberReference;
      }

      // Check if it's a field
      var field = targetObj.GetType().GetField(variableName);
      if (field != null)
      {
        memberReference.field = field;
        memberReference.type = field.FieldType;
        memberReference.memberType = MemberType.Field;
        return memberReference;
      }

      // Invalid
      throw new ArgumentException("The given variable is neither a property or a field!");
    }

    public object Get()
    {
      switch (memberType)
      {
        case MemberType.Field:
          return field.GetValue(target);
        case MemberType.Property:
          return property.GetValue(target, null);
      }
      throw new ArgumentException("The given member is neither a property or a field!");
    }

    public void Set(object value)
    {
      switch (memberType)
      {
        case MemberType.Field:
          field.SetValue(target, value);
            break;
        case MemberType.Property:
          property.SetValue(target, value);
          break;
      }
    }

    private object GetProperty => property.GetValue(target, null).ToString();
    private void SetProperty(object value) => property.SetValue(target, value);



  }
}