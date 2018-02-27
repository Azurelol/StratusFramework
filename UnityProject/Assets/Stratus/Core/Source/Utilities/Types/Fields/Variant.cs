using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Stratus
{
  namespace Types
  {
    /// <summary>
    /// A Variant is a dynamic value type which represent a variety of types.
    /// It can be used in situtations where you need a common interface
    /// for your types to represent a variety of data.
    /// </summary>
    //[Serializable, StructLayout(LayoutKind.Explicit)]
    [Serializable]
    public struct Variant
    {      
      public enum Types
      {
        Integer, Boolean, Float, String, Vector3
      }


      //--------------------------------------------------------------------/
      // Fields
      //--------------------------------------------------------------------/
      //[FieldOffset(0), SerializeField]
      [SerializeField]
      private Types Type;

      //[FieldOffset(4), SerializeField]
      [SerializeField]
      private int Integer;

      //[FieldOffset(4), SerializeField]
      [SerializeField]
      private float Float;

      //[FieldOffset(4), SerializeField]
      [SerializeField]
      private bool Boolean;

      //[FieldOffset(4), SerializeField]
      [SerializeField]
      private Vector3 Vector3;

      //[FieldOffset(16), SerializeField]
      [SerializeField]
      private string String;


      //--------------------------------------------------------------------/
      // Properties
      //--------------------------------------------------------------------/
      /// <summary>
      /// Retrieves the current type of this Variant
      /// </summary>
      public Types CurrentType { get { return Type; } }


      //--------------------------------------------------------------------/
      // Constructors
      //--------------------------------------------------------------------/
      public Variant(int value) : this()
      {
        Type = Types.Integer;
        this.Integer = value;
      }

      public Variant(float value) : this()
      {
        Type = Types.Float;
        this.Float = value;
      }

      public Variant(bool value) : this()
      {
        Type = Types.Boolean;
        this.Boolean = value;
      }

      public Variant(string value) : this()
      {
        Type = Types.String;
        this.String = value;
      }

      public Variant(Vector3 value) : this()
      {
        Type = Types.Vector3;
        this.Vector3 = value;
      }

      public Variant(Variant variant) : this()
      {
        Type = variant.Type;
        var getFunc = typeof(Variant).GetMethod("Get").MakeGenericMethod(Type.Convert());
        this.Set(getFunc.Invoke(null, new object[] { variant }));
      }

      /// <summary>
      /// Constructs a variant based from a given value. It only accepts supported types,
      /// which are found in the Types enum.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="value"></param>
      /// <returns></returns>
      public static Variant Make<T>(T value)
      {
        var type = typeof(T);

        if (type == typeof(int))
          return new Variant((int)(object)value);
        else if (type == typeof(float))
          return new Variant((float)(object)value);
        else if (type == typeof(bool))
          return new Variant((bool)(object)value);
        else if (type == typeof(string))
          return new Variant((string)(object)value);
        else if (type == typeof(Vector3))
          return new Variant((Vector3)(object)value);

        throw new Exception("Unsupported type being used (" + type.Name + ")");
      }

      //--------------------------------------------------------------------/
      // Accessors
      //--------------------------------------------------------------------/
      public T Get<T>()
      {
        var type = typeof(T);

        if (type == typeof(int) && Type == Types.Integer)
          return (T)Convert.ChangeType(Integer, typeof(T));
        else if (type == typeof(float) && Type == Types.Float)
          return (T)Convert.ChangeType(Float, typeof(T));
        else if (type == typeof(bool) && Type == Types.Boolean)
          return (T)Convert.ChangeType(Boolean, typeof(T));
        else if (type == typeof(string) && Type == Types.String)
          return (T)Convert.ChangeType(String, typeof(T));
        else if (type == typeof(Vector3) && Type == Types.Vector3)
          return (T)Convert.ChangeType(Vector3, typeof(T));

        throw new ArgumentException("The provided type '" + type.Name + "' is not the correct type for this value (" + Type.ToString() + ")");
      }      

      public object Get(Type type)
      {
        if (type == typeof(int) && Type == Types.Integer)
          return Convert.ChangeType(Integer, typeof(int));
        else if (type == typeof(float) && Type == Types.Float)
          return (float)Convert.ChangeType(Float, typeof(float));
        else if (type == typeof(bool) && Type == Types.Boolean)
          return Convert.ChangeType(Boolean, typeof(bool));
        else if (type == typeof(string) && Type == Types.String)
          return Convert.ChangeType(String, typeof(string));
        else if (type == typeof(Vector3) && Type == Types.Vector3)
          return Convert.ChangeType(Vector3, typeof(Vector3));

        throw new ArgumentException("The provided type '" + type.Name + "' is not the correct type for this value (" + Type.ToString() + ")");
      }

      public void Set<T>(T value)
      {
        var type = typeof(T);

        if (type == typeof(int) && Type == Types.Integer)
          Integer = (int)(object)value;
        else if (type == typeof(float) && Type == Types.Float)
          Float = (float)(object)value;
        else if (type == typeof(string) && Type == Types.String)
          String = value as string;
        else if (type == typeof(bool) && Type == Types.Boolean)
          Boolean = (bool)(object)value;
        else if (type == typeof(Vector3) && Type == Types.Vector3)
          Vector3 = (Vector3)(object)value;

        throw new ArgumentException("The provided type '" + type.Name + "' is not the correct type for this value (" + Type.ToString() + ")");
      }
      
      public override string ToString()
      {
        var builder = new StringBuilder();
        switch (this.Type)
        {
          case Types.Integer:
            builder.Append(Integer.ToString());
            break;
          case Types.Float:
            builder.Append(Float.ToString());
            break;
          case Types.Boolean:
            builder.Append(Boolean.ToString());
            break;
          case Types.String:
            builder.Append(String);
            break;
          case Types.Vector3:
            builder.Append(Vector3.ToString());
            break;
        }

        return builder.ToString();
      }

      public bool Compare(Variant other)
      {
        if (this.Type != other.Type)
          throw new Exception("Mismatching variants are being compared!");

        switch (other.Type)
        {
          case Types.Boolean:
            return this.Boolean == other.Boolean;
          case Types.Integer:
            return this.Integer == other.Integer;
          case Types.Float:
            return this.Float == other.Float;
          case Types.String:
            return this.String == other.String;
          case Types.Vector3:
            return this.Vector3 == other.Vector3;
        }

        throw new Exception("Wrong type?");
      }
       
    }

    public static class VariantHelper
    {
      public static Type Convert(this Variant.Types type)
      {
        switch (type)
        {
          case Variant.Types.Integer:
            return typeof(int);
          case Variant.Types.Float:
            return typeof(float);
          case Variant.Types.Boolean:
            return typeof(bool);
          case Variant.Types.String:
            return typeof(string);
          case Variant.Types.Vector3:
            return typeof(Vector3);
        }

        throw new Exception("Unsupported type");
      }
    }

  }
}