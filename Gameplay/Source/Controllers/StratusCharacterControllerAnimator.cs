using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Gameplay;

namespace Stratus.Gameplay
{
	[RequireComponent(typeof(StratusCharacterControllerMovement))]
	[StratusCustomExtension(typeof(StratusCharacterController))]
	public class StratusCharacterControllerAnimator : StratusCharacterAnimator, IStratusExtensionBehaviour<StratusCharacterController>
	{
		public StratusCharacterController extensible { get; set; }
		public StratusCharacterControllerMovement movement { get; private set; }

		public void OnExtensibleAwake(StratusExtensibleBehaviour extensible)
		{
			this.extensible = (StratusCharacterController)extensible;
		}

		public void OnExtensibleStart()
		{
		}

	}

}