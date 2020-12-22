using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

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
			object obj = fieldInfo.GetValue(property.serializedObject.targetObject);
			Type type = obj.GetType();
			if (obj == null) { return null; }

			T actualObject = null;
			//if (typeof(IEnumerable).IsAssignableFrom(obj.GetType()))
			if (type.IsArray)// || type.IsGenericType)
			{
				int index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
				actualObject = ((T[])obj)[index];
			}
			else
			{
				actualObject = obj as T;
			}
			return actualObject;
		}

		/// <summary>
		/// Get the object the serialized property holds by using reflection
		/// </summary>
		/// <typeparam name="T">The object type that the property contains</typeparam>
		/// <param name="property"></param>
		/// <returns>Returns the object type T if it is the type the property actually contains</returns>
		public static T GetValue<T>(this SerializedProperty property)
		{
			return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property));
		}

		/// <summary>
		/// Get the object the serialized property holds by using reflection
		/// </summary>
		/// <typeparam name="T">The object type that the property contains</typeparam>
		/// <param name="property"></param>
		/// <returns>Returns the object type T if it is the type the property actually contains</returns>
		public static T GetParent<T>(this SerializedProperty property)
		{
			//Iterate to parent object of the value, necessary if it is a nested object
			object obj = GetSerializedPropertyRootComponent(property);
			string[] fieldStructure = property.propertyPath.Split('.');
			for (int i = 0; i < fieldStructure.Length - 1; i++)
			{
				obj = obj.GetFieldOrPropertyValue<object>(fieldStructure[i]);
			}
			return (T)obj;
		}

		/// <summary>
		/// Set the value of a field of the property with the type T
		/// </summary>
		/// <typeparam name="T">The type of the field that is set</typeparam>
		/// <param name="property">The serialized property that should be set</param>
		/// <param name="value">The new value for the specified property</param>
		/// <returns>Returns if the operation was successful or failed</returns>
		public static bool SetValue<T>(this SerializedProperty property, T value)
		{
			//Iterate to parent object of the value, necessary if it is a nested object
			object obj = GetSerializedPropertyRootComponent(property);
			string[] fieldStructure = property.propertyPath.Split('.');
			for (int i = 0; i < fieldStructure.Length - 1; i++)
			{
				obj = obj.GetFieldOrPropertyValue<object>(fieldStructure[i]);
			}
			string fieldName = fieldStructure.Last();

			return obj.SetFieldOrPropertyValue(fieldName, value);

		}

		/// <summary>
		/// Get the component of a serialized property
		/// </summary>
		/// <param name="property">The property that is part of the component</param>
		/// <returns>The root component of the property</returns>
		public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
		{
			return (Component)property.serializedObject.targetObject;
		}

		/// <summary>
		/// Iterates through objects to handle objects that are nested in the root object
		/// </summary>
		/// <typeparam name="T">The type of the nested object</typeparam>
		/// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
		/// <param name="obj">The root object from which this path leads to the property</param>
		/// <param name="includeAllBases">Include base classes and interfaces as well</param>
		/// <returns>Returns the nested object casted to the type T</returns>
		public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
		{
			foreach (string part in path.Split('.'))
			{
				obj = obj.GetFieldOrPropertyValue<object>(part, includeAllBases);
			}
			return (T)obj;
		}

		/// <summary>
		/// Iterates through objects to handle objects that are nested in the root object
		/// </summary>
		/// <typeparam name="T">The type of the nested object</typeparam>
		/// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
		/// <param name="obj">The root object from which this path leads to the property</param>
		/// <param name="includeAllBases">Include base classes and interfaces as well</param>
		/// <returns>Returns the nested object casted to the type T</returns>
		public static T GetNestedObjectUntil<T>(string path, object obj, bool includeAllBases = false)
		{
			foreach (string part in path.Split('.'))
			{
				obj = obj.GetFieldOrPropertyValue<object>(part, includeAllBases);
				if (obj is T)
				{
					return (T)obj;
				}
			}
			return default(T);
		}


		//public static T GetFieldOrPropertyValue<T>(string memberName, object obj, bool includeAllBases = false, 
		//	BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		//{
		//	FieldInfo field = obj.GetType().GetField(memberName, bindings);
		//	if (field != null)
		//	{
		//		return (T)field.GetValue(obj);
		//	}
		//
		//	PropertyInfo property = obj.GetType().GetProperty(memberName, bindings);
		//	if (property != null)
		//	{
		//		return (T)property.GetValue(obj, null);
		//	}
		//
		//	if (includeAllBases)
		//	{
		//		foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
		//		{
		//			field = type.GetField(memberName, bindings);
		//			if (field != null)
		//			{
		//				return (T)field.GetValue(obj);
		//			}
		//
		//			property = type.GetProperty(memberName, bindings);
		//			if (property != null)
		//			{
		//				return (T)property.GetValue(obj, null);
		//			}
		//		}
		//	}
		//
		//	return default(T);
		//}

		//public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		//{
		//	FieldInfo field = obj.GetType().GetField(fieldName, bindings);
		//	if (field != null)
		//	{
		//		field.SetValue(obj, value);
		//		return true;
		//	}
		//
		//	PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
		//	if (property != null)
		//	{
		//		property.SetValue(obj, value, null);
		//		return true;
		//	}
		//
		//	if (includeAllBases)
		//	{
		//		Type objectType = obj.GetType();
		//		foreach (Type type in objectType.GetBaseClassesAndInterfaces())
		//		{
		//			field = type.GetField(fieldName, bindings);
		//			if (field != null)
		//			{
		//				field.SetValue(obj, value);
		//				return true;
		//			}
		//
		//			property = type.GetProperty(fieldName, bindings);
		//			if (property != null)
		//			{
		//				property.SetValue(obj, value, null);
		//				return true;
		//			}
		//		}
		//	}
		//	return false;
		//}




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
			MemberExpression body = exp.Body as MemberExpression;
			if (body == null)
			{
				UnaryExpression ubody = (UnaryExpression)exp.Body;
				body = ubody.Operand as MemberExpression;
			}
			return body.Member.Name;
		}

		/// <summary>
		/// Generates a hash unique to this specific property on its specific object. 
		/// Useful for mapping metadata to the editor representation of one of its specific properties.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static int GetPropertyObjectHash(this SerializedProperty property)
		{
			int h1 = property.serializedObject.targetObject.GetHashCode();
			int h2 = property.propertyPath.GetHashCode();


			return ((h1 << 5) + h1) ^ h2;
		}

		/// <summary>
		/// Get the visible children of a given SerializedProperty
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty property)
		{
			if (property.hasChildren == false)
			{
				yield break;
			}

			bool checkNext = false;
			property = property.Copy();
			SerializedProperty next = property.Copy();

			// We only consider checking next if we're not the root property (which can't have a next)
			if (property.depth != -1)
			{
				checkNext = next.NextVisible(false);
			}

			if (property.NextVisible(true))
			{
				do
				{
					if (checkNext && SerializedProperty.EqualContents(property, next))
					{
						yield break;
					}
					yield return property;
				}
				while (property.NextVisible(false));
			}

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

		/// <summary>
		/// Retrieves the FieldInfo for the field behind this serialized property
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static FieldInfo GetFieldInfo(this SerializedProperty property)
		{
			Type objectType = property.serializedObject.targetObject.GetType();
			return objectType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
		}

		/// <summary>
		/// Retrieves the Type of the field behind this serialized property
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static Type GetFieldType(this SerializedProperty property)
		{
			Type objectType = property.serializedObject.targetObject.GetType();
			Type type = objectType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;
			return type;
		}

		/// <summary>
		/// Retrieves all the attributes for the field behind this serialized property
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static Attribute[] GetFieldAttributes(this SerializedProperty property)
		{
			FieldInfo fi = property.GetFieldInfo();
			if (fi == null)
				throw new MissingFieldException($"No field info for {property.name}");

			var attributes = CustomAttributeExtensions.GetCustomAttributes<Attribute>(fi);

			return attributes != null ? attributes.ToArray() : new Attribute[] { };
		}

		public static int GetHashCodeForPropertyPathWithoutArrayIndex(this SerializedProperty prop)
		{
			return StratusReflection.GetProperty<int>("hashCodeForPropertyPathWithoutArrayIndex", typeof(SerializedProperty), prop);
		}

		public static int GetInspectorMode(this SerializedObject prop)
		{
			return StratusReflection.GetProperty<int>("inspectorMode", typeof(SerializedObject), prop);
		}

		/// <summary>
		/// Splits a given property into each of its multiple values.
		/// If it has a single value, only the same property is returned.
		/// </summary>
		public static IEnumerable<SerializedProperty> Multiple(this SerializedProperty property)
		{
			if (property.hasMultipleDifferentValues)
			{
				return property.serializedObject.targetObjects.Select(o => new SerializedObject(o).FindProperty(property.propertyPath));
			}
			else
			{
				return new[] { property };
			}
		}

	}

}