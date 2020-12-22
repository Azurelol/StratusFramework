using UnityEngine;

namespace Stratus.Gameplay
{
	public enum StratusCharacterMovementOffset
	{
		PlayerForward,
		CameraForward,
		CameraUp
	}

	/// <summary>
	/// A simple, modular player controller
	/// </summary>
	public class StratusCharacterController : StratusExtensibleBehaviour
	{
		//--------------------------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------------------------/
		public class MovementPreset
		{
			public VectorAxis horizontalAxisInput;
			public VectorAxis verticalAxisInput;
			public StratusCharacterMovementOffset offset;
		}

		public class JumpEvent : Stratus.StratusEvent { }

		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		[HideInInspector]
		public bool debug = false;

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnAwake()
		{
		}

		protected override void OnStart()
		{
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		[ContextMenu("Show Debug")]
		private void ShowDebug()
		{
			this.debug = true;
		}

		[ContextMenu("Hide Debug")]
		private void HideDebug()
		{
			this.debug = false;
		}

	}
}
