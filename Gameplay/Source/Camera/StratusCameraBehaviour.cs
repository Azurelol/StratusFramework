using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	public abstract class StratusCameraBehaviour : StratusCameraBehaviourBase
	{
		[SerializeField]
		public Camera _camera;
		public override Camera outputCamera => _camera;
	}



}