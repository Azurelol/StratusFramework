using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.AI
{
	[RequireComponent(typeof(StratusAgent))]
	public abstract class StratusAgentComponent : StratusManagedBehaviour
	{
		/// <summary>
		/// The agent this component is for
		/// </summary>
		public StratusAgent agent { get; private set; }

		protected override void OnManagedAwake()
		{
			base.OnManagedAwake();
			this.agent = GetComponent<StratusAgent>();
		}
	}

}