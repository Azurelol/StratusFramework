using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace Stratus.Types
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
    //--------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------/
    public enum VariantType
    {
      Integer, Boolean, Float, String, Vector3
    }

    //--------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------/
    //[FieldOffset(0), SerializeField]
    [SerializeField]
    private VariantType type;

    //[FieldOffset(4), SerializeField]
    [SerializeField]
    private int integerValue;

    //[FieldOffset(4), SerializeField]
    [SerializeField]
    private float floatValue;

    //[FieldOffset(4), SerializeField]
    [SerializeField]
    private bool booleanValue;

    //[FieldOffset(4), SerializeField]
    [SerializeField]
    private Vector3 vector3Value;

    //[FieldOffset(16), SerializeField]
    [SerializeField]
    private string stringValue;


    //--------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------/
    /// <summary>
    /// Retrieves the current type of this Variant
    /// </summary>
    public VariantType currentType { get { return type; } }    

    //--------------------------------------------------------------------/
    // Constructors
    //--------------------------------------------------------------------/
    public Variant(int value) : this()
    {
      type = VariantType.Integer;
      this.integerValue = value;
    }

    public Variant(float value) : this()
    {
      type = VariantType.Float;
      this.floatValue = value;
    }

    public Variant(bool value) : this()
    {
      type = VariantType.Boolean;
      this.booleanValue = value;
    }

    public Variant(string value) : this()
    {
      type = VariantType.String;
      this.stringValue = value;
    }

    public Variant(Vector3 value) : this()
    {
      type = VariantType.Vector3;
      this.vector3Value = value;
    }

    public Variant(Variant variant) : this()
    {
      type = variant.type;
      this.Set(variant.Get());
      //var getFunc = typeof(Variant).GetMethod("Get").MakeGenericMethod(Type.Convert());
      //this.Set(getFunc.Invoke(null, new object[] { variant }));
    }

    //--------------------------------------------------------------------/
    // Static Methods
    //--------------------------------------------------------------------/
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

    //public static bool IsInteger(Type type) => (type == typeof(int) && this.type == Types.Integer)

    //--------------------------------------------------------------------/
    // Methods: Accessors - Generic
    //--------------------------------------------------------------------/
    /// <summary>
    /// Gets the current value of this variant
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>()
    {
      var givenType = typeof(T);
      if (!Match(givenType))
        throw new ArgumentException("The provided type '" + givenType.Name + "' is not the correct type for this value (" + this.type.ToString() + ")");

      object value = null;
      switch (this.type)
      {
        case VariantType.Integer:
          value = integerValue;
          break;
        case VariantType.Boolean:
          value = booleanValue;
          break;
        case VariantType.Float:
          value = floatValue;
          break;
        case VariantType.String:
          value = stringValue;
          break;
        case VariantType.Vector3:
          value = vector3Value;
          break;
      }

      return (T)Convert.ChangeType(integerValue, typeof(T));
    }

    /// <summary>
    /// Gets the current value of this variant
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public object Get()
    {
      object value = null;
      switch (type)
      {
        case VariantType.Integer:
          value = integerValue;
          break;
        case VariantType.Boolean:
          value = booleanValue;
          break;
        case VariantType.Float:
          value = floatValue;
          break;
        case VariantType.String:
          value = stringValue;
          break;
        case VariantType.Vector3:
          value = vector3Value;
          break;
      }
      return value;
    }

    /// <summary>
    /// Sets the current value of this variant, by deducing the given value type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public void Set(object value)
    {
      var givenType = value.GetType();
      if (!Match(givenType))
        throw new ArgumentException("The provided type '" + givenType.Name + "' is not the correct type for this value (" + this.type.ToString() + ")");

      switch (type)
      {
        case VariantType.Integer:
          integerValue = (int)value;
          break;
        case VariantType.Boolean:
          booleanValue = (bool)value;
          break;
        case VariantType.Float:
          floatValue = (float)value;
          break;
        case VariantType.String:
          stringValue = value as string;
          break;
        case VariantType.Vector3:
          vector3Value = (Vector3)value;
          break;
      }

      //if ((givenType == typeof(int) || givenType == typeof(Int32)) && this.type == Types.Integer)
      //  integerValue = (int)value;
      //else if (givenType == typeof(float) && this.type == Types.Float)
      //  floatValue = (float)value;
      //else if (givenType == typeof(string) && this.type == Types.String)
      //  stringValue = value as string;
      //else if (givenType == typeof(bool) && this.type == Types.Boolean)
      //  booleanValue = (bool)value;
      //else if (givenType == typeof(Vector3) && this.type == Types.Vector3)
      //  vector3Value = (Vector3)value;

      //throw new ArgumentException("The provided type '" + givenType.Name + "' is not the correct type for this value (" + this.type.ToString() + ")");
    }

    //--------------------------------------------------------------------/
    // Methods: Accessors - Specific
    //--------------------------------------------------------------------/
    public void SetInteger(int value)
    {
      if (type != VariantType.Integer)
        throw new ArgumentException("This variant has not been set as an integer type");
      integerValue = value;
    }

    public int GetInteger()
    {
      if (type != VariantType.Integer)
        throw new ArgumentException("This variant has not been set as an integer type");
      return integerValue;
    }

    public void SetFloat(float value)
    {
      if (type != VariantType.Float)
        throw new ArgumentException("This variant has not been set as a float type");
      floatValue = value;
    }

    public float GetFloat()
    {
      if (type != VariantType.Integer)
        throw new ArgumentException("This variant has not been set as a float type");
      return floatValue;
    }

    public void SetString(string value)
    {
      if (type != VariantType.String)
        throw new ArgumentException("This variant has not been set as a string type");
      stringValue = value;
    }

    public string GetString()
    {
      if (type != VariantType.String)
        throw new ArgumentException("This variant has not been set as a string type");
      return stringValue;
    }

    public void SetBool(bool value)
    {
      if (type != VariantType.Boolean)
        throw new ArgumentException("This variant has not been set as a boolean type");
      booleanValue = value;
    }

    public bool GetBool()
    {
      if (type != VariantType.Boolean)
        throw new ArgumentException("This variant has not been set as a boolean type");
      return booleanValue;
    }


    public void SetVector3(Vector3 value)
    {
      if (type != VariantType.Vector3)
        throw new ArgumentException("This variant has not been set as a Vector3 type");
      vector3Value = value;
    }

    public Vector3 GetVector3()
    {
      if (type != VariantType.Vector3)
        throw new ArgumentException("This variant has not been set as a Vector3 type");
      return vector3Value;
    }


    ///// <summary>
    ///// Sets the current value of this variant
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <returns></returns>
    //public void Set<T>(T value)
    //{
    //  var givenType = typeof(T);
    //  if (!Match(givenType))
    //    throw new ArgumentException("The provided type '" + givenType.Name + "' is not the correct type for this value (" + this.type.ToString() + ")");        
    //
    //  if ((givenType == typeof(int) || givenType == typeof(Int32)) && this.type == Types.Integer)
    //    integerValue = (int)(object)value;
    //  else if (givenType == typeof(float) && this.type == Types.Float)
    //    floatValue = (float)(object)value;
    //  else if (givenType == typeof(string) && this.type == Types.String)
    //    stringValue = value as string;
    //  else if (givenType == typeof(bool) && this.type == Types.Boolean)
    //    booleanValue = (bool)(object)value;
    //  else if (givenType == typeof(Vector3) && this.type == Types.Vector3)
    //    vector3Value = (Vector3)(object)value;
    //
    //  throw new ArgumentException("The provided type '" + givenType.Name + "' is not the correct type for this value (" + this.type.ToString() + ")");
    //}

    //--------------------------------------------------------------------/
    // Methods: Helper
    //--------------------------------------------------------------------/
    /// <summary>
    /// Prints the current value of this variant
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public override string ToString()
    {
      var builder = new StringBuilder();
      switch (this.type)
      {
        case VariantType.Integer:
          builder.Append(integerValue.ToString());
          break;
        case VariantType.Float:
          builder.Append(floatValue.ToString());
          break;
        case VariantType.Boolean:
          builder.Append(booleanValue.ToString());
          break;
        case VariantType.String:
          builder.Append(stringValue);
          break;
        case VariantType.Vector3:
          builder.Append(vector3Value.ToString());
          break;
      }

      return builder.ToString();
    }

    /// <summary>
    /// Compares the value of this variant with another
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Compare(Variant other)
    {
      if (this.type != other.type)
        throw new Exception("Mismatching variants are being compared!");

      switch (other.type)
      {
        case VariantType.Boolean:
          return this.booleanValue == other.booleanValue;
        case VariantType.Integer:
          return this.integerValue == other.integerValue;
        case VariantType.Float:
          return this.floatValue == other.floatValue;
        case VariantType.String:
          return this.stringValue == other.stringValue;
        case VariantType.Vector3:
          return this.vector3Value == other.vector3Value;
      }

      throw new Exception("Wrong type?");
    }

    /// <summary>
    /// Checks whether the given type matches that of the variant
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool Match(Type type)
    {
      return type == this.type.Convert();
    }

  }

  public static class VariantUtilities
  { 
    private static Dictionary<Type, Variant.VariantType> systemTypeToVariantType { get; } = new Dictionary<Type, Variant.VariantType>()
    {
      {typeof(int), Variant.VariantType.Integer},
      {typeof(bool), Variant.VariantType.Boolean},
      {typeof(float), Variant.VariantType.Float},
      {typeof(string), Variant.VariantType.String},
      {typeof(Vector3), Variant.VariantType.Vector3},
    };

    private static Dictionary<Variant.VariantType, Type> variantTypeToSystemType { get; } = new Dictionary<Variant.VariantType, Type>()
    {
      {Variant.VariantType.Integer, typeof(int)},
      {Variant.VariantType.Boolean, typeof(bool)},
      {Variant.VariantType.Float, typeof(float)},
      {Variant.VariantType.String, typeof(string)},
      {Variant.VariantType.Vector3, typeof(Vector3)},
    };

    public static Type Convert(this Variant.VariantType type) => variantTypeToSystemType[type];
    public static Variant.VariantType Convert(Type type) => systemTypeToVariantType[type];

  }


}