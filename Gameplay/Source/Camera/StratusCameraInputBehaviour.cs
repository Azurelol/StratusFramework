using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	public abstract class StratusCameraInputBehaviour<CameraBehaviour> : StratusInputBehaviour
		where CameraBehaviour : StratusCameraBehaviourBase
	{
		public CameraBehaviour cameraBehaviour;
	}

}