using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// Base class for variables serialized using Unity''s Prefs system
	/// </summary>
	public abstract class StratusPrefsVariable 
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public enum VariableType
		{
			Integer,
			Float,
			String,
			Boolean,
			Object
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public string key { get; private set; }		
		public VariableType type { get; private set; }

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusPrefsVariable(string key, VariableType type)
		{
			this.key = key;
			this.type = type;
		}

		public StratusPrefsVariable(Type key, VariableType type) : this(key.GetNiceFullName(), type)
		{
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/	
		public void Set(object value)
		{
			switch (value)
			{
				case int i:
					if (type != VariableType.Integer)
						throw new ArgumentException("Tried to assign Integer value to a " + type + " variable");
					SetInt(i);
					break;

				case float f:
					if (type != VariableType.Float)
						throw new ArgumentException("Tried to assign Float value to a " + type + " variable");
					SetFloat(f);
					break;

				case string s:
					if (type != VariableType.String)
						throw new ArgumentException("Tried to assign String value to a " + type + " variable");
					SetString(s);
					break;

				case bool b:
					if (type != VariableType.Boolean)
						throw new ArgumentException("Tried to assign Boolean value to a " + type + " variable");
					SetBool(b);
					break;

				case object o:
					SetObject(value);
					break;
			}
		}

		public object Get()
		{
			switch (this.type)
			{
				case VariableType.Integer:
					return GetInt();
				case VariableType.Float:
					return GetFloat();
				case VariableType.String:
					return GetString();
				case VariableType.Boolean:
					return GetBool();
				case VariableType.Object:
					throw new NotSupportedException("For object types, the overoad using the type parameter must be used (in order for it to know what to deserialize)");
			}
			throw new NotSupportedException("Unsupported type given!");
		}

		public T Get<T>()
		{
			if (type == VariableType.Object)
				return GetObject<T>();
			return (T)Get();
		}

		public abstract void Delete();
		public abstract void SetInt(int value);
		public abstract int GetInt();

		public abstract void SetFloat(float value);
		public abstract float GetFloat();

		public abstract void SetBool(bool value);
		public abstract bool GetBool();

		public abstract void SetString(string value);
		public abstract string GetString();

		public void SetObject(object value)
		{
			SetString(StratusJSONSerializerUtility.Serialize(value));
		}

		public T GetObject<T>()
		{
			return StratusJSONSerializerUtility.Deserialize<T>(GetString());
		}

		public static VariableType DeduceType(Type type)
		{
			if (type.Equals(typeof(int)))
			{
				return VariableType.Integer;
			}
			else if (type.Equals(typeof(float)))
			{
				return VariableType.Float;
			}
			else if (type.Equals(typeof(bool)))
			{
				return VariableType.Boolean;
			}
			else if (type.Equals(typeof(string)))
			{
				return VariableType.String;
			}

			return VariableType.Object;
		}
	}

	/// <summary>
	/// A variable saved using PlayerPrefs
	/// </summary>
	public class StratusPlayerPrefsVariable : StratusPrefsVariable
	{
		public StratusPlayerPrefsVariable(string key, VariableType type) : base(key, type)
		{
		}

		public StratusPlayerPrefsVariable(Type key, VariableType type) : base(key, type)
		{
		}

		public override bool GetBool()
		{
			return PlayerPrefs.GetInt(key) == 0 ? true : false;
		}

		public override float GetFloat()
		{
			return PlayerPrefs.GetFloat(key);
		}

		public override int GetInt()
		{
			return PlayerPrefs.GetInt(key);
		}

		public override string GetString()
		{
			return PlayerPrefs.GetString(key);
		}

		public override void SetBool(bool value)
		{
			PlayerPrefs.SetInt(key, value ? 0 : 1);
		}

		public override void SetFloat(float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public override void SetInt(int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public override void SetString(string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public override void Delete()
		{
			PlayerPrefs.DeleteKey(key);
		}
	}

}