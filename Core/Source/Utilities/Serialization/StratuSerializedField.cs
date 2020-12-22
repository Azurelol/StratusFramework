using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Stratus
{
	/// <summary>
	/// Manages a property serialized in a custom way
	/// </summary>
	public class StratusSerializedField
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The reference to the field
		/// </summary>
		public object target { get; private set; }
		/// <summary>
		/// Reflection information of the field
		/// </summary>
		public FieldInfo field { get; private set; }
		/// <summary>
		/// Reflection information of the type of the field
		/// </summary>
		public Type type { get; private set; }
		/// <summary>
		/// Enumerated type of the field
		/// </summary>
		public StratusSerializedFieldType fieldType { get; private set; }
		/// <summary>
		/// The name of the field
		/// </summary>
		public string name => this.field.Name;
		/// <summary>
		/// The display name of the field (Nicefied)
		/// </summary>
		public string displayName { get; private set; }
		/// <summary>
		/// The name of subfields for this field
		/// </summary>
		public Dictionary<string, StratusSerializedField> childrenByName { get; private set; }
		/// <summary>
		/// Children fields for this field
		/// </summary>
		public StratusSerializedField[] children { get; private set; }
		/// <summary>
		/// Whether this field has children subfields
		/// </summary>
		public bool hasChildren => children.IsValid();
		/// <summary>
		/// Whether this field is of a primitive type
		/// </summary>
		public bool isPrimitive { get; private set; }
		/// <summary>
		/// Whether this field is of List type
		/// </summary>
		public bool isList { get; private set; }
		public IList asList { get; private set; }
		/// <summary>
		/// If this field is a list, the type of its elements
		/// </summary>
		public Type listElementType { get; private set; }
		/// <summary>
		/// If an enum, the names of its possible enum values
		/// </summary>
		public string[] enumValueNames { get; private set; }
		/// <summary>
		/// The value of this field
		/// </summary>
		public object value
		{		get { return field.GetValueOrSetDefault(target); }
			set
			{
				field.SetValue(target, value);
				NotifyValueChanged();
			}
		}
		/// <summary>
		/// Whether this field has been expanded (in an UI drawer)
		/// </summary>
		public bool isExpanded { get; set; } = true;
		/// <summary>
		/// The attributes for this field, by their type. Useful for querying present of attributes.
		/// </summary>
		public Dictionary<Type, Attribute> attributesByType
		{
			get
			{
				if (_attributesByType == null)
				{
					_attributesByType = new Dictionary<Type, Attribute>();
					var attributes = OdinSerializer.Utilities.MemberInfoExtensions.GetAttributes(field);
					foreach (Attribute attr in attributes)
					{
						_attributesByType.Add(attr.GetType(), attr);
					}

				}
				return _attributesByType;
			}
		}
		private Dictionary<Type, Attribute> _attributesByType;

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Invoked whenever the value changes
		/// </summary>
		public event Action<object> onValueChanged;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusSerializedField(FieldInfo field, object target)
		{
			this.field = field;
			this.type = this.field.FieldType;
			this.fieldType = SerializedFieldTypeExtensions.Deduce(this.field);
			this.target = target;
			this.isPrimitive = this.type.IsPrimitive;

			// Enum
			if (this.fieldType == StratusSerializedFieldType.Enum)
			{
				this.enumValueNames = Enum.GetNames(this.type);
			}

			// Array
			this.isList = typeof(IList).IsAssignableFrom(this.type);
			if (this.isList)
			{
				this.asList = this.field.GetValue(target) as IList;
				// Not yet instantiated
				if (this.asList == null)
				{
					this.value = Activator.CreateInstance(this.type);
					this.asList = this.field.GetValue(target) as IList;
				}
				this.listElementType = Utilities.StratusReflection.GetIndexedType(this.asList);
			}
			else
			{
				if (!isPrimitive && this.fieldType != StratusSerializedFieldType.Enum)
				{
					this.children = GetChildFields(this);
					this.childrenByName = this.children.ToDictionary<string, StratusSerializedField>((sf) => sf.name);

				}
			}

			// Get the display name
			this.displayName = field.GetNiceName();
		}

		public override string ToString()
		{
			return $"({type.GetNiceName()}) {field.Name}";
		}

		public string PrintHierarchy(int depth)
		{
			StringBuilder sb = new StringBuilder();
			string toString = ToString();
			sb.AppendLine(toString.PadLeft(toString.Length + depth, '-'));
			if (hasChildren)
			{
				foreach (var child in children)
				{
					sb.Append(child.PrintHierarchy(depth + 1));
				}
			}
			return sb.ToString();
		}

		public void SetValueWithoutNotify(object value)
		{
			field.SetValue(target, value);
		}

		public void NotifyValueChanged()
		{
			onValueChanged?.Invoke(this.value);
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private static StratusSerializedField[] GetChildFields(StratusSerializedField parent)
		{
			List<StratusSerializedField> children = new List<StratusSerializedField>();
			foreach (var childField in parent.field.FieldType.GetSerializedFields())
			{
				children.Add(new StratusSerializedField(childField, parent.value));
			}
			return children.ToArray();
		}


		//------------------------------------------------------------------------/
		// IList Methods
		//------------------------------------------------------------------------/
		public object GetArrayElementAtIndex(int index)
		{
			return this.asList[index];
		}

		public object AddArrayElement()
		{
			object value = asList.Add(Activator.CreateInstance(listElementType));
			return value;
		}

		public void ClearArrayElements()
		{
			asList.Clear();
		}

	}

	public enum StratusSerializedFieldType
	{
		Boolean,
		Integer,
		Float,
		String,
		Enum,
		Vector2,
		Vector3,
		Vector4,
		LayerMask,
		Color,
		Rect,
		ObjectReference,
		Generic,
		Unsupported
	}

	public static class SerializedFieldTypeExtensions
	{
		private static StratusBictionary<Type, StratusSerializedFieldType> serializedFieldTypeMap { get; set; }
			= new StratusBictionary<Type, StratusSerializedFieldType>()
			{
				{ typeof(bool), StratusSerializedFieldType.Boolean },
				{ typeof(int), StratusSerializedFieldType.Integer},
				{ typeof(float), StratusSerializedFieldType.Float },
				{ typeof(string), StratusSerializedFieldType.String },
				{ typeof(Enum), StratusSerializedFieldType.Enum},
				{ typeof(Vector2), StratusSerializedFieldType.Vector2 },
				{ typeof(Vector3), StratusSerializedFieldType.Vector3},
				{ typeof(Vector4), StratusSerializedFieldType.Vector4},
				{ typeof(Color), StratusSerializedFieldType.Color },
				{ typeof(Rect), StratusSerializedFieldType.Rect},
				{typeof(LayerMask), StratusSerializedFieldType.LayerMask },
			};

		public static Type ToType(this StratusSerializedFieldType serializedFieldType)
		{
			return serializedFieldTypeMap.reverse[serializedFieldType];
		}

		public static StratusSerializedFieldType ToType(this Type type)
		{
			return serializedFieldTypeMap.forward[type];
		}

		public static StratusSerializedFieldType Deduce(FieldInfo field)
		{
			Type type = field.FieldType;
			StratusSerializedFieldType serializedFieldType = StratusSerializedFieldType.Generic;

			if (type.IsEnum)
			{
				serializedFieldType = StratusSerializedFieldType.Enum;
			}
			else if (serializedFieldTypeMap.forward.Contains(type))
			{
				serializedFieldType = serializedFieldTypeMap.forward[type];
			}
			else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				serializedFieldType = StratusSerializedFieldType.ObjectReference;
			}

			return serializedFieldType;
		}
	}





}