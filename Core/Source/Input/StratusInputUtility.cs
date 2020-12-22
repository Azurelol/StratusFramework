using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Stratus
{
	public enum StratusInputActionPhase
	{
		Started,
		Performed,
		Canceled,
	}

	public delegate void StratusInputActionCallback(StratusInputActionPhase phase);
	public delegate void StratusInputActionCallback<T>(StratusInputActionPhase phase, T value);

	public static class StratusInputUtility
	{
		public static StratusInputActionPhase Convert(InputActionPhase phase)
		{
			switch (phase)
			{
				case InputActionPhase.Disabled:
					break;
				case InputActionPhase.Waiting:
					break;
				case InputActionPhase.Started:
					return StratusInputActionPhase.Started;
				case InputActionPhase.Performed:
					return StratusInputActionPhase.Performed;
				case InputActionPhase.Canceled:
					return StratusInputActionPhase.Canceled;
			}
			throw new NotImplementedException(phase.ToString());
		}
	}

	public class StratusPersistentInputAction<T>
	{
		public T currentValue { get; private set; }
		public StratusInputActionPhase phase { get; private set; }
		public bool active { get; private set; }
		public Action<T> callback { get; private set; }

		public StratusPersistentInputAction(Action<T> callback)
		{
			this.callback = callback;
		}

		public void Set(StratusInputActionPhase phase, T value)
		{			
			currentValue = value;
			switch (phase)
			{
				case StratusInputActionPhase.Started:
					active = true;
					break;
				case StratusInputActionPhase.Canceled:
					active = false;
					break;
			}
		}

		public void Update()
		{
			if (active)
			{
				callback(currentValue);
			}
		}
	}

}