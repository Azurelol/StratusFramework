using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Base class for callback-based input
	/// </summary>
	public abstract class StratusInputCallback
	{
		protected string axis;
		protected abstract void CheckInput();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="axis">The name of the virtual axis or button</param>
		public StratusInputCallback(string axis)
		{
			this.axis = axis;
		}

		/// <summary>
		/// Updates the timer. When there's no cooldown remaining, it will check for 
		/// the axis input. If it is not zero, it will invoke the provided callback function.
		/// </summary>
		/// <param name="dt">The delta time.</param>
		public virtual void Update(float dt)
		{
			this.CheckInput();
		}
	}

	/// <summary>
	/// Input that is restricted by a timer.
	/// </summary>
	public abstract class StratusTimedInputCallback : StratusInputCallback
	{
		protected StratusStopwatch Timer;
		public float CooldownLeft => this.Timer.current;
		public StratusTimedInputCallback(string name, float cooldown) : base(name)
		{
			this.Timer = new StratusStopwatch(cooldown);
		}

		public override void Update(float dt)
		{
			this.Timer.Update(Time.deltaTime);
			if (this.Timer.isFinished)
			{
				this.CheckInput();
			}
		}

	}

	/// <summary>
	/// Cooldown-based axis input that invokes a callback when input is detected
	/// </summary>
	public class AxisInput : StratusTimedInputCallback
	{
		public delegate void AxisCallback(float axisInput);

		private AxisCallback OnInput;

		public AxisInput(string name, float cooldown, AxisCallback onInput) : base(name, cooldown)
		{
			this.OnInput = onInput;
		}

		protected override void CheckInput()
		{
			float axisInput = Input.GetAxisRaw(this.axis);
			if (axisInput != 0f)
			{
				this.OnInput(axisInput);
				this.Timer.Reset();
			}
		}
	}

	/// <summary>
	/// Cooldown-based axis input that invokes a callback when input is detected
	/// </summary>
	public class ButtonInput : StratusTimedInputCallback
	{
		public delegate void ButtonCallback();

		private ButtonCallback OnButtonDown;
		private ButtonCallback OnButttonUp;

		public ButtonInput(string name, float cooldown, ButtonCallback onButtonDown, ButtonCallback onButtonUp)
		  : base(name, cooldown)
		{
			this.OnButtonDown = onButtonDown;
			this.OnButttonUp = onButtonUp;
		}

		public override void Update(float dt)
		{
			base.Update(dt);
			// Always check for when the button is released
			if (Input.GetButtonUp(this.axis) && this.OnButttonUp != null)
			{
				this.OnButttonUp();
			}
		}

		protected override void CheckInput()
		{
			if (Input.GetButtonDown(this.axis) && this.OnButtonDown != null)
			{
				this.OnButtonDown();
				this.Timer.Reset();
			}
		}
	}

}
