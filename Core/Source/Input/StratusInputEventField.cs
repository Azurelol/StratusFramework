using System;
using UnityEngine.Events;

namespace Stratus
{
	/// <summary>
	/// How the input changes state
	/// </summary>
	public enum ToggleMode
	{
		Hold,
		Toggle
	}

	/// <summary>
	/// A field for encapsulating custom actions depending on input read
	/// </summary>
	[Serializable]
	public class StratusInputEventField : StratusSerializable
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		[Serializable]
		public class OnButtonInput : UnityEvent { }

		[Serializable]
		public class OnAxisInput : UnityEvent<float> { }

		[Serializable]
		public class OnToggleInput : UnityEvent<bool> { }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The input being considered
		/// </summary>
		public StratusInputBinding input = new StratusInputBinding();

		/// <summary>
		/// Whether the axis input is being treated as an axis or a button
		/// </summary>
		//[DrawIf(nameof(isAxisType))]
		public StratusAxisType axisType = StratusAxisType.Button;

		/// <summary>
		/// What state of input we are looking for
		/// </summary>
		public StratusInputBinding.State state = StratusInputBinding.State.Down;

		/// <summary>
		/// Input whenever one of the state actions is detected
		/// </summary>
		//[DrawIf(nameof(isDefaultInput))]
		public OnButtonInput onInput = new OnButtonInput();

		/// <summary>
		/// Callback for whenever there's change detected on the axis
		/// </summary>
		//[DrawIf(nameof(isAxisInput))]
		public OnAxisInput onAxisInput = new OnAxisInput();

		/// <summary>
		/// Callback for when the input is toggled on/off
		/// </summary>
		//[DrawIf(nameof(isToggleInput))]
		public OnToggleInput onToggleInput = new OnToggleInput();

		private bool previouslyNeutral = false;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public StratusInputBinding.Type type => this.input.type;
		public bool toggled { get; private set; }
		public bool isAxisType => input.type == StratusInputBinding.Type.Axis;
		public bool isAxisInput => isAxisType && axisType == StratusAxisType.Button;
		public bool isToggleInput => !this.isAxisType && this.state == StratusInputBinding.State.Toggle;
		public bool isDefaultInput => !this.isAxisType && this.state != StratusInputBinding.State.Toggle;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public bool Update()
		{
			bool changed = false;
			switch (this.type)
			{
				case StratusInputBinding.Type.Key:
				case StratusInputBinding.Type.MouseButton:
					{
						switch (this.state)
						{
							case StratusInputBinding.State.Down:
								if (this.input.isDown)
								{
									this.onInput.Invoke();
									changed = true;
								}

								break;

							case StratusInputBinding.State.Up:
								if (this.input.isUp)
								{
									this.onInput.Invoke();
									changed = true;
								}

								break;

							case StratusInputBinding.State.Pressed:
								if (this.input.isPressed)
								{
									this.onInput.Invoke();
									changed = true;
								}
								break;

							case StratusInputBinding.State.Toggle:
								if (this.input.isDown)
								{
									this.toggled = !this.toggled;
									this.onToggleInput.Invoke(this.toggled);
									changed = this.toggled;
								}
								break;

						}
					}
					break;


				case StratusInputBinding.Type.Axis:

					// Not Neutral
					if (!this.input.isNeutral)
					{
						this.onAxisInput.Invoke(this.input.value);
						changed = true;
					}
					// Neutral
					else
					{
						if (!this.previouslyNeutral)
						{
							this.onAxisInput.Invoke(this.input.value);
							this.previouslyNeutral = true;
							changed = true;
						}
					}

					break;
			}
			return changed;
		}
	}

}