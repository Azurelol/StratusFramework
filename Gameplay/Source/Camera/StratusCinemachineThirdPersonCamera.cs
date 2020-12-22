using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// A Cinemachine 3rd person camera
	/// </summary>
	public class StratusCinemachineThirdPersonCamera : StratusCinemachineVirtualCamera<Cinemachine3rdPersonFollow, Cinemachine3rdPersonFollow>
	{
		public override void OffsetHorizontalAxis(float offset)
		{
		}

		public override void OffsetVerticalAxis(float offset)
		{
		}

		protected override void OnVirtualCameraUpdate()
		{
		}
	}

}