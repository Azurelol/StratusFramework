using System.Reflection;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// A type of action that modifies the value of
	/// a given property over a specified amount of time, using a specified
	/// interpolation formula(Ease).
	/// </summary>
	public abstract class StratusActionProperty : StratusAction
	{
		/// <summary>
		/// The supported types for this interpolator
		/// </summary>
		public enum Types
		{
			Integer,
			Float,
			Boolean,
			Vector2,
			Vector3,
			Vector4,
			Color,
			None
		}

		/// <summary>
		/// The types supported by this interpolator
		/// </summary>
		public static System.Type[] supportedTypes { get; } = new System.Type[7] { typeof(float), typeof(int), typeof(bool), typeof(Vector2), typeof(Vector3), typeof(Color), typeof(Vector4) };

		/// <summary>
		/// Deduces if the given type is one of the supported ones
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Types Deduce(System.Type type)
		{
			if (type == typeof(int))
			{
				return Types.Integer;
			}
			else if (type == typeof(float))
			{
				return Types.Float;
			}
			else if (type == typeof(bool))
			{
				return Types.Boolean;
			}
			else if (type == typeof(Vector2))
			{
				return Types.Vector2;
			}
			else if (type == typeof(Vector3))
			{
				return Types.Vector3;
			}
			else if (type == typeof(Vector4))
			{
				return Types.Vector4;
			}
			else if (type == typeof(Color))
			{
				return Types.Color;
			}

			return Types.None;
		}

		protected Ease easeType;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="duration">How long should the action delay for</param>
		/// <param name="ease">The interpolation algorithm to use</param>
		public StratusActionProperty(float duration, Ease ease)
		{
			this.duration = duration;
			this.easeType = ease;
		}

		/// <summary>
		/// Updates the action
		/// </summary>
		/// <param name="dt">The delta time</param>
		/// <returns>How much time was consumed during this action</returns>
		public override float Update(float dt)
		{
			return this.Interpolate(dt);
		}

		public abstract float Interpolate(float dt);
	}

	public abstract class StratusActionPropertyBase<T> : StratusActionProperty
	{
		//----------------------------------------------------------------------/
		// Fields
		//----------------------------------------------------------------------/
		protected T difference;
		protected T initialValue;
		protected T endValue;
		private bool initialized = false;

		protected object target;
		protected PropertyInfo property;
		protected FieldInfo field;

		//----------------------------------------------------------------------/
		// CTOR
		//----------------------------------------------------------------------/
		public StratusActionPropertyBase(object target, PropertyInfo property, T endValue, float duration, Ease ease)
	  : base(duration, ease)
		{
			this.target = target;
			this.property = property;
			this.endValue = endValue;
			base.duration = duration;
			this.easeType = ease;
		}

		public StratusActionPropertyBase(object target, FieldInfo field, T endValue, float duration, Ease ease)
	  : base(duration, ease)
		{
			this.target = target;
			this.field = field;
			this.endValue = endValue;
			base.duration = duration;
			this.easeType = ease;
		}

		/// <summary>
		/// Interpolates the given Property/Field.
		/// </summary>
		/// <param name="dt">The current delta time.</param>
		/// <returns></returns>
		public override float Interpolate(float dt)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}

			this.elapsed += dt;
			float timeLeft = this.duration - this.elapsed;

			// If done updating
			if (timeLeft <= dt)
			{
				this.isFinished = true;
				this.SetLast();
				return dt;
			}

			this.SetCurrent();
			return timeLeft;
		}

		/// <summary>
		/// Gets the initial value for the property. This is done separately
		/// because we want to capture the value at the time this action is beinig
		/// executed, not when it was created!
		/// </summary>
		public void Initialize()
		{
			if (this.property != null)
			{
				this.initialValue = (T)this.property.GetValue(this.target, null);
			}
			else if (this.field != null)
			{
				this.initialValue = (T)this.field.GetValue(this.target);
			}
			else
			{
				throw new System.Exception("Couldn't set initial value!");
			}

			// Now we can compute the difference
			this.ComputeDifference();

			if (logging)
			{
				StratusDebug.Log("InitialValue = '" + this.initialValue
								+ "', EndValue = '" + this.endValue + "'"
								+ "' Difference = '" + this.difference + "'");
			}

			this.initialized = true;
		}

		/// <summary>
		/// Sets the current value for the property.
		/// </summary>
		public virtual void SetCurrent()
		{
			float easeVal = this.easeType.Evaluate(this.elapsed / this.duration);
			T currentValue = this.ComputeCurrentValue((easeVal));
			if (logging)
			{
				StratusDebug.Log("CurrentValue = '" + currentValue + "'");
			}

			this.Set(currentValue);
		}

		/// <summary>
		/// Sets the last value for the property.
		/// </summary>
		public virtual void SetLast()
		{
			this.Set(this.endValue);
		}

		/// <summary>
		/// Sets the value for the property.
		/// </summary>
		/// <param name="val"></param>
		public void Set(T val)
		{
			if (this.property != null)
			{
				this.property.SetValue(this.target, val, null);
			}
			else
			{
				this.field.SetValue(this.target, val);
			}
		}

		public abstract void ComputeDifference();
		public abstract T ComputeCurrentValue(float easeVal);
	}



}

