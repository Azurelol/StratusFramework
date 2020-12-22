using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	public abstract class StratusCameraBehaviourBase : StratusBehaviour
	{
		//--------------------------------------------------------------------------------------------/
		// Inspector fields
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Whether the camera is active
		/// </summary>
		[SerializeField]
		private bool _active = true;
		/// <summary>
		/// Whether input is only accepted if the cursor is currently locked
		/// </summary>
		public bool lookOnCursorLocked = true;
		/// <summary>
		/// The default sensitivity multiplier for look input
		/// </summary>
		public Vector2 lookSensitivity = Vector2.one;
		/// <summary>
		/// The lock mode checked agaisnt for accepting input
		/// </summary>
		public const CursorLockMode lockMode = CursorLockMode.Locked;

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public bool active
		{
			get => _active;
			set
			{
				if (_active != value)
				{
					_active = value;
					OnCameraActive(value);
				}
			}
		}
		public abstract Camera outputCamera { get; }
		public float yaw => outputCamera.transform.rotation.y;
		public float pitch => outputCamera.transform.rotation.x;
		public float roll => outputCamera.transform.rotation.z;
		public float height => outputCamera.transform.position.y;
		public float deltaTime => Time.deltaTime;
		public bool acceptInput { get; set; } = true;
		public bool initialized { get; private set; }

		//--------------------------------------------------------------------------------------------/
		// Virtual
		//--------------------------------------------------------------------------------------------/
		protected abstract void OnCameraAwake();
		protected abstract void OnCameraActive(bool active);
		protected abstract void OnLook(Vector2 delta);

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		private void Awake()
		{
			OnCameraAwake();
			OnCameraActive(_active);
			initialized = true;
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Offsets the camera's rotation by the given delta with no normalization to it
		/// </summary>
		/// <param name="delta"></param>
		public void LookRaw(Vector2 delta)
		{
			if (!acceptInput)
			{
				return;
			}
			if (!ValidateCursorLockState())
			{
				this.LogWarning($"Cannot look while cursor lock mode not {lockMode}");
				return;
			}
			OnLook(delta);
		}

		public bool ValidateCursorLockState()
		{
			if (lookOnCursorLocked && (Cursor.lockState != lockMode))
			{
				return false;
			}
			return true;
		}
	}
}
