using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace Stratus
{
	/// <summary>
	/// A Variant is a dynamic value type which represent a variety of types.
	/// It can be used in situtations where you need a common interface
	/// for your types to represent a variety of data.
	/// </summary>
	[Serializable]
	public struct StratusVariant
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
		public StratusVariant(int value) : this()
		{
			type = VariantType.Integer;
			this.integerValue = value;
		}

		public StratusVariant(float value) : this()
		{
			type = VariantType.Float;
			this.floatValue = value;
		}

		public StratusVariant(bool value) : this()
		{
			type = VariantType.Boolean;
			this.booleanValue = value;
		}

		public StratusVariant(string value) : this()
		{
			type = VariantType.String;
			this.stringValue = value;
		}

		public StratusVariant(Vector3 value) : this()
		{
			type = VariantType.Vector3;
			this.vector3Value = value;
		}

		public StratusVariant(StratusVariant variant) : this()
		{
			type = variant.type;
			this.Set(variant.Get());
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
		public static StratusVariant Make<T>(T value)
		{
			var type = typeof(T);

			if (type == typeof(int))
				return new StratusVariant((int)(object)value);
			else if (type == typeof(float))
				return new StratusVariant((float)(object)value);
			else if (type == typeof(bool))
				return new StratusVariant((bool)(object)value);
			else if (type == typeof(string))
				return new StratusVariant((string)(object)value);
			else if (type == typeof(Vector3))
				return new StratusVariant((Vector3)(object)value);

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

			return (T)Convert.ChangeType(value, typeof(T));
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
		public bool Compare(StratusVariant other)
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
			VariantType givenVariantType = VariantUtilities.Convert(type);
			return givenVariantType == this.type;
		}

	}

	public static class VariantUtilities
	{
		private static Dictionary<Type, StratusVariant.VariantType> systemTypeToVariantType { get; } = new Dictionary<Type, StratusVariant.VariantType>()
	{
	  {typeof(int), StratusVariant.VariantType.Integer},
	  {typeof(bool), StratusVariant.VariantType.Boolean},
	  {typeof(float), StratusVariant.VariantType.Float},
	  {typeof(string), StratusVariant.VariantType.String},
	  {typeof(Vector3), StratusVariant.VariantType.Vector3},
	};

		private static Dictionary<StratusVariant.VariantType, Type> variantTypeToSystemType { get; } = new Dictionary<StratusVariant.VariantType, Type>()
	{
	  {StratusVariant.VariantType.Integer, typeof(int)},
	  {StratusVariant.VariantType.Boolean, typeof(bool)},
	  {StratusVariant.VariantType.Float, typeof(float)},
	  {StratusVariant.VariantType.String, typeof(string)},
	  {StratusVariant.VariantType.Vector3, typeof(Vector3)},
	};

		public static Type Convert(this StratusVariant.VariantType type) => variantTypeToSystemType[type];
		public static StratusVariant.VariantType Convert(Type type) => systemTypeToVariantType[type];

	}


}