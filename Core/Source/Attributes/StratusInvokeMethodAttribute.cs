using System;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// A button that signals that this method can be invoked from an inspector
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public sealed class StratusInvokeMethodAttribute : StratusInspectorAttribute
	{
		//--------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------/
		/// <summary>
		/// Set this false to make the button not work whilst in playmode
		/// </summary>
		public bool isPlayMode { get; set; }

		//--------------------------------------------------------------------------/
		// CTOR
		//--------------------------------------------------------------------------/
		public StratusInvokeMethodAttribute(string label, bool isPlayMode = false)
		{
			this.label = label;
			this.isPlayMode = isPlayMode;
		}

		public StratusInvokeMethodAttribute()
		{
		}




	}
}
