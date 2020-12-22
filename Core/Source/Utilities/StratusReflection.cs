using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace Stratus.Utilities
{
	public static partial class StratusReflection
	{
		/// <summary>
		/// An exception that is thrown whenever a field was not found inside of an object when using Reflection.
		/// </summary>
		[Serializable]
		public class FieldNotFoundException : Exception
		{
			public FieldNotFoundException() { }

			public FieldNotFoundException(string message) : base(message) { }

			public FieldNotFoundException(string message, Exception inner) : base(message, inner) { }

			protected FieldNotFoundException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		/// <summary>
		/// An exception that is thrown whenever a property was not found inside of an object when using Reflection.
		/// </summary>
		[Serializable]
		public class PropertyNotFoundException : Exception
		{
			public PropertyNotFoundException() { }

			public PropertyNotFoundException(string message) : base(message) { }

			public PropertyNotFoundException(string message, Exception inner) : base(message, inner) { }

			protected PropertyNotFoundException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		/// <summary>
		/// An exception that is thrown whenever a field or a property was not found inside of an object when using Reflection.
		/// </summary>
		[Serializable]
		public class PropertyOrFieldNotFoundException : Exception
		{
			public PropertyOrFieldNotFoundException() { }

			public PropertyOrFieldNotFoundException(string message) : base(message) { }

			public PropertyOrFieldNotFoundException(string message, Exception inner) : base(message, inner) { }

			protected PropertyOrFieldNotFoundException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		//----------------------------------------------------------------------/
		// Properties
		//----------------------------------------------------------------------/
		private static Assembly[] _allAssemblies;
		public static Assembly[] allAssemblies
		{
			get
			{
				if (_allAssemblies == null)
				{
					_allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				}

				return _allAssemblies;
			}
		}

		private static Dictionary<Type, Type[]> subclasses { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, string[]> subclassNames { get; set; } = new Dictionary<Type, string[]>();
		private static Dictionary<Type, Type[]> subclassesIncludeAbstract { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, Dictionary<Type, Type[]>> interfacesImplementationsByBaseType { get; set; } = new Dictionary<Type, Dictionary<Type, Type[]>>();
		private static Dictionary<Type, Type[]> interfaceImplementations { get; set; } = new Dictionary<Type, Type[]>();

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/
		/// <summary>
		/// Gets all the types that have at least one attribute in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllTypesWithAttributeAsEnumerable(this Assembly assembly, Type attribute)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.GetCustomAttributes(attribute.GetType(), true).Length > 0)
				{
					yield return type;
				}
			}
		}

		/// <summary>
		/// Get all the types that have at least one attribute in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static Type[] GetAllTypesWithAttribute(this Assembly assembly, Type attribute)
		{
			return assembly.GetAllTypesWithAttributeAsEnumerable(attribute).ToArray();
		}



		/// <summary>
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] GetSubclassNames<ClassType>(bool includeAbstract = false)
		{
			Type baseType = typeof(ClassType);
			return GetSubclassNames(baseType, includeAbstract);
		}

		[System.Diagnostics.DebuggerHidden]
		public static Type GetIndexedType(this ICollection poICollection)
		{
			PropertyInfo oPropertyInfo = poICollection == null ? null : poICollection.GetType().GetProperty("Item");
			return oPropertyInfo == null ? null : oPropertyInfo.PropertyType;
		}

		/// <summary>
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] GetSubclassNames(Type baseType, bool includeAbstract = false)
		{
			string[] typeNames;
			if (!subclassNames.ContainsKey(baseType))
			{
				Type[] types = GetSubclass(baseType, includeAbstract);
				//Type[] types = Assembly.GetAssembly(baseType).GetTypes();
				typeNames = (from Type type in types where type.IsSubclassOf(baseType) && !type.IsAbstract select type.Name).ToArray();
				subclassNames.Add(baseType, typeNames);
			}

			return subclassNames[baseType];
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] GetSubclass<ClassType>(bool includeAbstract = false)
		{
			return GetSubclass(typeof(ClassType), includeAbstract);
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] GetSubclass(Type baseType, bool includeAbstract = false)
		{
			// Done the first time this type is queried, in order to cache
			// Abstract
			if (includeAbstract)
			{
				if (!subclassesIncludeAbstract.ContainsKey(baseType))
				{
					List<Type> types = new List<Type>();
					foreach (Assembly assembly in allAssemblies)
					{
						Type[] assemblyTypes = (from Type t
												in assembly.GetTypes()
												where t.IsSubclassOf(baseType)
												select t).ToArray();
						types.AddRange(assemblyTypes);
					}
					subclassesIncludeAbstract.Add(baseType, types.ToArray());
				}
			}
			// Non-Abstract
			else
			{
				if (!subclasses.ContainsKey(baseType))
				{
					List<Type> types = new List<Type>();
					foreach (Assembly assembly in allAssemblies)
					{
						Type[] assemblyTypes = (from Type t
												in assembly.GetTypes()
												where t.IsSubclassOf(baseType) && !t.IsAbstract
												select t).ToArray();

						types.AddRange(assemblyTypes);
					}
					subclasses.Add(baseType, types.ToArray());
				}
			}

			return includeAbstract ? subclassesIncludeAbstract[baseType] : subclasses[baseType];
		}

		private static Dictionary<string, Type> s_TypeMap = new Dictionary<string, Type>();

		public static Type ResolveType(string classRef)
		{
			if (!s_TypeMap.TryGetValue(classRef, out Type type))
			{
				type = !string.IsNullOrEmpty(classRef) ? Type.GetType(classRef) : null;
				s_TypeMap[classRef] = type;
			}
			return type;
		}


		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] GetInterfaces(Type baseType, Type interfaceType, bool includeAbstract = false)
		{
			// First, map into the selected interface type
			if (!interfacesImplementationsByBaseType.ContainsKey(interfaceType))
			{
				interfacesImplementationsByBaseType.Add(interfaceType, new Dictionary<Type, Type[]>());
			}

			// Now for a selected interface type, find all implementations that derive from the base type
			if (!interfacesImplementationsByBaseType[interfaceType].ContainsKey(baseType))
			{
				Type[] implementedTypes = (from Type t
										   in GetSubclass(baseType)
										   where t.IsSubclassOf(baseType) && t.GetInterfaces().Contains((interfaceType))
										   select t).ToArray();
				interfacesImplementationsByBaseType[interfaceType].Add(baseType, implementedTypes);
			}

			return interfacesImplementationsByBaseType[interfaceType][baseType];
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] GetInterfaces(Type interfaceType, bool includeAbstract = false)
		{
			// First, map into the selected interface type
			if (!interfaceImplementations.ContainsKey(interfaceType))
			{
				List<Type> types = new List<Type>();
				foreach (Assembly assembly in allAssemblies)
				{
					Type[] implementedTypes = (from Type t
											   in assembly.GetTypes()
											   where t.GetInterfaces().Contains((interfaceType))
												&& (t.IsAbstract == includeAbstract)
											   select t).ToArray();

					types.AddRange(implementedTypes);
				}
				interfaceImplementations.Add(interfaceType, types.ToArray());
			}

			return interfaceImplementations[interfaceType];
		}



		/// <summary>
		/// Gets the loadable types for a given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null);
			}
		}

		public static FieldInfo[] GetFieldsWithNullReferences<T>(T behaviour) where T : Behaviour
		{
			Type type = behaviour.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			return (from f
				   in fields
					where (f.FieldType.IsSubclassOf(typeof(UnityEngine.Object))
					&& f.GetValue(behaviour).Equals(null))
					select f).ToArray();
		}

		public static Type GetPrivateType(string name, Type source)
		{
			Assembly assembly = source.Assembly;
			return assembly.GetType(name);
		}

		public static Type[] GetTypesFromAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return new Type[0];
			}
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException)
			{
				return new Type[0];
			}
		}

		public static Type GetPrivateType(string fqName)
		{
			return Type.GetType(fqName);
		}

		public static T GetField<T>(string name, Type type, bool isStatic = true, object instance = null)
		{
			BindingFlags bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo field = type.GetField(name, bindflags);

			return (T)field.GetValue(instance);
		}

		public static void SetField<T>(string name, Type type, T value, bool isStatic = true, object instantce = null)
		{
			BindingFlags bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			FieldInfo field = type.GetField(name, bindflags);

			if (instantce != null)
			{
				field = instantce.GetType().GetField(name, bindflags);
			}

			field.SetValue(instantce, value);
		}

		public static T GetProperty<T>(string name, Type type, object instance)
		{
			BindingFlags bindflags = BindingFlags.NonPublic | BindingFlags.Instance;
			PropertyInfo propInfo = type.GetProperty(name, bindflags);

			MethodInfo getAccessor = propInfo.GetGetMethod(true);

			return (T)getAccessor.Invoke(instance, null);
		}

		public static MethodInfo GetReflectedMethod(string name, Type type, bool isStatic = true, object instantce = null)
		{
			BindingFlags bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo method = type.GetMethod(name, bindflags);

			return method;
		}

		/// <summary>
		/// Retrieves the name of this property / field as well as its owning object.
		/// Note: This is quite an expensive call so use sparingly.
		/// </summary>
		/// <param name="varExpr">A lambda expression capturing a reference to a field or property</param>
		/// <returns></returns>
		public static StratusMemberReference GetReference<T>(Expression<Func<T>> varExpr)
		{
			// Slow, probs
			return StratusMemberReference.Construct(varExpr);
		}

		/// <summary>
		/// A list containing all the subclasses deriving from a particular class
		/// </summary>
		public class ClassList : List<KeyValuePair<string, Type>> { }

		/// <summary>
		/// Generates a list of key-value pairs of classes that derive from this one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <returns></returns>
		public static ClassList GenerateClassList<ClassType>(bool includeAbstract = true)
		{
			ClassList list = new ClassList();

			Type[] classes = StratusReflection.GetSubclass<ClassType>();
			foreach (Type e in classes)
			{
				string name = e.FullName.Replace('+', '.');
				Type type = e.ReflectedType;

				if (!includeAbstract && type.IsAbstract)
				{
					continue;
				}

				list.Add(new KeyValuePair<string, Type>(name, type));
			}
			return list;
		}

		/// <summary>
		/// Uses compiled expressions to instantiate types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static class New<T>
		{
			public static readonly Func<T> Instance = Creator();

			private static Func<T> Creator()
			{
				Type t = typeof(T);
				if (t == typeof(string))
				{
					return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();
				}

				if (t.HasDefaultConstructor())
				{
					return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();
				}

				return () => (T)FormatterServices.GetUninitializedObject(t);
			}
		}


		public static object Instantiate(Type t)
		{
			if (t == typeof(string))
			{
				return Expression.Lambda<Func<string>>(Expression.Constant(string.Empty)).Compile();
			}

			if (t.HasDefaultConstructor())
			{
				return Activator.CreateInstance(t);
			}

			return FormatterServices.GetUninitializedObject(t);
		}

		public static T Instantiate<T>()
		{
			Type t = typeof(T);
			if (t == typeof(string))
			{
				return (T)(object)(Expression.Lambda<Func<string>>(Expression.Constant(string.Empty)).Compile());
			}

			if (t.HasDefaultConstructor())
			{
				return (T)Activator.CreateInstance(t);
			}
			//return Expression.Lambda<Func<object>>(Expression.New(t)).Compile();

			return (T)FormatterServices.GetUninitializedObject(t);
		}



	}
}

