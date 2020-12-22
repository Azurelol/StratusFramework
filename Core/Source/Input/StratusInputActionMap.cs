using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Stratus
{
	/// <summary>
	/// Base class for input action maps
	/// </summary>
	public abstract class StratusInputActionMap
	{
		/// <summary>
		/// The name of the action map these inputs are for
		/// </summary>
		public abstract string map { get; }

		/// <summary>
		/// An abstract function that maps the context to the provided actions in the derived map
		/// </summary>
		/// <param name="context"></param>
		public abstract bool HandleInput(InputAction.CallbackContext context);

		/// <summary>
		/// Converts an input action phase enumerated value from Unity's to this system
		/// </summary>
		/// <param name="phase"></param>
		/// <returns></returns>
		public StratusInputActionPhase Convert(InputActionPhase phase) => StratusInputUtility.Convert(phase);
	}

	public interface IStratusInputUIActionHandler
	{
		void Navigate(Vector2 dir);
	}

	
}