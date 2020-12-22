using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// A Cinemachine POV camera, using POV and transposer components
	/// </summary>
	public class StratusCinemachinePOVCamera : StratusCinemachineVirtualCamera<CinemachineTransposer, CinemachinePOV>
	{
		//--------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------/
		public CinemachineConfiner confiner { get; private set; }
		public float minimumPitch => aim.m_VerticalAxis.m_MinValue;
		public float maximumPitch => aim.m_VerticalAxis.m_MaxValue;
		public float pitchDifference => Mathf.Abs(maximumPitch - minimumPitch);
		public float minimumHeight
		{
			get
			{
				if (confiner != null)
				{
					if (confiner.m_ConfineMode == CinemachineConfiner.Mode.Confine3D)
					{
						return confiner.m_BoundingVolume.bounds.min.y;
					}
					else
					{
						return confiner.m_BoundingShape2D.bounds.min.y;
					}
				}
				return float.MaxValue;
			}
		}
		public float maximumHeight
		{
			get
			{
				if (confiner != null)
				{
					if (confiner.m_ConfineMode == CinemachineConfiner.Mode.Confine3D)
					{
						return confiner.m_BoundingVolume.bounds.max.y;
					}
					else
					{
						return confiner.m_BoundingShape2D.bounds.max.y;
					}
				}
				return float.MaxValue;
			}
		}
		public float heightDifference
		{
			get
			{
				if (confined)
				{
					return Mathf.Abs(maximumHeight - minimumHeight);
				}
				return 0f;
			}
		}
		public bool confined => confiner != null;

		//--------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------/
		protected override void OnVirtualCameraAwake()
		{
			confiner = virtualCameraBase.GetComponent<CinemachineConfiner>();
		}

		protected override void OnVirtualCameraUpdate()
		{
		}

		//--------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------/
		/// <summary>
		/// Offsets the camera's current follow position
		/// </summary>
		/// <param name="offset"></param>
		public void OffsetPosition(Vector3 offset)
		{
			Vector3 result = body.m_FollowOffset + offset;
			if (confined)
			{
				if (result.y > maximumHeight)
				{
					result.y = maximumHeight;
				}
				else if (result.y < minimumHeight)
				{
					result.y = minimumHeight;
				}
			}

			body.m_FollowOffset = result;
		}

		/// <summary>
		/// Sets the camera's height
		/// </summary>
		/// <param name="height"></param>
		public void SetHeight(float height)
		{
			OffsetPosition(new Vector3(0f, height, 0f));
		}

		/// <summary>
		/// Offsets the camera's rotation along the horizontal axis
		/// </summary>
		/// <param name="offset"></param>
		public override void OffsetHorizontalAxis(float offset)
		{
			aim.m_HorizontalAxis.Value += offset;
		}

		/// <summary>
		/// Offsets the camera's rotation along the vertical axis
		/// </summary>
		/// <param name="offset"></param>
		public override void OffsetVerticalAxis(float offset)
		{
			aim.m_VerticalAxis.Value -= offset;
		}

		/// <summary>
		/// Sets the camera's pitch
		/// </summary>
		/// <param name="value"></param>
		public void SetVerticalAxis(float value)
		{
			aim.m_VerticalAxis.Value = value;
		}

		/// <summary>
		/// Sets the default values of the camera
		/// </summary>
		public void SetDefaults()
		{
			SetVerticalAxis(maximumPitch);
			SetHeight(maximumHeight);
		}
	}
}