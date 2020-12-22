using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Camera behaviours using Cinemachine's system
	/// </summary>
	public abstract class StratusCinemachineCamera : StratusCameraBehaviourBase
	{
		//--------------------------------------------------------------------------------------------/
		// Inspector fields
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Whether to disable Cinemachine's default input once this camera becomes active
		/// </summary>
		[Tooltip("Whether to disable Cinemachine's default input once this camera becomes active")]
		[SerializeField]
		private bool disableDefaultInput = true;

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// The virtual Cinemachine camera this behaviour works with
		/// </summary>
		public abstract CinemachineVirtualCameraBase virtualCameraBase { get; }
		/// <summary>
		/// Object the camera moves with
		/// </summary>
		public Transform follow { get => virtualCameraBase.Follow; set => virtualCameraBase.Follow = value; }
		/// <summary>
		/// Object the camera follows (by rotating itself)
		/// </summary>
		public Transform lookAt { get => virtualCameraBase.LookAt; set => virtualCameraBase.LookAt = value; }
		/// <summary>
		/// Determines which camera is active on the system. Higher numbers indicate greater priority.
		/// </summary>
		public int priority { get => virtualCameraBase.Priority; set => virtualCameraBase.Priority = value; }
		/// <summary>
		/// The main actual camera being used by the Cinemachine system
		/// </summary>
		public override Camera outputCamera
		{
			get
			{
				if (_outputCamera == null)
				{
					_outputCamera = brain.OutputCamera;
				}
				return _outputCamera;
			}
		}
		private Camera _outputCamera;
		/// <summary>
		/// Manages the Cinemachine system at runtime through virtual camera management
		/// </summary>
		public CinemachineBrain brain
		{
			get
			{
				if (_brain == null)
				{
					_brain = CinemachineCore.Instance.FindPotentialTargetBrain(virtualCameraBase);
				}
				return _brain;
			}
		}
		private CinemachineBrain _brain;

		//--------------------------------------------------------------------------------------------/
		// Properties: Static
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Whether the default Cinemachine input handling is enabled
		/// </summary>
		public static bool defaultInputEnabled => CinemachineCore.GetInputAxis != null;
		/// <summary>
		/// The default input delegate is stored here when disabled
		/// </summary>
		private static CinemachineCore.AxisInputDelegate defaultInputDelegate { get; set; }

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnCameraActive(bool active)
		{
			if (active && disableDefaultInput)
			{
				ToggleDefaultInput(false);
			}
			else
			{
				ToggleDefaultInput(true);
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		public static void ToggleDefaultInput(bool toggle, bool debug = false)
		{
			StratusDebug.LogIf(debug, $"Toggling default input: {toggle}");

			// Upon first invocation, store the default
			if (defaultInputDelegate == null && CinemachineCore.GetInputAxis != null)
			{
				defaultInputDelegate = CinemachineCore.GetInputAxis;
			}

			if (toggle)
			{
				CinemachineCore.GetInputAxis = defaultInputDelegate;
			}
			else
			{
				CinemachineCore.GetInputAxis = GetDisabledAxisInputValue;
			}
		}

		private static float GetDisabledAxisInputValue(string axisName) => 0f;
	}

}