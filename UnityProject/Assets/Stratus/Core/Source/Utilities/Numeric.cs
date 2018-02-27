/******************************************************************************/
/*!
@file   IsNumeric.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@note   Credit to Or Aviram: 
        https://forum.unity3d.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
*/
/******************************************************************************/
using UnityEngine;
using System;

namespace Stratus
{
  namespace Utilities
  {
    public static partial class Types
    {
      /// <summary>
      /// Whether this object is a numeric type
      /// </summary>
      /// <param name="obj"></param>
      /// <returns>True if its a numeric type, false otherwise.</returns>
      public static bool IsNumeric(this object obj)
      {
        switch (Type.GetTypeCode(obj.GetType()))
        {
          case TypeCode.Byte:
          case TypeCode.SByte:
          case TypeCode.UInt16:
          case TypeCode.UInt32:
          case TypeCode.UInt64:
          case TypeCode.Int16:
          case TypeCode.Int32:
          case TypeCode.Int64:
          case TypeCode.Decimal:
          case TypeCode.Double:
          case TypeCode.Single:
            return true;
          default:
            return false;
        }
      }

      /// <summary>
      /// Whether this type is numeric
      /// </summary>
      /// <param name="type"></param>
      /// <returns>True if the type is numeric, false otherwise.</returns>
      public static bool IsNumeric(this Type type)
      {
        switch (Type.GetTypeCode(type))
        {
          case TypeCode.Byte:
          case TypeCode.SByte:
          case TypeCode.UInt16:
          case TypeCode.UInt32:
          case TypeCode.UInt64:
          case TypeCode.Int16:
          case TypeCode.Int32:
          case TypeCode.Int64:
          case TypeCode.Decimal:
          case TypeCode.Double:
          case TypeCode.Single:
            return true;
          default:
            return false;
        }
      }
    }

    /// <summary>
    /// An exception that is thrown whenever a numeric type is expected as an input somewhere but the input wasn't numeric.
    /// </summary>
    [Serializable]
    public class NumericTypeExpectedException : Exception
    {
      public NumericTypeExpectedException() { }

      public NumericTypeExpectedException(string message) : base(message) { }

      public NumericTypeExpectedException(string message, Exception inner) : base(message, inner) { }

      protected NumericTypeExpectedException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class INumeric : IEquatable<INumeric>
    {
      object Value;
      Type Type;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="obj"></param>
      public INumeric(object obj)
      {
        if (!obj.IsNumeric())
          throw new NumericTypeExpectedException("The type of the object passed in the constructor must be numeric!");
        Value = obj;
        Type = obj.GetType();
      }

      public object GetValue() { return Value; }
      public void SetValue(object newValue) { Value = newValue; }
      public bool Equals(INumeric other) { return this == other; }
      public override bool Equals(object obj)
      {
        if (obj == null) return false;
        if (!(obj is INumeric)) return GetValue() == obj;
        return Equals(obj);
      }
      public override int GetHashCode() { return GetValue().GetHashCode(); }
      public override string ToString() { return GetValue().ToString(); }

      /// <summary>
      /// Checks if the value of the left is lesser than that of the right
      /// </summary>
      /// <param name="lhs"></param>
      /// <param name="rhs"></param>
      /// <returns></returns>
      public static bool operator <(INumeric lhs, INumeric rhs)
      {
        object leftValue = lhs.GetValue();
        object rightValue = rhs.GetValue();

        switch (Type.GetTypeCode(lhs.Type))
        {
          case TypeCode.Byte:
            return (byte)leftValue < (byte)rightValue;
          case TypeCode.SByte:
            return (sbyte)leftValue < (sbyte)rightValue;
          case TypeCode.UInt16:
            return (ushort)leftValue < (ushort)rightValue;
          case TypeCode.UInt32:
            return (uint)leftValue < (uint)rightValue;
          case TypeCode.UInt64:
            return (ulong)leftValue < (ulong)rightValue;
          case TypeCode.Int16:
            return (short)leftValue < (short)rightValue;
          case TypeCode.Int32:
            return (int)leftValue < (int)rightValue;
          case TypeCode.Int64:
            return (long)leftValue < (long)rightValue;
          case TypeCode.Decimal:
            return (decimal)leftValue < (decimal)rightValue;
          case TypeCode.Double:
            return (double)leftValue < (double)rightValue;
          case TypeCode.Single:
            return (float)leftValue < (float)rightValue;
        }
        throw new NumericTypeExpectedException("Please compare valid numeric types.");
      }

      /// <summary>
      /// Checks if the value of left is greater than the value of right.
      /// </summary>
      public static bool operator >(INumeric left, INumeric right)
      {
        object leftValue = left.GetValue();
        object rightValue = right.GetValue();

        switch (Type.GetTypeCode(left.Type))
        {
          case TypeCode.Byte:
            return (byte)leftValue > (byte)rightValue;

          case TypeCode.SByte:
            return (sbyte)leftValue > (sbyte)rightValue;

          case TypeCode.UInt16:
            return (ushort)leftValue > (ushort)rightValue;

          case TypeCode.UInt32:
            return (uint)leftValue > (uint)rightValue;

          case TypeCode.UInt64:
            return (ulong)leftValue > (ulong)rightValue;

          case TypeCode.Int16:
            return (short)leftValue > (short)rightValue;

          case TypeCode.Int32:
            return (int)leftValue > (int)rightValue;

          case TypeCode.Int64:
            return (long)leftValue > (long)rightValue;

          case TypeCode.Decimal:
            return (decimal)leftValue > (decimal)rightValue;

          case TypeCode.Double:
            return (double)leftValue > (double)rightValue;

          case TypeCode.Single:
            return (float)leftValue > (float)rightValue;
        }
        throw new NumericTypeExpectedException("Please compare valid numeric types.");
      }

      /// <summary>
      /// Checks if the value of left is the same as the value of right.
      /// </summary>
      public static bool operator ==(INumeric left, INumeric right)
      {
        return !(left > right) && !(left < right);
      }

      /// <summary>
      /// Checks if the value of left is not the same as the value of right.
      /// </summary>
      public static bool operator !=(INumeric left, INumeric right)
      {
        return !(left > right) || !(left < right);
      }

      /// <summary>
      /// Checks if left is either equal or smaller than right.
      /// </summary>
      public static bool operator <=(INumeric left, INumeric right)
      {
        return left == right || left < right;
      }

      /// <summary>
      /// Checks if left is either equal or greater than right.
      /// </summary>
      public static bool operator >=(INumeric left, INumeric right)
      {
        return left == right || left > right;
      }
    }

