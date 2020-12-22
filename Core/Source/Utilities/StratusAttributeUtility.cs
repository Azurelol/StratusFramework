using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stratus.Utilities
{
	public static class StratusAttributeUtility
	{
		/// <summary>
		/// Finds an attribute of the specified type inside the class
		/// </summary>
		/// <typeparam name="AttributeType">The attribute class which was used in the class declaration</typeparam>
		/// <param name="type">The type of the class that was declared with the attribute</param>
		/// <returns></returns>
		public static Dictionary<Type, Attribute> MapAttributes(MemberInfo memberInfo)
		{
			Attribute[] attributes = (Attribute[])memberInfo.GetCustomAttributes(typeof(Attribute), true);
			Dictionary<Type, Attribute> attributeMap = new Dictionary<Type, Attribute>();
			if (attributes.Length > 0)
			{
				attributeMap.AddRangeUnique((Attribute attr) => attr.GetType(), attributes);
				return attributeMap;
			}
			return null;
		}


	}
}
