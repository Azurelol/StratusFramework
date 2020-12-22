using Cinemachine;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// A camera using Cinemachine's virtual camera system
	/// </summary>
	public abstract class StratusCinemachineVirtualCameraBase : StratusCinemachineCamera
	{
		//--------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------/
		[SerializeField]
		private CinemachineVirtualCamera _virtualCamera;

		//--------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------/
		public CinemachineVirtualCamera virtualCamera => _virtualCamera;
		public override CinemachineVirtualCameraBase virtualCameraBase => _virtualCamera;

		//--------------------------------------------------------------------/
		// Virtual
		//--------------------------------------------------------------------/
		protected abstract void OnVirtualCameraAwake();
		protected abstract void OnVirtualCameraUpdate();

		//--------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------/
		protected override void OnCameraAwake()
		{
			OnVirtualCameraAwake();
		}

		private void Update()
		{
			if (active)
			{
				OnVirtualCameraUpdate();
			}
		}

		private void Reset()
		{
			_virtualCamera = GetComponent<CinemachineVirtualCamera>();
		}

	}

	/// <summary>
	/// A camera using Cinemachine's virtual camera system using specified body and aim components
	/// </summary>
	/// <typeparam name="Body"></typeparam>
	/// <typeparam name="Aim"></typeparam>
	public abstract class StratusCinemachineVirtualCamera<Body, Aim> : StratusCinemachineVirtualCameraBase
		where Body : CinemachineComponentBase
		where Aim : CinemachineComponentBase
	{
		public Body body { get; private set; }
		public Aim aim { get; private set; }

		public abstract void OffsetHorizontalAxis(float offset);
		public abstract void OffsetVerticalAxis(float offset);

		protected override void OnVirtualCameraAwake()
		{
			body = virtualCamera.GetCinemachineComponent<Body>();
			if (body == null)
			{
				this.LogError("No transposer component found in cinemachine virtual camera!");
			}
			aim = virtualCamera.GetCinemachineComponent<Aim>();
			if (aim == null)
			{
				this.LogError("No POV component found in cinemachine virtual camera!");
			}
		}

		protected override void OnLook(Vector2 delta)
		{
			OffsetAxesByDelta(delta);
		}

		public void OffsetAxesByDelta(Vector2 delta)
		{
			OffsetHorizontalAxis(delta.x);
			OffsetVerticalAxis(delta.y);
		}
	}
}