    /// <summary>
    /// For generic numeric types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class INumeric<T> : IEquatable<INumeric<T>>
    {
      private T value;
      private Type type;

      public INumeric(T obj)
      {
        if (!typeof(T).IsNumeric())
        {
          // Something bad happened.
          throw new NumericTypeExpectedException("The type inputted into the NumericType generic must be a numeric type.");
        }
        type = typeof(T);
        value = obj;
      }

      public T GetValue()
      {
        return value;
      }

      public object GetValueAsObject()
      {
        return value;
      }

      public void SetValue(T newValue)
      {
        value = newValue;
      }

      public bool Equals(INumeric<T> other)
      {
        return this == other;
      }

      public override bool Equals(object obj)
      {
        if (obj != null && !(obj is INumeric<T>))
          return false;

        return Equals(obj);
      }

      public override int GetHashCode()
      {
        return GetValue().GetHashCode();
      }

      public override string ToString()
      {
        return GetValue().ToString();
      }

      /// <summary>
      /// Checks if the value of left is smaller than the value of right.
      /// </summary>
      public static bool operator <(INumeric<T> left, INumeric<T> right)
      {
        object leftValue = left.GetValueAsObject();
        object rightValue = right.GetValueAsObject();

        switch (Type.GetTypeCode(left.type))
        {
          case TypeCode.Byte:
            return (byte)leftValue < (byte)rightValue;

          case TypeCode.SByte:
            return (sbyte)leftValue < (sbyte)rightValue;

          case TypeCode.UInt16:
            return (ushort)leftValue < (ushort)rightValue;

          case TypeCode.UInt32:
            return (uint)leftValue < (uint)rightValue;

          case TypeCode.UInt64:
            return (ulong)leftValue < (ulong)rightValue;

          case TypeCode.Int16:
            return (short)leftValue < (short)rightValue;

          case TypeCode.Int32:
            return (int)leftValue < (int)rightValue;

          case TypeCode.Int64:
            return (long)leftValue < (long)rightValue;

          case TypeCode.Decimal:
            return (decimal)leftValue < (decimal)rightValue;

          case TypeCode.Double:
            return (double)leftValue < (double)rightValue;

          case TypeCode.Single:
            return (float)leftValue < (float)rightValue;
        }
        throw new NumericTypeExpectedException("Please compare valid numeric types with numeric generics.");
      }

      /// <summary>
      /// Checks if the value of left is greater than the value of right.
      /// </summary>
      public static bool operator >(INumeric<T> left, INumeric<T> right)
      {
        object leftValue = left.GetValueAsObject();
        object rightValue = right.GetValueAsObject();

        switch (Type.GetTypeCode(left.type))
        {
          case TypeCode.Byte:
            return (byte)leftValue > (byte)rightValue;

          case TypeCode.SByte:
            return (sbyte)leftValue > (sbyte)rightValue;

          case TypeCode.UInt16:
            return (ushort)leftValue > (ushort)rightValue;

          case TypeCode.UInt32:
            return (uint)leftValue > (uint)rightValue;

          case TypeCode.UInt64:
            return (ulong)leftValue > (ulong)rightValue;

          case TypeCode.Int16:
            return (short)leftValue > (short)rightValue;

          case TypeCode.Int32:
            return (int)leftValue > (int)rightValue;

          case TypeCode.Int64:
            return (long)leftValue > (long)rightValue;

          case TypeCode.Decimal:
            return (decimal)leftValue > (decimal)rightValue;

          case TypeCode.Double:
            return (double)leftValue > (double)rightValue;

          case TypeCode.Single:
            return (float)leftValue > (float)rightValue;
        }
        throw new NumericTypeExpectedException("Please compare valid numeric types.");
      }

      /// <summary>
      /// Checks if the value of left is the same as the value of right.
      /// </summary>
      public static bool operator ==(INumeric<T> left, INumeric<T> right)
      {
        return !(left > right) && !(left < right);
      }

      /// <summary>
      /// Checks if the value of left is not the same as the value of right.
      /// </summary>
      public static bool operator !=(INumeric<T> left, INumeric<T> right)
      {
        return !(left > right) || !(left < right);
      }

      /// <summary>
      /// Checks if left is either equal or smaller than right.
      /// </summary>
      public static bool operator <=(INumeric<T> left, INumeric<T> right)
      {
        return left == right || left < right;
      }

      /// <summary>
      /// Checks if left is either equal or greater than right.
      /// </summary>
      public static bool operator >=(INumeric<T> left, INumeric<T> right)
      {
        return left == right || left > right;
      }
    }
    

  }
}

