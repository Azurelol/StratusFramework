using System;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Invokes a provided method when the value is changed
	/// </summary>
	public class StratusValueChangedAttribute : PropertyAttribute
	{
		public string method;

		public StratusValueChangedAttribute(string method)
		{
			this.method = method;
		}
	}

}