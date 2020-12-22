using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// Base class for inputs
	/// </summary>
	public abstract class StratusInput
	{
		public abstract bool PollState();

		public static bool GetMouseButtonDown(StratusMouseButton button) => Input.GetMouseButtonDown((int)button);
		public static bool GetMouseButtonUp(StratusMouseButton button) => Input.GetMouseButtonUp((int)button);
		public static bool GetMouseButton(StratusMouseButton button) => Input.GetMouseButton((int)button);
	}

	/// <summary>
	/// An action that is triggered from input
	/// </summary>
	/// <typeparam name="InputType"></typeparam>

	public class StratusInputAction : StratusLabeledAction
	{
		public StratusInput input { get; private set; }
		public StratusInput altInput { get; private set; }

		public bool assignedInput => input != null || altInput != null;
		public bool hasAltInput => altInput != null;

		public StratusInputAction(string label, Action action, StratusInput input)
			: base(label, action)
		{
			this.input = input;
		}

		public StratusInputAction(string label, Action action,
			StratusInput input, StratusInput altInput)
			: this(label, action, input)
		{
			this.altInput = altInput;
		}

		public StratusInputAction(string label, Action action) : base(label, action)
		{
		}
	}

	/// <summary>
	/// An action that is triggered from input
	/// </summary>
	/// <typeparam name="InputType"></typeparam>
	public class StratusInputAction<InputType> : StratusInputAction
		where InputType : StratusInput
	{
		public StratusInputAction(string label, Action action) : base(label, action)
		{
		}

		public StratusInputAction(string label, Action action, InputType input)
			: base(label, action, input)
		{
		}

		public StratusInputAction(string label, Action action, InputType input, InputType altInput)
			: base(label, action, input, altInput)
		{
		}
	}

	/// <summary>
	/// An action that is triggered from input
	/// </summary>
	/// <typeparam name="InputType"></typeparam>
	public class StratusInputAction<InputType, AltInputType> : StratusInputAction
		where InputType : StratusInput
		where AltInputType : StratusInput
	{
		public StratusInputAction(string label, Action action)
			: base(label, action)
		{
		}

		public StratusInputAction(string label, Action action,
			InputType input, AltInputType altInput)
			: base(label, action, input, altInput)
		{
		}
	}

	public static class StratusInputExtensions
	{
	}
}