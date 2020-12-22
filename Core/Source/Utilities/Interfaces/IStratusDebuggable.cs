using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Interface for globally toggling debug on and off
	/// </summary>
	public interface IStratusDebuggable
	{
		void Toggle(bool toggle);
	}
}