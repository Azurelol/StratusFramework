using System;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns true if the given member has the attribute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="memberInfo"></param>
		/// <returns></returns>
		public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
		{
			return memberInfo.HasAttribute(typeof(T));
		}

		/// <summary>
		/// Returns true if the given member has the attribute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="memberInfo"></param>
		/// <returns></returns>
		public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType)
		{
			return Attribute.GetCustomAttribute(memberInfo, attributeType) != null;
		}

		/// <summary>
		/// Returns the attribute of the specified type from the member
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="memberInfo"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this MemberInfo memberInfo) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(memberInfo, typeof(T));
		}

		/// <summary>
		/// Returns the attribute of the specified type from the member
		/// </summary>
		/// <param name="memberInfo"></param>
		/// <param name="attributeType"></param>
		/// <returns></returns>
		public static Attribute GetCustomAttribute(this MemberInfo memberInfo, Type attributeType)
		{
			return Attribute.GetCustomAttribute(memberInfo, attributeType);
		}

		/// <summary>
		/// Returns all attributes of the specified type.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <param name="inherit">If true, specifies to also search the ancestors of element for custom attributes.</param>
		public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo member, bool inherit = false) where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), inherit).Cast<T>();
		}

		/// <summary>
		/// Returns all attributes of the specified type.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <param name="inherit">If true, specifies to also search the ancestors of element for custom attributes.</param>
		public static IEnumerable<Attribute> GetAttributes(this MemberInfo member, bool inherit = false)
		{
			return member.GetCustomAttributes(typeof(Attribute), inherit).Cast<Attribute>();
		}

		/// <summary>
		/// Returns a dictionary by type of all the attributes present in the member
		/// </summary>
		/// <param name="memberInfo"></param>
		/// <returns></returns>
		public static Dictionary<Type, Attribute> MapAttributes(this MemberInfo memberInfo)
		{
			return StratusAttributeUtility.MapAttributes(memberInfo);
		}

		/// <summary>
		/// Returns the description of a given member (that has the DescriptionAttribute)
		/// </summary>
		/// <param name="memberInfo"></param>
		/// <returns></returns>
		public static string GetDescription(this MemberInfo memberInfo)
		{
			DescriptionAttribute description = memberInfo.GetAttribute<DescriptionAttribute>();
			return description != null ? description.Description : string.Empty;
		}

		/// <summary>
		/// Generic version of FieldInfo.GetValue
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldInfo"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static T GetValue<T>(this FieldInfo fieldInfo, object target)
		{
			return (T)fieldInfo.GetValue(target);
		}


		/// <summary>
		/// Returns the value of a field, setting it to default if null
		/// </summary>
		public static object GetValueOrSetDefault(this FieldInfo field, object target)
		{
			// Try to get the value from the taret
			object value;
			value = field.GetValue(target);
			// If the field hasn't been instantiated
			if (value == null)
			{
				if (field.FieldType.Equals(typeof(string)))
				{
					value = string.Empty;
				}
				else
				{
					value = Activator.CreateInstance(field.FieldType);
				}
				field.SetValue(target, value);
			}
			return value;
		}

		/// <summary>
		/// If this member is a method, returns the full method name (name + params) otherwise the member name paskal splitted
		/// </summary>
		public static string GetNiceName(this MemberInfo member)
		{
			var method = member as MethodBase;
			string result;
			if (method != null)
			{
				result = method.GetFullName();
			}
			else
			{
				result = member.Name;
			}

			return result.ToTitleCase();
		}

		/// <summary>
		/// Returns the specified method's full name "methodName(argType1 arg1, argType2 arg2, etc)"
		/// Uses the specified gauntlet to replaces type names, ex: "int" instead of "Int32"
		/// </summary>
		public static string GetFullName(this MethodBase method, string extensionMethodPrefix)
		{
			var builder = new StringBuilder();
			bool isExtensionMethod = method.IsExtensionMethod();

			if (isExtensionMethod)
			{
				builder.Append(extensionMethodPrefix);
			}

			builder.Append(method.Name);
			builder.Append("(");
			builder.Append(method.GetParameterNames());
			builder.Append(")");
			return builder.ToString();
		}

		/// <summary>
		/// Returns a string representing the passed method parameters names. Ex "int num, float damage, Transform target"
		/// </summary>
		public static string GetParameterNames(this MethodBase method)
		{
			ParameterInfo[] pinfos = method.IsExtensionMethod() ? method.GetParameters().Skip(1).ToArray() : method.GetParameters();
			var builder = new StringBuilder();

			for (int i = 0, len = pinfos.Length; i < len; i++)
			{
				var param = pinfos[i];
				var paramTypeName = param.ParameterType.GetNiceName();
				builder.Append(paramTypeName);
				builder.Append(" ");
				builder.Append(param.Name);

				if (i < len - 1)
				{
					builder.Append(", ");
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Tests if a method is an extension method.
		/// </summary>
		public static bool IsExtensionMethod(this MethodBase method)
		{
			var type = method.DeclaringType;
			return type.IsSealed &&
				!type.IsGenericType &&
				!type.IsNested &&
				method.IsDefined(typeof(ExtensionAttribute), false);
		}

		/// <summary>
		/// Returns the specified method's full name.
		/// </summary>
		public static string GetFullName(this MethodBase method)
		{
			return GetFullName(method, "[ext] ");
		}


		/// <summary>
		/// Determines if an enum has the given flag defined bitwise.
		/// Fallback equivalent to .NET's Enum.HasFlag().
		/// </summary>
		public static bool HasFlag(this Enum value, Enum flag)
		{
			long lValue = System.Convert.ToInt64(value);
			long lFlag = System.Convert.ToInt64(flag);
			return (lValue & lFlag) != 0;
		}

		private static MethodInfo[] extensionMethodsCache;

		/// <summary>
		/// Searches all assemblies for extension methods for a given type.
		/// </summary>
		public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type, bool inherited = true)
		{
			// http://stackoverflow.com/a/299526

			if (extensionMethodsCache == null)
			{
				extensionMethodsCache = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes())
					.Where(potentialType => potentialType.IsSealed && !potentialType.IsGenericType && !potentialType.IsNested)
					.SelectMany(extensionType => extensionType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					.Where(method => method.IsExtension())
					.ToArray();
			}

			if (inherited)
			{
				return extensionMethodsCache.Where(method => method.GetParameters()[0].ParameterType.IsAssignableFrom(type));
			}
			else
			{
				return extensionMethodsCache.Where(method => method.GetParameters()[0].ParameterType == type);
			}
		}

		public static bool IsExtension(this MethodInfo methodInfo)
		{
			return methodInfo.IsDefined(typeof(ExtensionAttribute), false);
		}


	}
}