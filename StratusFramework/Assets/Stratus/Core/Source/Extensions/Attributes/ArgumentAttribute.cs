using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

namespace Stratus
{
	[AttributeUsage(AttributeTargets.Field)]
	public abstract class ArgumentAttribute : Attribute
	{
		public abstract void Execute(string argument);
	}	

	[AttributeUsage(AttributeTargets.Class)]
	public class ClassDescriptionAttribute : DescriptionAttribute
	{
		public ClassDescriptionAttribute(string description) : base(description)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class FieldDescriptionAttribute : DescriptionAttribute
	{
		public FieldDescriptionAttribute(string description) : base(description)
		{
		}
	}

}