using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Stratus.Gameplay
{
	public abstract class StratusHealthModificationEffect<Parameters> : StratusCombatEffect<Parameters>
		where Parameters : IStratusCombatParameterModel
	{
		public float value = 100.0f;
		public bool percentage = false;
	}

}