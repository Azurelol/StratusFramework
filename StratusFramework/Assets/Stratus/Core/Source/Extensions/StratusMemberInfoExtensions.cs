using System;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;

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
	}
}