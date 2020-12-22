using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.UI
{
	public abstract class StratusRuntimeInspectorSettings : StratusScriptable
	{
		public StratusRuntimeInspectorFieldBehaviour fieldBehaviour;

		public abstract StratusRuntimeInspectorDrawer GetDrawer(StratusSerializedField field);
	}

}