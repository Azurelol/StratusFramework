using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Observes another object's messages
	/// </summary>
	public abstract class StratusProxy : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this is persistent after firing off the first time
		/// </summary>
		public bool persistent = true;
		/// <summary>
		/// Whether to log debug output
		/// </summary>
		public bool debug = false;

	}
}