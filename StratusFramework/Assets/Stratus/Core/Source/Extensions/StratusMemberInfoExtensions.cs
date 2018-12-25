using System;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;
using System.ComponentModel;

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


	}
}