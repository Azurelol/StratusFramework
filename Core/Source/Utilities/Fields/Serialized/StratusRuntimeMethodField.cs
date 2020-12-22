using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stratus
{
	/// <summary>
	/// Field that allows custom methods to be set for an inspector window and run
	/// </summary>
	[Serializable]
	public class StratusRuntimeMethodField
	{
		/// <summary>
		/// The method which this button will invoke
		/// </summary>
		public Action[] methods { get; private set; }

		public StratusRuntimeMethodField(params Action[] methods)
		{
			this.methods = methods;
		}

		public StratusRuntimeMethodField(Action method)
		{
			this.methods = new Action[] { method };
		}

	}
}
