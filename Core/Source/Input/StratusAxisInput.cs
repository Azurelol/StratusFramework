using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// How the given input axis is treated, as an axis (-1 to 1) or as a button
	/// </summary>
	public enum StratusAxisType
	{
		Axis,
		Button
	}

	public enum StratusAxisInputState
	{
		Positive,
		Negative,
		Neutral
	}

	/// <summary>
	/// Input for axis
	/// </summary>
	public class StratusAxisInput : StratusInput
	{
		public string axis = null;
		public StratusAxisInputState state = StratusAxisInputState.Positive;

		/// <summary>
		/// Returns true if the current value of the virtual axis is greater than 0
		/// </summary>
		public bool isPositive => Input.GetAxis(this.axis) > 0f;

		/// <summary>
		/// Returns true if the current value of the virtual axis is less than 0
		/// </summary>
		public bool isNegative => Input.GetAxis(this.axis) < 0f;

		/// <summary>
		/// Returns true if the current value of the virtual axis is 0
		/// </summary>
		public bool isNeutral => Input.GetAxis(this.axis) == 0f;

		/// <summary>
		/// Returns the value of the virtual axis
		/// </summary>
		public float value => Input.GetAxis(this.axis);

		/// <summary>
		/// Returns the value of the virtual axis with no smoothing filtering applied
		/// </summary>
		public float rawValue => Input.GetAxisRaw(this.axis);

		public override bool PollState()
		{
			bool result = false;
			switch (this.state)
			{
				case StratusAxisInputState.Positive:
					result = isPositive;
					break;
				case StratusAxisInputState.Negative:
					result = isNegative;
					break;
				case StratusAxisInputState.Neutral:
					result = isNeutral;
					break;
			}
			return result;
		}
	}

}