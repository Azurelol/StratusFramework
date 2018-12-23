using System;
using System.Reflection;
using OdinSerializer;
using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Provides a deep copy of the given object
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static object Clone(this object source)
		{
			return SerializationUtility.CreateCopy(source);
		}

		/// <summary>
		/// Provides a deep copy of another object's fields and properties onto this one
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static void Copy(this object self, object source)
		{
			// 1. Get the type of source object
			Type sourceType = source.GetType();

			// 2. Get all the properties of the source object type
			PropertyInfo[] propertyInfo = sourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			// 3. Assign all source property to target object properties
			foreach (PropertyInfo property in propertyInfo)
			{
				// Check whether the property can be written to
				if (property.CanWrite)
				{
					// Check whether the property type is value type, enum, or string type
					if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
					{
						property.SetValue(self, property.GetValue(source, null), null);
					}
					// Else the property type is object/complex types so need to recursively call this method
					// until the end of the tree is reached
					else
					{
						object objPropertyValue = property.GetValue(source, null);
						if (objPropertyValue == null)
						{
							property.SetValue(self, null, null);
						}
						else
						{
							property.SetValue(self, objPropertyValue.Clone(), null);
						}
					}
				}
			}
		}

		public static void OverwriteJSON(this object self, object other)
		{
			string otherData = JsonUtility.ToJson(other);
			JsonUtility.FromJsonOverwrite(otherData, self);
		}

		public static object CloneJSON(this object self)
		{
			string data = JsonUtility.ToJson(self);
			return JsonUtility.FromJson(data, self.GetType());
		}

		public static T GetAttribute<T>(this object obj) where T : Attribute
		{
			return obj.GetType().GetAttribute<T>();
		}




	}

}