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

		public static T GetAttribute<T>(this MemberInfo memberInfo) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(memberInfo, typeof(T));
		}

		public static Attribute GetAttribute(this MemberInfo memberInfo, Type attributeType)
		{
			return Attribute.GetCustomAttribute(memberInfo, attributeType);
		}

		public static Dictionary<Type, Attribute> MapAttributes(this MemberInfo memberInfo)
		{
			return AttributeUtility.MapAttributes(memberInfo);
		}

		public static string GetDescription(this MemberInfo memberInfo)
		{
			DescriptionAttribute description = memberInfo.GetAttribute<DescriptionAttribute>();
			return description != null ? description.Description : string.Empty;
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


	}
}