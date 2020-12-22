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

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ClassDescriptionAttribute : DescriptionAttribute
	{
		public ClassDescriptionAttribute(string description) : base(description)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	public class MemberDescriptionAttribute : DescriptionAttribute
	{
		public MemberDescriptionAttribute(string description) : base(description)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public class ParameterRange : Attribute
	{
		public float min;
		public float max;

		public ParameterRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}

}