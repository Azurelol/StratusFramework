using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	public abstract class StratusCombatControllerModule
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public StratusCombatController controller { get; private set; }
		public GameObject gameObject { get; private set; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnInitialize();
		public abstract void OnTimeStep(float step);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Initialize(StratusCombatController controller)
		{
			this.controller = controller;
			this.gameObject = controller.gameObject;
			this.OnInitialize();
		}
	}
}