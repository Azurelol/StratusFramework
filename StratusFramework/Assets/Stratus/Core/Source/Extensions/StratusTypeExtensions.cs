using System;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;
using System.Text;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Binding flags used for a full search of members
		/// </summary>
		private const BindingFlags bindingFlagsFullSearch =
			System.Reflection.BindingFlags.Public 
			| System.Reflection.BindingFlags.NonPublic 
			| System.Reflection.BindingFlags.Static
			| System.Reflection.BindingFlags.Instance;

		/// <summary>
		/// Type name alias lookup.
		/// TypeNameAlternatives["Single"] will give you "float", "UInt16" will give you "ushort", "Boolean[]" will give you "bool[]" etc..
		/// </summary>
		public static readonly Dictionary<string, string> TypeNameAlternatives = new Dictionary<string, string>()
		{
			{ "Single",     "float"     },
			{ "Double",     "double"    },
			{ "SByte",      "sbyte"     },
			{ "Int16",      "short"     },
			{ "Int32",      "int"       },
			{ "Int64",      "long"      },
			{ "Byte",       "byte"      },
			{ "UInt16",     "ushort"    },
			{ "UInt32",     "uint"      },
			{ "UInt64",     "ulong"     },
			{ "Decimal",    "decimal"   },
			{ "String",     "string"    },
			{ "Char",       "char"      },
			{ "Boolean",    "bool"      },
			{ "Single[]",   "float[]"   },
			{ "Double[]",   "double[]"  },
			{ "SByte[]",    "sbyte[]"   },
			{ "Int16[]",    "short[]"   },
			{ "Int32[]",    "int[]"     },
			{ "Int64[]",    "long[]"    },
			{ "Byte[]",     "byte[]"    },
			{ "UInt16[]",   "ushort[]"  },
			{ "UInt32[]",   "uint[]"    },
			{ "UInt64[]",   "ulong[]"   },
			{ "Decimal[]",  "decimal[]" },
			{ "String[]",   "string[]"  },
			{ "Char[]",     "char[]"    },
			{ "Boolean[]",  "bool[]"    },
		};

		private static readonly Dictionary<Type, string> cachedNiceNames = new Dictionary<Type, string>();

		/// <summary>
		/// Returns true if the given type is an array or list
		/// </summary>
		/// <param name="listType"></param>
		/// <returns></returns>
		public static bool IsArrayOrList(this Type listType)
		{
			return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
		}

		/// <summary>
		/// If the given type is a list, returns the underlying element type
		/// </summary>
		/// <param name="listType"></param>
		/// <returns></returns>
		public static Type GetArrayOrListElementType(this Type listType)
		{
			if (listType.IsArray)
			{
				return listType.GetElementType();
			}
			if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return listType.GetGenericArguments()[0];
			}

			return null;
		}

		/// <summary>
		/// Returns true if the type has a default constructor
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool HasDefaultConstructor(this Type t)
		{
			return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
		}

		public static FieldInfo GetFieldExhaustive(this Type t, string name)
		{
			return t.GetField(name, bindingFlagsFullSearch);
		}

		/// <summary>
		/// Returns true if its a nullable type (not primitive, value, or enum)
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsNullableType(this Type type)
		{
			return !(type.IsPrimitive || type.IsValueType || type.IsEnum);
		}

		private static readonly object CachedNiceNames_LOCK = new object();
		private static readonly Dictionary<Type, string> CachedNiceNames = new Dictionary<Type, string>();

		/// <summary>
		/// Returns a nicely formatted name of a type.
		/// </summary>
		public static string GetNiceName(this Type type)
		{
			if (type.IsNested && type.IsGenericParameter == false)
			{
				return type.DeclaringType.GetNiceName() + "." + GetCachedNiceName(type);
			}

			return GetCachedNiceName(type);
		}

		/// <summary>
		/// Returns a nicely formatted full name of a type.
		/// </summary>
		public static string GetNiceFullName(this Type type)
		{
			string result;

			if (type.IsNested && type.IsGenericParameter == false)
			{
				return type.DeclaringType.GetNiceFullName() + "." + GetCachedNiceName(type);
			}

			result = GetCachedNiceName(type);

			if (type.Namespace != null)
			{
				result = type.Namespace + "." + result;
			}

			return result;
		}

		/// <summary>
		/// Gets the name of the compilable nice.
		/// </summary>
		/// <param name="type">The type.</param>
		public static string GetCompilableNiceName(this Type type)
		{
			return type.GetNiceName().Replace('<', '_').Replace('>', '_').TrimEnd('_');
		}

		/// <summary>
		/// Gets the full name of the compilable nice.
		/// </summary>
		/// <param name="type">The type.</param>
		public static string GetCompilableNiceFullName(this Type type)
		{
			return type.GetNiceFullName().Replace('<', '_').Replace('>', '_').TrimEnd('_');
		}

		/// <summary>
		/// Determines whether a type inherits or implements another type. Also include support for open generic base types such as List&lt;&gt;.
		/// </summary>
		/// <param name="type"></param>
		public static bool InheritsFrom<T>(this Type type)
		{
			return type.InheritsFrom(typeof(T));
		}

		/// <summary>
		/// Determines whether a type inherits or implements another type. Also include support for open generic base types such as List&lt;&gt;.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="baseType"></param>
		public static bool InheritsFrom(this Type type, Type baseType)
		{
			if (baseType.IsAssignableFrom(type))
			{
				return true;
			}

			if (type.IsInterface && baseType.IsInterface == false)
			{
				return false;
			}

			if (baseType.IsInterface)
			{
				return type.GetInterfaces().Contains(baseType);
			}

			var t = type;
			while (t != null)
			{
				if (t == baseType)
				{
					return true;
				}

				if (baseType.IsGenericTypeDefinition && t.IsGenericType && t.GetGenericTypeDefinition() == baseType)
				{
					return true;
				}

				t = t.BaseType;
			}

			return false;
		}

		//--------------------------------------------------------------------/
		// Procedures
		//--------------------------------------------------------------------/
		private static string GetCachedNiceName(Type type)
		{
			string result;
			lock (CachedNiceNames_LOCK)
			{
				if (!CachedNiceNames.TryGetValue(type, out result))
				{
					result = CreateNiceName(type);
					CachedNiceNames.Add(type, result);
				}
			}
			return result;
		}

		private static string CreateNiceName(Type type)
		{
			if (type.IsArray)
			{
				int rank = type.GetArrayRank();
				return type.GetElementType().GetNiceName() + (rank == 1 ? "[]" : "[,]");
			}

			if (type.InheritsFrom(typeof(Nullable<>)))
			{
				return type.GetGenericArguments()[0].GetNiceName() + "?";
			}

			if (type.IsByRef)
			{
				return "ref " + type.GetElementType().GetNiceName();
			}

			if (type.IsGenericParameter || !type.IsGenericType)
			{
				return TypeNameGauntlet(type);
			}

			var builder = new StringBuilder();
			var name = type.Name;
			var index = name.IndexOf("`");

			if (index != -1)
			{
				builder.Append(name.Substring(0, index));
			}
			else
			{
				builder.Append(name);
			}

			builder.Append('<');
			var args = type.GetGenericArguments();

			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];

				if (i != 0)
				{
					builder.Append(", ");
				}

				builder.Append(GetNiceName(arg));
			}

			builder.Append('>');
			return builder.ToString();
		}

		/// <summary>
		/// Used to filter out unwanted type names. Ex "int" instead of "Int32"
		/// </summary>
		private static string TypeNameGauntlet(this Type type)
		{
			string typeName = type.Name;

			string altTypeName = string.Empty;

			if (TypeNameAlternatives.TryGetValue(typeName, out altTypeName))
			{
				typeName = altTypeName;
			}

			return typeName;
		}




	}

}