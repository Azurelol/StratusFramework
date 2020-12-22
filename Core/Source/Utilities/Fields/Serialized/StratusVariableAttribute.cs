using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	[Serializable]
	public class StratusVariableAttribute
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private float baseValue;

		[SerializeField]
		private float increment;

		[SerializeField]
		private float bonusValue;

		[SerializeField]
		private float floor;
		[SerializeField]
		private float ceiling;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Label for this attribute
		/// </summary>
		public string label { get; set; }
		/// <summary>
		/// The current value of this parameter
		/// </summary>
		public float current => maximum + increment;
		/// <summary>
		/// The maximum value of this parameter. Base + modifiers.
		/// </summary>
		public float maximum => baseValue + bonusValue;
		/// <summary>
		/// The current ratio of the parameter when compared to its maximum as a percentage
		/// </summary>
		public float percentage => (current / maximum) * 100.0f;
		/// <summary>
		/// Whether this parameter's current value is at its maximum value
		/// </summary>
		public bool isAtMaximum => current == maximum;
		/// <summary>
		/// Whether this parameter's current value is at its minimum value
		/// </summary>
		public bool isAtMinimum => current == floor;
		/// <summary>
		/// Returns an instance with a value of 1
		/// </summary>
		public static StratusVariableAttribute one => new StratusVariableAttribute(1f);
		/// <summary>
		/// If locked, prevents modifications
		/// </summary>
		public bool locked { get; set; }
		/// <summary>
		/// The last increment (from an increase/decrease call)
		/// </summary>
		public float lastIncrement { get; private set; }

		//------------------------------------------------------------------------/
		// Functions
		//------------------------------------------------------------------------/
		public Func<float, float> increaseModifier { get; set; }
		public Func<float, float> decreaseModifier { get; set; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Invoked when this attribute reaches its maximum value
		/// </summary>
		public event Action onMaximum;
		/// <summary>
		/// Emits the current percentage change of this attribute
		/// </summary>
		public event Action<float> onModified;
		/// <summary>
		/// Invoked when this attribute reaches its minimum value
		/// </summary>
		public event Action onMinimum;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value">The base value of the parameter</param>
		/// <param name="floor">The minimum value for this parameter</param>
		public StratusVariableAttribute(float value, float floor = 0.0f, float ceiling = float.MaxValue)
		{
			this.baseValue = value;
			this.floor = floor;
			this.ceiling = ceiling;
			this.bonusValue = 0.0f;
			Reset();
		}

		public StratusVariableAttribute() : this(0f)
		{
		}

		public override string ToString()
		{
			return $"({baseValue}, {current}, {bonusValue})";
		}

		public string ToPercentageString()
		{
			return $"{current}/{maximum}";
		}

		public static implicit operator float(StratusVariableAttribute attribute) => attribute.current;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Resets the current value of this parameter back to maximum
		/// </summary>
		public void Reset()
		{
			increment = 0;
		}

		/// <summary>
		/// Adds to the current value of this parameter, up to the maximum value
		/// </summary>
		/// <param name="value"></param>
		public bool Increase(float value)
		{
			if (locked)
			{
				return false;
			}

			if (isAtMaximum)
			{
				return false;
			}

			if (increaseModifier != null)
			{
				value = increaseModifier(value);
			}

			float previousPercentage = percentage;

			lastIncrement = value;
			increment += value;
			if (current > maximum) increment = maximum - current;
			if (current > ceiling) increment = ceiling - current;
			float percentageGained = percentage - previousPercentage;

			onModified?.Invoke(percentageGained);
			return true;
		}

		/// <summary>
		/// Reduces the current value of this parameter, up to its minimum value
		/// </summary>
		/// <param name="value"></param>
		/// <returns>How much was lost, as a percentage of the total value of this parameter</returns>
		public bool Decrease(float value)
		{
			if (locked)
			{
				return false;
			}

			if (value < 0f)
				throw new ArgumentException($"The input value for decrease '{value}' must be positive!");

			if (decreaseModifier != null)
			{
				value = decreaseModifier(value);
			}

			float previousPercentage = percentage;

			lastIncrement = value;
			increment -= value;
			if (current < floor) increment = -maximum;
			float percentageLost = previousPercentage - percentage;
			onModified?.Invoke(percentageLost);

			if (isAtMinimum)
			{
				onMinimum?.Invoke();
			}
			return true;
		}

		/// <summary>
		/// Adds a positive temporary modifier to this parameter
		/// </summary>
		/// <param name="bonus"></param>
		public void AddBonus(float bonus)
		{
			this.bonusValue += bonus;
		}

		/// <summary>
		/// Sets the modifier of this parameter to a flat value
		/// </summary>
		/// <param name="modifier"></param>
		public void SetBonus(float modifier)
		{
			this.bonusValue = modifier;
		}

		/// <summary>
		/// Sets the current value forcefully, ignoring modifiers
		/// </summary>
		/// <param name="value"></param>
		public void DecreaseToFloor()
		{
			float value = maximum - Math.Abs(increment);
			Decrease(value);
		}

		/// <summary>
		/// Clears all modifiers for this parameter
		/// </summary>
		public void ClearBonus()
		{
			bonusValue = 0.0f;
		}
	}

	/// <summary>
	/// An event driven variable
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusEventDrivenVariableAttribute<T> : StratusVariableAttribute
	{
		public StratusEventDrivenVariableAttribute(float value,
			float floor = 0,
			float ceiling = float.MaxValue)
			: base(value, floor, ceiling)
		{
			this.label = defaultLabel;
		}

		public abstract class BaseEvent : StratusEvent
		{
			public float value { get; set; }
		}

		public class IncreaseEvent : BaseEvent
		{
		}

		public class DecreaseEvent : BaseEvent
		{
		}

		public bool eventConnected { get; private set; }

		public abstract string defaultLabel { get; }

		public void Initialize(MonoBehaviour monoBehaviour)
		{
			monoBehaviour.gameObject.Connect<IncreaseEvent>(this.OnIncreaseEvent);
			monoBehaviour.gameObject.Connect<DecreaseEvent>(this.OnDecreaseEvent);
			eventConnected = true;
		}

		private void OnIncreaseEvent(IncreaseEvent e) => Increase(e.value);
		private void OnDecreaseEvent(DecreaseEvent e) => Decrease(e.value);

	}
}