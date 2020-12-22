using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	[StratusCustomExtension(typeof(StratusCharacterController))]
	public class StratusCharacterControllerMovement : StratusCharacterMovement, IStratusExtensionBehaviour<StratusCharacterController>
	{
		public StratusCharacterController extensible { get; set; }

		public void OnExtensibleAwake(StratusExtensibleBehaviour extensible)
		{
			this.extensible = (StratusCharacterController)extensible;
		}

		public void OnExtensibleStart()
		{
		}

	}

}