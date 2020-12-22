using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Stratus
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class StratusDropdownAttribute : PropertyAttribute
	{
		public string memberName { get; private set; }

		public StratusDropdownAttribute(string valuesName)
		{
			memberName = valuesName;
		}
	}

}