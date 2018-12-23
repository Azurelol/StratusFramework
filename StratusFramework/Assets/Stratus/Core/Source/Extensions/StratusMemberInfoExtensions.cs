using System;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;
using System.ComponentModel;

namespace Stratus
{
	public static partial class Extensions
	{
		public static T GetValue<T>(this FieldInfo fieldInfo, object target)
		{
			return (T)fieldInfo.GetValue(target);
		}

		public static void SetValue<T>(this FieldInfo fieldInfo, object target, object value)
		{
			fieldInfo.SetValue(target, value);
		}

		public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
		{
			return Attribute.GetCustomAttribute(memberInfo, typeof(T)) != null;
		}

		public static T GetAttribute<T>(this MemberInfo memberInfo) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(memberInfo, typeof(T));
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