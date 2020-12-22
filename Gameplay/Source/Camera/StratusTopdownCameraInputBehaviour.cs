using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.Android;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

namespace Stratus.Gameplay
{
	public class StratusTopdownCameraInput : IStratusLogger
	{
		public Action<Vector2> panAction;
		public Action<float> zoomAction;
		public Action<float> rotateAction;
		public bool rotateCamera;
		public float screenEdgeThreshold = 32f;

		public StratusTopdownCameraInput(Action<Vector2> panAction, Action<float> zoomAction, Action<float> rotateAction)
		{
			this.panAction = panAction;
			this.zoomAction = zoomAction;
			this.rotateAction = rotateAction;
		}

		public void ToggleMouseCameraRotation()
		{
			rotateCamera = !rotateCamera;
		}



		public void RotateByMouse(Vector2 value)
		{

		}
	}

	public class StratusTopdownCameraInputBehaviour : StratusCameraInputBehaviour<StratusCinemachineTopdownCamera>
	{
		[SerializeField]
		private float screenEdgeThreshold = 32f;
		[SerializeField]
		private StratusMouseButton rotateButton = StratusMouseButton.Middle;

		public bool rotateCamera { get; private set; }
		private Vector2 previousMousePosition { get; set; }

		protected override void OnInputSchemeChanged(StratusInputScheme inputScheme)
		{
		}

		protected override void PollInput(StratusInputScheme inputScheme)
		{
			switch (inputScheme)
			{
				case StratusInputScheme.Unknown:
					break;
				case StratusInputScheme.KeyboardMouse:
					PollKBM();
					break;
				case StratusInputScheme.DualShock:
					break;
				case StratusInputScheme.Xbox:
					break;
				default:
					break;
			}
		}

		private void PollKBM()
		{
			if (rotateCamera)
			{
				if (StratusInput.GetMouseButtonUp(rotateButton))
				{
					rotateCamera = false;
				}
				else
				{
					CheckRotation();
					RecordMousePosition();
				}
			}
			else
			{
				if (StratusInput.GetMouseButtonDown(rotateButton))
				{
					RecordMousePosition();
					rotateCamera = true;
				}
				else
				{
					CheckPan();
					CheckZoom();
				}
			}

		}

		private void CheckRotation()
		{
			Vector2 mouseDelta = mousePosition - previousMousePosition;
			float delta = mouseDelta.x;
			cameraBehaviour.OffsetYawByDelta(delta);
		}

		private void RecordMousePosition()
		{
			previousMousePosition = mousePosition;
		}

		private bool CheckZoom()
		{
			Vector2 mouseScrollDelta = Input.mouseScrollDelta;
			if (mouseScrollDelta != Vector2.zero)
			{
				cameraBehaviour.Zoom(mouseScrollDelta.y);
				return true;
			}
			return false;
		}

		private bool CheckPan()
		{
			Vector2 currentMousePosition = mousePosition;
			bool pan = false;
			Vector2 panVector = Vector2.zero;

			if (currentMousePosition.x >= 0 && currentMousePosition.x <= Screen.width
				&&
				currentMousePosition.y >= 0 && currentMousePosition.y <= Screen.height)
			{
				if (currentMousePosition.x <= screenEdgeThreshold)
				{
					panVector.x = -1;
					pan = true;
				}
				else if (Screen.width - currentMousePosition.x <= screenEdgeThreshold)
				{
					panVector.x = 1;
					pan = true;
				}

				if (currentMousePosition.y <= screenEdgeThreshold)
				{
					panVector.y = -1;
					pan = true;
				}
				else if (Screen.height - currentMousePosition.y <= screenEdgeThreshold)
				{
					panVector.y = 1;
					pan = true;
				}
			}

			if (pan)
			{
				cameraBehaviour.Pan(panVector);
			}
			return pan;
		}
	}

}