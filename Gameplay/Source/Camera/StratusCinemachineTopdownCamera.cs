using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	[Serializable]
	public class StratusTopdownCameraSettings
	{
		[SerializeField] private float _panSpeed = 5f;
		[SerializeField] private float _zoomSpeed = 5f;
		[SerializeField] private float _rotationSpeed = 5f;

		public float panSpeed => _panSpeed * panSpeedMultiplier;
		public float zoomSpeed => _zoomSpeed * zoomSpeedMultiplier;
		public float rotationSpeed => _rotationSpeed * rotationSpeedMultiplier;

		public const float panSpeedMultiplier = 5f;
		public const float zoomSpeedMultiplier = 20f;
		public const float rotationSpeedMultiplier = 30f;

		public bool modifyPitchOnZoom = true;
	}

	public class StratusCinemachineTopdownCamera : StratusCinemachinePOVCamera
	{
		//--------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------/
		public StratusTopdownCameraSettings settings = new StratusTopdownCameraSettings();

		//--------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------/
		protected override void OnLook(Vector2 delta) => Pan(delta);

		/// <summary>
		/// Pans the camera, moving it along the XZ axes
		/// </summary>
		/// <param name="delta"></param>
		public void Pan(Vector2 delta)
		{
			Vector3 offset = Vector3.zero;

			// FORWARD
			if (delta.y > 0.0f)
			{
				offset += transform.up * settings.panSpeed;
			}
			else if (delta.y < 0.0f)
			{
				offset += transform.up * -settings.panSpeed;
			}

			// RIGHT
			if (delta.x > 0.0f)
			{
				offset += transform.right * settings.panSpeed;
			}
			else if (delta.x < 0.0f)
			{
				offset += transform.right * -settings.panSpeed;
			}

			offset.y = 0f;
			offset *= deltaTime;
			OffsetPosition(offset);
		}


		/// <summary>
		/// Zooms the camera, moving it along the Y axis
		/// </summary>
		/// <param name="delta"></param>
		public void Zoom(float delta)
		{
			Vector3 offset = Vector3.zero;

			if (delta < 0.0f)
			{
				offset.y += settings.zoomSpeed;
			}
			else if (delta > 0.0f)
			{
				offset.y -= settings.zoomSpeed;
			}

			if (settings.modifyPitchOnZoom && confined)
			{
				float pitch = CalculatePitchAtHeight(height);
				SetVerticalAxis(pitch);
			}

			offset *= deltaTime;
			OffsetPosition(offset);
		}

		/// <summary>
		/// Rotates the camera around the y-axis
		/// </summary>
		/// <param name="yRotation"></param>
		public void OffsetYawByDelta(float delta)
		{
			float offset = 0f;
			if (delta > 0.0f)
			{
				offset = -settings.rotationSpeed;
			}
			else if (delta < 0.0f)
			{
				offset = settings.rotationSpeed;
			}
			offset *= deltaTime;
			OffsetHorizontalAxis(offset);
		}

		private float CalculatePitchAtHeight(float height)
		{
			if (height <= minimumHeight)
			{
				return minimumPitch;
			}
			else if (height >= maximumHeight)
			{
				return maximumPitch;
			}

			float offsetHeight = height - minimumHeight;
			float t = Mathf.Abs(offsetHeight / heightDifference);
			float pitch = Mathf.Lerp(minimumPitch, maximumPitch, t);
			//StratusDebug.Log($"height = {height}, minHeight = {minimumHeight}, maxHeight = {maximumHeight}, heightDiff = {heightDifference}, t = {t}, pitch = {pitch}");
			return pitch;
		}
	}

}