using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public enum StratusMouseInputState
	{
		Down,
		Up,
		Pressed
	}

	public enum StratusMouseButton
	{
		Left,
		Right,
		Middle
	}

	/// <summary>
	/// Input for mouse
	/// </summary>
	[Serializable]
	public class StratusMouseInput : StratusInput
	{
		public StratusMouseButton button;
		public StratusMouseInputState state;

		private int buttonValue => (int)button;

		public override bool PollState()
		{
			bool result = false;
			switch (this.state)
			{
				case StratusMouseInputState.Down:
					result = Input.GetMouseButtonDown(buttonValue);
					break;
				case StratusMouseInputState.Up:
					result = Input.GetMouseButtonUp(buttonValue);
					break;
				case StratusMouseInputState.Pressed:
					result = Input.GetMouseButton(buttonValue);
					break;
			}
			return result;

		}
	}

}