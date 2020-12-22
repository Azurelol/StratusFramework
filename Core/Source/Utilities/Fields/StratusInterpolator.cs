using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
	namespace Utilities
	{
		/// <summary>
		/// A general-purpose utility class for interpolation of struct types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public abstract class StratusInterpolator<T>
		{
			//----------------------------------------------------------------------/
			// Fields
			//----------------------------------------------------------------------/
			protected T _startingValue;
			protected T _currentValue;
			protected T _EndingValue;
			bool _Active;
			StratusStopwatch Timer;

			//----------------------------------------------------------------------/
			// Properties
			//----------------------------------------------------------------------/
			/// <summary>
			/// The starting value
			/// </summary>
			public T startingValue
			{
				set
				{
					_startingValue = value;
				}
			}

			/// <summary>
			/// The current value
			/// </summary>
			public T currentValue
			{
				get
				{
					if (Active)
						return _currentValue;
					return _EndingValue;
				}
			}

			/// <summary>
			/// The ending value
			/// </summary>
			public T endingValue
			{
				set
				{
					_EndingValue = value;
				}
			}

			/// <summary>
			/// Whether it is currently interpolating
			/// </summary>
			public bool Active { get { return _Active; } }

			//----------------------------------------------------------------------/
			// Configuration
			//----------------------------------------------------------------------/
			public StratusInterpolator()
			{
			}


			//----------------------------------------------------------------------/
			// Methods
			//----------------------------------------------------------------------/
			/// <summary>
			/// Begins interpolation
			/// </summary>
			/// <param name="time"></param>
			public void Start(float time)
			{
				Timer = new StratusStopwatch(time);
				_Active = true;
			}

			/// <summary>
			/// Updates the interpolator
			/// </summary>
			public void Update()
			{
				Update(Time.deltaTime);
			}

			/// <summary>
			/// Updates the interpolator
			/// </summary>
			/// <param name="dt"></param>
			public void Update(float dt)
			{
				if (Active)
				{
					if (Timer.Update(dt))
					{
						Timer.Reset();
						_Active = false;
					}

					float t = Timer.normalizedProgress;
					Interpolate(t);

					// Interpolate depending on type
					if (currentValue is Vector3)
					{

					}

				}
			}

			protected abstract void Interpolate(float t);
		}

		/// <summary>
		/// Interpolates a Vector3
		/// </summary>
		public class Vector3Interpolator : StratusInterpolator<Vector3>
		{
			protected override void Interpolate(float t)
			{
				_currentValue = Vector3.Slerp(_startingValue, _EndingValue, t);
			}
		}

		/// <summary>
		/// Interpolates a Vector2
		/// </summary>
		public class Vector2Interpolator : StratusInterpolator<Vector2>
		{
			protected override void Interpolate(float t)
			{
				_currentValue = Vector2.Lerp(_startingValue, _EndingValue, t);
			}
		}






	}
}

