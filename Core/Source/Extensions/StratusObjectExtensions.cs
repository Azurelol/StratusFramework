using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stratus.OdinSerializer;
using UnityEngine;

using static Stratus.Utilities.StratusReflection;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Provides a deep copy of the given object
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T Clone<T>(this T source)
		{
			return (T)SerializationUtility.CreateCopy(source);
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

		/// <summary>
		/// Finds the most nested object inside of an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T GetNestedObject<T>(this object obj, string path)
		{
			foreach (string part in path.Split('.'))
			{
				obj = obj.GetFieldOrPropertyValue<T>(part);
			}
			return (T)obj;
		}

		/// <summary>
		/// Gets a property or a field of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the field/property.</typeparam>
		/// <param name="obj">Object the field/property should be found in.</param>
		/// <param name="name">Name of the field/property.</param>
		/// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
		/// <returns>The field/property.</returns>
		public static T GetFieldOrPropertyValue<T>(this object obj, string memberName, bool includeAllBases = false,
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(memberName, bindingFlags);
			if (field != null)
			{
				return (T)field.GetValue(obj);
			}

			PropertyInfo property = obj.GetType().GetProperty(memberName, bindingFlags);
			if (property != null)
			{
				return (T)property.GetValue(obj, null);
			}

			if (includeAllBases)
			{
				foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
				{
					field = type.GetField(memberName, bindingFlags);
					if (field != null)
					{
						return (T)field.GetValue(obj);
					}

					property = type.GetProperty(memberName, bindingFlags);
					if (property != null)
					{
						return (T)property.GetValue(obj, null);
					}
				}
			}

			throw new PropertyOrFieldNotFoundException($"Couldn't find a field or property with the name of {memberName} inside of the object {obj.GetType().Name }");
		}

		/// <summary>
		/// Gets a field inside of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the field.</typeparam>
		/// <param name="obj">Object the field should be found in.</param>
		/// <param name="name">Name of the field.</param>
		/// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>
		/// <returns>The field.</returns>
		public static T GetField<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			if (field != null)
			{
				return (T)field.GetValue(obj);
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Gets a property inside of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="obj">Object the property should be found in.</param>
		/// <param name="name">Name of the property.</param>
		/// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
		/// <returns>The property.</returns>
		public static T GetProperty<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
			if (property != null)
			{
				return (T)property.GetValue(obj, null);
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Sets a field or a property inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the field/property.</typeparam>
		/// <param name="obj">Object contaning the field/property.</param>
		/// <param name="name">Name of the field/property.</param>
		/// <param name="value">New value of the field/property.</param>
		/// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
		public static bool SetFieldOrPropertyValue<T>(this object obj, string memberName, T value, bool includeAllBases = false,
			BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(memberName, bindings);
			if (field != null)
			{
				field.SetValue(obj, value);
				return true;
			}

			PropertyInfo property = obj.GetType().GetProperty(memberName, bindings);
			if (property != null)
			{
				property.SetValue(obj, value, null);
				return true;
			}

			if (includeAllBases)
			{
				Type objectType = obj.GetType();
				foreach (Type type in objectType.GetBaseClassesAndInterfaces())
				{
					field = type.GetField(memberName, bindings);
					if (field != null)
					{
						field.SetValue(obj, value);
						return true;
					}

					property = type.GetProperty(memberName, bindings);
					if (property != null)
					{
						property.SetValue(obj, value, null);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Sets a field inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the field.</typeparam>
		/// <param name="obj">Object contaning the field.</param>
		/// <param name="name">Name of the field.</param>
		/// <param name="value">New value of the field.</param>
		/// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>>
		public static void SetField<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			if (field != null)
			{
				field.SetValue(obj, value);
				return;
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Sets a property inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="obj">Object contaning the property.</param>
		/// <param name="name">Name of the property.</param>
		/// <param name="value">New value of the property.</param>
		/// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
		public static void SetProperty<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
			if (property != null)
			{
				property.SetValue(obj, value, null);
				return;
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Gets all the properties and fields in obj of type T.
		/// </summary>
		/// <typeparam name="T">The type of the fields/properties.</typeparam>
		/// <param name="obj">Object to find the fields/properties in.</param>
		/// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
		/// <returns>The fields/properties found.</returns>
		public static IEnumerable<T> GetAllFieldsOrProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get the fields and the properties in the object.
			T[] fields = obj.GetAllFields<T>(bindingFlags).ToArray();
			T[] properties = obj.GetAllProperties<T>(bindingFlags).ToArray();

			// Only return the fields if fields were found.
			if (fields != null && fields.Length != 0)
			{
				// Loop through the fields and return each one.
				for (int i = 0; i < fields.Length; i++)
				{
					yield return fields[i];
				}
			}

			// Only return the properties if properties were found.
			if (properties != null && properties.Length != 0)
			{
				// Loop through the properties and return each one if they have the right type.
				for (int i = 0; i < properties.Length; i++)
				{
					yield return properties[i];
				}
			}
		}

		/// <summary>
		/// Gets all the properties and fields in object
		/// </summary>
		/// <param name="obj">Object to find the fields/properties in.</param>
		/// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
		/// <returns>The fields/properties found.</returns>
		public static IEnumerable GetAllFieldsOrProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get the fields and the properties in the object.
			object[] fields = obj.GetAllFields(bindingFlags).Cast<object>().ToArray();
			object[] properties = obj.GetAllProperties(bindingFlags).Cast<object>().ToArray();

			// Only return the fields if fields were found.
			if (fields != null && fields.Length != 0)
			{
				// Loop through the fields and return each one.
				for (int i = 0; i < fields.Length; i++)
				{
					yield return fields[i];
				}
			}

			// Only return the properties if properties were found.
			if (properties != null && properties.Length != 0)
			{
				// Loop through the properties and return each one if they have the right type.
				for (int i = 0; i < properties.Length; i++)
				{
					yield return properties[i];
				}
			}
		}

		/// <summary>
		/// Gets all the fields in obj of type T.
		/// </summary>
		/// <typeparam name="T">Type of the fields allowed.</typeparam>
		/// <param name="obj">Object to find the fields in.</param>
		/// <param name="bindingFlags">Filters of the fields allowed.</param>
		/// <returns>The fields found.</returns>
		public static IEnumerable<T> GetAllFields<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			FieldInfo[] fields = obj.GetType().GetFields(bindingFlags);

			// If there are no properties, break.
			if (fields == null || fields.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < fields.Length; i++)
			{
				object currentValue = fields[i].GetValue(obj);

				if (currentValue.GetType() == typeof(T))
				{
					yield return (T)currentValue;
				}
			}
		}

		/// <summary>
		/// Returns all fields that are being serialized by Unity
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FieldInfo[] GetSerializedFields(this Type type, bool unitySerialized = true)
		{
			ISerializationPolicy policy = unitySerialized ? OdinSerializer.SerializationPolicies.Unity : OdinSerializer.SerializationPolicies.Strict;
			return type.GetSerializedFields(policy);
		}

		/// <summary>
		/// Returns all fields that are being serialized
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FieldInfo[] GetSerializedFields(this Type type, ISerializationPolicy policy)
		{
			MemberInfo[] members = OdinSerializer.FormatterUtilities.GetSerializableMembers(type, policy);
			return members.OfType<FieldInfo>().ToArray();
		}


		/// <summary>
		/// Gets all the fields in obj.
		/// </summary>
		/// <param name="obj">Object to find the fields in.</param>
		/// <param name="bindingFlags">Filters of the fields allowed.</param>
		/// <returns>The fields found.</returns>
		public static IEnumerable GetAllFields(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			FieldInfo[] fields = obj.GetType().GetFields(bindingFlags);

			// If there are no properties, break.
			if (fields == null || fields.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < fields.Length; i++)
			{
				yield return fields[i].GetValue(obj);
			}
		}

		/// <summary>
		/// Gets all the properties in obj of type T.
		/// </summary>
		/// <typeparam name="T">Type of the properties allowed.</typeparam>
		/// <param name="obj">Object to find the properties in.</param>
		/// <param name="bindingFlags">Filters of the properties allowed.</param>
		/// <returns>The properties found.</returns>
		public static IEnumerable<T> GetAllProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			PropertyInfo[] properties = obj.GetType().GetProperties(bindingFlags);

			// If there are no properties, break.
			if (properties == null || properties.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < properties.Length; i++)
			{
				object currentValue = properties[i].GetValue(obj, null);

				if (currentValue.GetType() == typeof(T))
				{
					yield return (T)currentValue;
				}
			}
		}

		/// <summary>
		/// Gets all the properties in obj.
		/// </summary>
		/// <param name="obj">Object to find the properties in.</param>
		/// <param name="bindingFlags">Filters of the properties allowed.</param>
		/// <returns>The properties found.</returns>
		public static IEnumerable GetAllProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			PropertyInfo[] properties = obj.GetType().GetProperties(bindingFlags);

			// If there are no properties, break.
			if (properties == null || properties.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < properties.Length; i++)
			{
				yield return properties[i].GetValue(obj, null);
			}
		}




	}

}