using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// A behaviour for manually controlling a freelook camera
	/// </summary>
	public class StratusCinemachineFreeLookCamera : StratusCinemachineCamera
	{
		public CinemachineFreeLook freeLook;
		public override CinemachineVirtualCameraBase virtualCameraBase => freeLook;

		protected override void OnCameraAwake()
		{
		}

		private void Reset()
		{
			freeLook = GetComponent<CinemachineFreeLook>();
		}

		protected override void OnLook(Vector2 delta) => OffsetByDelta(delta);

		/// <summary>
		/// Offsets the camera's rotation along the horizontal axis
		/// </summary>
		/// <param name="offset"></param>
		public void OffsetHorizontalAxis(float offset)
		{
			freeLook.m_XAxis.Value += offset;
		}

		/// <summary>
		/// Offsets the camera's rotation along the vertical axis
		/// </summary>
		/// <param name="offset"></param>
		public void OffsetVerticalAxis(float offset)
		{
			freeLook.m_YAxis.Value += offset;
		}

		/// <summary>
		/// Offsets the camera's rotation along the xy axes
		/// </summary>
		/// <param name="delta"></param>
		public void OffsetByDelta(Vector2 delta)
		{
			freeLook.m_XAxis.Value += delta.x;
			freeLook.m_YAxis.Value -= delta.y;
		}


	}

}