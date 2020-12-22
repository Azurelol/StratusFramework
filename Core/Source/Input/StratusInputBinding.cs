
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stratus
{
	/// <summary>
	/// Allows you to quickly set up an input (button/key/mouse button) as a field and check it with
	/// provided methods.
	/// </summary>
	[Serializable]
	public class StratusInputBinding
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// What type of input is being read
		/// </summary>
		public enum Type { Key, MouseButton, Axis }
		/// <summary>
		/// What action is being checked for
		/// </summary>
		public enum Action { Down, Up, Held }
		/// <summary>
		/// The state of the input
		/// </summary>
		public enum State
		{
			Down,
			Up,
			Pressed,
			Toggle
		}


		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		[FormerlySerializedAs("type")]
		private Type _type = Type.Key;
		[SerializeField]
		private KeyCode key;
		[SerializeField]
		private StratusMouseButton mouseButton;
#pragma warning disable 414
		[SerializeField]
		private string axis;
#pragma warning restore 414

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public Type type => this._type;		

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public StratusInputBinding()
		{
		}

		public StratusInputBinding(KeyCode key)
		{
			this.key = key;
			this._type = Type.Key;
		}

		public StratusInputBinding(StratusMouseButton button)
		{
			this.mouseButton = button;
			this._type = Type.MouseButton;
		}

		public StratusInputBinding(string axis)
		{
			this.axis = axis;
			this._type = Type.Axis;
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Returns true during the first frame the input is pressed down
		/// </summary>
		public bool isDown
		{
			get
			{
				switch (this._type)
				{
					case Type.Key:
						return Input.GetKeyDown(this.key);
					case Type.MouseButton:
						return Input.GetMouseButtonDown((int)this.mouseButton);
					case Type.Axis:
						return Input.GetButtonDown(this.axis);
				}

				throw new Exception("Input type not supported");
			}
		}

		/// <summary>
		/// Returns true while the input is pressed down
		/// </summary>
		public bool isPressed
		{
			get
			{
				switch (this._type)
				{
					case Type.Key:
						return Input.GetKey(this.key);
					case Type.MouseButton:
						return Input.GetMouseButton((int)this.mouseButton);
					case Type.Axis:
						return Input.GetButton(this.axis);
				}

				throw new Exception("Input type not supported");
			}
		}

		/// <summary>
		/// Returns true during the first frame the user releases the input
		/// </summary>
		public bool isUp
		{
			get
			{
				switch (this._type)
				{
					case Type.Key:
						return Input.GetKeyUp(this.key);
					case Type.MouseButton:
						return Input.GetMouseButtonUp((int)this.mouseButton);
					case Type.Axis:
						return Input.GetButtonUp(this.axis);
				}

				throw new Exception("Input type not supported");
			}
		}

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

		public override string ToString()
		{
			string value = null;
			switch (this._type)
			{
				case Type.Key:
					value = $"Key {this.key}";
					break;
				case Type.MouseButton:
					value = $"{this.mouseButton} Mouse Button";
					break;
			}

			return value;
		}

	}
}