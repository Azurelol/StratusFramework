using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	public class StratusCinemachineCameraInput : StratusBehaviour
	{
		/// <summary>
		/// A preset specifiying a control/camera scheme for this character
		/// </summary>
		[Serializable]
		public class Preset : StratusSerializable
		{
		}

		//private void ChangeCamera(Preset preset)
		//{
		//	this.cameraNavigation.previous.camera.priority = 10;
		//	preset.camera.priority = 15;
		//	this.currentPreset = preset;
		//	SetCameraLock();
		//	this.Log($"Camera set to {preset}");
		//	active = true;
		//}

		public void Look(Vector2 value)
		{

		}
	}

}