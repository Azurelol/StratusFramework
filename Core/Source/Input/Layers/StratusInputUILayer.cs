using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Stratus
{
	public enum StratusInputUIAction
	{
		Navigate,
		Submit,
		Cancel,
		Pause,
		Next,
		Previous,
		Reset,
	}

	/// <summary>
	/// Default input action map for UI
	/// </summary>
	public class StratusInputUIActionMap : StratusInputActionMap
	{
		public override string map => "UI";

		public Action<Vector2> onNavigate;
		public Action onSubmit;
		public Action onCancel;
		public Action onNext;
		public Action onPrevious;
		public Action onReset;
		public Action onPause;

		public const string navigationActionName = nameof(StratusInputUIAction.Navigate);// "Navigate";
		public const string submitActionName = nameof(StratusInputUIAction.Submit);
		public const string cancelActionName = nameof(StratusInputUIAction.Cancel);
		public const string nextActionName = nameof(StratusInputUIAction.Next);
		public const string previousActionName = nameof(StratusInputUIAction.Previous);
		public const string resetActionName = nameof(StratusInputUIAction.Reset);
		public const string pauseActionName = nameof(StratusInputUIAction.Pause);

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			bool handled = false;
			if (context.performed)
			{
				switch (context.action.name)
				{
					case navigationActionName:
						onNavigate?.Invoke(context.ReadValue<Vector2>());
						handled = true;
						break;

					case submitActionName:
						onSubmit?.Invoke();
						handled = true;
						break;

					case cancelActionName:
						onCancel?.Invoke();
						handled = true;
						break;

					case nextActionName:
						onNext?.Invoke();
						handled = true;
						break;

					case previousActionName:
						onPrevious?.Invoke();
						handled = true;
						break;

					case resetActionName:
						onReset?.Invoke();
						handled = true;
						break;

					case pauseActionName:
						onPause?.Invoke();
						handled = true;
						break;

				}
			}
			return handled;
		}
	}

	public class StratusInputUILayer : StratusInputLayer<StratusInputUIActionMap>
	{
		public StratusInputUILayer(string label) : base(label)
		{
		}

		public StratusInputUILayer(string label, StratusInputUIActionMap actions) : base(label, actions)
		{
		}

		protected override void OnActive(bool active)
		{
			if (active)
			{
				StratusCursorLock.ReleaseLock();
			}
			else
			{
				StratusCursorLock.RevertLock();
			}
		}
	}

}